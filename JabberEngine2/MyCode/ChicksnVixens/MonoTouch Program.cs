using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ChicksnVixens
{
	[Register("AppDelegate")]
	class Program : ExEnEmTouchApplication
	{
		public override void FinishedLaunching(UIApplication application)
		{
			var bounds = UIScreen.MainScreen.Bounds;
			Point size = new Point((int)bounds.Width, (int)bounds.Height);

			game = new ChicksnVixensGame();
			game.Run();
		}

		static void Main(string[] args)
		{
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}

