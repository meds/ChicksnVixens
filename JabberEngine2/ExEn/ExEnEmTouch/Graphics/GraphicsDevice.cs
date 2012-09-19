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

using MonoTouch.CoreAnimation;
using MonoTouch.OpenGLES;
using OpenTK.Graphics.ES11;
using System;
using Microsoft.Xna.Framework;
using MonoTouch.UIKit;

namespace Microsoft.Xna.Framework.Graphics
{
	public class GraphicsDevice : IDisposable
	{
		public ExEnEmTouchScaler Scaler { get; private set; }


		internal GraphicsDevice(ExEnEmTouchGameView gameView, UIInterfaceOrientation orientation)
		{
			Scaler = new ExEnEmTouchScaler(orientation, gameView.RenderbufferSize, gameView.DeviceSize);
			Scaler.Changed += new Action(ScalerWasChanged);
			ScalerWasChanged();
		}

		internal void ScalerWasChanged()
		{
			PresentationParameters.BackBufferWidth = Scaler.ClientSize.X;
			PresentationParameters.BackBufferHeight = Scaler.ClientSize.Y;
			PresentationParameters.IsFullScreen = true; // Always full screen

			PresentationParameters.iOSInterfaceOrientation = Scaler.Orientation;

			// NOTE: the XNA Device orientation (and the iOS *device* orientation for that matter)
			//       have LandscapeLeft and LandscapeRight reversed compared to the iOS *interface* orientation
			switch(Scaler.Orientation)
			{
				case UIInterfaceOrientation.LandscapeLeft: PresentationParameters.DisplayOrientation = DisplayOrientation.LandscapeRight; break;
				case UIInterfaceOrientation.LandscapeRight: PresentationParameters.DisplayOrientation = DisplayOrientation.LandscapeLeft; break;

				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					PresentationParameters.DisplayOrientation = DisplayOrientation.Portrait;
					break;

				default: PresentationParameters.DisplayOrientation = DisplayOrientation.Default; break;
			}

			Viewport = new Viewport(PresentationParameters.Bounds);
		}


		private PresentationParameters _presentationParameters = new PresentationParameters();
		public PresentationParameters PresentationParameters { get { return _presentationParameters; } }



		#region Disposal

		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			IsDisposed = true;
		}

		#endregion


		public void Clear(Color color)
		{
			Vector4 vector = color.ToVector4();
			GL.ClearColor(vector.X, vector.Y, vector.Z, vector.W);
			GL.Clear((uint)All.ColorBufferBit);
		}



		private Viewport viewport;
		public Viewport Viewport
		{
			get { return viewport; }
			set
			{
				viewport = value;

				Rectangle r = new Rectangle(value.X, value.Y, value.Width, value.Height);
				Scaler.LogicalToRender(ref r);
				GL.Viewport(r.X, r.Y, r.Width, r.Height);
			}
		}


		public void SetupClientProjection()
		{
			Scaler.SetMatrixModeProjection();
			GL.Ortho(0, viewport.Width, viewport.Height, 0, -1, 1);
		}


	}
}
