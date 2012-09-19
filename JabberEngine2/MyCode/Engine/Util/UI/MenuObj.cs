using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

using Jabber.Util;
using Jabber.Media;

namespace Jabber.Util.UI
{
    public class MenuEvent : Event
    {
        public MenuEvent(EventType t) : base() { Type = t; }
        public MenuEvent(JabJect sender, EventType t) : base() { this.sender = sender; Type = t; }
        public JabJect sender = null;

        public enum EventType
        {
            HOVEROVER,
            TAP,
            HOLD,
            RELEASE,
            NA
        }
        public EventType Type = EventType.HOLD;
    }

    public class MenuObj : Sprite
    {
        public MenuObj(string imagedir):base(imagedir)
        {
            RaiseFlag(Flags.ACCEPTINPUT);
        }

        public override JabRectangle GetRectangle()
        {
            SetScaledPos();

            JabRectangle rect = base.GetRectangle();

            SetRegularPos();


            return rect;
        }

        public string Name { get; set; }


        public Vector2 ScaledPos
        {
            get
            {
                if (inScaledPos)
                {
                    return position;
                }

                return Camera.Get.Position + Position / Camera.Get.UniformWorldScale;
            }
        }

        public float ScaledScale
        {
            get
            {
                return UniformScale / Camera.Get.UniformWorldScale;
            }
        }

        void SetScaledPos()
        {
            if (inScaledPos)
            {
                return;
            }
            inScaledPos = true;

            oldScale = UniformScale;
            oldPos  = Position;

            UniformScale /= Camera.Get.UniformWorldScale;
            Position = Camera.Get.Position + Position / Camera.Get.UniformWorldScale;
        }

        void SetRegularPos()
        {
            if (!inScaledPos)
            {
                return;
            }
            inScaledPos = false;

            UniformScale = oldScale;
            Position = oldPos;
        }

        bool inScaledPos = false;
        Vector2 oldPos = Vector2.Zero;
        float oldScale = 1.0f;

        public override void OnTap(Vector2 p)
        {
             JabRectangle rect = GetRectangle();

            if (Contains(p.X, p.Y))
            {
                MenuEvent ev = new MenuEvent(MenuEvent.EventType.TAP);
                ev.sender = this;
                EventManager.Get.SendEvent(ev);
            }
        }

        public override void Draw()
        {
            SetScaledPos();

            base.Draw();

            SetRegularPos();
        }

        
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void HandleInput(InputManager input)
        {
			/*
            Rectangle rect = GetRectangle();
            foreach (GestureSample gest in input.Gestures)
            {
                if (gest.GestureType == GestureType.Tap || gest.GestureType == GestureType.Hold)
                {
                    if (Contains((int)gest.Position.X, (int)gest.Position.Y))
                    {
                        MenuEvent ev = new MenuEvent();
                        ev.sender = this;
                        EventManager.Get.SendEvent(ev);
                    }
                }
            }
            */
        }
        public override void Update(Microsoft.Xna.Framework.GameTime dt)
        {
            base.Update(dt);
        }
    }



    public class Button : MenuObj
    {
        public Button(string dir)
            : base(dir)
        {
        }

        public override void OnPress(Vector2 pos)
        {
            base.OnPress(pos);

            if (Contains(pos.X, pos.Y))
                Selected = true;
            else
                Selected = false;
        }
        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            base.OnDrag(lastPos, thispos);

            if (Contains(thispos.X, thispos.Y))
                Selected = true;
            else
                Selected = false;
        }

        public override void OnRelease(Vector2 pos)
        {
            base.OnRelease(pos);

            if (Contains(pos.X, pos.Y))
            {
                EventManager.Get.SendEvent(new MenuEvent(this, MenuEvent.EventType.RELEASE));

                if (PlaySFXOnRelease != "")
                {
                    AudioManager.PlayOnce(PlaySFXOnRelease);
                }
            }
            Selected = false;
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (Selected)
            {
                scaleTimer += gttf(dt) * ScaleSpeed;
                if (scaleTimer > 1)
                    scaleTimer = 1.0f;

                UniformScale = JabMath.MoveTowards(UniformScale, ScaleOnHover, gttf(dt) * 5.0f, 0.01f);
            }
            else
            {
                scaleTimer -= gttf(dt) * ScaleSpeed;
                if (scaleTimer < 0)
                    scaleTimer = 0.0f;

                UniformScale = JabMath.MoveTowards(UniformScale, RegularScale, gttf(dt) * 5.0f, 0.01f);
            }

            //UniformScale = JabMath.LinearInterpolate(RegularScale, ScaleOnHover, scaleTimer);
        }

        public override void Draw()
        {
            base.Draw();

            if (text != null)
            {
                text.UniformScale = UniformScale * TextScaler;
                text.Position = Position;
                text.Draw();
            }
        }

        public void SetText(string t, string fontdir)
        {
            text = new TextDrawer(fontdir);
            text.Initialize(BaseGame.Get.Content);
            text.Text = t;
        }

        public float TextScaler = 1.0f;

        public TextDrawer Text
        {
            get
            {
                return text;
            }
        }

        public string PlaySFXOnRelease = "";
        public float ScaleOnHover = 1.1f * ScaleFactor;
        public float RegularScale = 1.0f * ScaleFactor;
        public float ScaleSpeed = 4.0f;
        bool Selected = false;
        float scaleTimer = 0.0f;

        TextDrawer text;
    }
}