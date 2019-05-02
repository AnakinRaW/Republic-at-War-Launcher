namespace RawLauncher.Framework.Games
{
    internal struct GameDetectionResult
    {
        public GameType FocType { get; set; }

        public string FocPath { get; set; }

        public string EawPath { get; set; }

        public DetectionError Error { get; set; }

        public bool IsError { get; set; }

        public override string ToString()
        {
            return $"HasError: {IsError}; Error: {Error}; EaW Path: {EawPath}; FoC Path:{FocPath}; FoC Type: {FocType}";
        }
    }
}