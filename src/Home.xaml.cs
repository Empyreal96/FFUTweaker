using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using W10M_Toolbox;

namespace FFUTweak
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {

        string _SYSTEMHIVE;
        string _SOFTWAREHIVE;
        RegistryKey _SYSTEMKEY;
        RegistryKey _SOFTWAREKEY;
        string _LOADEDHIVESYS;
        string _LOADEDHIVESOFT;


        public Home()
        {
            InitializeComponent();
            mountRing.Foreground = Global.accentColor;

            // Make sure UI controls are active when returning to the 'Home' page
            if (Global.MountedDirectory != null && Global._PhysicalDrive != null)
            {
                CancelBtn.IsEnabled = true;
                SaveFFUBtn.IsEnabled = true;
                FFUInfo.Text = Global.GlobalFFUInfo;
            }
            // TODO add sync for ppkg.cfg link file
        }


        /// <summary>
        /// Select a valid FFU file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFFUBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Full Flash Update (*.ffu)|*.ffu";
            if (openFile.ShowDialog() == true)
            {
                var filename = System.IO.Path.GetFileName(openFile.FileName);
                Global._FFUPath = openFile.FileName;

                MountFFUBtn.IsEnabled = true;
                FFUPath.Text = filename;
            }

        }

        /// <summary>
        /// Mount the selected image ready for editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MountFFUBtn_Click(object sender, RoutedEventArgs e)
        {
            mountRing.IsActive = true;
            FFUInfo.Text = "";

            if (Global.isMounted == true)
            {
                UnmountImage();
                while (Global.isMounted == true)
                {
                    await Task.Delay(500);
                }
            }
            Global.isProcessFinished = false;
            try
            {
                // Check if Test Signing Certs are installed, a file will be created if 'InstallOEMCerts.bat' was succesful
                if (File.Exists(@".\Tools\certs.installed") == false)
                {
                    MessageBox.Show("Installing OEM Certs, please wait", "OEM Certificates", MessageBoxButton.OK, MessageBoxImage.Information);
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = @".\Tools\installoemcerts.bat",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true

                    };


                    //oemCerts.StartInfo.FileName = @".\Tools\installoemcerts.bat";
                    //oemCerts.StartInfo.CreateNoWindow = true;

                    var oemCerts = Process.Start(processStartInfo);
                    var output = oemCerts.StandardOutput.ReadToEnd();
                    Global.GlobalFFUInfo = output;
                    FFUInfo.Text = output;
                    oemCerts.OutputDataReceived += Wpimage_OutputDataReceived;
                    oemCerts.ErrorDataReceived += Wpimage_ErrorDataReceived;

                    //oemCerts.WaitForExit();
                    //oemCerts.Close();

                }
                else
                {
                    FFUInfo.Text = "Certficates already installed";
                }

                //DEBUG: MessageBox.Show($"isMounted: {Global.isMounted}\nisProcessFinished: {Global.isProcessFinished}");


                var wpimageStartInfo = new ProcessStartInfo
                {
                    FileName = @".\Tools\bin\i386\wpimage.exe",
                    Arguments = $"mount {Global._FFUPath}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                mountRing.IsActive = true;
                await Task.Run(() =>
                {
                    var wpimage = Process.Start(wpimageStartInfo);



                    wpimage.OutputDataReceived += Wpimage_OutputDataReceived;
                    wpimage.BeginOutputReadLine();
                    wpimage.ErrorDataReceived += Wpimage_ErrorDataReceived;
                    wpimage.BeginErrorReadLine();



                    // wpimage.WaitForExit();
                    //   Global.GlobalFFUInfo = wpimage.StandardOutput.ReadToEnd();
                    // this.Dispatcher.Invoke(() =>
                    // FFUInfo.Text = $"\n{Global.GlobalFFUInfo}"

                    // ) ;

                    //wpimage.Close();
                });

                while (Global.isProcessFinished != true)
                {
                    await Task.Delay(500);
                }
                //FFUInfo.Text = RemoveLastLine(FFUInfo.Text);
                string[] splitOutput = Global.GlobalFFUInfo.Split('\n');
                foreach (var line in splitOutput)
                {
                    if (line.Contains("Main Mount Path:"))
                    {
                        var mPath = line.Replace("Main Mount Path: ", "").Trim();
                        Global.MountedDirectory = mPath;
                        Global.isMounted = true;
                    }
                    if (line.Contains("OpenDiskInternal: Mounted virtual disk at phys"))
                    {
                        var pdrive = line.Replace("OpenDiskInternal: Mounted virtual disk at phys", "").Trim().Replace("?", ".");
                        Global._PhysicalDrive = pdrive;
                        //FFUInfo.Text += "\n\n " + pdrive;
                    }
                }

                if (Global.isMounted)
                {


                    FFUInfo.Text += "\nReading Registry";
                    await Task.Delay(500);
                    RetrieveRegistry();
                    FFUInfo.Text += "\nAdding permission for 'Everyone' to allow file modifications";
                    await Task.Delay(1500);
                    SetPermissions();
                    FFUInfo.Text += "\nSuccessfully mounted image";
                    UnmountRegistry();
                    FFUInfo.Text += "\nReading Installed Packages";
                    CheckInstalledPackages();

                    while (isDUFinished == false)
                    {
                        Debug.WriteLine("Waiting for UpdateApp to finish");
                        await Task.Delay(500);
                    }

                    if (Global.InstalledPackages.Contains("hr = 0x80070522"))
                    {
                        MessageBox.Show("Error Reading Packages: Please unmount image and restart FFUTweak as Administrator, \"UpdateApp.exe\" failed to gain appropriate permissions.");
                    }
                    else if (Global.InstalledPackages.Contains("getinstalledpackages completed unsuccessfully"))
                    {
                        MessageBox.Show("Error when processing \"UpdateApp.exe getinstalledpackages\"");
                    }


                    FFUInfo.Text += $"\nVHD FIle: {Global.VHDPath}";

                    SaveFFUBtn.IsEnabled = true;
                    CancelBtn.IsEnabled = true;
                    MountFFUBtn.IsEnabled = false;
                    mountRing.IsActive = false;
                    FFUInfo.Text += "\nReady to modify";
                    Global.GlobalFFUInfo = FFUInfo.Text;
                }



            }
            catch (Exception ex)
            {
                mountRing.IsActive = false;

                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public String RemoveLastLine(String myStr)
        {
            if (myStr.Length > 2)
            {
                int lastNewLine;
                if ((lastNewLine = myStr.Substring(0, myStr.Length - 2).LastIndexOf("\r\n")) != -1)
                    return (myStr.Substring(0, lastNewLine));
            }
            return ("");
        }

        private void Wpimage_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FFUInfo.Text += $"\n{e.Data}";
                    FFUInfoScroller.ScrollToBottom();


                }));
                Global.GlobalFFUInfo += $"\n{e.Data}";

            }
        }

        private void Wpimage_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FFUInfo.Text += $"\n{e.Data}";
                    FFUInfoScroller.ScrollToBottom();


                }));
                Global.GlobalFFUInfo += $"\n{e.Data}";
                if (e.Data.Contains(".vhd"))
                {
                    Global.VHDPath = e.Data;
                }
            }
            else
            {
                Global.isProcessFinished = true;
            }
        }

        private void OemCerts_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            FFUInfo.Text += "\n" + e.Data;

        }


        /// <summary>
        /// Dismount and save the mounted image as a new FFU file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveFFUBtn_Click(object sender, RoutedEventArgs e)
        {
            FFUInfo.Text = "";
            string messageBoxText = "Do you want to save changes?";
            string caption = "Commit changes";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {

                // Here we replace the "ffuloader.efi" file with the developer menu to allow easy access to MSC, this is mainly when unlocking the bootloader during "flash fix" phase
                if (File.Exists($"{Global.MountedDirectory}\\Windows\\System32\\Boot\\ffuloader.efi"))
                {
                    File.Move($"{Global.MountedDirectory}\\Windows\\System32\\Boot\\ffuloader.efi", $"{Global.MountedDirectory}\\Windows\\System32\\Boot\\ffuloader.efi.bak");
                    File.Copy(@".\Assets\developermenu.efi", $"{Global.MountedDirectory}\\Windows\\System32\\Boot\\ffuloader.efi");
                }


                Global.isProcessFinished = false;
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "Full Flash Update (*.ffu)|*.ffu";
                if (saveFile.ShowDialog() == true)
                {
                    var savePath = saveFile.FileName;

                    var wpimageStartInfo = new ProcessStartInfo
                    {
                        FileName = @".\Tools\bin\i386\wpimage.exe",
                        Arguments = $"dismount -physicalDrive {Global._PhysicalDrive} -imageFile \"{savePath}\" -noSign",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    mountRing.IsActive = true;
                    await Task.Run(async () =>
                    {
                        var wpimage = Process.Start(wpimageStartInfo);



                        wpimage.OutputDataReceived += Wpimage_OutputDataReceived;
                        wpimage.BeginOutputReadLine();
                        wpimage.ErrorDataReceived += Wpimage_ErrorDataReceived;
                        wpimage.BeginErrorReadLine();



                        //wpimage.WaitForExit();


                        //wpimage.Close();
                        while (Global.isProcessFinished == false)
                        {
                            await Task.Delay(500);
                        }
                    });

                    if (FFUInfo.Text.Contains("Success"))
                    //if (FFUInfo.Text.ToLower().Contains("failed to dismount"))
                    {
                        MessageBox.Show("Successfully saved the image", "Success", MessageBoxButton.OK);
                        Global.SavedFFUPath = savePath;
                    }
                    else
                    {
                        MessageBox.Show("Error saving image, check output for info", "Error", MessageBoxButton.OK);
                    }
                    mountRing.IsActive = false;
                    SaveFFUBtn.IsEnabled = false;
                    CancelBtn.IsEnabled = false;
                }

                Global.MountedDirectory = null;
                Global._PhysicalDrive = null;
                Global.isMounted = false;

                CancelBtn.IsEnabled = false;
                SaveFFUBtn.IsEnabled = false;
            }
            else
            {

            }
        }


        /// <summary>
        /// Dismount the mounted image, without saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            mountRing.IsActive = true;
            UnmountImage();
            mountRing.IsActive = false;

        }

        private async void CheckInstalledPackages()
        {
            Process getinstalledpkg = new Process();
            getinstalledpkg.StartInfo.FileName = @".\Tools\bin\i386\updateapp.exe";
            getinstalledpkg.StartInfo.Arguments = "getinstalledpackages";
            getinstalledpkg.StartInfo.UseShellExecute = false;
            getinstalledpkg.StartInfo.CreateNoWindow = true;
            getinstalledpkg.StartInfo.RedirectStandardOutput = true;
            getinstalledpkg.StartInfo.RedirectStandardError = true;
            //MessageBox.Show("Running updateapp");
            await Task.Run(() =>
            {
                getinstalledpkg.OutputDataReceived += Getinstalledpkg_OutputDataReceived;
                getinstalledpkg.ErrorDataReceived += Getinstalledpkg_ErrorDataReceived;
                // MessageBox.Show("Starting updateapp");
                getinstalledpkg.Start();
                isDUFinished = false;
                getinstalledpkg.BeginOutputReadLine();
                getinstalledpkg.BeginErrorReadLine();
            });





        }

        private void Getinstalledpkg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {


            if (!String.IsNullOrEmpty(e.Data))
            {

                Global.InstalledPackages += $"\n{e.Data}";
                Debug.WriteLine(e.Data);
                if (e.Data.Contains("getinstalledpackages completed successfully"))
                {
                    isDUFinished = true;
                }

            }
            else
            {
                isDUFinished = true;
                //Global.isUpdateInProgress = false;


            }
        }
        bool isDUFinished;
        private void Getinstalledpkg_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Global.InstalledPackages += $"\n{e.Data}";

                Debug.WriteLine(e.Data);
                if (e.Data.Contains("getinstalledpackages completed successfully"))
                {
                    isDUFinished = true;
                }

            }
            else
            {
                isDUFinished = true;



            }
        }
        private async void UnmountImage()
        {
            //mountRing.IsActive = true;
            string messageBoxText = "Do you want unmount the image?";
            string caption = "Cancel changes";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {

                Global.isProcessFinished = false;
                FFUInfo.Text = "";
                //mountRing.IsActive = true;
                var wpimageStartInfo = new ProcessStartInfo
                {
                    FileName = @".\Tools\bin\i386\wpimage.exe",
                    Arguments = $"dismount -physicalDrive {Global._PhysicalDrive}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                await Task.Run(async () =>
                {
                    var wpimage = Process.Start(wpimageStartInfo);



                    wpimage.OutputDataReceived += Wpimage_OutputDataReceived;
                    wpimage.BeginOutputReadLine();
                    wpimage.ErrorDataReceived += Wpimage_ErrorDataReceived;
                    wpimage.BeginErrorReadLine();



                    //wpimage.WaitForExit();
                    while (Global.isProcessFinished == false)
                    {
                        await Task.Delay(500);
                    }

                    // wpimage.Close();


                });

                if (FFUInfo.Text.Contains("Storage Service: Dismounting the image in "))
                {

                    Global.isMounted = false;
                    Global.MountedDirectory = null;
                    Global._PhysicalDrive = null;
                    if (File.Exists(Global.VHDPath))
                    {
                        File.Delete(Global.VHDPath);
                    }
                    await Dispatcher.BeginInvoke(new Action(() => CancelBtn.IsEnabled = false));
                    await Dispatcher.BeginInvoke(new Action(() => SaveFFUBtn.IsEnabled = false));

                }
                else
                {
                    MessageBox.Show("There as an issue unmounting the image try again later");
                    await Dispatcher.BeginInvoke(new Action(() => CancelBtn.IsEnabled = true));
                }


                //mountRing.IsActive = false;

            }
        }

        bool isRegistryMounted = false;
        private async void RetrieveRegistry()
        {

            string ConfigPath = Global.MountedDirectory + @"Windows\System32\Config";

            _SOFTWAREHIVE = ConfigPath + @"\SOFTWARE";
            _SYSTEMHIVE = ConfigPath + @"\SYSTEM";
            //MessageBox.Show($"{_SOFTWAREHIVE} \n{_SYSTEMHIVE}");
            _LOADEDHIVESOFT = RegistryInterop.Load(_SOFTWAREHIVE);
            _LOADEDHIVESYS = RegistryInterop.Load(_SYSTEMHIVE);
            _SYSTEMKEY = Registry.Users.OpenSubKey(_LOADEDHIVESYS, true);
            _SOFTWAREKEY = Registry.Users.OpenSubKey(_LOADEDHIVESOFT, true);
            isRegistryMounted = true;

            RegistryKey DeviceBranding = _SYSTEMKEY.CreateSubKey(@"Platform\DeviceTargetingInfo", RegistryKeyPermissionCheck.ReadSubTree);
            RegistryKey OSVersion = _SOFTWAREKEY.CreateSubKey(@"Microsoft\Windows NT\CurrentVersion", RegistryKeyPermissionCheck.ReadSubTree);
            try
            {

                if (DeviceBranding != null)
                {
                    string modelName = DeviceBranding.GetValue("PhoneModelName").ToString();
                    string modelvariant = DeviceBranding.GetValue("PhoneHardwareVariant").ToString();

                    string manufacturer = DeviceBranding.GetValue("PhoneManufacturerDisplayName").ToString();

                    string socversion = DeviceBranding.GetValue("PhoneSOCVersion").ToString();

                    FFUInfo.Text += $"\nModel: {modelName}\n" +
                        $"Variant: {modelvariant}\n" +
                        $"Manufacturer: {manufacturer}\n" +
                        $"SOC: msm{socversion}";


                }

                if (OSVersion != null)
                {
                    string CurrentBuild = OSVersion.GetValue("CurrentBuild").ToString();
                    string BuildBranch = OSVersion.GetValue("BuildBranch").ToString();

                    string ReleaseId = OSVersion.GetValue("ReleaseId").ToString();
                    string EditionId = OSVersion.GetValue("EditionId").ToString();

                    FFUInfo.Text += $"\nBuild: {CurrentBuild}.{BuildBranch}\n" + $"Release: {ReleaseId}\n" + $"Edition: {EditionId}";

                    Global.FFUBuildVersion = CurrentBuild;

                }
                await Task.Delay(500);
            }
            catch (Exception ex)
            {

            }
        }

        private async void UnmountRegistry()
        {
            if (isRegistryMounted == true)
            {
                GC.Collect();
                _SYSTEMKEY.Close();
                _SOFTWAREKEY.Close();
                _SOFTWAREKEY.Dispose();
                _SYSTEMKEY.Dispose();
                await Task.Delay(1000);

                var regUnloadResult = RegistryInterop.Unload();
                if (regUnloadResult == "Error unloading Registry. Try again in a minute.")
                {
                    MessageBox.Show("There was an issue unloading registry, trying with \"reg.exe\"");
                    await Task.Delay(1000);

                    Process unmountSys = new System.Diagnostics.Process();
                    unmountSys.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                    unmountSys.StartInfo.FileName = @$"reg.exe";
                    unmountSys.StartInfo.Arguments = "unload HKEY_USERS\\RTSYSTEM";
                    unmountSys.StartInfo.UseShellExecute = false;
                    unmountSys.StartInfo.CreateNoWindow = true;

                    unmountSys.StartInfo.RedirectStandardOutput = true;
                    unmountSys.StartInfo.RedirectStandardInput = true;
                    unmountSys.StartInfo.RedirectStandardError = true;
                    unmountSys.Start();

                    Process unmountSoft = new System.Diagnostics.Process();
                    unmountSoft.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                    unmountSoft.StartInfo.FileName = @$"reg.exe";
                    unmountSoft.StartInfo.Arguments = "unload HKEY_USERS\\RTSOFTWARE";
                    unmountSoft.StartInfo.UseShellExecute = false;
                    unmountSoft.StartInfo.CreateNoWindow = true;

                    unmountSoft.StartInfo.RedirectStandardOutput = true;
                    unmountSoft.StartInfo.RedirectStandardInput = true;
                    unmountSoft.StartInfo.RedirectStandardError = true;
                    unmountSoft.Start();
                }

            }
        }

        /// <summary>
        /// Some folders are Read/Write protected, so here we add the Administrators group to the Folders/Files privilages to allow access
        /// </summary>
        private void SetPermissions()
        {
            Process backgrounds = new Process();
            backgrounds.StartInfo.FileName = "icacls.exe";
            backgrounds.StartInfo.Arguments = $"{Global.MountedDirectory}\\Windows\\System32\\Backgrounds /grant everyone:(OI)(CI)F /T";
            backgrounds.StartInfo.UseShellExecute = false;
            backgrounds.StartInfo.CreateNoWindow = true;
            backgrounds.Start();

            Process locks = new Process();
            locks.StartInfo.FileName = "icacls.exe";
            locks.StartInfo.Arguments = $"{Global.MountedDirectory}\\Windows\\System32\\Lockscreens /grant everyone:(OI)(CI)F /T";
            locks.StartInfo.UseShellExecute = false;
            locks.StartInfo.CreateNoWindow = true;
            locks.Start();

            Process boot = new Process();
            boot.StartInfo.FileName = "icacls.exe";
            boot.StartInfo.Arguments = $"{Global.MountedDirectory}\\EFIESP\\Windows\\System32\\Boot /grant everyone:(OI)(CI)F /T";
            boot.StartInfo.UseShellExecute = false;
            boot.StartInfo.CreateNoWindow = true;
            boot.Start();

            Process xaps = new Process();
            xaps.StartInfo.FileName = "icacls.exe";
            xaps.StartInfo.Arguments = $"{Global.MountedDirectory}\\PROGRAMS\\CommonFiles\\Xaps /grant everyone:(OI)(CI)F /T";
            xaps.StartInfo.UseShellExecute = false;
            xaps.StartInfo.CreateNoWindow = true;
            xaps.Start();

            Process sysApps = new Process();
            sysApps.StartInfo.FileName = "icacls.exe";
            sysApps.StartInfo.Arguments = $"{Global.MountedDirectory}\\Windows\\SystemApps\\* /grant everyone:(OI)(CI)F /T";
            sysApps.StartInfo.UseShellExecute = false;
            sysApps.StartInfo.CreateNoWindow = true;
            sysApps.StartInfo.Verb = "runas";

            sysApps.Start();

            Process fileesown = new Process();
            fileesown.StartInfo.FileName = "takeown.exe";
            fileesown.StartInfo.Arguments = $"/r /F {Global.MountedDirectory}\\SystemApps\\Microsoft.Windows.FileExplorer_cw5n1h2txyewy\\Assets\\*";
            fileesown.StartInfo.UseShellExecute = false;
            fileesown.StartInfo.CreateNoWindow = false;
            fileesown.StartInfo.Verb = "runas";
            fileesown.Start();

            fileesown.WaitForExit();





            Process filepsown = new Process();
            filepsown.StartInfo.FileName = "takeown.exe";
            filepsown.StartInfo.Arguments = $"/r /F {Global.MountedDirectory}\\SystemApps\\Microsoft.Windows.FilePicker_cw5n1h2txyewy\\Assets\\*";
            filepsown.StartInfo.UseShellExecute = false;
            filepsown.StartInfo.CreateNoWindow = false;
            filepsown.StartInfo.Verb = "runas";

            filepsown.Start();

            filepsown.WaitForExit();



            Process setsown = new Process();
            setsown.StartInfo.FileName = "takeown.exe";
            setsown.StartInfo.Arguments = $"/r /F {Global.MountedDirectory}\\SystemApps\\Microsoft.Windows.SystemSettings_cw5n1h2txyewy\\Assets\\*";
            setsown.StartInfo.UseShellExecute = false;
            setsown.StartInfo.CreateNoWindow = false;
            setsown.StartInfo.Verb = "runas";

            setsown.Start();

            setsown.WaitForExit();


            Process corsown = new Process();
            corsown.StartInfo.FileName = "takeown.exe";
            corsown.StartInfo.Arguments = $"/r /F {Global.MountedDirectory}\\SystemApps\\Microsoft.Windows.Cortana_cw5n1h2txyewy\\Assets\\*";
            corsown.StartInfo.UseShellExecute = false;
            corsown.StartInfo.CreateNoWindow = false;
            corsown.StartInfo.Verb = "runas";

            corsown.Start();

            corsown.WaitForExit();



            Process sounds = new Process();
            sounds.StartInfo.FileName = "icacls.exe";
            sounds.StartInfo.Arguments = $"{Global.MountedDirectory}\\PROGRAMS\\CommonFiles\\Sounds /grant everyone:(OI)(CI)F /T";
            sounds.StartInfo.UseShellExecute = false;
            sounds.StartInfo.CreateNoWindow = true;
            sounds.Start();



            backgrounds.WaitForExit();
            locks.WaitForExit();
            boot.WaitForExit();
            xaps.WaitForExit();
            sysApps.WaitForExit();
            sounds.WaitForExit();

            Process fontsown = new Process();
            fontsown.StartInfo.FileName = "takeown.exe";
            fontsown.StartInfo.Arguments = $"/F {Global.MountedDirectory}\\Windows\\Fonts\\seg*";
            fontsown.StartInfo.UseShellExecute = false;
            fontsown.StartInfo.CreateNoWindow = false;
            fontsown.Start();

            fontsown.WaitForExit();

            // Process appsown = new Process();
            //appsown.StartInfo.FileName = "takeown.exe";
            //appsown.StartInfo.Arguments = $"/r /F {Global.MountedDirectory}\\Windows\\SystemApps\\*";
            //appsown.StartInfo.UseShellExecute = false;
            //appsown.StartInfo.CreateNoWindow = false;
            //appsown.Start();

            //appsown.WaitForExit();


            Process fonts = new Process();
            fonts.StartInfo.FileName = "icacls.exe";
            fonts.StartInfo.Arguments = $"{Global.MountedDirectory}\\Windows\\Fonts\\seg* /grant everyone:F /C /T";
            fonts.StartInfo.UseShellExecute = false;
            fonts.StartInfo.CreateNoWindow = true;
            fonts.Start();

            fonts.WaitForExit();
        }

        private void FlashAppBtn_Click(object sender, RoutedEventArgs e)
        {
            FlashApp flashApp = new FlashApp();
            flashApp.Show();
        }

        private void WP8UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            // WP81Updater updater = new WP81Updater();
            //updater.Show();

            img2ffu img2ffu = new img2ffu();
            img2ffu.Show();
        }
    }
}
