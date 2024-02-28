using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for FlashApp.xaml
    /// </summary>
    public partial class FlashApp : Window
    {

        bool isFlashing;
        bool isWPIRunning;

        public FlashApp()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isFlashing)
            {
                e.Cancel = true;
            }
        }

        private void FlashButton_Click(object sender, RoutedEventArgs e)
        {
            if (Global.SavedFFUPath.Length > 0)
            {
                if (!File.Exists(Global.SavedFFUPath))
                {
                    MessageBox.Show("Error retrieving saved ffu file");
                    return;
                }
                else if (Global.SavedFFUPath == null)
                {

                    string messageBoxText = "Would you like to open an FFU file?";
                    string caption = "Open FFU";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                    if (result == MessageBoxResult.Yes)
                    {
                        OpenFileDialog openFile = new OpenFileDialog();
                        openFile.Filter = "Full Flash Update (*.ffu)|*.ffu";
                        if (openFile.ShowDialog() == true)
                        {
                            Global.SavedFFUPath = openFile.FileName;

                            FlashFFU(openFile.FileName);
                        }

                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    FlashFFU(Global.SavedFFUPath);
                }
            }
        }

        private void UnlockBtn_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "This will start the unlock process, continue?";
            string caption = "Unlock Bootloader";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("You NEED to hold \"Volume Up\" key after phone reboots, and enter Mass Storage Mode. If you fail to do this you will be in a half unlocked state.");
                UnlockBootloader();
            }
            else
            {
                return;
            }
        }

        private void RelockBtn_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "This will start the relock process, continue?";
            string caption = "Relock Bootloader";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                RelockBootloader();
            }
            else
            {
                return;
            }
        }

        private async void UnlockBootloader()
        {
            isWPIRunning = true;

            Process wpinternals = new Process();
            wpinternals.StartInfo.FileName = @".\Tools\bin\flashtools\WPinternals.exe";
            wpinternals.StartInfo.Arguments = "-EnableTestSigning";
            wpinternals.StartInfo.UseShellExecute = false;
            wpinternals.StartInfo.CreateNoWindow = true;
            wpinternals.OutputDataReceived += Wpinternals_OutputDataReceived;
            wpinternals.ErrorDataReceived += Wpinternals_ErrorDataReceived;

            wpinternals.Start();
            while (isWPIRunning)
            {
                await Task.Delay(500);
            }


            if (FlashInfoBox.Text.Contains("Custom flash succeeded!"))
            {
                isWPIRunning = true;
                MessageBox.Show("Part 1 complete, please hold \"Volume Up\" and enter Mass Strage Mode. Press OK when it is connected");
                string messageBoxText = "This will start the unlock process, continue?";
                string caption = "Unlock Bootloader";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Question;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);

                if (result == MessageBoxResult.OK)
                {

                    Process wpinternals1 = new Process();
                    wpinternals1.StartInfo.FileName = @".\Tools\bin\flashtools\WPinternals.exe";
                    wpinternals1.StartInfo.Arguments = "-test";
                    wpinternals1.StartInfo.UseShellExecute = false;
                    wpinternals1.StartInfo.CreateNoWindow = true;
                    wpinternals1.OutputDataReceived += Wpinternals_OutputDataReceived;
                    wpinternals1.ErrorDataReceived += Wpinternals_ErrorDataReceived;

                    wpinternals1.Start();

                    while (isWPIRunning)
                    {
                        await Task.Delay(500);
                    }

                }
            }
            else
            {
                MessageBox.Show("Issue occured while Enabling Test Signing");
            }
        }

        private void Wpinternals_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FlashInfoBox.Text += $"{e.Data}\n";
                }));
            }
            else
            {
                isWPIRunning = false;
            }
        }

        private void Wpinternals_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FlashInfoBox.Text += $"{e.Data}\n";
                }));
            }
            else
            {
                isWPIRunning = false;
            }


        }

        private async void RelockBootloader()
        {
            isWPIRunning = true;
            Process wpinternals = new Process();
            wpinternals.StartInfo.FileName = @".\Tools\bin\flashtools\WPinternals.exe";
            wpinternals.StartInfo.Arguments = "-RelockPhone";
            wpinternals.StartInfo.UseShellExecute = false;
            wpinternals.StartInfo.CreateNoWindow = true;
            wpinternals.OutputDataReceived += Wpinternals_OutputDataReceived;
            wpinternals.ErrorDataReceived += Wpinternals_ErrorDataReceived;

            wpinternals.Start();
            while (isWPIRunning)
            {
                await Task.Delay(500);
            }



        }

        private async void FlashFFU(string path)
        {
            isFlashing = true;
            Debug.WriteLine(@".\Tools\bin\flashtools\thor2.exe " + $"-mode uefiflash -ffufile \"{Global.SavedFFUPath}\"");
            Process thor2 = new Process();
            thor2.StartInfo.FileName = @".\Tools\bin\flashtools\thor2.exe";
            thor2.StartInfo.Arguments = $"-mode uefiflash -ffufile \"{Global.SavedFFUPath}\"";
            thor2.StartInfo.UseShellExecute = false;
            thor2.StartInfo.CreateNoWindow = true;
            thor2.OutputDataReceived += Thor2_OutputDataReceived;
            thor2.ErrorDataReceived += Thor2_ErrorDataReceived;

            thor2.Start();
            while (isFlashing)
            {
                await Task.Delay(500);
            }
        }

        private void Thor2_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FlashInfoBox.Text += $"{e.Data}\n";
                }));
            }
            else
            {
                isFlashing = false;
            }
        }

        private void Thor2_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    FlashInfoBox.Text += $"{e.Data}\n";
                }));
            }
            else
            {
                isFlashing = false;
            }
        }
    }
}
