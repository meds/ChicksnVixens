#region License
/*
Microsoft Public License (Ms-PL)
XnaTouch - Copyright © 2009 The XnaTouch Team

All rights reserved.

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
purpose and non-infringement.
*/
#endregion License

using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.ES11;
using MonoTouch.Foundation;
using System.IO;

// TODO: figure out some way of supporting / hinting RGBA4444 and other low-bandwidth textures
// See also: http://developer.apple.com/library/ios/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesForWorkingWithTextureData/TechniquesForWorkingWithTextureData.html#//apple_ref/doc/uid/TP40008793-CH104-SW1

namespace Microsoft.Xna.Framework.Graphics
{
	internal class ESTexture2D : IDisposable
	{
		#region Buffer for Loading Texture

		static object textureLoadBufferLockObject = new object();
		static byte[] textureLoadBuffer = null;
		const int textureLoadBufferLength = 1024 * 1024 * 4; // 4MB = 1024*1024 RGBA texture

		/// <summary>Requires textureLoadBufferLockObject is locked!</summary>
		static void CreateTextureLoadBuffer()
		{
			if(textureLoadBuffer == null)
				textureLoadBuffer = new byte[textureLoadBufferLength];
		}

		#endregion


		#region Texture Dimensions and ID

		private uint name;
		/// <summary>OpenGL Texture Name</summary>
		public uint OpenGLName { get { return name; } }

		// Size of the texture according to XNA
		private Point logicalSize;
		public Point LogicalSize { get { return logicalSize; } set { logicalSize = value; RecalculateRatio(); } }

		// Physical size of the used texture area in pixels (at bottom left of OpenGL texture surface)
		private int pixelWidth, pixelHeight;
		public Point PixelSize { get { return new Point(pixelWidth, pixelHeight); } }
		// Power of two texture size in pixels (actual width of OpenGL texture)
		private int potWidth, potHeight;

		#endregion


		#region Construct and Dispose

		public ESTexture2D(UIImage uiImage) : this(uiImage, All.Linear) { }

		public ESTexture2D(UIImage uiImage, All filter)
		{
			CGImage image = uiImage.CGImage;
			if(uiImage == null)
				throw new ArgumentNullException("uiImage");

			// TODO: could use this to implement lower-bandwidth textures
			//bool hasAlpha = (image.AlphaInfo == CGImageAlphaInfo.First || image.AlphaInfo == CGImageAlphaInfo.Last
			//		|| image.AlphaInfo == CGImageAlphaInfo.PremultipliedFirst || image.AlphaInfo == CGImageAlphaInfo.PremultipliedLast);


			// Image dimentions:
			logicalSize = new Point((int)uiImage.Size.Width, (int)uiImage.Size.Height);

			pixelWidth = uiImage.CGImage.Width;
			pixelHeight = uiImage.CGImage.Height;

			// Round up the target texture width and height to powers of two:
			potWidth = pixelWidth;
			potHeight = pixelHeight;
			if(( potWidth & ( potWidth-1)) != 0) { int w = 1; while(w <  potWidth) { w *= 2; }  potWidth = w; }
			if((potHeight & (potHeight-1)) != 0) { int h = 1; while(h < potHeight) { h *= 2; } potHeight = h; }
			
			// Scale down textures that are too large...
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			while((potWidth > 1024) || (potHeight > 1024))
			{
				potWidth /= 2;    // Note: no precision loss - it's a power of two
				potHeight /= 2;
				pixelWidth /= 2;  // Note: precision loss - assume possibility of dropping a pixel at each step is ok
				pixelHeight /= 2;
				transform.Multiply(CGAffineTransform.MakeScale(0.5f, 0.5f));
			}

			RecalculateRatio();


			lock(textureLoadBufferLockObject)
			{
				CreateTextureLoadBuffer();

				unsafe
				{
					fixed(byte* data = textureLoadBuffer)
					{
						var colorSpace = CGColorSpace.CreateDeviceRGB();
						var context = new CGBitmapContext(new IntPtr(data), potWidth, potHeight,
								8, 4 * potWidth, colorSpace, CGImageAlphaInfo.PremultipliedLast);

						context.ClearRect(new RectangleF(0, 0, potWidth, potHeight));
						context.TranslateCTM(0, potHeight - pixelHeight); // TODO: this does not play nice with the precision-loss above (keeping half-pixel to the edge)

						if(!transform.IsIdentity)
							context.ConcatCTM(transform);

						context.DrawImage(new RectangleF(0, 0, image.Width, image.Height), image);
						SetupTexture(new IntPtr(data), filter);

						context.Dispose();
						colorSpace.Dispose();
					}
				}
			}
		}

		public void Dispose()
		{
			if(name != 0)
			{
				GL.DeleteTextures(1, ref name);
			}
		}

		private void SetupTexture(IntPtr data, All filter)
		{
			GL.GenTextures(1, ref name);
			GL.BindTexture(All.Texture2D, name);
			GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)filter);
			GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)filter);

			GL.TexImage2D(All.Texture2D, 0, (int)All.Rgba, (int)potWidth, (int)potHeight, 0, All.Rgba, All.UnsignedByte, data);
		}

		#endregion


		#region Texture-Coordinate Conversion

		// Ratio of XNA pixels to texture coordinates
		private float texWidthRatio, texHeightRatio;

		private void RecalculateRatio()
		{
			// Determine conversion ratio from pixels to texture-coordinates
			texWidthRatio  = ((float)pixelWidth  / (float)logicalSize.X) / (float)potWidth;
			texHeightRatio = ((float)pixelHeight / (float)logicalSize.Y) / (float)potHeight;
		}

		internal void FillTextureCoordinates(Rectangle r, SpriteEffects flip, ref SpriteVertices verts)
		{
			int fh = (int)flip & 1; // SpriteEffects.FlipHorizontally = 1
			int fv = (int)flip >> 8; // SpriteEffects.FlipVertically = 256

			verts.v0.uv = new Vector2(texWidthRatio * (r.X + ((1-fh) * r.Width)), texHeightRatio * (r.Y + ((  fv) * r.Height)));
			verts.v1.uv = new Vector2(texWidthRatio * (r.X + ((1-fh) * r.Width)), texHeightRatio * (r.Y + ((1-fv) * r.Height)));
			verts.v2.uv = new Vector2(texWidthRatio * (r.X + ((  fh) * r.Width)), texHeightRatio * (r.Y + ((  fv) * r.Height)));
			verts.v3.uv = new Vector2(texWidthRatio * (r.X + ((  fh) * r.Width)), texHeightRatio * (r.Y + ((1-fv) * r.Height)));
		}

		#endregion
	}
}
