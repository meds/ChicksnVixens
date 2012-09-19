using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using System.Text;

using Jabber.Util;
using Jabber.Media;
using Jabber.Scene;
using ChicksnVixens.Screens;
using ChicksnVixens;
using Jabber;

namespace ChicksnVixens
{
    public class LevelSelectCam : Camera
    {
        public LevelSelectCam()
            : base()
        {
            RaiseFlag(Jabber.Flags.FADE_IN);
            UniformWorldScale = 3;


            EventManager.Get.RegisterListner(this);

            RaiseFlag(Jabber.Flags.ACCEPTINPUT);
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);
            PosY = -13.0f * ScaleFactor;

            if (CheckFlag(Jabber.Flags.FADE_IN) && !CheckFlag(Jabber.Flags.FADE_OUT))
            {
                fadeTimer += gttf(dt) * 2.0f;
                if (fadeTimer > 1)
                {
                    fadeTimer = 1;
                }
                UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, 0.99f, gttf(dt) * 6);
                if (UniformWorldScale <= 1.0f)
                {
                    UniformWorldScale = 1.0f;
                    LowerFlag(Jabber.Flags.FADE_IN);
                }
            }
            else if (CheckFlag(Jabber.Flags.FADE_OUT))
            {
                LowerFlag(Jabber.Flags.FADE_IN);

                UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, 3.1f, gttf(dt) * 6);
                if (UniformWorldScale >= 3.0f)
                {
                    UniformWorldScale = 3.0f;
                    LowerFlag(Jabber.Flags.FADE_OUT);
                    OnFadeOutComplete();
                }
            }


            if (pressingDown)
            {
                xOffset = targetDrag * BaseGame.Get.BackBufferWidth;
            }
            else
            {
                xOffset = JabMath.MoveTowards(xOffset, targetDrag * BaseGame.Get.BackBufferWidth, gttf(dt) * 5);
            }
            Camera.Get.PosX = -xOffset;
        }


        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            base.OnDrag(lastPos, thispos);


            if (!pressingDown)
            {
                pressingDown = true;
                targetDrag = xOffset / BaseGame.Get.BackBufferWidth;
            }
            else
            {
                targetDrag += (thispos - lastPos).X;
            }
            if (targetDrag > 50.0f * ScaleFactor / BaseGame.Get.BackBufferWidth)
            {
                targetDrag = 50.0f * ScaleFactor / BaseGame.Get.BackBufferWidth;
            }
            if (-targetDrag > 50.0f * ScaleFactor / BaseGame.Get.BackBufferWidth + (NumPages - 1))
            {
                targetDrag = -(50.0f * ScaleFactor / BaseGame.Get.BackBufferWidth + (NumPages - 1));// NumPages - 1;
            }


        }

        public float NumPages
        {
            get
            {
                return (float)Math.Ceiling((double)(NumLevels / 20.0f));
            }
        }
        public int NumLevels = 1;
        public override void OnRelease(Vector2 pos)
        {
            base.OnRelease(pos);

            float dif = targetDrag - finalDragPos;
            targetDrag = JabMath.RoundToNearest(targetDrag * BaseGame.Get.BackBufferWidth + dif * 1400 * ScaleFactor, (int)(BaseGame.Get.BackBufferWidth)) / BaseGame.Get.BackBufferWidth;
            pressingDown = false;

            finalDragPos = targetDrag;
        }

        float fadeTimer = 0;

        float xOffset;
        float finalDragPos = 0;
        int gridWidth = 100;
        float currentDrag = 0;
        float targetDrag = 0;
        bool pressingDown = false;
    }
}
