using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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
    /// Interaction logic for Folders.xaml
    /// </summary>
    public partial class Folders : Page
    {
        public Folders()
        {
            InitializeComponent();
            if (Global.MountedDirectory != null)
            {
                StartWallBtn.IsEnabled = true;
                LockWallBtn.IsEnabled = true;
                EFIESPBootBtn.IsEnabled = true;
                CommonFilesBtn.IsEnabled = true;
                InboxAppsBtn.IsEnabled = true;
                StockRingtones.IsEnabled = true;


            }
            else
            {
                StartWallBtn.IsEnabled = false;
                LockWallBtn.IsEnabled = false;
                EFIESPBootBtn.IsEnabled = false;
                CommonFilesBtn.IsEnabled = false;
                InboxAppsBtn.IsEnabled = false;
                StockRingtones.IsEnabled = false;
            }
        }

        private void StartWallBtn_Click(object sender, RoutedEventArgs e)
        {
            // Process.Start(@"Explorer.exe " + Global.MountedDirectory + @"Windows\System32\Backgrounds");
            var path = Global.MountedDirectory + @"Windows\System32\Backgrounds";
            var pinfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = path,
                Verb = "runas"
            };
            
            try
            {
                Process.Start(pinfo);
            } catch (Exception ex)
            {
                MessageBox.Show($"issue opening path: {path}");
            }
        }

        private void LockWallBtn_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("explorer.exe", $"{Global.MountedDirectory}\\Windows\\System32\\Lockscreen");
            var path = Global.MountedDirectory + @"Windows\System32\Lockscreen";
            var pinfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = path,
                Verb = "runas"
            };

            try
            {
                Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"issue opening path: {path}");
            }
        }

        private void EFIESPBootBtn_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("explorer.exe", $"{Global.MountedDirectory}\\EFIESP\\Windows\\System32\\Boot");
            var path = Global.MountedDirectory + @"EFIESP\Windows\System32\Boot";
            var pinfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = path,
                Verb = "runas"
            };

            try
            {
                Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"issue opening path: {path}");
            }
        }

        private void CommonFilesBtn_Click(object sender, RoutedEventArgs e)
        {
            // Process.Start("explorer.exe", $"{Global.MountedDirectory}\\PROGRAMS\\CommonFiles\\Xaps");
            var path = Global.MountedDirectory + @"PROGRAMS\CommonFiles\Xaps";

            var pinfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = path,
                Verb = "runas"
            };

            try
            {
                Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"issue opening path: {path}");
            }
        }

        private void InboxAppsBtn_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("explorer.exe", $"{Global.MountedDirectory}\\Windows\\SystemApps");
            var path = Global.MountedDirectory + @"Windows\SystemApps";

            var pinfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = path,
                Verb = "runas"
            };

            try
            {
                Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"issue opening path: {path}");
            }
        }

        private void StockRingtones_Click(object sender, RoutedEventArgs e)
        {
            // Process.Start("Explorer.exe", Global.MountedDirectory + @"PROGRAMS\CommonFiles\Sounds");
            var path = Global.MountedDirectory + @"PROGRAMS\CommonFiles\Sounds";

            var pinfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = path,
                Verb = "runas"
            };

            try
            {
                Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"issue opening path: {path}");
            }
        }




       
    }
}
