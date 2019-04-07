namespace RawLauncher.Framework.Screens
{
    public interface IHasProgressBar
    {
        double Progress { get; set; }

        string ProcessStatus { get; set; }
    }
}