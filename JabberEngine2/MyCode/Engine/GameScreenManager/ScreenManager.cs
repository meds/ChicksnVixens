using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

using Jabber;
using Jabber.Media;
using Jabber.Util;
using Jabber.Physics;
//using Jabber.GameStateManagement;
using Jabber.Scene;
using Jabber.J3D;
using System.Xml.Linq;
using System.Diagnostics;

using Microsoft.Phone.Tasks;

using Jabber.Util.UI;

namespace Jabber.GameScreenManager
{
    public class ScreenEvent : MenuEvent
    {
        public ScreenEvent(Screen s):base(s, EventType.RELEASE)
        {
            this.Screen = s;
        }

        public Screen Screen;
    }

    public class ScreenAddedEvent : ScreenEvent
    {
        public ScreenAddedEvent(Screen s) : base(s) { }
    }

    public class ScreenRemovedEvent : ScreenEvent
    {
        public ScreenRemovedEvent(Screen s) : base(s) { }
    }

    public class ScreenManager : JabJect
    {
        #region singleton
        static ScreenManager manager = null;
        public static ScreenManager Get
        {
            get{
                if(manager == null)
                {
                    manager = new ScreenManager();
                    manager.Initialize(BaseGame.Get.Content);
                }
                return manager;
            }
        }
        #endregion

        public void AddScreen(Screen screen)
        {
            lsScreens.Add(screen);
            EventManager.Get.SendEvent(new ScreenAddedEvent(screen));
        }

     //   Sprite s;
        public override void Draw()
        {
           // if (s == null)
            {
                /*
                s = new Sprite("textures/backgrounds/paris/paris");
                s.Initialize(BaseGame.Get.Content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
                s.CurrentFrame = "rawbg";
                s.ResetDimensions();*/
            }
			BaseGame.Get.GraphicsDevice.Clear(Color.Black);
            for (int i = 0; i < lsScreens.Count; i++)
            {
                Screen screen = lsScreens[i];
                if (!screen.CheckFlag(Flags.PASSRENDER) && screen.Initialized)
                {
                    Camera.CurrentCamera = screen.screenCamera;
					BaseGame.Get.Begin();
					
					screen.Draw();
                    //s.Draw();
					
					BaseGame.Get.End();
					
                    Camera.CurrentCamera = null;
                }
            }

            base.Draw();
        }

        public Screen TopScreen
        {
            get
            {
                if (lsScreens.Count == 0)
                {
                    return null;
                }
                else
                {
                    return lsScreens[lsScreens.Count - 1];
                }
            }
        }

        public List<Screen> Screens
        {
            get
            {
                return lsScreens;
            }
        }

        public override void Update(GameTime dt)
        {
            for (int i = 0; i < lsScreens.Count; i++)
            {
                Screen screen = lsScreens[i];
                if (!screen.CheckFlag(Flags.PASSUPDATE) && screen.Initialized)
                {
                    Camera.CurrentCamera = screen.screenCamera;
                    screen.screenCamera.Update(dt);
                    screen.Update(dt);
                    Camera.CurrentCamera = null;
                }

            }

            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.DELETE))
                {
                    lsScreens[i].UnloadContent();
                    EventManager.Get.SendEvent(new ScreenRemovedEvent(lsScreens[i]));
                    lsScreens.RemoveAt(i); --i;
                }
            }
            base.Update(dt);
        }


        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Camera.CurrentCamera = lsScreens[i].screenCamera;
                    lsScreens[i].OnDrag(lastPos, thispos);
                }
            }
        }

#if WINDOWS
        public override void OnMouseScroll(int delta)
        {
            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Camera.CurrentCamera = lsScreens[i].screenCamera;
                    lsScreens[i].OnMouseScroll(delta);
                }
            }
        }
#endif

        public override void OnTap(Vector2 pos)
        {
            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Camera.CurrentCamera = lsScreens[i].screenCamera;
                    lsScreens[i].OnTap(pos);
                }
            }
        }
        public override void OnRelease(Vector2 pos)
        {
            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Camera.CurrentCamera = lsScreens[i].screenCamera;
                    lsScreens[i].OnRelease(pos);
                }
            }
        }

#if WINDOWS_PHONE || ANDROID
        public override void  OnBackPress()
        {
            base.OnBackPress();

            if (lsScreens.Count > 0)
            {
                lsScreens.Last<Screen>().OnBackPress();
            }
            /*
            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Camera.CurrentCamera = lsScreens[i].screenCamera;
                    lsScreens[i].OnBackPress();
                    break;
                }
            }
            */
        }
#endif

        public override void OnMove(Vector2 pos)
        {
            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Camera.CurrentCamera = lsScreens[i].screenCamera;
                    lsScreens[i].OnMove(pos);
                }
            }
        }
        public override void OnPress(Vector2 pos)
        {
            for (int i = 0; i < lsScreens.Count; i++)
            {
                if (lsScreens[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Camera.CurrentCamera = lsScreens[i].screenCamera;
                    lsScreens[i].OnPress(pos);
                }
            }
        }

        List<Screen> lsScreens = new List<Screen>();
    }
}
