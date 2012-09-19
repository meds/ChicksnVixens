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

using Jabber.Physics;
using Jabber.GameScreenManager;
using Jabber.Util.UI;

namespace ChicksnVixens
{
    public class BlankNess : MenuObj
    {
        public BlankNess()
            : base("ui/ui")
        {
            Layer = SpriteLayer.BACKGROUND_LAYER6;
        }

        public float fullBlankity = 0.5f;
        public float fadeSpeed = 2.0f;
        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("ui/ui_frames");
            CurrentFrame = "whitecore";
            Colour = Color.Black;
            ResetDimensions();
            Width = BaseGame.Get.BackBufferWidth;
            Height = BaseGame.Get.BackBufferHeight;
            Dimension *= 3.0f;
            Position = Vector2.Zero;
            UniformScale = 1.0f;
        }
        public float fadeInTimer = 0.0f;

        public override void Draw()
        {
            if (!update)
            {
                return;// int k = 0;
            }

            base.Draw();
        }
        bool update = false;
        public override void Update(GameTime dt)
        {
            update = true;
            base.Update(dt);

            if (CheckFlag(Flags.FADE_OUT))
            {
                fadeInTimer -= gttf(dt) * fadeSpeed;
                if (fadeInTimer < 0)
                {
                    fadeInTimer = 0.0f;
                    SetStateFlag(StateFlag.FADE_OUT_COMPLETE);
                    OnFadeOutComplete();
                }
            }
            else if(CheckFlag(Flags.FADE_IN))
            {
                fadeInTimer += gttf(dt) * fadeSpeed;
                if (fadeInTimer > 1)
                {
                    fadeInTimer = 1.0f;
                    StateFlag = Jabber.StateFlag.FADE_IN_COMPLETE;
                }
            }

            Colour = Color.Black * fadeInTimer * fullBlankity;

           // Position = Camera.Get.Position;
           // Scale = Vector2.One / Camera.Get.WorldScale;
        }
    }
}