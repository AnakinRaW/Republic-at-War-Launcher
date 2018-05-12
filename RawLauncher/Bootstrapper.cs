namespace RawLauncher.Framework
{
    internal class Bootstrapper : ModernApplicationFramework.Basics.Services.Bootstrapper
    {
        public Bootstrapper(bool useApplication) : base(useApplication)
        {
            Initialize();
        }
    }
}
