using System.Collections.Generic;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Server
{
    public interface IVersionServer : IHostServer
    {
        IEnumerable<ModVersion> GetAllVersions();
    }
}
