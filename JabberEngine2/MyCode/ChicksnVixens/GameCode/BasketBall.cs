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
using ChicksnVixens.Screens;
namespace ChicksnVixens
{
    public class BasketBall : PhysicSprite
    {
        public BasketBall()
            : base()
        {
            Layer = SpriteLayer.LAYER8;

            EventManager.Get.RegisterListner(this);
        }

        public bool IsTyre = false;

        public override void Draw()
        {
            base.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            lastPlayedTimer -= gttf(gameTime);
        }

        public override void Initialize(ContentManager Content)
        {
            Sprite = new Sprite("misc");
            AsType.CreateFramesFromXML("misc_frames");
            if (IsTyre)
            {
                AsType.CurrentFrame = "tyre";

                AsType.Initialize(Content);
                AsType.ResetDimensions();
                AsType.Dimension *= Scale;
                AsType.UniformScale = 1.0f;
                AsType.Rot = sprite.Rot;

                DoDimensions = false;

                Body = World.CreateSphere(AsType.Dimension.X / 2.0f, sprite.Position, JabActor.BodyType.DYNAMIC);
                Body.UserData = this;
                Body.Restitution = 0.3f;
                Body.Rot = 0;
                Body.LinearDamping = 0.2f;
                Body.AngularDamping = 0.1f;
                Body.Mass = 6.0f;
            }
            else
            {
                AsType.CurrentFrame = "basketball";

                AsType.Initialize(Content);
                AsType.ResetDimensions();
                AsType.Dimension *= Scale;
                AsType.UniformScale = 1.0f;
                AsType.Rot = sprite.Rot;

                DoDimensions = false;

                Body = World.CreateSphere(AsType.Dimension.X / 2.0f, sprite.Position, JabActor.BodyType.DYNAMIC);
                Body.UserData = this;
                Body.Restitution = 0.8f;
                Body.Rot = 0;
                Body.LinearDamping = 0.05f;
                Body.AngularDamping = 0.1f;
            }
           
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if ((ev is CollisionEvent) && (ev as CollisionEvent).ActorPresent(Body) != null && lastPlayedTimer < 0)
            {
                if (((ev as CollisionEvent).ActorPresent(Body).LinearVelocity - Body.LinearVelocity).Length() > 2.0f && !World.IsEntityGroup(((ev as CollisionEvent).ActorPresent(Body).CollisionGroup)))
                {
                    lastPlayedTimer = 0.5f;
                    AudioManager.PlayOnce("Sounds/basketball");
                }
            }
        }
        float lastPlayedTimer = 0.003f;
    }
}
