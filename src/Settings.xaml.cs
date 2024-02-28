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
using System.Windows.Navigation;

namespace FFUTweak
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
        }

        string ppkgFile;
        string answerFile;
        private void LoadPPKGBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Provisioning Package (*.ppkg)|*.ppkg";
            
            if (openFile.ShowDialog() == true)
            {
                ppkgFile = openFile.FileName;
                var fileName = Path.GetFileNameWithoutExtension(ppkgFile);
                Process extract = new Process();
                extract.StartInfo.FileName = @".\Tools\7z\7z.exe";
                extract.StartInfo.Arguments = $"x {ppkgFile} -o.\\Temp\\extract\\{fileName}";
                extract.StartInfo.UseShellExecute = false;
                extract.StartInfo.CreateNoWindow = true;

                extract.Start();
                extract.WaitForExit();

                if (File.Exists($".\\Temp\\extract\\{fileName}\\CommonSettings\\1\\AnswerFile.xml"))
                {
                    answerFile = $".\\Temp\\extract\\{fileName}\\CommonSettings\\1\\AnswerFile.xml";
                    PPKGInfo.Text = $"{fileName}";
                    ShowAnswerFileBtn.IsEnabled = true;
                 //  var results =  ppkgparser.ReadAnswerFile($".\\Temp\\extract\\{fileName}\\CommonSettings\\1\\AnswerFile.xml");
                }
            }
        }

        private void ShowAnswerFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Process preview = new Process();
            preview.StartInfo.FileName = "notepad.exe";
            preview.StartInfo.Arguments = answerFile;

            preview.Start();
        }

        private void DefaultThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (DefaultThemeCombo.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }

        private void DefaultAccentCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (DefaultAccentCombo.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    break;
                case 17:
                    break;
                case 18:
                    break;
                case 19:
                    break;
            }
        }
    }
}
