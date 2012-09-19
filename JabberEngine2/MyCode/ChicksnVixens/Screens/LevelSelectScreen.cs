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
    public class BackButton : Sprite
    {
        public BackButton()
            : base("ui/ui")
        {
            Layer = SpriteLayer.LAYER0;
            RaiseFlag(Flags.ACCEPTINPUT);
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("ui/ui_frames");
            CurrentFrame = "quit";
            ResetDimensions();
            UniformScale = ScaleFactor / Camera.Get.UniformWorldScale;

            Handle = SpriteHandle.BOTTOMCENTER;
        }

        public override void OnTap(Vector2 pos)
        {
            base.OnTap(pos);
            if(Contains(pos.X, pos.Y) && fadeTimer == 1)
            {
                EventManager.Get.SendEvent(new MenuEvent(this, MenuEvent.EventType.TAP));
            }
        }
        float fadeTimer = 0;
        public override void Update(GameTime dt)
        {
            UniformScale = ScaleFactor / Camera.Get.UniformWorldScale * 0.4f;
            PosY = Camera.Get.ScreenToWorld(new Vector2(0, 0.975f)).Y;
            PosX = Camera.Get.ScreenToWorld(Vector2.Zero).X + Width*ScaleX;// Camera.Get.Position.X - 0.4f * BaseGame.Get.BackBufferWidth / Camera.Get.UniformWorldScale;

            fadeTimer += gttf(dt);
            if (fadeTimer > 1)
            {
                fadeTimer = 1;
            }
            Colour = Color.White * fadeTimer;
            base.Update(dt);
        }
    }
    public class LevelSelectScreen : Screen
    {
#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            EventManager.Get.SendEvent(new ScreenCancelEvent(this));

            for (int i = 0; i < levelButtons.Count; i++)
            {
                // levelButtons[i].fadeOut = true;
                levelButtons[i].RaiseFlag(Flags.FADE_OUT);
            }

            screenCamera.RaiseFlag(Flags.FADE_OUT);

            blank.RaiseFlag(Flags.FADE_OUT);
            RaiseFlag(Flags.FADE_OUT);
        }
#endif
        public LevelSelectScreen(string location)
            : base()
        {
            this.location = location;
            RaiseFlag(Flags.ACCEPTINPUT);
            EventManager.Get.RegisterListner(this);
        }
        bool firstTap = true;

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if ((ev is MenuEvent))
            {
                if ((ev as MenuEvent).sender is BackButton)
                {
                    EventManager.Get.SendEvent(new ScreenCancelEvent(this));

                    for (int i = 0; i < levelButtons.Count; i++)
                    {
                        // levelButtons[i].fadeOut = true;
                        levelButtons[i].RaiseFlag(Flags.FADE_OUT);
                    }

                    screenCamera.RaiseFlag(Flags.FADE_OUT);

                    blank.RaiseFlag(Flags.FADE_OUT);
                    RaiseFlag(Flags.FADE_OUT);
                }
                else if ((ev as MenuEvent).sender is LevelButton)
                {
                    GameplayScreen s = new GameplayScreen(location, ((ev as MenuEvent).sender as LevelButton).Level);
                    s.LevelDir = "Content/Levels/" + location + "/Level" + ((ev as MenuEvent).sender as LevelButton).Level + ".xml";
                    s.Initialize(null);
                    ScreenManager.Get.AddScreen(s);

                    LowerFlag(Flags.ACCEPTINPUT);
                }
                else if ((ev as MenuEvent).sender is Button)
                {
                    Button b = (ev as MenuEvent).sender as Button;
                    if (b.CurrentFrame == "movie")
                    {
                        RaiseFlag(Flags.FADE_OUT);
                        enterMovieSequence = true;
                        LowerFlag(Flags.ACCEPTINPUT);

                        for (int i = 0; i < levelButtons.Count; i++)
                        {
                            levelButtons[i].RaiseFlag(Flags.FADE_OUT);
                        }

                        screenCamera.RaiseFlag(Flags.FADE_OUT);

                        blank.RaiseFlag(Flags.FADE_OUT);
                        RaiseFlag(Flags.FADE_OUT);
                    }
                }
            }

            if (ev is FadeOutEvent)
            {
                if ((ev as FadeOutEvent).Sender is FallingFeathers)
                {
                    RaiseFlag(Flags.DELETE);
                }
            }
        }
        bool enterMovieSequence = false;

        public static int NumLevelsUluru    = 27;
        public static int NumLevelsParis = 18;
        public static int NumLevelsBavaria = 18;
        public static int NumLevelsVesuvius = 18;
        public static int NumLevelsPolar = 11;

        public override void OnTap(Vector2 pos)
        {
            base.OnTap(pos);
            /*
            if (firstTap)
            {
                firstTap = false;
                return;
            }
            base.OnTap(pos);
            EventManager.Get.SendEvent(new ScreenCancelEvent(this));
           // fadeOut = true;

            for (int i = 0; i < levelButtons.Count; i++)
            {
               // levelButtons[i].fadeOut = true;
                levelButtons[i].RaiseFlag(Flags.FADE_OUT);
            }

            screenCamera.RaiseFlag(Flags.FADE_OUT);

            blank.RaiseFlag(Flags.FADE_OUT);
            RaiseFlag(Flags.FADE_OUT);
           // blank.fadeOut = true;
          * */
        }
        public override void Draw()
        {
            base.Draw();
        }
        public override void Update(GameTime dt)
        {
#if WINDOWS_PHONE
            AdSystem.TargetTop = false;
#endif
            base.Update(dt);

            if (true)//CheckFlag(Flags.FADE_OUT))
            {
                bool anyIncomplete = false;
                for (int i = 0; i < levelButtons.Count; i++)
                {
                    if (!levelButtons[i].CheckStateFlag(StateFlag.FADE_OUT_COMPLETE))
                    {
                        anyIncomplete = true;
                        break;
                    }
                }
                if (!screenCamera.CheckStateFlag(StateFlag.FADE_OUT_COMPLETE))
                {
                    anyIncomplete = true;
                }
                anyIncomplete = !blank.CheckStateFlag(StateFlag.FADE_OUT_COMPLETE);
                if (!anyIncomplete)
                {
                    RaiseFlag(Flags.DELETE);


                    if (enterMovieSequence)
                    {
                        RaiseFlag(Flags.DELETE);
                        for (int i = 0; i < ScreenManager.Get.Screens.Count; i++)
                        {
                            if (ScreenManager.Get.Screens[i] is WorldSelectScreen)
                            {
                                ScreenManager.Get.Screens[i].RaiseFlag(Flags.DELETE);
                            }
                        }
                        ChicksnVixensGame.Get.PlayCurrentLocationSotry(location);
                    }
                }
            }

        }
        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            base.OnDrag(lastPos, thispos);
        }

        public override void OnRelease(Vector2 pos)
        {
            base.OnRelease(pos);
        }

        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);
            screenCamera = new LevelSelectCam();
            scene = new Jabber.Scene.GameScene(new FarWorld(), Content);
            scene.Initialize(Content);

            BackButton b = new BackButton();
            b.Initialize(Content);
            scene.AddNode(b);

            Button movie = new Button("ui/ui");
            movie.Initialize(Content);
            movie.CreateFramesFromXML("ui/ui_frames");
            movie.CurrentFrame = "movie";
            movie.ResetDimensions();
            movie.RegularScale = ScaleFactor * 0.48f;
            movie.ScaleOnHover = movie.RegularScale * 1.1f;
            movie.UniformScale = ScaleFactor * 0.48f;
            movie.PosX = -0.37f * BaseGame.Get.BackBufferWidth;
            movie.PosY = -0.42f * BaseGame.Get.BackBufferHeight;
            scene.AddNode(movie);

            int numLevels = 0;
            switch (location)
            {
                case "uluru":
                    numLevels = NumLevelsUluru;
                    break;
                case "polar":
                    numLevels = NumLevelsPolar;
                    break;
                case "bavaria":
                    numLevels = NumLevelsBavaria;
                    break;
                case "paris":
                    numLevels = NumLevelsParis;
                    break;
                case "vesuvius":
                    numLevels = NumLevelsVesuvius;
                    break;
            }

            (screenCamera as LevelSelectCam).NumLevels = numLevels;
            /*
#if WINDOWS_PHONE
            using (IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                while (true)
                {

                    string dir = "Content/Levels/" + location + "/Level" + (numLevels + 1) + ".xml";
                    try
                    {
                        //  "Content/Levels/paris/Level2.xml";

                        XDocument.Load(dir);
                        ++numLevels;
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
            }
#else
            while (true)
            {
                if (File.Exists("Content/Levels/" + location + "/Level" + (numLevels + 1) + ".xml"))
                    ++numLevels;
                else
                    break;
            }
#endif
            */
            while (numLevels > 0)
            {
                int currentColumn = 0;
                while (true)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (numLevels == 0)
                        {
                            break;
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            LevelButton but = new LevelButton(i, j, currentColumn, location);
                            but.Initialize(Content);
                            scene.AddNode(but);
                            levelButtons.Add(but);
                            --numLevels;
                            if (numLevels == 0)
                            {
                                break;
                            }
                        }
                    }
                    if(numLevels == 0)
                    {
                        break;
                    }
                    ++currentColumn;
                }

            }

            blank = new BlankNess();
            blank.fullBlankity = 0.75f;
            blank.Initialize(Content);
            scene.AddNode(blank);
            blank.RaiseFlag(Flags.FADE_IN);

            Components.Add(scene);
        }

        BlankNess blank;
        List<LevelButton> levelButtons = new List<LevelButton>();
        GameScene scene;
        string location;
    }
}