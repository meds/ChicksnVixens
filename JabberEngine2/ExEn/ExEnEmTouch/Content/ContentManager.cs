using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace Microsoft.Xna.Framework.Content
{
	/// <summary>
	/// Inheriting from ContentManager is not supported in ExEn
	/// </summary>
	public class ContentManager
	{
		Dictionary<string, object> assets = new Dictionary<string, object>();

		public IServiceProvider ServiceProvider { get; private set; }

		private string _rootDirectory;
		public string RootDirectory
		{
			get { return _rootDirectory; }
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				_rootDirectory = value;
			}
		}


		#region Construction

		public ContentManager(IServiceProvider serviceProvider) : this(serviceProvider, string.Empty) { }
		public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
		{
			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");
			if(rootDirectory == null)
				throw new ArgumentNullException("rootDirectory");

			this.ServiceProvider = serviceProvider;
			this.RootDirectory = rootDirectory;
		}

		#endregion


		#region Unload and Dispose

		// API Difference: In XNA this method is virtual
		public void Unload()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());

			foreach(object asset in assets)
			{
				IDisposable disposableAsset = asset as IDisposable;
				if(disposableAsset != null)
					disposableAsset.Dispose();
			}
			assets.Clear();
		}

		private bool IsDisposed { get { return assets == null; } }
		public void Dispose()
		{
			if(!IsDisposed)
				Unload();

			assets = null;
		}

		#endregion


		#region Loading

		static readonly string[] texture2DExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".tif", ".tiff", ".gif" };
		static readonly string[] soundEffectExtensions = { ".wav", ".mp3", ".aiff", ".ac3" };
		static readonly string[] spriteFontTextureExtensions = { "-exenfont.png" };
		static readonly string[] spriteFontMetricsExtensions = { "-exenfont.exenfont" };
		static readonly string[] spriteFontTextureAt2xExtensions = { "-exenfont@2x.png" };
		static readonly string[] spriteFontMetricsAt2xExtensions = { "-exenfont@2x.exenfont" };

		private string TryGetAssetFullPath(string assetBasePath, string[] extensions)
		{
			foreach(string extension in extensions)
			{
				string path = assetBasePath + extension;
				if(File.Exists(path))
					return path;
			}
			return null;
		}

		private string GetAssetFullPath(string assetName, string assetBasePath, string[] extensions)
		{
			string fullPath = TryGetAssetFullPath(assetBasePath, extensions);
			if(fullPath != null)
				return fullPath;

			throw new ContentLoadException("Failed to load \"" + assetName
					+ "\", could not find a file with a valid extension for \""
					+ assetBasePath + "\". Remember that iOS is case-sensitive.");
		}

		private GraphicsDevice GetGraphicsDevice()
		{
			Debug.Assert(ServiceProvider != null);
			var gds = ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
			if(gds == null)
				throw new InvalidOperationException("No graphics device service");
			var gd = gds.GraphicsDevice;
			if(gd == null)
				throw new InvalidOperationException("No graphics device");
			return gd;
		}


		// API Difference: In XNA this method is virtual
		public T Load<T>(string assetName)
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());
			if(string.IsNullOrEmpty(assetName))
				throw new ArgumentNullException("assetName");

			// Check database of loaded assets
			object result = null;
			if(assets.TryGetValue(assetName, out result))
			{
				if(!(result is T))
					throw new ContentLoadException("Cannot load \"" + assetName + "\" as " + typeof(T).Name + ", it is already loaded as " + result.GetType().Name);
				return (T)result;
			}

			// Generate base path for asset (no extension or @2x)
			// Switch out windows-style directory seperators for the platform separator
			string assetBasePath = RootDirectory.Replace('\\', Path.DirectorySeparatorChar)
					+ Path.DirectorySeparatorChar + assetName.Replace('\\', Path.DirectorySeparatorChar);

			// Load up the asset depending on its type:
			if(typeof(T) == typeof(Texture2D))
			{
				string assetPath = GetAssetFullPath(assetName, assetBasePath, texture2DExtensions);
				result = Texture2D.FromBundle(GetGraphicsDevice(), assetPath);
			}
			else if(typeof(T) == typeof(SoundEffect))
			{
				string assetPath = GetAssetFullPath(assetName, assetBasePath, soundEffectExtensions);
				result = new SoundEffect(assetPath, false);
			}
			else if(typeof(T) == typeof(Song))
			{
				string assetPath = GetAssetFullPath(assetName, assetBasePath, soundEffectExtensions);
				result = new Song(assetPath);
			}
			else if(typeof(T) == typeof(SpriteFont))
			{
				GraphicsDevice graphicsDevice = GetGraphicsDevice();

				string texturePath = null;
				string metricsPath = null;
				if(graphicsDevice.Scaler.AssetLoadScale == 2)
				{
					texturePath = TryGetAssetFullPath(assetBasePath, spriteFontTextureAt2xExtensions);
					metricsPath = TryGetAssetFullPath(assetBasePath, spriteFontMetricsAt2xExtensions);

					if(texturePath == null && metricsPath != null)
						throw new ContentLoadException("@2x texture file for font \"" + assetName + "\" is missing");
					if(texturePath != null && metricsPath == null)
						throw new ContentLoadException("@2x metrics file for font \"" + assetName + "\" is missing");
				}

				bool loadingFontAt2x = (texturePath != null && metricsPath != null);
				if(!loadingFontAt2x)
				{
					texturePath = TryGetAssetFullPath(assetBasePath, spriteFontTextureExtensions);
					metricsPath = TryGetAssetFullPath(assetBasePath, spriteFontMetricsExtensions);
				}

				Texture2D texture = Texture2D.FromFile(GetGraphicsDevice(), texturePath);
				if(texture == null)
					throw new ContentLoadException("Failed to load texture \"" + texturePath + "\" for font \"" + assetName + "\"");

				// Disable retina scaling (it will be handled by SpriteFont)
				texture.texture.LogicalSize = texture.texture.PixelSize;

				using(FileStream metricsStream = File.Open(metricsPath, FileMode.Open, FileAccess.Read))
				{
					result = new SpriteFont(texture, metricsStream, loadingFontAt2x ? 0.5f : 1f);
				}
			}
			else
			{
				throw new ContentLoadException("Asset type " + typeof(T).Name + " is not supported by ExEn");
			}

			if(result == null)
				throw new ContentLoadException("Failed to load asset \"" + assetName + "\"");

			// Loading successful
			assets.Add(assetName, result);
			return (T)result;
		}

		#endregion

	}
}

