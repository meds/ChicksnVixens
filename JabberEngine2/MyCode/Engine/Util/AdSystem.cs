using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using System.Text;

using Jabber.Util;
using Jabber.Media;
using Jabber.Physics;
using Jabber.Scene;
using Jabber;

#if WINDOWS_PHONE || WINDOWS
using SOMAWP7;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
#endif

namespace Jabber.Util
{
#if WINDOWS_PHONE
    public static class AdSystem
    {
        static SomaAd somaAd;
        static Texture2D textureSomaAd;
        static Sprite AdSprite;// = new Sprite("ui/ui");
        static Texture2D sampleAd;
        static ContentManager thisContent = new Microsoft.Xna.Framework.Content.ContentManager(BaseGame.Get.Services);
        //static ContentManager Content;
        public static void Initialize()
        {
            thisContent.RootDirectory = "Content";
            //Content = new ContentManager(BaseGame.Get.Services);
            //Content.RootDirectory = "Content";
            // Set up SomaAd to get ads
            /*
            somaAd = new SomaAd();
            somaAd.Adspace = 65737880;   // Developer Ads
            somaAd.Pub = 923834393;       // Developer Ads
            somaAd.GetAd();
            AdSprite.Initialize(BaseGame.Get.Content);
            textureSomaAd = BaseGame.Get.Content.Load<Texture2D>("sampleAd");

            AdSprite.Texture = textureSomaAd;
            AdSprite.Width = 480 * 0.7f;
            AdSprite.Height = 79 * 0.7f;
            AdSprite.Handle = BaseSprite.SpriteHandle.BOTTOMCENTER;

            timer = new System.Diagnostics.Stopwatch();
            timer.Start();*/
            GetNewAd();
        }

        static void GetNewAd()
        {
            
            if (AdShowing)
            {
                //AdSprite.Texture.Dispose();
            }

            if (AdSprite == null)
            {
                somaAd = new SomaAd();
                somaAd.Adspace = 65737880;
                somaAd.Pub = 923834393;

                AdSprite = new Sprite("ui/ui");
                AdSprite.Initialize(BaseGame.Get.Content);
                textureSomaAd = thisContent.Load<Texture2D>("sampleAd");

                AdSprite.Texture = textureSomaAd;
                AdSprite.Width = 480 * 0.7f;
                AdSprite.Height = 79 * 0.7f;
                AdSprite.Handle = BaseSprite.SpriteHandle.BOTTOMCENTER;
            }
            AdShowing = false;
            somaAd.GetAd();
            timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            AdDelayTillNext = BaseGame.Random.Next(20, 40);
        }
        static int AdDelayTillNext = 0;
        public static void Draw()
        {

            Camera.CurrentCamera = new Camera();
            // draw ad
            BaseGame.Get.SpriteBatch.Begin();
            //BaseGame.Get.SpriteBatch.Draw(textureSomaAd, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1.0f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);//, new Rectangle(0, 720, 480, 80), Color.White);

            if (AdShowing)
            {
                AdSprite.Handle = BaseSprite.SpriteHandle.CENTER;
                AdSprite.PosX = BaseGame.Get.BackBufferWidth - AdSprite.Width / 2.0f;
                AdSprite.UpdateOrigin();
                AdSprite.Draw();
            }
            BaseGame.Get.SpriteBatch.End();
            Camera.CurrentCamera = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        static Vector2 direction = new Vector2(3, 2);
        static Vector2 position = new Vector2(100, 100);
        static Vector2 somaAdPosition = new Vector2(0, 720);
        static Vector2 somaAdSize = new Vector2(480, 80);
        static string currentAdImageFileName = "";


        public static void OnTap(Vector2 pos)
        {
            float leftMost = AdSprite.PosX - AdSprite.Width / 2.0f;
            float rightMost = AdSprite.PosX + AdSprite.Width / 2.0f;
            float topMost = AdSprite.PosY + AdSprite.Height / 2.0f;
            float bottomMost = AdSprite.PosY - AdSprite.Height / 2.0f;

            topMost *= -1;
            bottomMost *= -1;

            Camera cam = new Camera();
            //pos = cam.ScreenToWorld(pos);
            pos *= BaseGame.Get.BackBufferDimensions;          
            /*
            leftMost = cam.WorldToScreen(new Vector2(leftMost, 0)).X;
            rightMost = cam.WorldToScreen(new Vector2(rightMost, 0)).X;
            topMost = cam.WorldToScreen(new Vector2(0, topMost)).Y;
            bottomMost = cam.WorldToScreen(new Vector2(0, bottomMost)).Y;
            */

            //Camera.CurrentCamera = new Camera();
            if (pos.X > leftMost && pos.X < rightMost && pos.Y < bottomMost && pos.Y > topMost)
            {
                if (AdShowing)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.URL = somaAd.Uri;
                    webBrowserTask.Show();
                }
                else
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.URL = "http://twitter.com/#!/JabberWorx";
                    webBrowserTask.Show();
                }
            }
        }

        static void LoadNewAd()
        {
            lock (syncLock)
            {
                try
                {
                  // if (currentAdImageFileName != somaAd.AdImageFileName)
                    {
                        currentAdImageFileName = somaAd.AdImageFileName;
                        IsolatedStorageFile myIsoStore = IsolatedStorageFile.GetUserStoreForApplication();
                        IsolatedStorageFileStream myAd = new IsolatedStorageFileStream(somaAd.AdImageFileName, FileMode.Open, myIsoStore);
                        textureSomaAd = Texture2D.FromStream(BaseGame.Get.GraphicsDevice, myAd);

                        myAd.Close();
                        myAd.Dispose();
                        myIsoStore.Dispose();

                        if(AdSprite.Texture != textureSomaAd)
                            AdSprite.Texture.Dispose();

                        AdSprite.Texture = textureSomaAd;
                        AdShowing = true;
                    }
                }
                catch (IsolatedStorageException ise)
                {
                    string message = ise.Message;
                }
            }
        }

        static System.Threading.Thread adLoadThread;
        static object syncLock = new object();
        static  System.Diagnostics.Stopwatch timer = null;
        public static void Update(GameTime gameTime)
        {
            lock (syncLock)
            {
                if (timer.Elapsed.Seconds > AdDelayTillNext  && AdShowing)
                {
                    GetNewAd();
                }
                
                // calculate new ball position
                position = Vector2.Add(position, direction);
                if ((position.X + 100) > 480 || position.X < 0) direction.X *= -1;
                if ((position.Y + 100) > 720 || position.Y < 0) direction.Y *= -1;
            }
            // if there is a new ad, get it from Isolated Storage and  show it
            if (somaAd.Status == "success" && somaAd.AdImageFileName != null && somaAd.ImageOK)
            {
                try
                {
                    if (currentAdImageFileName != somaAd.AdImageFileName)
                    {
                        adLoadThread = new System.Threading.Thread(LoadNewAd);
                        adLoadThread.Start();
                    }
                }
                catch (IsolatedStorageException ise)
                {
                    string message = ise.Message;
                }
            }
            IsVisible = true;

            if (IsVisible)
            {
                if (false)//TargetTop)
                {
                    AdTargetPosY = -AdSprite.Height / 2.0f;
                }
                else
                {
                    if (timer.Elapsed.Seconds > AdDelayTillNext - 2 || !AdShowing)
                    {
                        AdTargetPosY = -BaseGame.Get.BackBufferHeight - AdSprite.Height / 2.0f;

                        float distToTarget = AdSprite.PosY - AdTargetPosY;
                        float maxDistToTarget = -BaseGame.Get.BackBufferHeight + AdSprite.Height / 2.0f - AdTargetPosY;
                        float prop = 1.0f - distToTarget / maxDistToTarget;
                        AdSprite.Colour = Color.White * prop;
                    }
                    else
                    {
                        AdTargetPosY = -BaseGame.Get.BackBufferHeight + AdSprite.Height / 2.0f;


                        float distToTarget = AdSprite.PosY - AdTargetPosY;
                        float maxDistToTarget = -BaseGame.Get.BackBufferHeight - AdSprite.Height / 2.0f - AdTargetPosY;
                        float prop = 1.0f - distToTarget / maxDistToTarget;
                        AdSprite.Colour = Color.White * prop;


                    }
                }

               
            }

            AdSprite.PosY = JabMath.MoveTowards(AdSprite.PosY, AdTargetPosY, gameTime.ElapsedGameTime.Milliseconds / 1000.0f * 3.5f, 3);
        }
        static float AdTargetPosY = BaseGame.Get.BackBufferHeight + 200;
        public static bool TargetTop = true;
        public static bool IsVisible = false;
        static bool AdShowing = false;
    }
#else
    public static class AdSystem
    {
        public static void Initialize()
        {
        }
        public static void Draw()
        {
        }

        public static void Update(GameTime td)
        {
        }

    public static void OnTap(Vector2 pos)
    {
    }
    }
#endif
}
