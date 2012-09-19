using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Phone.Tasks;
using System.Text;


using Jabber.Physics;
using Jabber.GameScreenManager;
using Jabber.Util;

using Jabber.Media;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace Jabber
{
    public class BaseGame : Microsoft.Xna.Framework.Game
    {
        #region singleton
        protected static BaseGame baseGame = null;
        protected static SpriteBatch spriteBatch = null;
        private static Random random = null;
        
        public BaseGame()
        {
            TouchPanel.EnabledGestures = GestureType.Tap;
            baseGame = this;
            // Frame rate is 30 fps by default for Windows Phone.
#if !IPHONE && !SILVERLIGHT
#if WINDOWS_PHONE
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif
#if WINDOWS
            TargetElapsedTime = TimeSpan.FromTicks(333333/2);
#endif
#endif
            //bTrial = Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode;

            graphics = new GraphicsDeviceManager(this);
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
			
#if IPHONE
			BackBufferWidth = 480;
			BackBufferHeight = 320;
#endif

#if WINDOWS
            BackBufferWidth = 800;
			BackBufferHeight = 480;
#endif

#if WINDOWS_PHONE
            BackBufferWidth = 800;
			BackBufferHeight = 480;
#endif
            graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;
#if WINDOWS_PHONE
            //Switch to full screen for best game experience
            graphics.IsFullScreen = true;
#endif
            Jabber.Media.AudioManager.Initialize(this);
            Jabber.Util.Camera.Initialize();

			Debug.Assert(Content.ServiceProvider != null);
			Content.RootDirectory = "Content";
        }

        protected override void Draw(GameTime dt)
        {
            if (drawer == null)
            {
                drawer = new TextDrawer("font");
                drawer.Initialize(Content);
            }
            ScreenManager.Get.Draw();
            base.Draw(dt);
            
            spriteBatch.Begin();
            int milliSeconds = watch.Elapsed.Milliseconds;
            drawer.Text = ((int)(1000.0f / watch.ElapsedMilliseconds)).ToString();
            drawer.Text = ((int)(1000.0f / dt.ElapsedGameTime.Milliseconds)).ToString();
            drawer.DrawIn = BaseSprite.DrawSpace.SCREENSPACE;
            drawer.PosX = 0.1f;
            drawer.PosY = 0.1f;
            //drawer.Draw();
            watch.Stop();
            watch.Reset();
            spriteBatch.End();
            

            AdSystem.Draw();
        }
        protected override void Update(GameTime dt)
        {
            watch.Start();
            Jabber.Util.EventManager.Get.Update(dt);
            InputManager.Get.Update(dt);
            ScreenManager.Get.Update(dt);
            //Jabber.Util.EventManager.Get.Update(dt);

            base.Update(dt);


            AdSystem.Update(dt);
        }
        static public Random Random
        {
            get
            {
                if (random == null)
                {
                    random = new Random();
                }
                return random;
            }
        }
        static public BaseGame Get
        {
            get
            {
              //  if (baseGame == null) System.Windows.MessageBox.Show("ERROR: BASE GAME NOT YET CREATED!");

                return baseGame;
            }
        }
        private bool bTrial = false;
        public bool IsTrial
        {
            get { return this.bTrial; }
        }

        #endregion

        public void BeginUI()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, new SamplerState(), null, null,
                        null, Matrix.Identity);

        }

		
		public void Begin()
		{
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, new SamplerState(), null, null,
                        null, Camera.Get.CameraView);
		}
		
		public void End()
		{
			SpriteBatch.End();
		}
		

        public Stopwatch watch = new Stopwatch();
        public TextDrawer drawer;



        #region Public Interface
        #endregion

        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Initialize()
        {
            base.Initialize();
            AdSystem.Initialize();
        }
        protected GraphicsDeviceManager graphics;

        public Vector2 BackBufferDimensions { get { return new Vector2(BackBufferWidth, BackBufferHeight); } }
		public int BackBufferWidth { get; private set; }
		public int BackBufferHeight { get; private set; }
		
		public int HalfBackBufferWidth { get{ return (int)(BackBufferWidth / 2.0f); } }
		public int HalfBackBufferHeight{ get{ return (int)(BackBufferHeight / 2.0f); } }

        public float ScalerFactor { get { return BackBufferWidth / 800.0f; } }

        public Vector2 ScaleToAspect(float Width, float Height)
        {
            return new Vector2(800.0f / Width, 480.0f / Height);
        }
    };
}
