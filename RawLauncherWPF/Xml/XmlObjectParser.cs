using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RawLauncherWPF.Xml
{
    public class XmlObjectParser<T> where T : class
    {
        public XmlObjectParser(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(nameof(path));
            FileStream = new FileStream(path, FileMode.Open);
        }

        public XmlObjectParser(Stream fileStream)
        {
            FileStream = fileStream;
        }

        private Stream FileStream { get; }

        public T Parse()
        {
            var reader = XmlReader.Create(FileStream,
                new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Document});
            T instance;
            try
            {
                instance = new XmlSerializer(typeof (T)).Deserialize(reader) as T;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to deserialize the xml stream.", e.InnerException);
            }
            finally
            {
                FileStream.Close();
            }
            return instance;
        }
    }
}