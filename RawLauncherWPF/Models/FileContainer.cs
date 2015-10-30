using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using RawLauncherWPF.Hash;
using RawLauncherWPF.Utilities;

namespace RawLauncherWPF.Models
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class FileContainer
    {
        [XmlElement("Version", Order = 1)]
        public string StringVersion { get; set; }

        [XmlIgnore]
        public Version Version => new Version(StringVersion);

        [XmlElement("File", Order = 2)]
        public List<FileContainerFile> Files;

        [XmlElement("Folder", Order = 3)]
        public List<FileContainerFolder> Folders;

        public List<FileContainerFolder> GetFoldersOfType(TargetType type)
        {
            return Folders.Where(folder => folder.TargetType == type).ToList();
        } 

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class FileContainerFile
    {
        private string _hashField;
        private string _nameField;
        private string _sourcePathField;
        private string _targetPathField;
        private TargetType _targetTypeField;

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Name
        {
            get { return _nameField; }
            set { _nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TargetPath
        {
            get { return _targetPathField; }
            set { _targetPathField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TargetType TargetType
        {
            get { return _targetTypeField; }
            set { _targetTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Hash
        {
            get { return _hashField; }
            set { _hashField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SourcePath
        {
            get { return _sourcePathField; }
            set { _sourcePathField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [Serializable]
    public enum TargetType
    {
        /// <remarks/>
        Mod,

        /// <remarks/>
        Ai,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class FileContainerFolder
    {
        private string _countField;
        private string _hashField;
        private string _targetPathField;
        private TargetType _targetTypeField;

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TargetPath
        {
            get { return _targetPathField; }
            set { _targetPathField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TargetType TargetType
        {
            get { return _targetTypeField; }
            set { _targetTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Hash
        {
            get { return _hashField; }
            set { _hashField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string Count
        {
            get { return _countField; }
            set { _countField = value; }
        }

        //TODO: Throwing Messages should not be done here ....
        public bool Check(string referencePath)
        {
            if (!Directory.Exists(referencePath))
            {
                MessageProvider.Show("Exists Fail: " + referencePath);
                return false;
            }
            if (Directory.GetFiles(referencePath).Length.ToString() != Count)
            {
                MessageProvider.Show("Count Fail: " + referencePath);
                return false;
            }
            var hashProvider = new HashProvider();
            if (hashProvider.GetDirectoryHash(referencePath) != Hash)
            {
                MessageProvider.Show("Hash Fail: " + referencePath);
                return false;
            }
            return true;
        }

        public static List<FileContainerFolder> ListFromExcludeList(List<FileContainerFolder> folderList,
            List<string> excludeList)
        {
            List<FileContainerFolder> list = new List<FileContainerFolder>();
            if (excludeList == null)
                return folderList;
            var subExclude = new List<string>();

            foreach (var folder in folderList)
            {
                var exclude = false;
                foreach (var s in excludeList)
                {
                    if (s.Replace(folder.TargetPath, "") == "")
                        exclude = true;
                    if (s.Replace(folder.TargetPath, "") == "*")
                        subExclude.Add(folder.TargetPath);
                }
                if (!exclude)
                    list.Add(folder);
            }
            foreach (FileContainerFolder folder in from folder in folderList from s in subExclude where folder.TargetPath.Contains(s) select folder)
                list.Remove(folder);
            return list;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class NewDataSet
    {
        private FileContainer[] _itemsField;

        /// <remarks/>
        [XmlElement("FileContainer")]
        public FileContainer[] Items
        {
            get { return _itemsField; }
            set { _itemsField = value; }
        }
    }
}