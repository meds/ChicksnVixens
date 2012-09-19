using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
//using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

using Jabber.Util;
using Jabber.Media;
//using Jabber.GameStateManagement;

namespace Jabber.GameScreenManager
{
    public class Screen : JabJect
    {
        public class ScreenCancelEvent : Event
        {
            public ScreenCancelEvent(Screen s)
            {
                Screen = s;
            }

            public Screen Screen;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].UnloadContent();
            }

            Components.Clear();
        }

        public class ScreenFadeOutEvent : Event
        {
            public ScreenFadeOutEvent(Screen s)
            {
                Screen = s;
            }

            public Screen Screen;
        }

        public class ScreenFadeOutCompleteEvent : Event
        {
            public ScreenFadeOutCompleteEvent(Screen s)
            {
                Screen = s;
            }

            public Screen Screen;
        }

        public class ScreenFadeInCompleteEvent : Event
        {
            public ScreenFadeInCompleteEvent(Screen s)
            {
                Screen = s;
            }

            public Screen Screen;
        }

        public class ScreenFadeInEvent : Event
        {
            public ScreenFadeInEvent(Screen s)
            {
                Screen = s;
            }

            public Screen Screen;
        }


        public override void OnFadeOutComplete()
        {
            base.OnFadeOutComplete();
            EventManager.Get.SendEvent(new ScreenFadeOutCompleteEvent(this));
            LowerFlag(Flags.FADE_OUT);
        }

        public override void OnFadeInComplete()
        {
            base.OnFadeInComplete();
            EventManager.Get.SendEvent(new ScreenFadeInCompleteEvent(this));
            LowerFlag(Flags.FADE_IN);
        }


        public  virtual void ScreenFadeOut()
        {
            if (CheckFlag(Flags.FADE_OUT))
            {
                throw new InvalidOperationException();
            }
            RaiseFlag(Flags.FADE_OUT);
            EventManager.Get.SendEvent(new ScreenFadeOutEvent(this));

           // for (int i = 0; i < Components.Count; i++)
            {
            //    Components[i].RaiseFlag(Flags.FADE_OUT);
            }
        }
        public void ScreenFadeIn()
        {
            if (CheckFlag(Flags.FADE_IN))
            {
                throw new InvalidOperationException();
            }
            RaiseFlag(Flags.FADE_IN);
            EventManager.Get.SendEvent(new ScreenFadeInEvent(this));
        }

        public Screen()
            : base()
        {
            Content = BaseGame.Get.Content;//new ContentManager(BaseGame.Get.Services);
            //Content.RootDirectory = "Content";

            RaiseFlag(Flags.ACCEPTINPUT);
        }

        public bool IsTopScreen
        {
            get
            {
                return ScreenManager.Get.TopScreen == this;
            }
        }
        
        public override void Draw()
        {
            base.Draw();
            foreach (JabJect comp in Components)
            {
                comp.Draw();
            }
        }
        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);

            initialized = true;
        }
        public override void Update(GameTime dt)
        {
            base.Update(dt);
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.DELETE))
                {
                    Components[i].UnloadContent();
                    Components.RemoveAt(i); --i;
                }
                else
                {
                    Components[i].Update(dt);
                }
            }

            HandleInput(InputManager.Get);
        }
        protected ContentManager Content;

        bool initialized = false;
        public bool Initialized
        {
            get { return initialized; }
        }


        public void AddComponent(JabJect comp)
        {
            Components.Add(comp);
        }
        protected List<JabJect> Components = new List<JabJect>();

        /// <summary>
        /// Every screen has its own camera which is set to be the default camera
        /// by the screen manager before draw and update calls
        /// </summary>
        public Camera screenCamera = new Camera();


        #region inputs

        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            if (screenCamera.CheckFlag(Flags.ACCEPTINPUT))
            {
                screenCamera.OnDrag(lastPos, thispos);
            }

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Components[i].OnDrag(lastPos, thispos);
                }
            }
        }

#if WINDOWS
        public override void OnMouseScroll(int delta)
        {
            if (screenCamera.CheckFlag(Flags.ACCEPTINPUT))
            {
                screenCamera.OnMouseScroll(delta);
            }


            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Components[i].OnMouseScroll(delta);
                }
            }
        }
#endif

#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Components[i].OnBackPress();
                }
            }
        }
#endif

        public override void OnTap(Vector2 pos)
        {
            if (screenCamera.CheckFlag(Flags.ACCEPTINPUT))
            {
                screenCamera.OnTap(pos);
            }


            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Components[i].OnTap(pos);
                }
            }
        }
        public override void OnRelease(Vector2 pos)
        {
            if (screenCamera.CheckFlag(Flags.ACCEPTINPUT))
            {
                screenCamera.OnRelease(pos);
            }

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Components[i].OnRelease(pos);
                }
            }
        }
        public override void OnMove(Vector2 pos)
        {
            if (screenCamera.CheckFlag(Flags.ACCEPTINPUT))
            {
                screenCamera.OnMove(pos);
            }

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Components[i].OnMove(pos);
                }
            }
        }
        public override void OnPress(Vector2 pos)
        {
            if (screenCamera.CheckFlag(Flags.ACCEPTINPUT))
            {
                screenCamera.OnPress(pos);
            }

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Components[i].OnPress(pos);
                }
            }
        }
        #endregion
    }

    public class MenuScreen : Screen
    {
        public MenuScreen(string bgdir)
            : base()
        {
            this.bgdir = bgdir;
        }

        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);

            bgSprite = new Sprite(bgdir);
            bgSprite.Initialize(content);
            bgSprite.DrawIn = BaseSprite.DrawSpace.SCREENSPACE;
            bgSprite.Handle = BaseSprite.SpriteHandle.TOPLEFT;

            bgSprite.Width = bgSprite.Height = 1.0f;
        }

        public override void Draw()
        {
            //bgSprite.Draw();
            base.Draw();
        }

        private string bgdir;
        private Sprite bgSprite = null;
    }
}