using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlankGame
{
	public class BlankGameGame : Game
	{
		protected GraphicsDeviceManager graphics;

		public BlankGameGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 320;
			graphics.PreferredBackBufferHeight = 480;

			IsMouseVisible = true;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			base.Draw(gameTime);
		}

	}
}
