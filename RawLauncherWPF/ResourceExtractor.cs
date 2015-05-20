using System.Collections.Generic;

namespace RawLauncherWPF
{
    internal class ResourceExtractor
    {
        private string Assembly { get; }


        public ResourceExtractor()
        {
            //TODO: set default assembly
        }

        public ResourceExtractor(string AssemblyName)
        {
            Assembly = AssemblyName;
        }

        /// <summary>
        /// Extracts internal resources out of the assembly into a directory if the files are not already in it
        /// </summary>
        /// <param name="directory">Target directory</param>
        /// <param name="files">Set of Files </param>
        public void ExtractFilesIfRequired(string directory, IEnumerable<string> files)
        {
        }
    }
}
