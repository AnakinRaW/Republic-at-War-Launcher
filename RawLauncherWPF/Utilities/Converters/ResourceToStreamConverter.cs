using System.IO;
using System.Reflection;

namespace RawLauncherWPF.Utilities.Converters
{
    public static class ResourceToStreamConverter
    {
        public static Stream Convert(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(assembly.GetName().Name + "." + path);
        }
    }
}
