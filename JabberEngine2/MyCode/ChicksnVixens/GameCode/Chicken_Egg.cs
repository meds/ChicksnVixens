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
    public class Chicken_Egg : Chicken
    {
        public Chicken_Egg(ThisGamesScene world)
            : base(world)
        {
            RaiseFlag(Jabber.Flags.ACCEPTINPUT);

            this.scene = world;
        }
        GameScene scene;

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            AccelerateAnimation = "Accelerate_Egg";
        }

        public override void OnPress(Vector2 pos)
        {
            base.OnPress(pos);

            if (active)
            {
                active = false;

                Egg e = new Egg(Position, scene);
                e.Initialize(Jabber.BaseGame.Get.Content);
                scene.AddNode(e);

                Body.AddLinearImpulse(new Vector2(0, 13));
                AudioQueue.PlayOnce("Sounds/EggLay");
                Deactivate();
            }
        }
    }

    public class Egg : PhysicSprite
    {
        public Egg(Vector2 pos, GameScene scene)
            : base(64.0f, pos, true, scene.World, "chickens")
        {
            EventManager.Get.RegisterListner(this);
            this.scene = scene;
        }
        GameScene scene;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            distTravelled += Body.LinearVelocity.Length();
            if (AsType.UniformScale < 0.69f)
            {
                AsType.UniformScale = JabMath.MoveTowards(AsType.UniformScale, 0.7f, 0.25f);
            }
            else
            {
                AsType.UniformScale = 0.7f;
            }
            if (distTravelled > 32)
            {
                Body.CollisionGroup = 0;
            }

            if (destructionTimer > 0)
            {
                destructionTimer -= gttf(gameTime);
                if (destructionTimer < 0)
                {
                    RaiseFlag(Jabber.Flags.DELETE);


                    Explosion exp = new Explosion(10, 300, 1.0f, 0.7f, scene.World, Position);
                    Body.IgnoreRayCast = true;
                    exp.Initialize(Jabber.BaseGame.Get.Content);
                    exp.Position = Position;
                    scene.AddNode(exp);
                    RaiseFlag(Jabber.Flags.DELETE);
                    Body.IgnoreRayCast = false;

                    AudioQueue.PlayOnce("Sounds/Explode_Chicken");
                }
                else
                {
                    float val = destructionTimer;
                    Colour = new Color(1.0f, val, val, 1.0f);

                   // AsType.UniformScale = (float)Math.Sin(destructionTimer * 5.0f * JabMath.PI) + 1;

                    AsType.UniformScale = JabMath.LinearInterpolate(0.8f, 0.85f, (float)Math.Sin(destructionTimer * 5.0f * JabMath.PI) + 1);
                    DoDimensions = false;
                    int k = 0;
                }
            }
            if (Body.LinearVelocity.Length() < 0.001f)
            {
                noMoveTimer += gttf(gameTime);
                if (noMoveTimer > 1.0f && destructionTimer < 0)
                {
                    destructionTimer = 1.0f;
                }
            }
            else
            {
                noMoveTimer = 0.0f;
            }
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is CollisionEvent)
            {
                JabActor other = (ev as CollisionEvent).ActorPresent(Body);
                if (other != null)
                {
                    if (!other.CheckFlag(Jabber.Flags.DELETE))
                        if ((ev as CollisionEvent).ActorPresent(Body) != null && other.CollisionGroup != Fox.FOX_NONE_COLLISION_GROUP && other.CollisionGroup != Fan.FanEntitySpaceGroup && !World.IsEntityGroup(Body.CollisionGroup) &&
                            other.CollisionGroup != Donut.DonutCollisionGroup && other.Friction > 0.005f && !(other.UserData is Chicken))
                        {
                            if (destructionTimer < 0)
                            {
                                destructionTimer = 1.0f;
                            }
                        }
                }
                if (other != null)
                {
                    Body.LinearDamping = 0.3f;
                    AudioQueue.PlayOnce("Sounds/Hit_Egg_0" + Jabber.BaseGame.Random.Next(1, 3));
                }
            }
        }
        float noMoveTimer = 0.0f;

        float destructionTimer = -1;
        float distTravelled = 0;
        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            
            Body.Restitution = 0.5f;

            Body.AddLinearImpulse(new Vector2(0, -3));
            Body.LinearDamping = 0.5f;
            Body.Mass = 1.15f;
            AsType.CreateFramesFromXML("Chickens_Frames");
            AsType.CurrentFrame = "egg00000";
            AsType.ResetDimensions();
            AsType.UniformScale = 0.1f;
            Body.CollisionGroup = Fox.FOX_NONE_COLLISION_GROUP;

            DoDimensions = false;
        }
    }
}
