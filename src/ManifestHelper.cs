using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFUTweak.ManifestHelper
{
    class ManifestHelper
    {
        static string ICDOutput;
        static bool ICDRunning;
        private async static Task<string> MakePPKG(string appPath, string Dependencies)
        {
            Process ICD = new Process();
            ICD.StartInfo.FileName = @".\Tools\bin\i386\ICD.exe";
            ICD.StartInfo.Arguments = $"ICD.exe /Build-ProvisioningPackage  /CustomizationXML: /PackagePath:x ";
            ICD.StartInfo.UseShellExecute = false;
            ICD.OutputDataReceived += ICD_OutputDataReceived;
            ICD.ErrorDataReceived += ICD_ErrorDataReceived;

            ICD.Start();
            ICDRunning = true;
            ICD.BeginOutputReadLine();
            ICD.BeginErrorReadLine();

            while (ICDRunning)
            {
                await Task.Delay(500);
            }

            string[] ICDOutArray = ICDOutput.Split("\n");
            string ppkgPath = null;
            foreach (var res in ICDOutArray)
            {
                if (ICDOutput.Contains(""))
                {
                    ppkgPath = res;
                }
            }

            return ppkgPath;


        }

        /// <summary>
        /// dependencies string needs to be in this format: 
        /// "Microsoft.Net.Native.Framework.2.2">Path\To\Appx
        /// </summary>
        /// <param name="appx"></param>
        /// <param name="dependencies"></param>
        /// <param name="pkgName"></param>
        /// <param name="PFN"></param>
        /// <param name="version"></param>
        /// <param name="ownerType"></param>
        /// <returns></returns>
        private async static Task<string> CreateCustomizationXML(string appx, string[] dependencies, string pkgName, string PFN, string version, string ownerType)
        {
            string XML;
            Guid guid = Guid.NewGuid();
            string guidstring = "{" + guid.ToString() + "}";

            XML =
                "<?xml version=\"1.0\" encoding=\"utf - 8\"?>\n" +
                "<WindowsCustomizations>\n" +
                "  <PackageConfig xmlns=\"urn:schemas-Microsoft-com:Windows-ICD-Package-Config.v1.0\">\n" +
                $"    <ID>{guidstring}</ID>\n" +
                $"    <Name>{pkgName}</Name>\n" +
                $"    <Version>{version}</Version>\n" +
                $"    <OwnerType>{ownerType}</OwnerType>\n" +
                $"    <Rank>0</Rank>\n" +
                $"  </PackageConfig>\n" +
                $"  <Settings xmlns=\"urn:schemas-microsoft-com:windows-provisioning\">\n" +
                $"    <Customizations>\n" +
                $"      <Common>\n" +
                $"        <UniversalAppInstall>\n" +
                $"          <UserContextApp>\n" +
                $"            <Application PackageFamilyName=\"{PFN}\" Name=\"{PFN}\">\n" +
                $"              <ApplicationFile>{appx}</ApplicationFile>\n" +
                $"              <DependencyAppxFiles>\n";

            foreach (var deps in dependencies)
            {
                XML += $"                <Dependency Name={deps}</Dependency>\n";
            }
            XML +=
                "              </DependencyAppxFiles>\n" +
                "            </Application>\n" +
                "          </UserContextApp>\n" +
                "        </UniversalAppInstall>\n" +
                "      </Common>\n" +
                "    </Customizations>\n" +
                "  </Settings>\n" +
                "</WindowsCustomizations>";

            string outPath = ".\\ppkg\\" + pkgName + $"\\{pkgName}.xml";

            File.WriteAllText(outPath, XML);
            return outPath;
        }
       

        public static void AppProvisionXML(string ppkgPath, string outPath, string owner, string SubComponent)
        {
            string XML =
                    "<?xml version=\"1.0\" encoding=\"utf - 8\"?>\n" +
                    "<Package xmlns=\"urn: Microsoft.WindowsPhone / PackageSchema.v8.00\" \n" +
                    $"   Owner=\"{owner}\" \n" +
                    $"   OwnerType=\"OEM\" \n" +
                    $"   ReleaseType=\"Production\" \n" +
                    $"   Platform=\"Generic\"\n" +
                    $"   Component=\"MainOS\"\n" +
                    $"   SubComponent=\"{SubComponent}\"> \n" +
                    $"   <Components> \n" +
                    $"      <OSComponent> \n" +
                    $"         <Files>\n" +
                    $"			<File Source=\"{ppkgPath}\"\n" +
                    $"				  DestinationDir=\"$(runtime.windows)\\provisioning\\packages\" />\n" +
                    $"		  </Files>\n" +
                    $"      </OSComponent>\n" +
                    $"   </Components>\n" +
                    $"</Package> ";

        }

        private static void ICD_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                ICDOutput += $"{e.Data}\n";
            }
            else
            {
                ICDRunning = false;
            }
        }

        private static void ICD_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                ICDOutput += $"{e.Data}\n";

            }
            else
            {
                ICDRunning = false;
            }
        }
    }
}
