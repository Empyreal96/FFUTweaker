using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using Path = System.IO.Path;

namespace FFUTweak
{
    /// <summary>
    /// Interaction logic for Updates.xaml
    /// </summary>
    public partial class Updates : Page
    {
        public string getdulog;
        public List<string> updateList;
        public bool isDUFinished = false;
        public bool downloadInProgress;
        public Updates()
        {
            InitializeComponent();

            DownloadProgressBar.Foreground = Global.accentColor;
            if (Global.isMounted == false)
            {
                UpdateappOutput.IsEnabled = false;
                UpdateList.IsEnabled = false;
                ApplyUpdBtn.IsEnabled = false;
                ApplyCustomBtn.IsEnabled = false;
            }
            else
            {
                switch (Global.FFUBuildVersion)
                {
                    case "10240":
                        UpdateList.Items.Add("14393");
                        UpdateList.SelectedIndex = 0;
                        break;
                    case "10586":
                        UpdateList.Items.Add("14393");
                        UpdateList.SelectedIndex = 0;
                        break;
                    case "14393":
                        UpdateList.Items.Add("15063");
                        UpdateList.Items.Add("15254");
                        UpdateList.SelectedIndex = 0;

                        break;
                    case "15063":
                        UpdateList.Items.Add("15254");
                        UpdateList.SelectedIndex = 0;

                        break;
                    case "15254":
                        UpdateList.IsEnabled = false;
                        ApplyUpdBtn.IsEnabled = false;
                        UpdateappOutput.Text = "No updates available";
                        break;
                    case "16212":
                        UpdateList.IsEnabled = false;
                        ApplyUpdBtn.IsEnabled = false;
                        UpdateappOutput.Text = "No updates available";
                        break;
                    default:
                        UpdateList.Items.Add("15254");
                        UpdateList.SelectedIndex = 0;
                        break;
                }
            }
        }
        int n = 0;

        private async void ApplyUpdBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadProgressBar.IsIndeterminate = true;

            MessageBox.Show(DownloadProgressBar.IsIndeterminate.ToString());
            //getdulog = new List<string>();

            MessageBox.Show("Downloading files, then the update will start.\nThis will take some time and may appear frozen at times.");
            DownloadProgressBar.IsIndeterminate = false;
            var selectedItem = UpdateList.SelectedItem.ToString();
            if (updateList != null)
            {
                updateList.Clear();
            }
            else
            {
                updateList = new List<string>();
            }
            //MessageBox.Show(selectedItem);
            switch (selectedItem)
            {
                

                case "14393":
                    string[] List_14393 = File.ReadAllLines(@".\Resources\buildlists\14393.1066.txt");

                    foreach (var item in List_14393)
                    {
                        string altfiletype = item.Replace(".cab", "");
                        string filename1 = System.IO.Path.GetFileNameWithoutExtension(altfiletype);
                        Debug.WriteLine(filename1);

                        if (Global.InstalledPackages.ToLower().Contains(filename1.ToLower()))
                        {
                            updateList.Add(item);
                        }

                    }
                    break;
                case "15063":
                    string[] List_15063 = File.ReadAllLines(@".\Resources\buildlists\15063.297.txt");

                    foreach (var item in List_15063)
                    {
                        //Debug.WriteLine(item);
                        string altfiletype = item.Replace(".cab", "");
                        string filename1 = System.IO.Path.GetFileNameWithoutExtension(altfiletype);
                        Debug.WriteLine(filename1);

                        if (Global.InstalledPackages.ToLower().Contains(filename1.ToLower()))
                        {
                            updateList.Add(item);
                        }
                    }
                    break;
                case "15254":
                    string[] List_15254 = File.ReadAllLines(@".\Resources\buildlists\15254.603.txt");

                    foreach (var item in List_15254)
                    {
                        string altfiletype = item.Replace(".cab", "");
                        string filename1 = System.IO.Path.GetFileNameWithoutExtension(altfiletype);
                        Debug.WriteLine(filename1);

                        if (Global.InstalledPackages.ToLower().Contains(filename1.ToLower()))
                        {
                            updateList.Add(item);
                        }
                    }
                    break;
                case "16212":
                    //TODO
                    break;
            }
            UpdateappOutput.Text = "Starting Download of " + updateList.Count + " files\n";
            var sorted = updateList.Distinct();
            try
            {
                foreach (var item in sorted)
                {

                    StartDownload(item, Path.GetFileName(item), sorted.Count());
                    while (downloadInProgress == true)
                    {
                        await Task.Delay(500);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} \n{ex.StackTrace}\n{ex.InnerException}");
            }


        }

        private void Getinstalledpkg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {


            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    UpdateappOutput.Text += $"{e.Data}\n";
                    UpdateOutScroller.ScrollToBottom();
                    if (e.Data.Contains("hr = 0x80070522"))
                    {
                        MessageBox.Show("Please restart FFUTweak as Administrator, \"UpdateApp.exe\" failed to gain appropriate permissions.");
                    }

                }));
                if (!Global.isUpdateInProgress)
                {
                    getdulog += "\n" + e.Data;
                }

            }
            else
            {
                isDUFinished = true;
                Global.isUpdateInProgress = false;


            }
        }

        private void Getinstalledpkg_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    UpdateappOutput.Text += $"{e.Data}\n";
                    UpdateOutScroller.ScrollToBottom();
                    if (e.Data.Contains("hr = 0x80070522"))
                    {
                        MessageBox.Show("Please restart FFUTweak as Administrator, \"UpdateApp.exe\" failed to gain appropriate permissions.");
                    }

                }));
                if (!Global.isUpdateInProgress)
                {
                    getdulog += "\n" + e.Data;
                }


            }
            else
            {
                isDUFinished = true;
                Global.isUpdateInProgress = false;


            }
        }

        private void ApplyCustomBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartDownload(string url, string filename, int count)
        {
            try
            {
                downloadInProgress = true;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    UpdateappOutput.Text += url + "\n";
                }));
                WebClient webClient = new WebClient();
                webClient.DownloadProgressChanged += (s, e) =>
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                     {
                         DownloadProgressBar.Maximum = e.TotalBytesToReceive;
                         DownloadProgressBar.Value = e.BytesReceived;
                         UpdateProgressCount.Text = $"{e.BytesReceived.ToFileSize()}/{e.TotalBytesToReceive.ToFileSize()} | {n}/{count}";
                     }));
                };
                webClient.DownloadFileCompleted += (s, e) =>
                {
                    n++;
                    Dispatcher.BeginInvoke((Action)(() =>
                     {
                         //DownloadProgress.IsEnabled = false;
                         UpdateProgressCount.Text = $"{n}/{count}";

                         UpdateappOutput.Text += "Downloaded: " + filename + "\n";
                         UpdateOutScroller.ScrollToBottom();
                         downloadInProgress = false;

                         if (n == count)
                         {
                             // DownloadProgress.IsIndeterminate = false;
                             //MessageBox.Show("Downloads completed, starting update");
                             FFUUpdate();
                         }
                         // any other code to process the file
                     }));
                };
                Directory.CreateDirectory(@".\Temp\update");
                webClient.DownloadFileTaskAsync(new Uri(url),
                    $".\\Temp\\update\\{filename}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading {url}\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }

        private async void FFUUpdate()
        {
            Global.isUpdateInProgress = true;
            UpdateappOutput.Text = "Starting Update";
            UpdateProgressCount.Text = "";
            Process update = new Process();
            update.StartInfo.FileName = @".\Tools\bin\i386\updateapp.exe";
            update.StartInfo.Arguments = "install \".\\Temp\\update\"";
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

            }
            if (UpdateappOutput.Text.Contains("install completed successfully"))
            {
                MessageBox.Show("Update successful, don't forget to save as a new FFU file");
                var selectedItem = UpdateList.SelectedItem.ToString();
                Global.FFUBuildVersion = selectedItem;
            }


        }


    }
}
