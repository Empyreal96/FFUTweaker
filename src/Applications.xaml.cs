using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
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

namespace FFUTweak
{
    /// <summary>
    /// Interaction logic for Applications.xaml
    /// </summary>
    public partial class Applications : Page
    {
        int n;
        List<string> ppkgcfg;

       
        public Applications()
        {
            InitializeComponent();
            InternetLine.Stroke = Global.accentColor;
            InteropLine.Stroke = Global.accentColor;
            SocialLine.Stroke = Global.accentColor;
            StoresLine.Stroke = Global.accentColor;
            ToolsLine.Stroke = Global.accentColor;
            CMDInjectorCheck.Visibility = Visibility.Collapsed;
            mobilecmdCheck.Visibility = Visibility.Collapsed;
            CMDInjectorCheck.IsEnabled = false;
            mobilecmdCheck.IsEnabled = false;

            switch (Global.FFUBuildVersion)
            {
                case "10586":
                    PenguinBtn.IsEnabled = false;
                    WUTBtn.IsEnabled = false;
                    ZellaBtn.IsEnabled = false;
                    unigramCheck.IsEnabled = false;
                    OnitorCheck.IsEnabled = false;
                    mobilecmdCheck.IsEnabled = false;
                    twitterCheck.IsEnabled = false;
                    appinstallerCheck.IsEnabled = false;
                    break;
                case "14393":
                    unigramCheck.IsEnabled = false;
                    mobilecmdCheck.IsEnabled = false;
                    OnitorCheck.IsEnabled = false;
                    break;
                case "15063":
                    break;
                case "15254":
                    break;
                default:
                    PenguinBtn.IsEnabled = false;
                    WUTBtn.IsEnabled = false;
                    ZellaBtn.IsEnabled = false;
                    unigramCheck.IsEnabled = false;
                    OnitorCheck.IsEnabled = false;
                    mobilecmdCheck.IsEnabled = false;
                    twitterCheck.IsEnabled = false;
                    appinstallerCheck.IsEnabled = false;
                    InteropToolsCheck.IsEnabled = false;
                    wingotagCheck.IsEnabled = false;
                    zipCheck.IsEnabled = false;
                    CMDInjectorCheck.IsEnabled = false;
                    MyTubeCheck.IsEnabled = false;
                    vcregCheck.IsEnabled = false;

                    break;
            }


            var ppkgbin = File.ReadAllLines(@".\Assets\ppkg.cfg.bin");
            if (ppkgcfg == null)
            {
                ppkgcfg = new List<string>();
            }
            else
            {
                ppkgcfg.Clear();
            }
            foreach (var item in ppkgbin)
            {
                ppkgcfg.Add(item);
            }



        }
        List<string> FileList;
        bool downloadInProgress;
        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {

          


            if (Directory.Exists(@".\Resources\PPKG"))
            {
                Directory.Delete(@".\Resources\PPKG", true);
            }

            bool isAUBuild = false;
            if (Global.FFUBuildVersion == "14393")
            {
                isAUBuild = true;
            }
            if (Global.FFUBuildVersion == "10586")
            {

            }

            if (FileList == null)
            {
                FileList = new List<string>();

            }
            else
            {
                FileList.Clear();
            }

            //MessageBox.Show("Fetching packages for install");
            n = 1;
            if (PenguinBtn.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("penguin,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},Pinguin2001,PenguinStore");
                    }
                }
            }
            if (WUTBtn.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (isAUBuild == true)
                    {
                        if (item.Contains("wut14393,"))
                        {
                            string[] appcfg = item.Split(",");
                            FileList.Add($"{appcfg[1]},BasharAstifan,WinUniversalTool");
                        }
                    }
                    else
                    {
                        if (item.Contains("wut,"))
                        {
                            string[] appcfg = item.Split(",");
                            FileList.Add($"{appcfg[1]},BasharAstifan,WinUniversalTool");

                        }
                    }
                }
            }
            if (ZellaBtn.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("zella,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},ZellaSoft,ZellaStore");

                    }
                }
            }
            if (unigramCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("unigram,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},abaculi,UnigramMobile");

                    }
                }
            }
            if (OnitorCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("onitor,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},empyreal96,OnitorBrowser");
                    }
                }
            }
            if (twitterCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("twitter,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},Pedro1234-code,Twitter");

                    }
                }
            }
            if (appinstallerCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("app-inst,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},Microsoft,AppInstaller");

                    }
                }
            }
            if (InteropToolsCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("interop,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},ITDevTeam,InteropTools");
                    }
                }
            }
            if (wingotagCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("wingotag,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},MahStudios,WingoTag");

                    }
                }
            }
            if (zipCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("8zip,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},Finebits,8Zip");

                    }
                }
            }
            if (MyTubeCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (isAUBuild == true)
                    {
                        if (item.Contains("mytube14393,"))
                        {
                            string[] appcfg = item.Split(",");
                            FileList.Add($"{appcfg[1]},RykenStudio,myTube");
                        }
                    }
                    else
                    {
                        if (item.Contains("mytube,"))
                        {
                            string[] appcfg = item.Split(",");
                            FileList.Add($"{appcfg[1]},RykenStudio,myTube");

                        }
                    }
                }
            }
            if (vcregCheck.IsChecked == true)
            {

                foreach (var item in ppkgcfg)
                {
                    if (item.Contains("vcreg,"))
                    {
                        string[] appcfg = item.Split(",");
                        FileList.Add($"{appcfg[1]},m,VCReg");
                    }
                }
            }

            DownloadBackground.Visibility = Visibility.Visible;
            DownloadPanel.Visibility = Visibility.Visible;
            if (!Global.InstalledPackages.ToLower().Contains("empyreal96.mainos.developer_mode"))
            {
                UpdateProgressCount.Text = "Enabling Developer Mode";
                PushDeveloperModeCab();
                while (Global.isUpdateInProgress == true)
                {
                    await Task.Delay(500);
                }

                if (UpdateappOutput.Contains("install completed successfully"))
                {
                    UpdateProgressCount.Text = "Enabled Developer Mode";
                }

            }
            foreach (var i in FileList)
            {
                string[] split = i.Split(",");
                string filename = System.IO.Path.GetFileName(split[0]);
                string[] filenamefix = filename.Split("?");
                Debug.WriteLine($"{split[0]}, {filenamefix[0]}");
                StartDownload(split[0], filenamefix[0], FileList.Count, split[1], split[2]);
                while (downloadInProgress == true)
                {
                    await Task.Delay(500);
                }
            }
        }


        public string UpdateappOutput;
        private void Getinstalledpkg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {


            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    Debug.WriteLine(e.Data);
                    if (e.Data.Contains("hr = 0x80070522"))
                    {
                        MessageBox.Show("Please restart FFUTweak as Administrator, \"UpdateApp.exe\" failed to gain appropriate permissions.");
                    }

                }));

                UpdateappOutput += $"{e.Data}\n";
            }
            else
            {
                
                Global.isUpdateInProgress = false;


            }
        }

        private void Getinstalledpkg_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    Debug.WriteLine(e.Data);
                    if (e.Data.Contains("hr = 0x80070522"))
                    {
                        MessageBox.Show("Please restart FFUTweak as Administrator, \"UpdateApp.exe\" failed to gain appropriate permissions.");
                    }

                }));
                UpdateappOutput += $"{e.Data}\n";


            }
            else
            {

                Global.isUpdateInProgress = false;


            }
        }

        private async void PushDeveloperModeCab()
        {
            Global.isUpdateInProgress = true;
            
            
            Process update = new Process();
            update.StartInfo.FileName = @".\Tools\bin\i386\updateapp.exe";
            update.StartInfo.Arguments = "install \".\\Resources\\cabs\\Builtin\\DevMode\"";
            update.StartInfo.UseShellExecute = false;
            update.StartInfo.RedirectStandardOutput = true;
            update.StartInfo.RedirectStandardError = true;
            update.StartInfo.CreateNoWindow = true;
            await Task.Run(() =>
            {
                update.OutputDataReceived += Getinstalledpkg_OutputDataReceived;
                update.ErrorDataReceived += Getinstalledpkg_ErrorDataReceived;
                update.Start();
                update.BeginOutputReadLine();
                update.BeginErrorReadLine();
            });
            while (Global.isUpdateInProgress)
            {
                await Task.Delay(500);
            }
           

        }

        private void StartDownload(string url, string filename, int count, string pkgOwner, string SubComponent)
        {
            try
            {
                downloadInProgress = true;

                WebClient webClient = new WebClient();
                webClient.DownloadProgressChanged += (s, e) =>
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        DownloadProgressBar.Maximum = e.TotalBytesToReceive;
                        DownloadProgressBar.Value = e.BytesReceived;
                        UpdateProgressCount.Text = $"{filename}: {e.BytesReceived.ToFileSize()}/{e.TotalBytesToReceive.ToFileSize()} : {n}/{count}";
                    }));
                };
                webClient.DownloadFileCompleted += (s, e) =>
                {
                    n++;
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        //DownloadProgress.IsEnabled = false;
                        UpdateProgressCount.Text = $"{n}/{count}";


                        downloadInProgress = false;

                        if (n == count)
                        {
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                //DownloadBackground.Visibility = Visibility.Collapsed;
                                //DownloadPanel.Visibility = Visibility.Collapsed;
                                ApplyProvisioningPkg($".\\Resources\\PPKG\\{filename}", pkgOwner, SubComponent);
                            }));
                        }
                        // any other code to process the file
                    }));
                };
                Directory.CreateDirectory(@".\Resources\PPKG");
                webClient.DownloadFileTaskAsync(new Uri(url),
                    $".\\Resources\\PPKG\\{filename}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading {url}\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }

        bool isProvisioningActive;
        private async void ApplyProvisioningPkg(string ppkgPath, string owner, string SubComponent)
        {
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                DownloadProgressBar.IsIndeterminate = true;
                UpdateProgressCount.Text = "Copying Apps to image";
            }));

            var enumPPKGs = Directory.GetFiles(@".\Resources\PPKG\");
            foreach (var file in enumPPKGs)
            {
                Debug.WriteLine(file);

                
            }

            //AppProvision(ppkgPath, @".\Resources\cabs\", owner, SubComponent);
            ProvisionApplications(enumPPKGs);
            while (isProvisioningActive == true)
            {
                await Task.Delay(500);
            }
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                DownloadPanel.Visibility = Visibility.Collapsed;
                DownloadBackground.Visibility = Visibility.Collapsed;
            }));
            MessageBox.Show("Apps should be successfully installed");
        }


        public void ProvisionApplications(string[] ppkgFileList)
        {
            isProvisioningActive = true;
            string Manifest =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<assembly xmlns=\"urn:schemas-microsoft-com:asm.v3\" manifestVersion=\"1.0\" displayName=\"default\" company=\"Microsoft Corporation\" copyright=\"Microsoft Corporation\">\n" +
                "  <assemblyIdentity name=\"FFUTweaker.MainOS.ProvisionedApps\" version=\"1.0.0.0\" language=\"neutral\" processorArchitecture=\"arm\" publicKeyToken=\"628844477771337a\" buildType=\"release\" />\n" +
                "  <package identifier=\"FFUTweaker.MainOS.ProvisionedApps\" releaseType=\"Update\" restart=\"possible\" targetPartition=\"MainOS\" binaryPartition=\"false\" >\n" +
                "    <customInformation>\n" +
                "      <phoneInformation phoneRelease=\"Production\" phoneOwnerType=\"OEM\" phoneOwner=\"FFUTweaker\" phoneComponent=\"MainOS\" phoneSubComponent=\"ProvisionedApps\" phoneGroupingKey=\"\"/>\n" +
                "      <file name=\"\\update.mum\" size=\"0\" staged=\"0\" compressed=\"0\" cabpath=\"update.mum\"/>\n";
            foreach (var ppkg in ppkgFileList)
            {
                var filename = System.IO.Path.GetFileName(ppkg);
                Debug.WriteLine(filename);
                Manifest += $"      <file name=\"$(runtime.systemroot)\\Provisioning\\PACKAGES\\{filename}\" size=\"0\" staged=\"0\" compressed=\"0\" cabpath=\"arm_ffutweaker.mainos.provisionedapps0_628844477771337a_1.0.0.0_none_a612d72c2b6a12cb\\windows\\provisioning\\packages\\{filename}\"/>\n";
            }
            Manifest +=
                "    </customInformation>\n" +
                "    <update name=\"FFUTweaker_MainOS_ProvisionedApps\">\n" +
                "      <component>\n" +
                "        <assemblyIdentity name=\"FFUTweaker.MainOS.ProvisionedApps.Deployment\" version=\"1.0.0.0\" processorArchitecture=\"arm\" language=\"neutral\" publicKeyToken=\"628844477771337a\" versionScope=\"nonSxS\" />\n" +
                "      </component>\n" +
                "    </update>\n" +
                "  </package>\n" +
                "</assembly>";

            File.WriteAllText($"{Global.MountedDirectory}\\windows\\servicing\\packages\\ffutweaker.mainos.provisionedapps0_628844477771337a_1.0.0.0_none_a612d72c2b6a12cb.mum", Manifest);
            foreach(var file in ppkgFileList)
            {
                var filename = System.IO.Path.GetFileName(file);
                File.Copy(file, $"{Global.MountedDirectory}\\windows\\provisioning\\packages\\{filename}", true);
            }
            isProvisioningActive = false;
        }


      
    }
}
