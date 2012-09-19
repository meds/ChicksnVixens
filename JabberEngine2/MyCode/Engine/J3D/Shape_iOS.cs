using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Jabber;
using Jabber.Util;
using Jabber.Media;

using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using System.Runtime.InteropServices;
using OpenTK.Graphics.ES11;

using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using OpenTK.Graphics;
using OpenTK.Platform;
using OpenTK.Platform.iPhoneOS;

namespace Jabber.J3D
{
	public partial class Shape : BaseSprite
	{
		public unsafe void Draw_IOS()
		{
			return;
			
			SpriteBatch.End();
			
			/*float[] squareVertices = {
				0, -110.5f,
				110.5f, -110.5f,
				0, 110.5f,
				110.5f, 110.5f,
			};*/
			Viewport oldview = BaseGame.Get.GraphicsDevice.Viewport;
			BaseGame.Get.GraphicsDevice.SetupClientProjection();
			
			
			Vector2 pos = Camera.Get.Position;
			Vector2 camscale = Camera.Get.WorldScale;
			
			float[] squareVertices = {
				0 - Camera.Get.PosX, 0,
				100 - Camera.Get.PosX, 0,
				100 - Camera.Get.PosX, 100,
				0 - Camera.Get.PosX, 0,
				0 - Camera.Get.PosX, 100
			};
			
			byte[] squareColors = {
				255, 255, 0, 255,
				0, 255, 255, 255,
				0, 0, 0, 0,
				255, 0, 255, 255,
				255, 0, 255, 255
			};
			
			Viewport view = new Viewport();
			view.X = 0;//(int)(-Camera.Get.PosX * camscale.X );
			view.Y = 0;//(int)(Camera.Get.PosY * camscale.Y);
			view.Width = (int)(480 * camscale.X);
			view.Height = (int)(320 * camscale.Y);
			
			
			//pos = Camera.Get.ScreenToWorld(pos);
			//GL.Translate(-pos.X * camscale.X, 200, 0);
			
			BaseGame.Get.GraphicsDevice.Viewport = view;
			
			/*
			GL.Viewport((int)Camera.Get.PosY, (int)-Camera.Get.PosX, 
			            (int)(320 * Camera.Get.WorldScale.Y), (int)(480 * Camera.Get.WorldScale.X));
			*/
			GL.VertexPointer (2, All.Float, 0, squareVertices);
			GL.EnableClientState (All.VertexArray);
			GL.ColorPointer (4, All.UnsignedByte, 0, squareColors);
			GL.EnableClientState (All.ColorArray);
			
			GL.DrawArrays (All.TriangleStrip, 0, 4);
			
			for(int i = 0; i < squareColors.Length; i++)
			{
				squareColors[i] = 1;
			}
			//GL.ColorPointer(5,All.UnsignedByte,0,squareColors);
			
			GL.DisableClientState(All.ColorArray);
			
			
			BaseGame.Get.GraphicsDevice.Viewport = oldview;
			SpriteBatch.Begin();
		}
		
		public void Initialize_IOS(ContentManager content)
		{
		}
		
		public void FinalizeVertices_IOS()
		{
		}
	}
}

