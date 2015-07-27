using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Schema;

namespace RawLauncherWPF.Xml
{
    public class XmlValidator
    {
        private int _errors;

        public XmlValidator(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(nameof(file));

            FileStream = new FileStream(file, FileMode.Open);
        }

        public XmlValidator(Stream fileStream)
        {
            FileStream = fileStream;
        }

        private Stream FileStream { get; }

        public bool Validate(string fileName)
        {
            bool result;
            try
            {
                var settings = new XmlReaderSettings {ValidationType = ValidationType.Schema};
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation |
                                            XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += Settings_ValidationEventHandler;
                if (FileStream != null)
                    using (var schemaReader = XmlReader.Create(FileStream))
                    {
                        settings.Schemas.Add(null, schemaReader);
                    }

                var reader = XmlReader.Create(fileName, settings);
                while (reader.Read())
                {
                }
                reader.Close();
                if (_errors > 0)
                    throw new Exception();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        private void Settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            _errors++;
            //TODO: Remove this in final
            MessageBox.Show(e.Exception.Message + "\r\n");
        }
    }
}