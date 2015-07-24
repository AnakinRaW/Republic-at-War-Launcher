using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace RawLauncherWPF.Xml
{
    public static class XmlToObjectParser
    {
        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T ParseXml<T>(this string @this) where T : class
        {
            var reader = XmlReader.Create(@this.Trim().ToStream(),
                new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Document});

            T instance;
            try
            {
                instance = new XmlSerializer(typeof (T)).Deserialize(reader) as T;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to deserialize the xml stream." , e.InnerException);
            }
            return instance;
        } 
    }
}
