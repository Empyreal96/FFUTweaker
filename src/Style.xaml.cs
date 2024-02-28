using Microsoft.Win32;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
using System.Windows.Shapes;

namespace FFUTweak
{
    /// <summary>
    /// Interaction logic for Style.xaml
    /// </summary>
    public partial class Style : System.Windows.Controls.Page
    {
        public string UserSelectedFile;
        public string UserSelectedFileEmj;
        public string UserSelectedFileMdl;


        public Style()
        {
            InitializeComponent();
            FontsLine.Stroke = Global.accentColor;
            EmojiLine.Stroke = Global.accentColor;
            MDLLine.Stroke = Global.accentColor;
            FontsComboBox.SelectionChanged += FontsComboBox_SelectionChanged;
            MdlComboBox.SelectionChanged += MdlComboBox_SelectionChanged;
            EmojiComboBox.SelectionChanged += EmojiComboBox_SelectionChanged;
        }



        private void FontsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string RootPath = Environment.CurrentDirectory + @"\Resources\System Fonts\";
            switch (FontsComboBox.SelectedIndex)
            {
                case 0:
                    //Default
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/Font/#Segoe UI")));
                    UserSelectedFile = Environment.CurrentDirectory + "\\Resources\\Defaults\\Font\\segoeui.ttf";
                    break;
                case 1:
                    //Blue
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Colored Fonts/Blue/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Colored Fonts\\Blue\\segoeui.ttf";

                    break;
                case 2:
                    //Cyan
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Colored Fonts/Cyan/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Colored Fonts\\Blue\\segoeui.ttf";

                    break;
                case 3:
                    //Green
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Colored Fonts/Green/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Colored Fonts\\Green\\segoeui.ttf";

                    break;
                case 4:
                    //Light Green
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Colored Fonts/Light Green/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Colored Fonts\\Light Green\\segoeui.ttf";

                    break;
                case 5:
                    //Magenta
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Colored Fonts/Magenta (Dark Purple)/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Colored Fonts\\Magenta (Dark Purple)\\segoeui.ttf";

                    break;
                case 6:
                    //Orange
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Colored Fonts/Orange/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Colored Fonts\\Orange\\segoeui.ttf";

                    break;
                case 7:
                    //Red
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Colored Fonts/Red/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Colored Fonts\\Red\\segoeui.ttf";

                    break;
                case 8:
                    //7Love
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/7Love/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\7Love\\segoeui.ttf";

                    break;
                case 9:
                    //Art
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Art/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Art\\segoeui.ttf";

                    break;
                case 10:
                    //Bailey
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Bailey/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Bailey\\segoeui.ttf";

                    break;
                case 11:
                    //Cake
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Cake Segoe/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Cake Segoe\\segoeui.ttf";

                    break;
                case 12:
                    //Catholic
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Catholic School/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Catholic School\\segoeui.ttf";

                    break;
                case 13:
                    //Cupid
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Cupid/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Cupid\\segoeui.ttf";

                    break;
                case 14:
                    //Digital
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Digital/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Digital\\segoeui.ttf";

                    break;
                case 15:
                    //DIN Alternative
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/DIN Alternate/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\DIN Alternate\\segoeui.ttf";

                    break;
                case 16:
                    //Edgy
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Edgy/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Edgy\\segoeui.ttf";

                    break;
                case 17:
                    //Helvetica
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Helvetica Segoe/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Helvetica Segoe\\segoeui.ttf";

                    break;
                case 18:
                    //Kids
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Kids/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Kids\\segoeui.ttf";

                    break;
                case 19:
                    //Mvboli
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Mvboli/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Mvboli\\segoeui.ttf";

                    break;
                case 20:
                    //Oksana
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Oksana/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Oksana\\segoeui.ttf";

                    break;
                case 21:
                    //Oswald Light
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Oswald Light/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Oswald Light\\segoeui.ttf";

                    break;
                case 22:
                    //Rio
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Rio Segoe/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Rio Segoe\\segoeui.ttf";

                    break;
                case 23:
                    //Rix Love Fool
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Rix Love Fool/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Rix Love Fool\\segoeui.ttf";

                    break;
                case 24:
                    //San Francisco
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/San Francisco/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\San Francisco\\segoeui.ttf";

                    break;
                case 25:
                    //SFiOS9
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/SFiOS9/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\SFiOS9\\segoeui.ttf";

                    break;
                case 26:
                    //SketchFlow
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/SketchFlow/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\SketchFlow\\segoeui.ttf";

                    break;
                case 27:
                    //Tampus Sans
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Tampus Sans ITC/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Tampus Sans ITC\\segoeui.ttf";

                    break;
                case 28:
                    //Ubuntu Light
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/UBUNTU Light/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\UBUNTU Light\\segoeui.ttf";

                    break;
                case 29:
                    //Willow
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Custom Font/Willow/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Custom Font\\Willow\\segoeui.ttf";

                    break;
                case 30:
                    //Windows 10 Updated
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + RootPath + "/Windows 10 Updated/#Segoe UI")));
                    UserSelectedFile = RootPath + "\\Windows 10 Updated\\segoeui.ttf";

                    break;
            }
        }

        private void LoadFontFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "True Type Font (*.ttf)|*.ttf";
                if (openFile.ShowDialog() == true)
                {
                    UserSelectedFile = openFile.FileName;
                    var path = System.IO.Path.GetDirectoryName(UserSelectedFile);

                    string fontName = GetFontName(UserSelectedFile);
                    UserFontName.Text = "Loaded: " + fontName;
                    Dispatcher.BeginInvoke(new Action(() => FontPreview.FontFamily = new FontFamily("file:///" + path.Replace("\\", "/") + "/#" + fontName)));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }

        public static string GetFontName(string filePath)
        {
            using (PrivateFontCollection fontCollection = new PrivateFontCollection())
            {
                fontCollection.AddFontFile(filePath);
                if (fontCollection.Families.Length > 0)
                {
                    return fontCollection.Families[0].Name;
                }
                else
                {
                    throw new Exception("No font found in the specified file.");
                }
            }
        }


        private void ApplyFontBtn_Click(object sender, RoutedEventArgs e)
        {

            File.Copy(UserSelectedFile, $"{Global.MountedDirectory}\\Windows\\Fonts\\segoeui.ttf", true);
        }


        private void EmojiComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string RootPath = Environment.CurrentDirectory + @"\Resources\System Emojis\";
            switch (EmojiComboBox.SelectedIndex)
            {
                case 0:
                    //Default
                    //  Dispatcher.BeginInvoke(new Action(() => EmojiPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/emoji/#Segoe UI Emoji")));
                    UserSelectedFileEmj = Environment.CurrentDirectory + "\\Resources\\Defaults\\emoji\\seguiemj.ttf";

                    break;
                case 1:
                    //Dispatcher.BeginInvoke(new Action(() => EmojiPreview.FontFamily = new FontFamily("file:///" + RootPath + "/W10 Updated Emoji/#Segoe UI Emoji")));
                    UserSelectedFileEmj = RootPath + "\\W10 Updated Emoji\\seguiemj.ttf";

                    break;
                case 2:
                    //Dispatcher.BeginInvoke(new Action(() => EmojiPreview.FontFamily = new FontFamily("file:///" + RootPath + "/W10M 1511 Emoji/#Segoe UI Emoji")));
                    UserSelectedFileEmj = RootPath + "\\W10M 1511 Emoji\\seguiemj.ttf";


                    break;
                case 3:
                    //Dispatcher.BeginInvoke(new Action(() => EmojiPreview.FontFamily = new FontFamily("file:///" + RootPath + "/W10M 1607 Emoji/#Segoe UI Emoji")));
                    UserSelectedFileEmj = RootPath + "\\W10M 1607 Emoji\\seguiemj.ttf";


                    break;
                case 4:
                    //Dispatcher.BeginInvoke(new Action(() => EmojiPreview.FontFamily = new FontFamily("file:///" + RootPath + "/W10M 1709 Emoji/#Segoe UI Emoji")));
                    UserSelectedFileEmj = RootPath + "\\W10M 1709 Emoji\\seguiemj.ttf";


                    break;
                case 5:
                    //Dispatcher.BeginInvoke(new Action(() => EmojiPreview.FontFamily = new FontFamily("file:///" + RootPath + "/W11 Fulent Emoji/#Segoe UI Emoji")));
                    UserSelectedFileEmj = RootPath + "\\W11 Fluent Emoji\\seguiemj.ttf";


                    break;
                case 6:
                    //Dispatcher.BeginInvoke(new Action(() => EmojiPreview.FontFamily = new FontFamily("file:///" + RootPath + "/W8.1 B&W Emoji/#Segoe UI Emoji")));
                    UserSelectedFileEmj = RootPath + "\\W8.1 B&W Emoji\\seguiemj.ttf";


                    break;

            }
        }

        private void LoadEmojiFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ApplyEmojiBtn_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(UserSelectedFileEmj, $"{Global.MountedDirectory}\\Windows\\Fonts\\seguiemj.ttf", true);
        }


        public class FontBindings
        {
            public FontFamily mdlFontFamily { get; set; }
        }
        private void MdlComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontIconsPanel.Children.Count != 0)
            {
                fontIconsPanel.Children.Clear();
            }


            /*FontIcon ficon1 = new FontIcon();
            ficon1.Glyph = "\uE052";
            FontIcon ficon2 = new FontIcon();
            ficon2.Glyph = "\uE105";
            FontIcon ficon3 = new FontIcon();
            ficon3.Glyph = "\uE1E8";
            FontIcon ficon4 = new FontIcon();
            ficon4.Glyph = "\uE2AF";
            FontIcon ficon5 = new FontIcon();
            ficon5.Glyph = "\uE2F6";
            */
            //<TextBlock x:Name="MDLPreview" Text="&#xE012; &#xE105; &#xE125; &#xE1D3; &#xE1E9; &#xE2F6; &#xE008;" Margin="75,5,75,0"/>


            string RootPath = Environment.CurrentDirectory + @"\Resources\System Icons\";
            switch (MdlComboBox.SelectedIndex)
            {
                case 0:
                    //Default
                    // Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets")));

                    /*       ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets");
                           ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets");
                           ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets");
                           ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets");
                           ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/Defaults/mdl2/#Segoe MDL2 Assets");

                           fontIconsPanel.Children.Add(ficon1);
                           fontIconsPanel.Children.Add(ficon2);
                           fontIconsPanel.Children.Add(ficon3);
                           fontIconsPanel.Children.Add(ficon4);
                           fontIconsPanel.Children.Add(ficon5); */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\Defaults\\mdl2\\segmdl2.ttf";

                    break;
                case 1:
                    //Android
                    // Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets")));

                    /*    ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets");
                        ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets");
                        ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets");
                        ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets");
                        ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Android/#Segoe MDL2 Assets");

                        fontIconsPanel.Children.Add(ficon1);
                        fontIconsPanel.Children.Add(ficon2);
                        fontIconsPanel.Children.Add(ficon3);
                        fontIconsPanel.Children.Add(ficon4);
                        fontIconsPanel.Children.Add(ficon5);  */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Android\\segmdl2.ttf";

                    break;
                case 2:
                    //Circular Battery
                    // Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets")));

                    /*       ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets");
                           ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets");
                           ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets");
                           ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets");
                           ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Circular battery/#Segoe MDL2 Assets");

                           fontIconsPanel.Children.Add(ficon1);
                           fontIconsPanel.Children.Add(ficon2);
                           fontIconsPanel.Children.Add(ficon3);
                           fontIconsPanel.Children.Add(ficon4);
                           fontIconsPanel.Children.Add(ficon5); */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Circular battery\\segmdl2.ttf";

                    break;
                case 3:
                    //Full Color
                    // Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets")));

                    /*        ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets");
                            ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets");
                            ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets");
                            ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets");
                            ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Full Color/#Segoe MDL2 Assets");

                            fontIconsPanel.Children.Add(ficon1);
                            fontIconsPanel.Children.Add(ficon2);
                            fontIconsPanel.Children.Add(ficon3);
                            fontIconsPanel.Children.Add(ficon4);
                            fontIconsPanel.Children.Add(ficon5);  */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Full Color\\segmdl2.ttf";

                    break;
                case 4:
                    //Half Colored
                    // Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets")));

                    /*     ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets");
                         ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets");
                         ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets");
                         ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets");
                         ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Half Colored/#Segoe MDL2 Assets");

                         fontIconsPanel.Children.Add(ficon1);
                         fontIconsPanel.Children.Add(ficon2);
                         fontIconsPanel.Children.Add(ficon3);
                         fontIconsPanel.Children.Add(ficon4);
                         fontIconsPanel.Children.Add(ficon5);  */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Half Colored\\segmdl2.ttf";

                    break;
                case 5:
                    //iOS
                    //Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets")));

                    /*     ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets");
                         ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets");
                         ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets");
                         ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets");
                         ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/IOS/#Segoe MDL2 Assets");

                         fontIconsPanel.Children.Add(ficon1);
                         fontIconsPanel.Children.Add(ficon2);
                         fontIconsPanel.Children.Add(ficon3);
                         fontIconsPanel.Children.Add(ficon4);
                         fontIconsPanel.Children.Add(ficon5);   */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\IOS\\segmdl2.ttf";

                    break;
                case 6:
                    //Segmdl2 Colored
                    //Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets")));

                    /*        ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets");
                            ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets");
                            ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets");
                            ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets");
                            ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Segmdl2 Colored/#Segoe MDL2 Assets");

                            fontIconsPanel.Children.Add(ficon1);
                            fontIconsPanel.Children.Add(ficon2);
                            fontIconsPanel.Children.Add(ficon3);
                            fontIconsPanel.Children.Add(ficon4);
                            fontIconsPanel.Children.Add(ficon5);   */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Segmdl2 Colored\\segmdl2.ttf";

                    break;
                case 7:
                    //Violet & Colored
                    // Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets")));

                    /*     ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets");
                         ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets");
                         ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets");
                         ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets");
                         ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Violet & FullColors/#Segoe MDL2 Assets");

                         fontIconsPanel.Children.Add(ficon1);
                         fontIconsPanel.Children.Add(ficon2);
                         fontIconsPanel.Children.Add(ficon3);
                         fontIconsPanel.Children.Add(ficon4);
                         fontIconsPanel.Children.Add(ficon5);   */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Violet & FullColors\\segmdl2.ttf";

                    break;
                case 8:
                    //10X
                    // Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets")));

                    /*         ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets");
                             ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets");
                             ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets");
                             ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets");
                             ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 10X Icons/#Segoe MDL2 Assets");

                             fontIconsPanel.Children.Add(ficon1);
                             fontIconsPanel.Children.Add(ficon2);
                             fontIconsPanel.Children.Add(ficon3);
                             fontIconsPanel.Children.Add(ficon4);
                             fontIconsPanel.Children.Add(ficon5);   */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Windows 10X Icons\\segmdl2.ttf";

                    break;
                case 9:
                    //11
                    //Dispatcher.BeginInvoke(new Action(() => iconOne.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconTwo.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconThree.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconFour.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets")));
                    //Dispatcher.BeginInvoke(new Action(() => iconFive.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets")));
                    // Dispatcher.BeginInvoke(new Action(() => MDLPreview.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets")));

                    /*       ficon1.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets");
                           ficon2.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets");
                           ficon3.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets");
                           ficon4.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets");
                           ficon5.FontFamily = new FontFamily("file:///" + Environment.CurrentDirectory + "/Resources/System Icons/Windows 11 Icons/#Segoe MDL2 Assets");

                           fontIconsPanel.Children.Add(ficon1);
                           fontIconsPanel.Children.Add(ficon2);
                           fontIconsPanel.Children.Add(ficon3);
                           fontIconsPanel.Children.Add(ficon4);
                           fontIconsPanel.Children.Add(ficon5); */
                    UserSelectedFileMdl = Environment.CurrentDirectory + "\\Resources\\System Icons\\Windows 11 Icons\\segmdl2.ttf";

                    break;
            }
        }



        private void ApplyMDLBtn_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(UserSelectedFileMdl, $"{Global.MountedDirectory}\\Windows\\Fonts\\segmdl2.ttf", true);

        }



        private void LoadMDLFile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
