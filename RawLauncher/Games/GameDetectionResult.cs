namespace RawLauncher.Framework.Games
{
    internal struct GameDetectionResult
    {
        public GameTypes Type { get; set; }

        public string FocPath { get; set; }

        public DetectionError Error { get; set; }

        public bool IsError { get; set; }
    }
}