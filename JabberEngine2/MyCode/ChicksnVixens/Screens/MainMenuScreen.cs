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
    public class MainMenuScreen : Screen
    {
        public MainMenuScreen()
            : base()
        {
            EventManager.Get.RegisterListner(this);
        }

#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            BaseGame.Get.Exit();
        }
#endif

        public override void ProcessEvent(Event ev)
        {
            if (!IsTopScreen)
            {
                return;
            }
            base.ProcessEvent(ev);

            if ((ev is MenuEvent))
            {
                if ((ev as MenuEvent).sender is Button && fader == null)
                {
                    if (((ev as MenuEvent).sender as Button).Text != null)
                    {
                        fader = new BlankNess();
                        fader.Initialize(Content);
                        fader.fullBlankity = 1.0f;
                        fader.RaiseFlag(Flags.FADE_IN);

                        Components.Add(fader);
                    }
                    else
                    {
                        Options p = new Options();
                        p.Initialize(Content);
                        ScreenManager.Get.AddScreen(p);
                    }
                }
            }
        }


        public override void Update(GameTime dt)
        {
            if (!IsTopScreen)
            {
                return;
            }
#if WINDOWS_PHONE
            AdSystem.TargetTop = false;
#endif
            base.Update(dt);

            if (fader != null)
            {
                if (fader.StateFlag == Jabber.StateFlag.FADE_IN_COMPLETE)
                {
                    if (ChicksnVixensGame.Get.GetTotalStars("uluru") == 0)
                    {
                        ChicksnVixensGame.Get.PlayCurrentLocationSotry("uluru");
                        RaiseFlag(Flags.DELETE);
                    }
                    else
                    {
                        WorldSelectScreen s = new WorldSelectScreen();
                        s.Initialize(Content);
                        ScreenManager.Get.AddScreen(s);
                        RaiseFlag(Flags.DELETE);
                    }
                }
            }
        }

        BlankNess fader = null;

        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);
			
            MenuObj bg = new MenuObj("ui/mainmenu");
            bg.Initialize(Content);
            bg.CreateFramesFromXML("ui/mainmenu_frames");
            bg.CurrentFrame = "mainmenu";
            bg.ResetDimensions();

            float widthDif = BaseGame.Get.BackBufferWidth / 1000.0f;
            bg.Width *= widthDif;
            bg.Height *= widthDif;
            Components.Add(bg);

            Button b = new Button("ui/ui");
            b.CreateFramesFromXML("ui/ui_frames");
            b.Initialize(Content);
            b.CurrentFrame = "levelbutton";
            b.ResetDimensions();
            b.RegularScale = ScaleFactor * 1.5f;
            b.ScaleOnHover = b.RegularScale * 1.1f;
            b.PosX = -0.24f * BaseGame.Get.BackBufferWidth;
            b.PosY = -0.3f * BaseGame.Get.HalfBackBufferHeight;
            b.SetText("Play", "ui/Play");
            b.Text.Colour = Color.Black;
            b.PlaySFXOnRelease = "Sounds/PlayStateSelect";
            b.TextScaler = 0.75f;
            b.UniformScale = b.RegularScale;
            Components.Add(b);

            b = new Button("ui/ui");
            b.CreateFramesFromXML("ui/ui_frames");
            b.Initialize(Content);
            b.CurrentFrame = "settings";
            b.ResetDimensions();
            b.RegularScale = ScaleFactor * 0.5f;
            b.ScaleOnHover = b.RegularScale * 1.1f;
            b.PosX = -0.45f * BaseGame.Get.BackBufferWidth;
            b.PosY = -0.5f * BaseGame.Get.HalfBackBufferHeight;
            b.UniformScale = b.RegularScale;
            Components.Add(b);

            AudioManager.PlayMusic("troublemaker");


            BlankNess blank = new BlankNess();
            blank.Initialize(Content);
            blank.RaiseFlag(Flags.FADE_OUT);
            blank.fadeInTimer = 1.0f;
            blank.fullBlankity = 1.0f;
            Components.Add(blank);
        }
    }
}
