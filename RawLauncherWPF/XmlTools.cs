using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RawLauncherWPF
{
    public static class XmlTools
    {
        public static string GetNodeValue(string filePath, string nodePath)
        {
            var xml = new XmlDocument();
            xml.Load(filePath);
            var selectSingleNode = xml.SelectSingleNode(nodePath);
            return selectSingleNode?.InnerText;
        }
    }
}
