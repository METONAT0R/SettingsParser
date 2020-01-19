using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MeTonaTOR.NFSW {
    class SettingsParser {
        XmlDocument userSettingsXml = new XmlDocument();

        #if DEBUG
            String file = Path.Combine(Environment.CurrentDirectory, "UserSettings.xml");
        #else
            String file = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "Need for Speed World", "Settings", "UserSettings.xml");
        #endif

        XmlNode check = null;

        public SettingsParser() {
            if (File.Exists(file)) {
                try {
                    userSettingsXml.Load(file);
                    check = userSettingsXml.SelectSingleNode("Settings");
                } catch(Exception ex) {
                    Console.WriteLine(ex);
                    this.UseDefault();
                    userSettingsXml.Load(file);
                    check = userSettingsXml.SelectSingleNode("Settings");
                }
            } else {
                this.UseDefault();
                userSettingsXml.Load(file);
                check = userSettingsXml.SelectSingleNode("Settings");
            }
        }

        public string AudioMode {
            get { return this.getNode("Settings/VideoConfig/audiomode"); }
            set { this.setNode("Settings/VideoConfig/audiomode", value, "int"); }
        }
        public string AudioQuality {
            get { return this.getNode("Settings/VideoConfig/audioquality"); }
            set { this.setNode("Settings/VideoConfig/audioquality", value, "int"); }
        }

        # region HELPERS
        private void UseDefault() {
            Console.WriteLine("Failed to parse {0}, deleting.", file);
            File.Delete(file);

            var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
            userSettingsXml.Save(file);
        }

        public string getNode(string xpath) {
            Console.WriteLine("Getting {0} value.", xpath);
            XmlNode node = userSettingsXml.SelectSingleNode(xpath);
            return node.InnerText;
        }

        private void setNode(string xpath, string value, string typeOf) {
            Console.WriteLine("Setting {0} value to {1} as {2}.", xpath, value, typeOf);

            XmlElement contentElement = (XmlElement)this.makeXPath(userSettingsXml, xpath);
            contentElement.SetAttribute("Type", typeOf);
            contentElement.InnerText = value;
            userSettingsXml.Save(file);
        }

        private XmlNode makeXPath(XmlDocument doc, string xpath) {
            return makeXPath(doc, doc as XmlNode, xpath);
        }

        private XmlNode makeXPath(XmlDocument doc, XmlNode parent, string xpath) {
            string[] partsOfXPath = xpath.Trim('/').Split('/');
            string nextNodeInXPath = partsOfXPath.First();
            if (string.IsNullOrEmpty(nextNodeInXPath))
                return parent;

            XmlNode node = parent.SelectSingleNode(nextNodeInXPath);
            if (node == null)
                node = parent.AppendChild(doc.CreateElement(nextNodeInXPath));

            string rest = String.Join("/", partsOfXPath.Skip(1).ToArray());
            return makeXPath(doc, node, rest);
        }

        #endregion
    }
}
