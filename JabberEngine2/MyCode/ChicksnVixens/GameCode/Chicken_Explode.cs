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
using Jabber.Scene;

namespace ChicksnVixens
{
    public class Chicken_Explode : Chicken
    {
        public Chicken_Explode(ThisGamesScene world)
            : base(world)
        {
            RaiseFlag(Jabber.Flags.ACCEPTINPUT);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Body.LinearVelocity.Length() + Math.Abs(Body.AngularVelocity) < 0.00001f || !active)
            {
                idleTimer += gttf(gameTime);
                if (idleTimer > 0.1f)
                {
                    Explosion exp = new Explosion(10, 300, 1.0f, 1.0f, scene.World, Position);
                    Body.IgnoreRayCast = true;
                    exp.Initialize(Jabber.BaseGame.Get.Content);
                    scene.AddNode(exp);
                    RaiseFlag(Jabber.Flags.DELETE);
                    Body.IgnoreRayCast = false;

                    AudioQueue.PlayOnce("Sounds/Explosion");
                }
            }
        }
        float idleTimer = 0.0f;
        public override void OnPress(Vector2 pos)
        {
            base.OnPress(pos);

            if (active)
            {
                active = false;
                Body.IgnoreRayCast = true;
                /*
                int angleIterations = 32;
                for (int j = 0; j <= angleIterations; j++)
                {
                    float part = (float)j / (float)angleIterations * 2.0f * (float)Math.PI;

                    Vector2 dir = new Vector2((float)Math.Cos((float)part), (float)Math.Sin((float)part));
                    dir.Normalize();

                    RayCastHit hit = World.RayCast(Position, Position + dir * 1000);
                    if (hit.actor != null)
                    {
                        float totalDistProportion = hit.Distance / 1000.0f;
                        hit.actor.AddLinearImpulse(dir * 10 * totalDistProportion, hit.worldImpact);
                    }
                }*/

                Explosion exp = new Explosion(10, 300, 1.0f, 1.5f, scene.World, Position);
                Body.IgnoreRayCast = true;
                exp.Initialize(Jabber.BaseGame.Get.Content);
                scene.AddNode(exp);
                RaiseFlag(Jabber.Flags.DELETE);
                Body.IgnoreRayCast = false;

                AudioQueue.PlayOnce("Sounds/Explosion");
            }
        }
        /*

        public override void OnBreakableDestroyed(BreakableBody body)
        {
            if (body.Material == BreakableBody.BodyMaterial.GLASS)
            {
                Body.LinearVelocity *= 0.3f;
            }
            else if (body.Material == BreakableBody.BodyMaterial.WOOD)
            {
                Body.LinearVelocity *= 0.25f;
            }
            else if (body.Material == BreakableBody.BodyMaterial.CONCRETE)
            {
                Body.LinearVelocity *= 0.1f;
            }
        }
        */
        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            Body.Mass = 0.7f;
            AccelerateAnimation = "Accelerate_Boom";
        }
    }
}
