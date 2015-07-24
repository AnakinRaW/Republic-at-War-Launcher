using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RawLauncherWPF.Xml
{
    /// <remarks/>
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class RequiredCheckFiles
    {
        /// <remarks/>
        public string Version { get; set; }

        /// <remarks/>
        [XmlElement("File")]
        public List<RequiredCheckFilesFile> Files;
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    public class RequiredCheckFilesFile
    {
        public FileContentType FileContentType { get; set; }

        /// <remarks/>
        public string Target { get; set; }

        /// <remarks/>
        public string TargetPath { get; set; }

        /// <remarks/>
        public string Path { get; set; }
    }

    public enum FileContentType
    {
        [XmlEnum("")] None,
        [XmlEnum("Hash")] Hash,
        [XmlEnum("Count")] Count,
    }
}