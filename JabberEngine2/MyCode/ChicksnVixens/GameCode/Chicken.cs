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
using Jabber;
using ChicksnVixens.Screens;
namespace ChicksnVixens
{
    public class Chicken : PhysicAnimSprite
    {
        public Chicken(ThisGamesScene world)
            : base(75, Vector2.Zero, true, world.World, "chickens")
        {
            RaiseFlag(Jabber.Flags.ACCEPTINPUT);
            Body.Restitution = 0.5f;
            Body.Mass = 0.5f;

            PhysicsRotate = false;

            this.scene = world;
            EventManager.Get.RegisterListner(this);
        }

        protected SoundInst launchInst = null;
        protected ThisGamesScene scene;
        float oldRestitution = 0.5f;
        public void OnBreakableHit(BreakableBody body)
        {
           // Body.Restitution = 0;
           // Body.LinearVelocity *= 0;// new Vector2(0, Body.LinearVelocity.Y);

            return;
            if (Body.LinearVelocity.Length() > 3.0f)
            {
                Vector2 vel = Body.LinearVelocity;
                vel.Normalize();
                vel *= 3.0f;
                Body.LinearVelocity = vel;
            }
        }

        public void AfterBreakableHit(BreakableBody body)
        {
            // Body.Restitution = 0;
            // Body.LinearVelocity *= 0;// new Vector2(0, Body.LinearVelocity.Y);

            if (Body == null)
            {
                return;
            }
            if (Body.LinearVelocity.Length() > 3.0f)
            {
                Vector2 vel = Body.LinearVelocity;
                vel.Normalize();
                vel *= 3.0f;
                Body.LinearVelocity = vel;
            }
        }

        public void OnBreakableDestroyed(BreakableBody body)
        {/*
            if (body.Material == BreakableBody.BodyMaterial.GLASS)
            {
                Body.LinearVelocity *= 0.8f;
            }
            else
                Body.LinearVelocity *= 0.2f;*/

            if (Body == null)
            {
                return;
            }
            if (Body.LinearVelocity.Length() > 5.0f)
            {
                Vector2 vel = Body.LinearVelocity;
                float speed = Body.LinearVelocity.Length();
                speed *= 0.7f;
                vel.Normalize();
                Body.LinearVelocity = vel * speed;
            }
        }

        public override void  OnPress(Vector2 pos)
        {
            base.OnTap(pos);
            pos = Camera.Get.ScreenToWorld(pos);
            Vector2 dir = Position - pos;
            dir.Normalize();
            dir *= -10;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            launchInst.RaiseFlag(Flags.DELETE);
            World.BeginContact -= OnContact;
            World.EndContact -= EndContact;

            if (IsSliding)
            {
                IsSliding = false;
                AudioQueue.PlaySlide(false);
            }
        }


        public void EndContact(Collision col)
        {
            if (col.HasActor(Body) != null)
            {
                JabActor other = col.HasActor(Body);
                if (IsSliding)
                {
                    IsSliding = false;
                    AudioQueue.PlaySlide(false);
                }
            }
        }
        bool hasCollidedOnce = false;
        bool IsSliding = false;
        public void OnContact(Collision col)
        {
            if (col.HasActor(Body) != null)
            {
                Body.LinearDamping = 0.5f;
                JabActor other = col.HasActor(Body);
                if (!(other.UserData is Chicken) && !World.IsEntityGroup(other.CollisionGroup))
                {
                    hasCollidedOnce = true;
                }
                if (other.Friction <= 0.005f)
                {
                    Body.Restitution = 0.0f;
                    if (Body.LinearVelocity.Length() > 0.5f && !IsSliding)
                    {
                        AudioQueue.PlaySlide(true);
                        IsSliding = true;
                    }
                    else if(IsSliding)
                    {
                        IsSliding = false;
                        AudioQueue.PlaySlide(false);
                    }
                }
                else
                {
                    if (IsSliding)
                    {
                        IsSliding = false;
                        AudioQueue.PlaySlide(false);
                    }
                    Body.Restitution = 0.5f;
                }
            }
        }

        float targetDamping = 0;
        int numContacts = 0;
        bool hasHitContact = false;
        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is CollisionEvent)
            {
                if ((ev as CollisionEvent).ActorPresent(Body) != null)
                {
                    if (!World.IsEntityGroup((ev as CollisionEvent).ActorPresent(Body).CollisionGroup))
                    {
                        if ((ev as CollisionEvent).contactType == CollisionEvent.ContactType.ONCONTACT)
                        {
                            ++numContacts;
                        }
                        else
                        {
                            --numContacts;
                        }
                    }
                    JabActor other = (ev as CollisionEvent).ActorPresent(Body);
                    
                    if (launchInst.Volume == 1.0f && !World.IsEntityGroup(other.CollisionGroup))
                    {
                        launchInst.Volume = 0.99999f;
                    }
                    if ((Body.LinearVelocity - other.LinearVelocity).Length() > 2.0f && !World.IsEntityGroup(other.CollisionGroup))
                    {
                        if (hitTimer < 0.0f && other.BodyState == JabActor.BodyType.STATIC)
                        {
                            AudioQueue.PlayOnce("Sounds/Hit_Chicken_" + BaseGame.Random.Next(2, 5));
                            hitTimer = 0.1f;
                        }
                    }

                    if (numContacts > 1)
                    {
                        targetDamping = 4.5f;
                    }
                    else
                    {
                        targetDamping = 1.5f;
                        hasHitContact = true;
                        if (Body.LinearVelocity.Length() > 5.0f)
                        {
                        }
                    }
                }
            }
        }

        float hitTimer = 0.0f;

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            AsType.CreateFramesFromXML("Chickens_Frames");

          //  PosY = 500;

            AsType.CurrentFrame = "bird-chicken-00000";

            AsType.Width = 128;
            AsType.Height = 128;

            AsType.Animation = AccelerateAnimation;

            launchInst = AudioManager.CreateSound("Sounds/Chicken_Launch_0" + BaseGame.Random.Next(1, 7));
            launchInst.Play(1.0f);

            Sprite.Layer = SpriteLayer.LAYER10;


            World.BeginContact += OnContact;
            World.EndContact += EndContact;
        }

        bool firstUpdate = true;
        List<Sprite> featherTrail = new List<Sprite>();
        protected virtual Sprite CreateFeather()
        {
            Sprite s = new Sprite("ui/ui");
            s.Initialize(BaseGame.Get.Content);
            s.CreateFramesFromXML("ui/ui_frames");
            s.CurrentFrame = "circle";
            s.ResetDimensions();
            s.Position = Body.Position;
            return s;
        }
        public override void Draw()
        {
            base.Draw();

            if (!ChicksnVixensGame.Get.UseTrajectory)
            {
                for (int i = 0; i < featherTrail.Count; i++)
                {
                    featherTrail[i].Draw();
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (!hasCollidedOnce)
            {
                featherTrail.Add(CreateFeather());
            }
            if (Body.LinearVelocity.Length() < 0.5f && IsSliding)
            {
                IsSliding = false;
                AudioQueue.PlaySlide(false);
            }
            if (Body.LinearVelocity.Length() < 0.5f)
            {
                AsType.CurrentAnim.Speed = 0;
            }
            else
            {
                AsType.CurrentAnim.Speed = 20;
            }

            hitTimer -= gttf(gameTime);
            if (launchInst.Volume != 1.0f)
            {
                launchInst.Volume = JabMath.MoveTowards(launchInst.Volume, 0.0f, gttf(gameTime) * 5.0f);
            }
            if (hasHitContact)
            {
                targetDamping = 1.5f;
                hasHitContact = true;
                if (Body.PosX > scene.GetRightMaxPos() + 5000 || Body.PosY < -10 || Body.PosX < scene.startPos.X - 5000)
                {
                    RaiseFlag(Jabber.Flags.DELETE);
                }
                //if (Body.LinearVelocity.Length() > 4.0f)
                {
                    //Vector2 vel = Body.LinearVelocity;
                    //vel.Normalize();
                    //vel *= 4.0f;
                    //Body.LinearVelocity = vel;
                }
            }


            //Body.LinearDamping = JabMath.MoveTowards(Body.LinearDamping, targetDamping, gttf(gameTime) * 50, 0.03f);
            base.Update(gameTime);
            //Body.Restitution = oldRestitution;

            if (Body.LinearVelocity.Length() > 0.5f)
            {
                AsType.Animation = AccelerateAnimation;
                Vector2 dir = Body.LinearVelocity;
                dir.Normalize();

                AsType.Rot = (float)Math.Atan2((float)dir.X, (float)dir.Y);
                /*
                if (firstUpdate)
                    AsType.Rot = (float)Math.Atan2((float)dir.X, (float)dir.Y);
                else
                    AsType.Rot = JabMath.MoveTowards(Rot, (float)Math.Atan2((float)dir.X, (float)dir.Y), gttf(gameTime) * 3.0f);*/
            }
            else
            {
                AsType.Rot = Body.Rot;// JabMath.MoveTowards(Rot, 0, gttf(gameTime) * 3.0f);
            }

            // Check to see if chicken should be made to be in-active (if it is currently active)..
            if (active)
            {
                if (Body.LinearVelocity.Length() <= 0.00001f)
                {
                    zeroTimer += gttf(gameTime);
                    if (zeroTimer > 1)
                    {
                        active = false;
                    }
                }
                else
                {
                    zeroTimer = 0;
                }
            }

            firstUpdate = false;
        }
        float zeroTimer = 0;

        protected bool active = true;
        public bool IsActive
        {
            get { return active; }
        }
        public void Deactivate()
        {
            active = false;
        }


        protected string AccelerateAnimation = "Accelerate_Normal";
    }
}
