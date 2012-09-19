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
using Jabber.Util.UI;

namespace ChicksnVixens
{
    public class VolumeEvent : MenuEvent
    {
        public VolumeEvent(JabJect sender, float vol)
            : base(sender, MenuEvent.EventType.NA)
        {
            SetVolume = vol;
        }

        public float SetVolume = 0;
    }
    public class VolumeControl : MenuObj
    {
        public VolumeControl(float vol)
            : base("ui/ui")
        {
            CurrentVolume = vol;
            RaiseFlag(Flags.ACCEPTINPUT);
        }
        public bool Disabled = false;
        public bool PlayTickWhenDone = false;
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);
            CreateFramesFromXML("ui/ui_frames");
        }

        public override void OnPress(Vector2 pos)
        {
            if (Disabled)
            {
                return;
            }
            base.OnPress(pos);

            Width = 0.45f * BaseGame.Get.BackBufferWidth;
            float maxLeft = VolMaxLeft;
            CurrentFrame = "volume_handle";
            PosX = VolHandleXPos;
            ResetDimensions();
            UniformScale = ScaleFactor;

            IsPressedOn = Contains(pos.X, pos.Y);
        }

        public override void OnRelease(Vector2 pos)
        {
            base.OnRelease(pos);

            if (IsPressedOn && PlayTickWhenDone)
            {
                AudioManager.PlayOnce("Sounds/EggLay");
            }
            IsPressedOn = false;
        }

        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            base.OnDrag(lastPos, thispos);


            if (IsPressedOn)
            {
                ScreenSpaceToVolume(thispos.X);

                EventManager.Get.SendEvent(new VolumeEvent(this, CurrentVolume));
            }
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);
            GlobalXFudge = 55.0f / 2.0f * ScaleFactor;
            return;

            if (!CheckFlag(Flags.FADE_OUT))
            {
                GlobalXFudge = JabMath.MoveTowards(GlobalXFudge, 55.0f / 2.0f * ScaleFactor, gttf(dt) * 2.5f, 1.5f);

                float totalDistToTravel = (-100.0f / 2.0f * ScaleFactor) - (55.0f / 2.0f * ScaleFactor);
                totalDistToTravel = Math.Abs(totalDistToTravel);

                float distTravelled = (-100.0f / 2.0f * ScaleFactor) - GlobalXFudge;
                distTravelled = Math.Abs(distTravelled);

                Colour = Color.White * (distTravelled / totalDistToTravel);
            }
            else
            {
                GlobalXFudge = JabMath.MoveTowards(GlobalXFudge, -100.0f / 2.0f * ScaleFactor, gttf(dt) * 2.5f, 1.5f);

                float totalDistToTravel = (-100.0f / 2.0f * ScaleFactor) - (55.0f / 2.0f * ScaleFactor);
                totalDistToTravel = Math.Abs(totalDistToTravel);

                float distTravelled = (-100.0f / 2.0f * ScaleFactor) - GlobalXFudge;
                distTravelled = Math.Abs(distTravelled);

                Colour = Color.White * (1.0f - (distTravelled / totalDistToTravel));
            }
        }
        
        public override void Draw()
        {
            if (Disabled)
            {
                Colour = Color.Gray;
            }
            CurrentFrame = "volume_bar";
            ResetDimensions();
            UniformScale = ScaleFactor;
            PosX = VolBarZero;
            Width = 0.45f * BaseGame.Get.BackBufferWidth;
            base.Draw();

            float maxLeft = VolMaxLeft;

            CurrentFrame = "speaker_icon";
            ResetDimensions();
            UniformScale = ScaleFactor;
            Handle = SpriteHandle.CENTERRIGHT;
            PosX = VolMaxLeft - Width * ScaleFactor / 2.0f;
            base.Draw();

            Handle = SpriteHandle.CENTER;
            CurrentFrame = "volume_handle";
            PosX = VolHandleXPos;
            ResetDimensions();
            UniformScale = ScaleFactor;
            base.Draw();
        }

        float VolHandleXPos
        {
            get
            {
                return VolMaxLeft + CurrentVolume * VolBarWidth;
            }
        }

        void ScreenSpaceToVolume(float val)
        {
            val = Camera.Get.ScreenToWorld(new Vector2(val, 0)).X;
            if (val < VolMaxLeft)
            {
                CurrentVolume = 0;
            }
            else if (val > VolMaxRight)
            {
                CurrentVolume = 1.0f;
            }
            else
            {
                float left = VolMaxLeft;
                float right = VolMaxRight;
                right -= left;
                val -= left;

                val /= right;
                CurrentVolume = val;
            }
        }
        float VolBarZero
        {
            get
            {
                return GlobalXFudge;
            }
        }
        float VolBarWidth
        {
            get
            {
                return 0.45f * BaseGame.Get.BackBufferWidth * ScaleFactor;
            }
        }

        float VolMaxLeft
        {
            get
            {
                PosX = 0;
                float w = 0.45f * BaseGame.Get.BackBufferWidth;
                float maxLeft = PosX - w * ScaleFactor / 2.0f;
                return maxLeft + GlobalXFudge;
            }
        }

        float VolMaxRight
        {
            get
            {
                PosX = 0;
                float w = 0.45f * BaseGame.Get.BackBufferWidth;
                float maxRight = PosX + w * ScaleFactor / 2.0f;
                return maxRight + GlobalXFudge;
            }
        }

        public float CurrentVolume { get; set; }
        bool IsPressedOn = false;
        float GlobalXFudge = -100.0f / 2.0f * ScaleFactor;

    }
}
