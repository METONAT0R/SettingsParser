using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MeTonaTOR {
    class SettingsParser {
        XmlDocument userSettingsXml = new XmlDocument();
        String file = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "Need for Speed World", "Settings", "UserSettings.xml");
        XmlNode check = null;

        public SettingsParser() {
            if (File.Exists(file)) {
                try {
                    userSettingsXml.Load(file);
                    check = userSettingsXml.SelectSingleNode("Settings");
                } catch {
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

        public void setAudioMode(int audioMode) {
            this.setNode("Settings/VideoConfig/audiomode", audioMode.ToString(), "Int");
        }

        public void setAudioQuality(int audioQuality) {
            this.setNode("Settings/VideoConfig/audioquality", audioQuality.ToString(), "Int");
        }

        private void UseDefault() {
            Console.WriteLine("Failed to parse {0}, deleting.", file);
            File.Delete(file);

            var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
            userSettingsXml.Save(file);
        }

        private void setNode(string xpath, string value, string typeOf) {
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
    }
}
