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
using Jabber;
using Jabber.Util.UI;
namespace ChicksnVixens.Screens
{
    class PauseScreenPusher : MenuObj
    {
        public PauseScreen screen;
        public PauseScreenPusher()
            : base("ui/ui")
        {
            RaiseFlag(Flags.ACCEPTINPUT);
        }

#if WINDOWS_PHONE || ANDROID
        public override void  OnBackPress()
        {
            screen.FadeMeOut();
        }
#endif

        public void ExitScreen()
        {
            screen.FadeMeOut();
            for (int j = 0; j < ScreenManager.Get.Screens.Count; j++)
            {
                if (ScreenManager.Get.Screens[j] is GameplayScreen)
                {
                    (ScreenManager.Get.Screens[j] as GameplayScreen).IsQuitting = true;
                    (ScreenManager.Get.Screens[j] as GameplayScreen).ScreenFadeOut();
                }
            }
        }

        public override void OnTap(Vector2 p)
        {
            base.OnTap(p);
        }

        public override void OnPress(Vector2 pos)
        {
            base.OnPress(pos);
            OnDrag(pos, pos);
        }

        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            base.OnDrag(lastPos, thispos);

            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i].text.Contains(thispos.X, thispos.Y))
                {
                    texts[i].isSelected = true;
                }
                else
                {
                    texts[i].isSelected = false;
                }
            }
        }

        public override void OnRelease(Vector2 pos)
        {
            base.OnRelease(pos);

            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i].text.Contains(pos.X, pos.Y))
                {
                    if (texts[i].text.Name == "Resume")
                    {
                        screen.FadeMeOut();
                    }
                    else if (texts[i].text.Name == "Restart")
                    {
                        screen.FadeMeOut();
                        for (int j = 0; j < ScreenManager.Get.Screens.Count; j++)
                        {
                            if (ScreenManager.Get.Screens[j] is GameplayScreen)
                            {
                                (ScreenManager.Get.Screens[j] as GameplayScreen).Restart();
                                AudioManager.PlayOnce("Sounds/Restart");
                            }
                        }
                        RaiseFlag(Flags.PASSRENDER);
                    }
                    else if (texts[i].text.Name == "Quit")
                    {
                        ExitScreen();
                    }
                    else if (texts[i].text.Name == "Settings")
                    {
                        Options o = new Options();
                        o.Initialize(BaseGame.Get.Content);
                        ScreenManager.Get.AddScreen(o);
                    }
                }

                texts[i].isSelected = false;
            }
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);
            CreateFramesFromXML("ui/ui_frames");

            CurrentFrame = "chickenincannon";
            ResetDimensions();
            UniformScale = ScaleFactor * 0.75f;
            PosX = chickenCurrentX;
            chickenCurrentX = -BaseGame.Get.BackBufferWidth - Width * ScaleX;
            chickenPosY = -BaseGame.Get.BackBufferHeight / 2.0f + Height * ScaleY / 2.0f;
            chickenTargetX = -BaseGame.Get.BackBufferWidth / 2.0f + Width * ScaleX * 0.35f;

            int num = 1;
            float dist = 200;
            {
                Button restart = new Button("ui/ui");
                restart.Initialize(Content);
                restart.CreateFramesFromXML("ui/ui_frames");
                restart.CurrentFrame = "doublearrow";
                restart.Name = "Resume";
                restart.Colour = Color.Green;
                restart.Effect = SpriteEffect.FLIPHORIZONTAL;
                restart.ResetDimensions();
                restart.Colour = Color.LightBlue;
                restart.PosX = -BaseGame.Get.BackBufferWidth - 1 * ScaleFactor - dist * ScaleFactor * num;
                restart.RegularScale = ScaleFactor / 2.0f;
                restart.ScaleOnHover = restart.RegularScale * 1.1f;
                restart.UniformScale = restart.RegularScale * 10.0f;
                restart.PosY = -BaseGame.Get.HalfBackBufferHeight * 0.5f;
                texts.Add(new MenuInst(restart));
            }
            ++num;
            dist = 1000;
            {
                Button restart = new Button("ui/ui");
                restart.Initialize(Content);
                restart.CreateFramesFromXML("ui/ui_frames");
                restart.CurrentFrame = "restart";
                restart.Name = "Restart";
                restart.ResetDimensions();
                restart.Colour = Color.LightGreen;
                restart.PosX = -BaseGame.Get.BackBufferWidth - 1 * ScaleFactor - dist * ScaleFactor * num;
                restart.RegularScale = ScaleFactor / 2.0f;
                restart.ScaleOnHover = restart.RegularScale * 1.1f;
                restart.UniformScale = restart.RegularScale * 60.0f;
                restart.PosY = -BaseGame.Get.HalfBackBufferHeight * 0.5f;
                texts.Add(new MenuInst(restart));
            }
            ++num;
            dist = 2000;
            {
                Button restart = new Button("ui/ui");
                restart.Initialize(Content);
                restart.CreateFramesFromXML("ui/ui_frames");
                restart.CurrentFrame = "settings";
                restart.Colour = Color.Red;
                restart.Name = "Settings";
                restart.PlaySFXOnRelease = "Sounds/PlayStateSelect";
                restart.ResetDimensions();
                restart.Colour = Color.White;
                restart.PosX = -BaseGame.Get.BackBufferWidth - 1 * ScaleFactor - dist * ScaleFactor * num;
                restart.RegularScale = ScaleFactor / 2.0f;
                restart.ScaleOnHover = restart.RegularScale * 1.1f;
                restart.UniformScale = restart.RegularScale * 85.0f;
                restart.PosY = -BaseGame.Get.HalfBackBufferHeight * 0.5f;
                texts.Add(new MenuInst(restart));
            }
            ++num;
            dist = 3000;
            {
                Button restart = new Button("ui/ui");
                restart.Initialize(Content);
                restart.CreateFramesFromXML("ui/ui_frames");
                restart.CurrentFrame = "quit";
                restart.PlaySFXOnRelease = "Sounds/PlayStateSelect";
                restart.Name = "Quit";
                restart.ResetDimensions();
                restart.Colour = Color.Red;
                restart.PosX = -BaseGame.Get.BackBufferWidth - 1 * ScaleFactor - dist * ScaleFactor * num;
                restart.RegularScale = ScaleFactor / 2.0f;
                restart.ScaleOnHover = restart.RegularScale * 1.1f;
                restart.UniformScale = restart.RegularScale * 220.0f;
                restart.PosY = -BaseGame.Get.HalfBackBufferHeight * 0.5f;
                texts.Add(new MenuInst(restart));
            }
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);
            if (CheckFlag(Flags.FADE_OUT) && fadeOutTimer != 0)
            {
                fadeOutTimer -= gttf(dt) * 6.0f;
                if (fadeOutTimer < 0)
                {
                    fadeOutTimer = 0.0f;
                    OnFadeOutComplete();
                }
            }
            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i].isSelected)
                {
                    texts[i].text.UniformScale = JabMath.MoveTowards(texts[i].text.UniformScale, ScaleFactor / 1.75f, gttf(dt) * textMoveSpeed, 0.01f);
                }
                else
                {
                    texts[i].text.UniformScale = JabMath.MoveTowards(texts[i].text.UniformScale, ScaleFactor / 2.0f, gttf(dt) * textMoveSpeed, 0.01f);
                }
            }
            chickenCurrentX = JabMath.MoveTowards(chickenCurrentX, chickenTargetX, gttf(dt) * chickenMoveSpeed);

            float targetWidth = BaseGame.Get.BackBufferWidth * 0.15f;
            currentWidth = JabMath.MoveTowards(currentWidth, targetWidth, gttf(dt) * widthMoveSpeed, 10 * ScaleFactor);


            for (int i = texts.Count - 1; i >= 0; i--)
            {
                texts[i].text.PosX = JabMath.MoveTowards(texts[i].text.PosX, textTargetX, gttf(dt) * textMoveSpeed);
                int mirror = texts.Count - i;
                texts[i].text.PosY = BaseGame.Get.HalfBackBufferHeight * mirror / texts.Count - 150.0f * ScaleFactor;
                texts[i].text.PosY *= 1.2f;
            }
        }        

        public override void Draw()
        {
            if (currentWidth > 0)
            {
                CurrentFrame = "whitecore";
                ResetDimensions();

                Height = BaseGame.Get.BackBufferHeight;
                Width = currentWidth;
                UniformScale = 1.0f;
                Handle = SpriteHandle.CENTERLEFT;
                Colour = Color.Black * 0.4f * fadeOutTimer;
                PosX = -BaseGame.Get.HalfBackBufferWidth;
                PosY = 0;
                base.Draw();
            }

            {
                Colour = Color.White * fadeOutTimer;
                Handle = SpriteHandle.CENTER;
                CurrentFrame = "chickenincannon";
                ResetDimensions();
                UniformScale = ScaleFactor * 0.75f;
                PosX = chickenCurrentX;
                PosY = chickenPosY;
            }


            for (int i = 0; i < texts.Count; i++)
            {
                Color c = texts[i].text.Colour;
                texts[i].text.Colour *= fadeOutTimer;
                texts[i].text.Draw();
                texts[i].text.Colour = c;
            }
        }
        float chickenCurrentX = -100;
        float chickenTargetX = -100;
        float chickenPosY = 100;
        float chickenMoveSpeed = 3;


        float currentWidth = 0;
        float widthMoveSpeed = 4.0f;

        float textTargetX = -BaseGame.Get.BackBufferWidth / 2.0f + 60.0f * ScaleFactor;
        float textMoveSpeed = 12.0f;


        class MenuInst
        {
            public MenuInst(MenuObj text)
            {
                this.text = text;
            }
            public MenuObj text;
            public bool isSelected = false;
        }
        List<MenuInst> texts = new List<MenuInst>();

        float fadeOutTimer = 1.0f;
    }

    public class PauseScreen : Screen
    {
        public PauseScreen()
            : base()
        {
            RaiseFlag(Flags.ACCEPTINPUT);
            EventManager.Get.RegisterListner(this);

            AudioManager.PlayOnce("Sounds/Pause");
        }


        public override void ProcessEvent(Event ev)
        {
            if (!IsTopScreen)
            {
                return;
            }
            base.ProcessEvent(ev);

            if ((ev is FadeOutEvent))
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i].StateFlag != Jabber.StateFlag.FADE_OUT_COMPLETE)
                    {
                        return;
                    }
                }

                RaiseFlag(Flags.DELETE);
            }
        }

        public override void OnTap(Vector2 pos)
        {
            if (!IsTopScreen)
            {
                return;
            }
            base.OnTap(pos);

            if (pos.X > 0.15f)
            {
                FadeMeOut();
            }
        }

        public void FadeMeOut()
        {
            if (!CheckFlag(Flags.FADE_OUT))
            {
                AudioManager.PlayOnce("Sounds/Unpause");
            }
            RaiseFlag(Flags.FADE_OUT);
            
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].RaiseFlag(Flags.FADE_OUT);
            }
        }

        public override void Update(GameTime dt)
        {
            if (!IsTopScreen)
            {
                return;
            }
            if (ScreenManager.Get.TopScreen != this)
            {
                return;
            }
            base.Update(dt);
        }

        float fadeOutTimer = 0;

        public override void OnRelease(Vector2 pos)
        {
            if (!IsTopScreen)
            {
                return;
            }
            base.OnRelease(pos);
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.Initialize(content);

            BlankNess n = new BlankNess();
            n.Initialize(Content);
            n.fadeSpeed = 3.0f;
            n.fullBlankity = 0.2f;
            Components.Add(n);


            PauseScreenPusher pauseScreenPusher = new PauseScreenPusher();
            pauseScreenPusher.Initialize(Content);
            pauseScreenPusher.screen = this;
            Components.Add(pauseScreenPusher);
        }
    }
}
