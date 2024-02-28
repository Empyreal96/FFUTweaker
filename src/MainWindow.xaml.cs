using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Directory.Exists(".\\Temp") == false)
            {
                Directory.CreateDirectory(".\\Temp");
            } else
            {
                Directory.Delete(@".\Temp", true);
            }
            if (Directory.Exists(".\\ppkg") == false)
            {
                Directory.CreateDirectory(".\\ppkg");
            }

            MainFrame.Navigate(new Uri("Home.xaml", UriKind.RelativeOrAbsolute));

            Global.accentColor = SystemParameters.WindowGlassBrush;

        }

        private void MainNavView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                switch (args.SelectedItemContainer.Tag.ToString())
                {
                    case "NavHomeTag":
                        MainFrame.Navigate(new Uri("Home.xaml", UriKind.RelativeOrAbsolute));
                        break;
                    case "NavFoldersTag":
                        MainFrame.Navigate(new Uri("Folders.xaml", UriKind.RelativeOrAbsolute));

                        break;
                    case "NavStylesTag":
                        MainFrame.Navigate(new Uri("Style.xaml", UriKind.RelativeOrAbsolute));

                        break;
                    // case "NavStoresTag":
                    //     MainFrame.Navigate(new Uri("Stores.xaml", UriKind.RelativeOrAbsolute));

                    //     break;
                    case "NavSettingsTag":
                        MainFrame.Navigate(new Uri("Settings.xaml", UriKind.RelativeOrAbsolute));

                        break;
                    case "NavUpdatesTag":
                        MainFrame.Navigate(new Uri("Updates.xaml", UriKind.RelativeOrAbsolute));
                        break;
                    case "NavAppsTag":
                        MainFrame.Navigate(new Uri("Applications.xaml", UriKind.RelativeOrAbsolute));
                        break;
                }
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Global.MountedDirectory != null)
            {
                string messageBoxText = "Closing the application will undo changes and unmount image, continue?";
                string caption = "Close Applcation";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                if (result == MessageBoxResult.Yes)
                {
                    if (Directory.Exists(@".\Temp"))
                    {
                        Directory.Delete(@".\Temp", true);
                    }

                    var wpimageStartInfo = new ProcessStartInfo
                    {
                        FileName = @".\Tools\bin\i386\wpimage.exe",
                        Arguments = $"dismount -physicalDrive {Global._PhysicalDrive}",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,

                    };

                    // await Task.Run(async () =>
                    //  {
                    var wpimage = Process.Start(wpimageStartInfo);
                    wpimage.ErrorDataReceived += Wpimage_ErrorDataReceived;
                    wpimage.OutputDataReceived += Wpimage_OutputDataReceived;
                    wpimage.BeginOutputReadLine();
                    wpimage.BeginErrorReadLine();
                    wpimage.WaitForExit();
                    //});

                    if (outputtext.Contains("Storage Service: Dismounting the image in"))
                    {
                        if (File.Exists(Global.VHDPath))
                        {
                            File.Delete(Global.VHDPath);
                        }
                        MessageBox.Show("Image dismounted");
                    }
                    else
                    {
                        MessageBox.Show("Issue unmounting the Image, please unmount with Windows Disk Management.\nVHD file is still present in: " + Global.VHDPath + $"\n\n{outputtext}");
                    }

                }
                else
                {
                    e.Cancel = true;
                }

            }
        }
        string outputtext;
        private void Wpimage_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                outputtext += e.Data + "\n";

            }
            else
            {

            }
        }

        private void Wpimage_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                outputtext += e.Data + "\n";


            }
            else
            {

            }
        }



    }


}


