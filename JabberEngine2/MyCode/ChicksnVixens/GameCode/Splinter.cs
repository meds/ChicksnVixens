using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabber.Physics;
using Jabber.Media;
using Jabber.Scene;
using Microsoft.Xna.Framework;
using Jabber.Util;
using Jabber;

namespace ChicksnVixens
{
    public class Splinter : Sprite
    {
        public Splinter(BreakableBody.BodyMaterial material)
            : base("break")
        {
            bodyMaterial = material;
        }
        public float ScaleMultiplier = 1;
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.Initialize(content);
            spinRight = BaseGame.Random.Next(1, 3) == 1;
            CreateFramesFromXML("break_frames");
            CurrentFrame = RandomCorrectMaterialFrame;// "wood_broken";
            ResetDimensions();

            fudgeDir = new Vector2(RandomFloatInRange(-1, 1), RandomFloatInRange(-1, 0.0f));
            fudgeDir.Normalize();

            initialScale = RandomFloatInRange(0.8f, 1.1f);
            UniformScale = 1.0f;
            if (bodyMaterial == BreakableBody.BodyMaterial.CONCRETE)
            {
                variableFallSpeedScaler = 0.1f;
            }
            else
            {
                variableFallSpeedScaler = 0.2f;
            }
        }
        float initialScale = 1.0f;

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (fadeTimer == 0)
            {
                fadeTimer = 0;
            }


            if (spinRight)
            {
                Rot += gttf(dt);
            }
            else
            {
                Rot -= gttf(dt);
            }

            colour = Color.White * fadeTimer;
            UniformScale = fadeTimer * initialScale * ScaleMultiplier;
            Position += fudgeDir * gttf(dt) * 1000.0f * variableFallSpeedScaler;
            fadeTimer -= gttf(dt);// / 2.0f;
            if (fadeTimer < 0)
            {
                fadeTimer = 0;
            }

            fallTimer += gttf(dt) * 10.0f;
            PosY -= gttf(dt) * 40.0f * fallTimer * variableFallSpeedScaler + initialScale * 3.0f;
        }



        string RandomCorrectMaterialFrame
        {
            get
            {
                string ret = "";

                switch (bodyMaterial)
                {
                    case BreakableBody.BodyMaterial.WOOD:
                        ret = "wood_splinter_" + BaseGame.Random.Next(1, 1).ToString();
                        break;
                    case BreakableBody.BodyMaterial.GLASS:
                        ret = "glass_crack_" + BaseGame.Random.Next(1, 4).ToString();
                        break;
                    case BreakableBody.BodyMaterial.CONCRETE:
                        ret = "rock_bit_" + BaseGame.Random.Next(1, 1).ToString();
                        break;
                }

                return ret;
            }
        }

        float variableFallSpeedScaler = 1;
        float fadeTimer = 1.0f;
        float fallTimer = 0;
        Vector2 fudgeDir = Vector2.Zero;
        bool spinRight = false;
        BreakableBody.BodyMaterial bodyMaterial;
    }
}
