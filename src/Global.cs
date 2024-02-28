using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FFUTweak
{
    class Global
    {
        public static bool isMounted;
        public static bool isProcessFinished;
        public static bool isUpdateInProgress;
        public static string _FFUPath;
        public static string GlobalFFUInfo;
        public static string MountedDirectory = null;
        public static string _PhysicalDrive = null;
        public static Brush accentColor;
        public static string FFUBuildVersion;
        public static string InstalledPackages;
        public static string VHDPath;
        public static string SavedFFUPath = null;


        public static TextBlock img2ffuOutput = new TextBlock();



    }
}
