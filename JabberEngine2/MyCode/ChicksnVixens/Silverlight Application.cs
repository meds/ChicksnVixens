using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ExEnSilver;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ChicksnVixens
{
	public class App : ExEnSilverApplication
	{
		protected override void SetupMainPage(MainPage mainPage)
		{
			FontSource uiFontSource = new FontSource(Application.GetResourceStream(
					new Uri("/ChicksnVixens;component/Content/NGO_____.TTF", UriKind.Relative)).Stream);
			FontFamily uiFontFamily = new FontFamily("News Gothic");

			ContentManager.SilverlightFontTranslation("UIFont", new SpriteFontTTF(uiFontSource, uiFontFamily, 16));


			var game = new ChicksnVixensGame(NamedScreenSizes.Get("web"));
			mainPage.Children.Add(game);
			game.Play();
		}
	}
}
