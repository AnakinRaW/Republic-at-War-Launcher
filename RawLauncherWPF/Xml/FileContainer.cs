using System.Collections.Generic;
using System.Xml.Serialization;

namespace RawLauncherWPF.Xml
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [System.Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class FileContainer
    {
        [XmlElement("Version", Order = 1)]
        public string Version { get; set; }

        [XmlElement("File", Order = 2)]
        public List<FileContainerFile> Files;

        [XmlElement("Folder", Order = 3)]
        public List<FileContainerFolder> Folders;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [System.Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class FileContainerFile
    {
        private FileType _fileContentTypeField;
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
        public FileType FileContentType
        {
            get { return _fileContentTypeField; }
            set { _fileContentTypeField = value; }
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
    [System.Serializable]
    public enum FileType
    {
        /// <remarks/>
        Hash,

        /// <remarks/>
        Count,

        /// <remarks/>
        Delete,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [System.Serializable]
    public enum TargetType
    {
        /// <remarks/>
        Mod,

        /// <remarks/>
        Ai,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [System.Serializable]
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.81.0")]
    [System.Serializable]
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