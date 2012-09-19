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
using Jabber.Physics;
using Jabber;

namespace ChicksnVixens
{
    public class FallingFeathers : Sprite
    {
        public FallingFeathers()
            : base("ui/ui")
        {
            RaiseFlag(Flags.FADE_IN);
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            Rot = RandomFloatInRange(0, 2.0f * JabMath.PI);

            CreateFramesFromXML("ui/ui_frames");
            CurrentFrame = "feather";
            ResetDimensions();
            UniformScale = ScaleFactor * 0.45f;// *2.5f;

            for (float y = Camera.Get.ScreenToWorld(Vector2.Zero).X + BaseGame.Get.BackBufferHeight * 3.0f; y > 0; y -= Height * ScaleY * 1.0f)
            {
                for (float i = Camera.Get.ScreenToWorld(Vector2.Zero).X; i < Camera.Get.ScreenToWorld(Vector2.One).X; i += 5 * ScaleFactor)// / 1.5f)
                {
                    if (RandomFloatInRange(0, 400 * ScaleFactor) < 10)
                    {
                        Inst s = new Inst();
                        s.PosX = i;
                        s.PosY = Camera.Get.ScreenToWorld(Vector2.Zero).Y + ScaleFactor * 2.5f + y;
                        s.InitialYPos = s.PosY;
                        feathers.Add(s);
                    }
                }
            }
            /*
            for (int i = -100; i < 100; i++)
            {
                Inst s = new Inst();
                s.PosX = i * Width * ScaleX;
                s.PosY = Camera.Get.ScreenToWorld(Vector2.Zero).Y + ScaleFactor * 2.5f; ;
                feathers.Add(s);
            }*/
            UniformScale = ScaleFactor *2.5f;
        }

        public class Inst : BaseSprite
        {
            public float Timer = 0;

            public Inst()
            {
                Rot = RandomFloatInRange(-10, 10);
                rotDir = RandomFloatInRange(-0.1f, 0.1f);
                scale = RandomFloatInRange(1.0f, 1.5f);

                Colour = new Color(RandomFloat, RandomFloat, RandomFloat, 1.0f);
            }
            float rotDir = 0;
            public float scale = 1.0f;
            public override void Update(GameTime dt)
            {
                base.Update(dt);

                PosY -= (gttf(dt) * 350 * ScaleFactor * (scale*scale)) * 0.7f;
                Rot += rotDir * gttf(dt) * 40.0f / scale;
            }

            public float InitialYPos = 0;
        }
        public float fadeInTimer = 0;
        bool firstDroppedBelow = false;
        List<Inst> feathers = new List<Inst>();
        
        public override void Draw()
        {
            //base.Draw();

            Handle = SpriteHandle.CENTER;
            CurrentFrame = "whitecore";

            Width = Camera.Get.HorizontalCoverSpace;
            Height = Camera.Get.VerticalCoverSpace;
            Position = Camera.Get.Position;
            Rot = 0.0f;
            Colour = Color.Black * fadeInTimer;
            base.Draw();
            /*

            Handle = SpriteHandle.TOPLEFT;
            
            for (int y = (int)Camera.Get.ScreenToWorld(Vector2.Zero).Y; y > (int)Camera.Get.ScreenToWorld(Vector2.One).Y; y -= (int)( 100 / Camera.Get.UniformWorldScale))
            {
                for (int i = (int)Camera.Get.ScreenToWorld(Vector2.Zero).X; i < (int)Camera.Get.ScreenToWorld(Vector2.One).X; i += (int)(100 / Camera.Get.UniformWorldScale))
                {
                    Rot = 0.0f;
                    Position = new Vector2(i, y);// Camera.Get.Position;
                    Width = 100.1f;// *Camera.Get.UniformWorldScale;// Camera.Get.HorizontalCoverSpace;
                    Height = 100.1f;// *Camera.Get.UniformWorldScale;// Camera.Get.VerticalCoverSpace;
                    UniformScale = 1.0f / Camera.Get.UniformWorldScale;
                    //UniformScale = ScaleFactor;
                    base.Draw();
                }
            }*/
            
            CurrentFrame = "feather";
            ResetDimensions();

            float heighestYPos = float.MinValue;

            for (int i = 0; i < feathers.Count; i++)
            {
                if (feathers[i].PosY > heighestYPos)
                {
                    heighestYPos = feathers[i].PosY;
                }
            }
            float top = Camera.Get.ScreenToWorld(Vector2.Zero).Y;
            float bottom = Camera.Get.ScreenToWorld(Vector2.One).Y;

            top += 200 * ScaleFactor;
            bottom -= 100 * ScaleFactor;
            for (int i = 0; i < feathers.Count; i++)
            {
                Vector2 camPos = Camera.Get.Position;// *Camera.Get.UniformWorldScale;
                //camPos.Y *= -1;
                Position = camPos + feathers[i].Position / Camera.Get.UniformWorldScale;

                Rot = feathers[i].Rot;
                UniformScale = ScaleFactor / Camera.Get.UniformWorldScale * 1.5f * feathers[i].scale;

                float alpha = 1.0f;
                if (CheckFlag(Flags.FADE_OUT) && !inRoutine)
                    Colour = feathers[i].Colour * featherFadeOutAlpha;
                else
                    Colour = feathers[i].Colour;
                if (bottom > PosY + UniformScale * Height && inRoutine)
                {
                    feathers[i].PosY = heighestYPos + Height * ScaleFactor * 0.5f;// feathers[i].InitialYPos / Camera.Get.UniformWorldScale;
                    firstDroppedBelow = true;
                }
                else if (bottom > PosY + UniformScale * Height && !inRoutine)
                {
                    feathers.Remove(feathers[i]); --i;
                }
                else if (PosY > top && !inRoutine)
                {
                    feathers.Remove(feathers[i]); --i;
                }

                if (PosY < top && PosY > bottom)
                    base.Draw();
            }
        }

        bool inRoutine = true;
        float featherFadeOutAlpha = 1.0f;
        public override void  Update(GameTime dt)
        {
            inRoutine = !CheckFlag(Flags.FADE_OUT);

            if (!inRoutine && !firstDroppedBelow)
            {
                inRoutine = true;
            }
            if (fadeInTimer != 1 && CheckFlag(Flags.FADE_IN))
            {
                fadeInTimer = JabMath.MoveTowards(fadeInTimer, 1.0f, gttf(dt) * 6, 0.01f);
                if (fadeInTimer == 1.0f)
                {
                    OnFadeInComplete();
                }
            }
            else if (fadeInTimer != 0 && CheckFlag(Flags.FADE_OUT) && !inRoutine)
            {
                fadeInTimer = JabMath.MoveTowards(fadeInTimer, 0.0f, gttf(dt) * 3, 0.01f);
            }
            if (CheckFlag(Flags.FADE_OUT) && feathers.Count == 0)
            {
                OnFadeOutComplete();
                RaiseFlag(Flags.DELETE);
            }
            for(int i = 0; i < feathers.Count; i++)
            {
                feathers[i].Update(dt);
            }
            if (CheckFlag(Flags.FADE_OUT) && !inRoutine)
            {
                featherFadeOutAlpha -= gttf(dt) / 5.0f * ScaleFactor;
                if (featherFadeOutAlpha < 0)
                {
                    featherFadeOutAlpha = 0;
                }
            }
            if (StateFlag == Jabber.StateFlag.FADE_OUT_COMPLETE)
            {
                LowerFlag(Flags.FADE_IN);
                fadeInTimer = JabMath.MoveTowards(fadeInTimer, 0.0f, gttf(dt) * 3, 0.01f);
            }
        }
    }
}
