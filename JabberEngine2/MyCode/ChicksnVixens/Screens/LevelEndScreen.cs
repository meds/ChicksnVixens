using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabber.GameScreenManager;
using Jabber.Media;
using Microsoft.Xna.Framework;
using Jabber.Scene;
using Jabber.Physics;
using Jabber.Util;
using Jabber.Util.UI;
using Jabber;

namespace ChicksnVixens.Screens
{
    class ThinBlackLine : MenuObj
    {
        public ThinBlackLine()
            : base("ui/ui")
        {
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);
            CreateFramesFromXML("ui/ui_frames");

            CurrentFrame = "whitecore";

            Width = BaseGame.Get.BackBufferWidth * 0.31f;
            Height = 0.0f;// BaseGame.Get.BackBufferHeight;
            PosY = BaseGame.Get.BackBufferHeight / 2.0f;
            Handle = SpriteHandle.TOPCENTER;
        }


        public override void Update(GameTime dt)
        {
            base.Update(dt);

            fadeInTimer += gttf(dt);
            if (fadeInTimer > 1)
                fadeInTimer = 1.0f;

            Colour = Color.Black * 1 * 0.4f;
            Height = JabMath.MoveTowards(Height, BaseGame.Get.BackBufferHeight, gttf(dt) * 15, 10);
        }

        float fadeInTimer = 0.0f;
    }

    public class FadeInButton : Button
    {
        public FadeInButton()
            : base("ui/ui")
        {
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            fadeInTimer += gttf(dt);// / 5.0f;
            if (fadeInTimer > 1.0f)
                fadeInTimer = 1.0f;

           
        }

        public override void Draw()
        {
            Color oldcol = Colour;
            Colour *= fadeInTimer;
            base.Draw();

            Colour = oldcol;
        }

        float fadeInTimer = 0.0f;
    }

    public class LevelEndScreen : Screen
    {
        public LevelEndScreen(GameplayScreen gp)
            : base()
        {
            gameplay = gp;
            EventManager.Get.RegisterListner(this);
        }
        GameplayScreen gameplay;
        MenuObj Donut = new MenuObj("misc");
        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is MenuEvent)
            {
                if ((ev as MenuEvent).sender is Button && (ev as MenuEvent).Type == MenuEvent.EventType.RELEASE)
                {
                    string text = ((ev as MenuEvent).sender as Button).CurrentFrame;

                    if (text == "restart" && !(ScreenManager.Get.TopScreen is GameplayScreen))
                    {
                        gameplay.Restart();
                        RaiseFlag(Flags.DELETE);
                    }
                    else if (text == "quit")
                    {
                        blank = new BlankNess();
                        blank.Initialize(Content);
                        blank.fullBlankity = 1.0f;
                        blank.RaiseFlag(Flags.FADE_IN);
                        Components.Add(blank);
                    }
                    else if (text == "doublearrow")
                    {
                        blank = new BlankNess();
                        blank.Initialize(Content);
                        blank.fullBlankity = 1.0f;
                        blank.RaiseFlag(Flags.FADE_IN);
                        Components.Add(blank);
                        NextLevelLoad = true;
                    }
                }
            }
        }
        bool NextLevelLoad = false;
        public override void Update(GameTime dt)
        {
            base.Update(dt);
#if WINDOWS_PHONE
            AdSystem.TargetTop = false;
#endif
            if (blank != null)
            {
                if (blank.StateFlag == Jabber.StateFlag.FADE_IN_COMPLETE)
                {
                    gameplay.RaiseFlag(Flags.DELETE);
                    RaiseFlag(Flags.DELETE);

                    if (NextLevelLoad)
                    {
                        if (ChicksnVixensGame.Get.GetTotalLevels(gameplay.location) >= gameplay.levelNum + 1)
                        {
                            GameplayScreen s = new GameplayScreen(gameplay.location, gameplay.levelNum + 1);
                            s.Initialize(Content);
                            ScreenManager.Get.AddScreen(s);
                        }
                        else
                        {
                            if (gameplay.location != "vesuvius")
                            {
                                ChicksnVixensGame.Get.PlayNextLocationStory(gameplay.location);
                            }
                            else
                            {
                                MainMenuScreen s = new MainMenuScreen();
                                s.Initialize(Content);
                                ScreenManager.Get.AddScreen(s);
                            }
                        }
                    }
                    else
                    {
                        WorldSelectScreen s = new WorldSelectScreen();
                        s.Initialize(Content);
                        s.SetCurrentCountry(gameplay.location);
                        ScreenManager.Get.AddScreen(s);
                    }
                }
            }
        }
        BlankNess blank = null;
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.Initialize(content);

            BlankNess n = new BlankNess();
            n.Initialize(Content);
            n.RaiseFlag(Jabber.Flags.FADE_IN);
            n.fullBlankity = 0.25f;
            Components.Add(n);

            if (gameplay.GetRemainingFox() == 0)
            {
                MenuObj logo = new MenuObj("ui/ui");
                logo.Initialize(Content);
                logo.CreateFramesFromXML("ui/ui_frames");
                logo.CurrentFrame = "cannonfire";
                logo.ResetDimensions();
                logo.UniformScale = ScaleFactor;
                logo.PosX = 0.1f * BaseGame.Get.BackBufferWidth;

                logo.PosY = 0.1f * BaseGame.Get.BackBufferHeight;
                logo.PosY -= 0.1f * BaseGame.Get.BackBufferHeight;
                logo.Colour = Color.White * 0.5f;
                Components.Add(logo);
            }
            else
            {
                MenuObj logo = new MenuObj("ui/ui");
                logo.Initialize(Content);
                logo.CreateFramesFromXML("ui/ui_frames");
                logo.CurrentFrame = "chickenincannon";
                logo.ResetDimensions();
                logo.UniformScale = ScaleFactor;

                logo.Colour = Color.White * 0.5f;
                logo.PosY -= 0.1f * BaseGame.Get.BackBufferHeight;
                Components.Add(logo);
            }

            ThinBlackLine b = new ThinBlackLine();
            b.Initialize(Content);
            Components.Add(b);

            Button restart = new FadeInButton();
            restart.Initialize(Content);
            restart.CreateFramesFromXML("ui/ui_frames");
            restart.CurrentFrame = "restart";
            restart.ResetDimensions();
            restart.Colour = Color.LightGreen;
            restart.RegularScale = ScaleFactor / 2.0f;
            restart.ScaleOnHover = restart.RegularScale * 1.1f;
            restart.PosY = -BaseGame.Get.HalfBackBufferHeight * 0.5f;
            Components.Add(restart);

            if (gameplay.GetRemainingFox() == 0)
            {
                restart = new FadeInButton();
                restart.Initialize(Content);
                restart.CreateFramesFromXML("ui/ui_frames");
                restart.CurrentFrame = "doublearrow";
                restart.ResetDimensions();
                restart.Colour = Color.LightBlue;
                restart.RegularScale = ScaleFactor / 2.0f;
                restart.ScaleOnHover = restart.RegularScale * 1.1f;
                restart.PosX = BaseGame.Get.BackBufferWidth * 0.14f;
                restart.PosY = -BaseGame.Get.HalfBackBufferHeight * 0.5f;
                Components.Add(restart);
            }

            restart = new FadeInButton();
            restart.Initialize(Content);
            restart.CreateFramesFromXML("ui/ui_frames");
            restart.CurrentFrame = "quit";
            restart.ResetDimensions();
            restart.Colour = Color.Red * 0.8f;
            restart.RegularScale = ScaleFactor / 2.0f;
            restart.ScaleOnHover = restart.RegularScale * 1.1f;
            restart.PosX = -BaseGame.Get.BackBufferWidth * 0.14f;
            restart.PosY = -BaseGame.Get.HalfBackBufferHeight * 0.5f;
            Components.Add(restart);

            TextDrawer text = new TextDrawer("ui/LevelFont");
            text.Handle = BaseSprite.SpriteHandle.CENTER;
            text.Initialize(Content);
            text.Text = "Best: " + ChicksnVixensGame.Get.GetLevelState(gameplay.location, gameplay.levelNum).Score;
            text.PosX = 0 * BaseGame.Get.BackBufferWidth;
            text.PosY = 0.1f * BaseGame.Get.BackBufferHeight;
            text.UniformScale = ScaleFactor * 0.7f;
            Components.Add(text);

            text = new TextDrawer("ui/LevelFont");
            text.Handle = BaseSprite.SpriteHandle.CENTER;
            text.Initialize(Content);
            text.Text = "Score: " + gameplay.score.score.ToString();
            text.Colour = Color.OrangeRed;
            text.UniformScale = ScaleFactor;
            text.PosX = 0;
            text.PosY = 0.175f * BaseGame.Get.BackBufferHeight;
            Components.Add(text);

            StarDrawer s = new StarDrawer(gameplay.NumStars);
            s.Initialize(Content);
            Components.Add(s);

            Donut.Initialize(Content);
            Donut.CreateFramesFromXML("misc_frames");
            Donut.CurrentFrame = "donut";
            Donut.ResetDimensions();
            Donut.UniformScale = ScaleFactor;
            Donut.PosY = -0.06f * BaseGame.Get.BackBufferHeight;
            Donut.Colour = Color.White * 0.4f;
            Components.Add(Donut);
            

            text = new TextDrawer("ui/LevelFont");
            text.Handle = BaseSprite.SpriteHandle.CENTER;
            text.Initialize(Content);
            text.Text = "Donuts: " + gameplay.donutScore.TargetScore;
            text.Colour = Color.OrangeRed;
            text.UniformScale = ScaleFactor;
            text.PosX = 0;
            text.PosY = -0.025f * BaseGame.Get.BackBufferHeight;
            Components.Add(text);

            text = new TextDrawer("ui/LevelFont");
            text.Handle = BaseSprite.SpriteHandle.CENTER;
            text.Initialize(Content);
            text.Text = "Best: " + ChicksnVixensGame.Get.GetLevelState(gameplay.location, gameplay.levelNum).NumDonuts;
            text.UniformScale = ScaleFactor * 0.7f;
            text.PosX = 0;
            text.PosY = -0.1f * BaseGame.Get.BackBufferHeight;
            Components.Add(text);
        }
    }

    public class StarDrawer : MenuObj
    {
        public StarDrawer(int numstars)
            : base("ui/ui")
        {
            maxStars = numstars;
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("ui/ui_frames");
            CurrentFrame = "star";
            ResetDimensions();
            UniformScale = ScaleFactor;
            Star s;
            float ypos = 0.35f * BaseGame.Get.BackBufferHeight;
            switch (maxStars)
            {
                case 1:
                    s = new Star();
                    s.TargetPos = new Vector2(0, ypos);
                    stars.Add(s);
                    break;
                case 2:
                    s = new Star();
                    s.TargetPos = new Vector2(0.1f * BaseGame.Get.BackBufferWidth, ypos);
                    stars.Add(s);
                    
                    s = new Star();
                    s.TargetPos = new Vector2(-0.1f * BaseGame.Get.BackBufferWidth, ypos);
                    stars.Add(s);
                    break;
                case 3:
                    s = new Star();
                    s.TargetPos = new Vector2(0.1f * BaseGame.Get.BackBufferWidth, ypos);
                    stars.Add(s);
                    
                    s = new Star();
                    s.TargetPos = new Vector2(-0.1f * BaseGame.Get.BackBufferWidth, ypos);
                    stars.Add(s);

                    s = new Star();
                    s.TargetPos = new Vector2(0, ypos);
                    stars.Add(s);
                    break;
            }

            Colour = Color.Gold;
            for (int i = 0; i < stars.Count; i++)
                stars[i].PosX = stars[i].TargetPos.X;

            Dictionary<float, float> dic = new Dictionary<float, float>();
            dic.Add(-0.1f * BaseGame.Get.BackBufferWidth, BaseGame.Get.BackBufferHeight);
            dic.Add(0.0f * BaseGame.Get.BackBufferWidth, BaseGame.Get.BackBufferHeight * 2);
            dic.Add(0.1f * BaseGame.Get.BackBufferWidth, BaseGame.Get.BackBufferHeight * 3);
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].PosY = dic[stars[i].PosX];
            }
        }

        float fadeInTimer = 0.0f;
        int maxStars = 0;
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            Dictionary<float, float> dic = new Dictionary<float, float>();
            dic.Add(-0.1f * BaseGame.Get.BackBufferWidth, 3);
            dic.Add(0.0f * BaseGame.Get.BackBufferWidth, 3);
            dic.Add(0.1f * BaseGame.Get.BackBufferWidth, 3);
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].PosX = stars[i].TargetPos.X;
                stars[i].PosY = JabMath.MoveTowards(stars[i].PosY, stars[i].TargetPos.Y, 
                                                    gttf(dt) * dic[stars[i].PosX], 3);
            }
        }

        class Star : JabJect
        {
            public Star()
                : base()
            {
                PosY = BaseGame.Get.BackBufferHeight;
            }
            public float Alpha = 1.0f;
            public Vector2 TargetPos = Vector2.Zero;
        }
        List<Star> stars = new List<Star>();
        public override void Draw()
        {
            for (int i = 0; i < stars.Count; i++)
            {
                Position = stars[i].Position;
                base.Draw();
            }
        }
    }
}