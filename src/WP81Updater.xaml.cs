using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FFUTweak
{
    /// <summary>
    /// Interaction logic for WP81Updater.xaml
    /// </summary>
    public partial class WP81Updater : Window
    {

        string FFUPath;
        bool isUpdating;
        public WP81Updater()
        {
            InitializeComponent();
        }

        private void LoadFFU_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Full Flash Update (*.ffu)|*.ffu";
            if (openFile.ShowDialog() == true)
            {
                FFUPath = openFile.FileName;

                StartProcess.IsEnabled = true;
                FFUOutputText.Text = $"{FFUPath}\n\n";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private async void StartProcess_Click(object sender, RoutedEventArgs e)
        {
            isUpdating = true;
            // convert to cbs // 10586 tools
            Process convert = new Process();
            convert.StartInfo.FileName = @".\Tools\bin\i386\WP8Tools\updateapp.exe";
            convert.StartInfo.Arguments = "converttocbs MainOS Data EFIESP UpdateOS";
            convert.StartInfo.UseShellExecute = false;
            convert.StartInfo.CreateNoWindow = true;
            convert.OutputDataReceived += UpdateApp_OutputDataReceived;
            convert.ErrorDataReceived += UpdateApp_ErrorDataReceived;

            convert.Start();
            convert.BeginOutputReadLine();
            convert.BeginErrorReadLine();
            while (isUpdating)
            {
                await Task.Delay(500);
            }
            // get installed packages

            // download packages

            // install

            // cleanup

            //postcommitfixup // 10586 tools

            //save ffu
        }

        private void UpdateApp_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FFUOutputText.Text += $"{e.Data}\n";
                }));
            }
        }

        private void UpdateApp_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FFUOutputText.Text += $"{e.Data}\n";
                }));
            } else
            {
                isUpdating = false;
            }
        }
    }
}
