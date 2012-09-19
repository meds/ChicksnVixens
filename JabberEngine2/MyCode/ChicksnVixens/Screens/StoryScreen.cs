using Jabber.Media;
using Jabber.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

using Jabber.Util;
using Jabber;
using System.Xml.Linq;

using Jabber.Physics;

using Jabber.GameScreenManager;
using ChicksnVixens;
using Jabber.Util.UI;

namespace ChicksnVixens.Screens
{
    public class VesuviusStory : Screen
    {
        public VesuviusStory()
            : base()
        {
        }
#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();
            RaiseFlag(Flags.DELETE);
            WorldSelectScreen s = new WorldSelectScreen();
            s.Initialize(BaseGame.Get.Content);
            s.SetCurrentCountry("vesuvius");
            ScreenManager.Get.AddScreen(s);
        }
#endif
        public override void Initialize(ContentManager _Content)
        {
            base.Initialize(_Content);

            //Content = content;
            content.RootDirectory = "Content";

            longstrip = new Sprite("Story/Vesuvius/p1");
            longstrip.Initialize(content);
            finalimage = new Sprite("Story/Vesuvius/p2");
            finalimage.Initialize(content);

            finalimage.Dimension = BaseGame.Get.BackBufferDimensions;// / finalimage.Dimension;
           // finalimage.Width *= 2.0f;

            float oldWidth = longstrip.Width;
            longstrip.Width = Jabber.BaseGame.Get.BackBufferWidth;// *2.0f;
            longstrip.Height = Jabber.BaseGame.Get.BackBufferHeight;
           // longstrip.PosX = Jabber.BaseGame.Get.BackBufferWidth / 2.0f;

            finalimage.PosX = Jabber.BaseGame.Get.BackBufferWidth / 2.0f;

            AudioManager.StopTheMusic();
            AudioManager.PlayMusic("vesuviusmusic");
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (MediaPlayer.PlayPosition.TotalSeconds > 6)
            {
                timer1 = 1;
            }
            if (MediaPlayer.PlayPosition.TotalSeconds > 11)
            {
                RaiseFlag(Flags.DELETE);

                WorldSelectScreen s = new WorldSelectScreen();
                s.Initialize(BaseGame.Get.Content);
                s.SetCurrentCountry("vesuvius");
                ScreenManager.Get.AddScreen(s);

                return;
            }
            if (timer1 < 1 || !AudioManager.MusicPlaying)
            {
                timer1 += gttf(dt) / 3.0f;
            }
            else if (timer2 < 1)
            {
                longstrip.PosX = JabMath.LinearMoveTowards(longstrip.PosX, -Jabber.BaseGame.Get.BackBufferWidth / 2.0f, gttf(dt) * 200 * ScaleFactor);

                if (Math.Abs(longstrip.PosX - -Jabber.BaseGame.Get.BackBufferWidth / 2.0f) < 400 * ScaleFactor)
                {
                    float f = longstrip.Colour.ToVector4().W;
                    f = JabMath.MoveTowards(f, 0, gttf(dt) * 1.3f);
                    longstrip.Colour = Color.White * f;
                    if (f == 0)
                    {
                        timer2 = 1.0f;
                    }
                }

                if (longstrip.PosX == -Jabber.BaseGame.Get.BackBufferWidth / 2.0f)
                {
                    timer2 += gttf(dt) / 4.0f;
                }
            }
            if (Math.Abs(longstrip.PosX - -Jabber.BaseGame.Get.BackBufferWidth / 2.0f) < 400 * ScaleFactor)
            {
                finalimage.PosX = JabMath.LinearMoveTowards(finalimage.PosX, 0, gttf(dt) * 150 * ScaleFactor);

                if (finalimage.PosX == 0)//-Jabber.BaseGame.Get.BackBufferWidth / 3.0f)
                    timer3 += gttf(dt) / 5.5f;
                if (timer3 > 1.0f)
                {
                    RaiseFlag(Flags.DELETE);

                    WorldSelectScreen s = new WorldSelectScreen();
                    s.Initialize(BaseGame.Get.Content);
                    s.SetCurrentCountry("vesuvius");
                    ScreenManager.Get.AddScreen(s);
                }
            }
        }


        public override void Draw()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null,
                        null, Camera.Get.CameraView);
            base.Draw();

            //if (timer2 >= 1)
            {
                finalimage.Draw();
            }
            //else
            {
                longstrip.Draw();
            }
        }

        float timer1 = 0;
        float timer2 = 0;
        float timer3 = 0;
        float timer4 = 0;
        Sprite longstrip;
        Sprite finalimage;

        public override void UnloadContent()
        {
            base.UnloadContent();
            content.Dispose();
        }
        ContentManager content = new ContentManager(BaseGame.Get.Services);
    }



    public class PolarStory : Screen
    {
        public PolarStory()
            : base()
        {
        }
#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();
            RaiseFlag(Flags.DELETE);
            WorldSelectScreen s = new WorldSelectScreen();
            s.Initialize(BaseGame.Get.Content);
            s.SetCurrentCountry("polar");
            ScreenManager.Get.AddScreen(s);
        }
#endif
        public override void Initialize(ContentManager _Content)
        {
            base.Initialize(_Content);

            //Content = content;
            content.RootDirectory = "Content";

            longstrip = new Sprite("Story/polar/p1");
            longstrip.Initialize(content);
            finalimage = new Sprite("Story/polar/p2");
            finalimage.Initialize(content);

            finalimage.Dimension = Jabber.BaseGame.Get.BackBufferDimensions;


            longstrip.Width = Jabber.BaseGame.Get.BackBufferWidth * 2.0f;
            longstrip.Height = Jabber.BaseGame.Get.BackBufferHeight;

            longstrip.PosX = Jabber.BaseGame.Get.BackBufferWidth / 2.0f;
            finalimage.Width *= 2.0f;

            finalimage.PosX = Jabber.BaseGame.Get.BackBufferWidth / 2.0f;

            AudioManager.StopTheMusic();
            AudioManager.PlayMusic("polarmusic");
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (timer1 < 1)
            {
                timer1 += gttf(dt) / 3.0f;
            }
            else if (timer2 < 1)
            {
                longstrip.PosX = JabMath.LinearMoveTowards(longstrip.PosX, -Jabber.BaseGame.Get.BackBufferWidth / 2.0f, gttf(dt) * 150 * ScaleFactor);

                if (longstrip.PosX == -Jabber.BaseGame.Get.BackBufferWidth / 2.0f)
                    timer2 += gttf(dt) / 4.0f;
            }
            else
            {
                finalimage.PosX = JabMath.LinearMoveTowards(finalimage.PosX, -Jabber.BaseGame.Get.BackBufferWidth / 2.0f, gttf(dt) * 150 * ScaleFactor);

                if (finalimage.PosX == -Jabber.BaseGame.Get.BackBufferWidth / 2.0f)
                    timer3 += gttf(dt) / 4.0f;
                if (timer3 > 1.0f)
                {
                    RaiseFlag(Flags.DELETE);


                    WorldSelectScreen s = new WorldSelectScreen();
                    s.Initialize(BaseGame.Get.Content);
                    s.SetCurrentCountry("polar");
                    ScreenManager.Get.AddScreen(s);
                }
            }
        }


        public override void Draw()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null,
                        null, Camera.Get.CameraView);
            base.Draw();

            if (timer2 >= 1)
            {
                finalimage.Draw();
            }
            else
            {
                longstrip.Draw();
            }
        }

        float timer1 = 0;
        float timer2 = 0;
        float timer3 = 0;
        float timer4 = 0;
        Sprite longstrip;
        Sprite finalimage;

        public override void UnloadContent()
        {
            base.UnloadContent();
            content.Dispose();
        }
        ContentManager content = new ContentManager(BaseGame.Get.Services);
    }




    public class GermanyStory : Screen
    {
        public GermanyStory()
            : base()
        {
        }
#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();
            RaiseFlag(Flags.DELETE);
            WorldSelectScreen s = new WorldSelectScreen();
            s.Initialize(BaseGame.Get.Content);
            s.SetCurrentCountry("bavaria");
            ScreenManager.Get.AddScreen(s);
        }
#endif
        public override void Initialize(ContentManager _Content)
        {
            base.Initialize(_Content);

            //Content = content;
            content.RootDirectory = "Content";

            longstrip = new Sprite("Story/germany/p1");
            longstrip.Initialize(content);
            //finalimage = new Sprite("Story/france/p2");
            //finalimage.Initialize(content);

           // finalimage.Dimension = Jabber.BaseGame.Get.BackBufferDimensions;


            longstrip.Width = Jabber.BaseGame.Get.BackBufferWidth * 2.0f;
            longstrip.Height = Jabber.BaseGame.Get.BackBufferHeight;

            longstrip.PosX = Jabber.BaseGame.Get.BackBufferWidth/2.0f;

            AudioManager.StopTheMusic();
            AudioManager.PlayMusic("germanymusic");
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (timer1 < 1)
            {
                timer1 += gttf(dt) / 3.0f;
            }
            else if (timer2 < 1)
            {
                longstrip.PosX = JabMath.LinearMoveTowards(longstrip.PosX, -Jabber.BaseGame.Get.BackBufferWidth / 2.0f, gttf(dt) * 150 * ScaleFactor);

                if (longstrip.PosX == -Jabber.BaseGame.Get.BackBufferWidth / 2.0f)
                    timer2 += gttf(dt) / 3.0f;
            }
            else
            {
               // timer3 += gttf(dt);
               // if (timer3 > 4.0f)
                {
                   // Vector4 col = finalimage.Colour.ToVector4();
                   // float a = JabMath.LinearMoveTowards(col.W, 0.0f, gttf(dt));
                   // finalimage.Colour = new Color(a, a, a, a);

                    //if (a == 0)
                    {
                        RaiseFlag(Flags.DELETE);
                        WorldSelectScreen s = new WorldSelectScreen();
                        s.Initialize(BaseGame.Get.Content);
                        s.SetCurrentCountry("bavaria");
                        ScreenManager.Get.AddScreen(s);
                    }
                }
            }
        }


        public override void Draw()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null,
                        null, Camera.Get.CameraView);

            base.Draw();

            if (timer2 >= 1)
            {
               // RaiseFlag(Flags.DELETE);
             //   finalimage.Draw();
            }
            else
            {
                longstrip.Draw();
            }
        }

        float timer1 = 0;
        float timer2 = 0;
        float timer3 = 0;
        Sprite longstrip;
       // Sprite finalimage;

        public override void UnloadContent()
        {
            base.UnloadContent();
            content.Dispose();
        }
        ContentManager content = new ContentManager(BaseGame.Get.Services);
    }

    public class FranceStory : Screen
    {
        public FranceStory()
            : base()
        {
        }

#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();
            RaiseFlag(Flags.DELETE);
            WorldSelectScreen s = new WorldSelectScreen();
            s.Initialize(BaseGame.Get.Content);
            s.SetCurrentCountry("paris");
            ScreenManager.Get.AddScreen(s);
        }
#endif
        public override void Initialize(ContentManager _Content)
        {
            base.Initialize(_Content);

            //Content = content;
            content.RootDirectory = "Content";

            longstrip = new Sprite("Story/france/p1");
            longstrip.Initialize(content);
            finalimage = new Sprite("Story/france/p2");
            finalimage.Initialize(content);

            finalimage.Dimension = Jabber.BaseGame.Get.BackBufferDimensions;


            longstrip.Width = Jabber.BaseGame.Get.BackBufferWidth * 2.0f;
            longstrip.Height = Jabber.BaseGame.Get.BackBufferHeight;

            longstrip.PosX = Jabber.BaseGame.Get.BackBufferWidth/2.0f;
        }
        bool firstUpdate = true;
        public override void Update(GameTime dt)
        {
            if (firstUpdate)
            {
                AudioManager.StopTheMusic();
                AudioManager.PlayMusic("francemusic");
                firstUpdate = false;
            }

            base.Update(dt);

            if (AudioManager.PlayingMusicPosition > 11.7)
            {
                longstrip.PosX = -Jabber.BaseGame.Get.BackBufferWidth / 2.0f;
                timer1 = 1;
                timer2 = 1;
            }

            if (timer1 < 1)
            {
                timer1 += gttf(dt) / 3.0f;
            }
            else if (timer2 < 1)
            {
                longstrip.PosX = JabMath.LinearMoveTowards(longstrip.PosX, -Jabber.BaseGame.Get.BackBufferWidth / 2.0f, gttf(dt) * 150 * ScaleFactor);

                if (longstrip.PosX == -Jabber.BaseGame.Get.BackBufferWidth / 2.0f)
                    timer2 += gttf(dt) / 3.0f;
            }
            else
            {
                timer3 += gttf(dt);
                if (timer3 > 4.0f)
                {
                    Vector4 col = finalimage.Colour.ToVector4();
                    float a = JabMath.LinearMoveTowards(col.W, 0.0f, gttf(dt));
                    finalimage.Colour = new Color(a, a, a, a);

                    if (a == 0)
                    {
                        RaiseFlag(Flags.DELETE);
                        WorldSelectScreen s = new WorldSelectScreen();
                        s.Initialize(BaseGame.Get.Content);
                        s.SetCurrentCountry("paris");
                        ScreenManager.Get.AddScreen(s);
                    }
                }
            }
        }

        public override void Draw()
        {
            SpriteBatch.End();

            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null,
                        null, Camera.Get.CameraView);
            base.Draw();
            if (timer2 >= 1)
            {
                finalimage.Draw();
            }
            else
            {
                longstrip.Draw();
            }
        }

        float timer1 = 0;
        float timer2 = 0;
        float timer3 = 0;
        Sprite longstrip;
        Sprite finalimage;

        public override void UnloadContent()
        {
            base.UnloadContent();
            content.Dispose();
        }
        ContentManager content = new ContentManager(BaseGame.Get.Services);
    }
    
}
