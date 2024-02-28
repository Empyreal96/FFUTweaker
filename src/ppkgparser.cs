///
/// This class is unfinished and not used at this time, it is safe to remove from project
///
///
///
/// Empyreal96 (2023)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FFUTweak
{
    class ppkgparser
    {

        public static string ReadAnswerFile(string xmlpath)
        {
            var XML = File.ReadAllText(xmlpath);
            XmlReader reader = XmlReader.Create(new StringReader(XML));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    Debug.WriteLine(reader.Name);
                    
                }
                if (reader.NodeType == XmlNodeType.Text)
                {
                    Debug.WriteLine(reader.Value);
                }
            }

            return null;
        }
    }
}
