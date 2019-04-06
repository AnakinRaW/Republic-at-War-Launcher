using System.Collections.Generic;

namespace RawLauncher.Framework.Versioning
{
    public interface IVersionComparer : IEqualityComparer<SemanticVersion>, IComparer<SemanticVersion>
    {
    }
}