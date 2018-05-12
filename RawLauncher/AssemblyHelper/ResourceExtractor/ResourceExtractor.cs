using System.Collections.Generic;
using System.IO;
using System.Windows;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Hash;

namespace RawLauncher.Framework.AssemblyHelper.ResourceExtractor
{
    public class ResourceExtractor
    {
        private string Assembly { get; }

        public string ResourcePath { get;}

        public ResourceExtractor()
        {
            Assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            ResourcePath = "";
        }

        /// <summary>
        /// Allows to specify a special resource Path
        /// </summary>
        /// <param name="resourcePath">Do not enter a "." at last character</param>
        public ResourceExtractor(string resourcePath)
        {
            Assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            ResourcePath = resourcePath + @".";
        }

        public void ExtractFilesIfRequired(string directory, string file)
        {
            ExtractFilesIfRequired(directory, new []{ file });
        }


        /// <summary>
        /// Extracts internal resources out of the assembly into a directory if the files are not already in it.
        /// Creates Directory if it does not exists.
        /// </summary>
        /// <param name="directory">Target directory</param>
        /// <param name="files">Set of Files </param>
        public void ExtractFilesIfRequired(string directory, IEnumerable<string> files)
        {
            if (string.IsNullOrEmpty(Assembly))
                throw new ResourceExtractorException("Assemby not setted: '" + Assembly + "'");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                MessageBox.Show("File extraction error. Please call the developers");
            }

            foreach (var file in files)
            {
                var filePath = Path.Combine(directory, file);
                if (File.Exists(filePath))
                {
                    var hp = new HashProvider();
                    if (!hp.GetFileHash(filePath).Equals(Config.CurrentMafHash))
                        InternalExtractFile(directory, file);
                }
                else
                    InternalExtractFile(directory, file);
            }             
        }

        private void InternalExtractFile(string directory, string file)
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly + @"." + ResourcePath + file))
            {
                if (stream == null || stream == Stream.Null)
                    throw new ResourceExtractorException("ExceptionResourceExtractorNotFound");     

                using (var fileStream = new FileStream(Path.Combine(directory, file), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
    }
}
