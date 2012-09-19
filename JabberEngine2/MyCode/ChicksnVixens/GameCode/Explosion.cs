using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Jabber;
using ChicksnVixens.Screens;
using Jabber.GameScreenManager;
using Jabber.Util;
using Jabber.Media;
using Jabber.Physics;

namespace ChicksnVixens
{
    public class ExplosionEvent : Event
    {
    }
    public class Explosion : Sprite
    {
        public Explosion(float initialRadius, float radius, float maxRadiusScale, float power, JabWorld world, Vector2 pos)
            : base("chickens")
        {
            this.maxRadiusScale = maxRadiusScale;
            maxPower = power;
            Position = pos;
            this.World = world;
            maxRadius = radius;
            this.initialRadius = initialRadius;

            Rot = RandomFloatInRange(0, 2.0f * JabMath.PI);
        }
        float maxRadiusScale = 1.0f;
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            EventManager.Get.SendEvent(new ExplosionEvent());
            base.Initialize(Content);
            CreateFramesFromXML("Chickens_Frames");
            CurrentFrame = "bird-boom-00005";
            ResetDimensions();
            Width = Height = initialRadius;// JabMath.LinearInterpolate(initialRadius, maxRadius, Timer);
            UniformScale = 1.0f;

            Colour = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            
            int angleIterations = 16;
            for (int j = 0; j <= angleIterations; j++)
            {
                float part = (float)j / (float)angleIterations * 2.0f * (float)Math.PI;

                Vector2 dir = new Vector2((float)Math.Cos((float)part), (float)Math.Sin((float)part));
                dir.Normalize();
                RayCastHit hit = World.RayCast(Position, Position + dir * maxRadius);

                if (hit.actor != null && hit.Distance <= maxRadius && hit.actor.BodyState != JabActor.BodyType.STATIC)
                {
                    float totalDistProportion = (maxRadius - hit.Distance) / maxRadius;

                    if (hit.actor.UserData is Fox)
                    {
                        Fox f = hit.actor.UserData as Fox;
                        if (f.IsBox)
                        {
                            f.SwitchActiveBody();
                            f.AnimSprite.Animation = "Roll";
                        }
                    }

                    hit.actor.AddLinearImpulse(dir * 15 * totalDistProportion, hit.worldImpact);

                    if (hit.actor.UserData is BreakableBody)
                    {
                        (hit.actor.UserData as BreakableBody).TakeHit(totalDistProportion * 2.0f);
                    }
                }
            }
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            Timer += gttf(dt) * 5.0f;
            if (Timer > 1.0f)
            {
                RaiseFlag(Flags.DELETE);
                Timer = 1.0f;
            }
            Colour = Color.White * (1.0f - Timer);
            Width = Height = JabMath.MoveTowards(Width, maxRadius, gttf(dt) * 20.0f) * maxRadiusScale;
        }

        public override void Draw()
        {
            base.Draw();
        }

        float maxRadius = 300;
        float initialRadius = 0.0f;
        float Timer = 0.0f;
        float maxPower = 1.0f;

        JabWorld World;
    }
}
