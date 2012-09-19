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
    public class StoryScreen : Screen
    {
        public StoryScreen(List<string> pagedirs)
            : base()
        {
            for (int i = 0; i < pagedirs.Count; i++)
            {
                this.PageDirs.Add(pagedirs[i]);
            }
        }

        public override void Initialize(ContentManager manager)
        {
            base.Initialize(manager);

            content.RootDirectory = "Content";

            for (int i = 0; i < PageDirs.Count; i++)
            {
                Sprite s = new Sprite(PageDirs[i]);
                Frame f = new Frame();
                s.Initialize(content);
                f.sprite = s;
                if (i == 0)
                {
                    f.timer = 0.1f;
                }
                else
                {
                    f.timer = 1.0f;
                }
                f.sprite.Dimension = Jabber.BaseGame.Get.BackBufferDimensions;
                frames.Add(f);
            }

            shiftWhen.Add(4);
            shiftWhen.Add(9);
            shiftWhen.Add(14);
            shiftWhen.Add(20);
            shiftWhen.Add(25);
            shiftWhen.Add(30);
            shiftWhen.Add(35);
            shiftWhen.Add(40);


            AudioManager.StopTheMusic();
            AudioManager.PlayMusic("australiamusic");
        }

        Double lastPlayPos = 0;
        Double currentPlayPos = 0;
        List<Double> shiftWhen = new List<double>();

#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();
            RaiseFlag(Flags.DELETE);
            WorldSelectScreen s = new WorldSelectScreen();
            s.Initialize(BaseGame.Get.Content);
            s.SetCurrentCountry("uluru");
            ScreenManager.Get.AddScreen(s);
        }
#endif
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            currentPlayPos = AudioManager.PlayingMusicPosition;
            for (int i = 0; i < frames.Count; i++)
            {
                frames[i].sprite.PosX = i * BaseGame.Get.BackBufferWidth + shiftx;
            }

            shiftx = JabMath.MoveTowards(shiftx, newShiftXTarget, gttf(dt) * 3.0f, 150);
            if (shiftx == newShiftXTarget && shiftWhen.Count == 0)
            {
                RaiseFlag(Flags.DELETE);
                WorldSelectScreen s = new WorldSelectScreen();
                s.Initialize(Content);
                s.SetCurrentCountry("uluru");
                ScreenManager.Get.AddScreen(s);
            }
            timer += gttf(dt);
            if (shiftWhen.Count > 0)
            {
                if (currentPlayPos > shiftWhen[0] || (!AudioManager.MusicPlaying && timer > shiftWhen[0]))// || (shiftx == 0 && timer > 4.6f))
                {
                    shiftWhen.RemoveAt(0);
                    newShiftXTarget -= BaseGame.Get.BackBufferWidth;
                }
            }
            lastPlayPos = AudioManager.PlayingMusicPosition;
        }
        float newShiftXTarget = 0;

        public override void Draw()
        {
            BaseGame.Get.End();

            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null,
                        null, Camera.Get.CameraView);

            base.Draw();

            for (int i = 0; i < frames.Count; i++)
            {
                frames[i].sprite.Draw();
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            content.Dispose();
        }

        List<string> PageDirs = new List<string>();
        ContentManager content = new ContentManager(BaseGame.Get.Services);

        class Frame
        {
            public Sprite sprite = null;
            public float timer = 0.0f;
            public float fadeofftimer = 0.0f;
            public float fadeintimer = 0.0f;
        }
        List<Frame> frames = new List<Frame>();

        float shiftx = 0;
        float timer = 0.0f;
    }
}
