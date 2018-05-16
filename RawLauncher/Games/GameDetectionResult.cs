namespace RawLauncher.Framework.Games
{
    internal struct GameDetectionResult
    {
        public GameTypes FocType { get; set; }

        public string FocPath { get; set; }

        public string EawPath { get; set; }

        public DetectionError Error { get; set; }

        public bool IsError { get; set; }
    }
}