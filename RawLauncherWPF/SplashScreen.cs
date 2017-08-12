using System.Drawing;
using System.Windows.Forms;

namespace RawLauncherWPF
{
	public sealed class SplashScreen : Form
	{
		static SplashScreen _splashScreen;
		Bitmap _bitmap;


		public static SplashScreen SplashScreenForm
		{
			get => _splashScreen;
			set => _splashScreen = value;
		}


		public SplashScreen()
		{
            FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.CenterScreen;
			ShowInTaskbar = false;
		    var r = typeof(SplashScreen).Assembly.GetManifestResourceStream("RawLauncherWPF.RaW_Logo.png");
            _bitmap = new Bitmap(r);
			ClientSize = _bitmap.Size;
			BackgroundImage = _bitmap;
		}

		public static void ShowSplashScreen()
		{
			_splashScreen = new SplashScreen();
			_splashScreen.Show();
		}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
