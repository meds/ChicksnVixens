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
    public class Chicken_Mini : Chicken
    {
        static int Chicken_Mini_None_Collision = 14238;
        public Chicken_Mini(ThisGamesScene world)
            : base(world)
        {
            world.World.MakeEntityGroup(Chicken_Mini_None_Collision);
            EventManager.Get.RegisterListner(this);
            AccelerateAnimation = "MiniChicken_Dash";

            Body.RaiseFlag(Jabber.Flags.DELETE);
            Body.CollisionGroup = Chicken_Mini_None_Collision;
            Body = world.World.CreateSphere(40.0f / 2.0f, Vector2.Zero, JabActor.BodyType.DYNAMIC);
            Body.UserData = this;
            Body.Restitution = 0.5f;
            Body.Mass = 0.5f;

            Body.Width = Body.Height = 20.0f;

            DoDimensions = true;
        }

        public override void ProcessEvent(Event ev)
        {
            if (ev is CollisionEvent)
            {
                JabActor other = (ev as CollisionEvent).ActorPresent(Body);
                if (other != null && !(other.UserData is Chicken))
                {
                    hasHit = true;
                    Body.CollisionGroup = 0;
                    Body.AngularVelocity = RandomFloatInRange(-6.0f, 6.0f);
                }
            }
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            launchInst.Volume = 0.0f;
            AccelerateAnimation = "MiniChicken_Dash";
            Deactivate();

            Body.Mass = 0.25f;
            //Body.LinearDamping = 0.25f;
        }
        bool firstUpdate = true;
        bool hasHit = false;
        public Vector2 vectorDir = Vector2.Zero;
        float timer = 0.2f;
        public override void Update(GameTime gameTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                AsType.Animation = AccelerateAnimation;
                Vector2 dir = Body.LinearVelocity;
                dir.Normalize();

                Body.Rot = (float)Math.Atan2((float)dir.X, (float)dir.Y);

                PhysicsRotate = true;
                vectorDir = Body.LinearVelocity;
                Body.AngularVelocity = RandomFloatInRange(-3, 3);
            }
            if (!hasHit)
            {
                Body.LinearVelocity = vectorDir;
            }
            timer -= gttf(gameTime);
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }

    public class Chicken_Mini_Launcher : Chicken
    {
        public Chicken_Mini_Launcher(ThisGamesScene world)
            : base(world)
        {
            EventManager.Get.RegisterListner(this);
            AccelerateAnimation = "MiniChicken_Dash_Egg";
            PhysicsRotate = true;

            RaiseFlag(Jabber.Flags.ACCEPTINPUT);
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            launchInst.Stop();
            launchInst.RaiseFlag(Jabber.Flags.DELETE);
            Body.AngularVelocity = RandomFloatInRange(-10.0f, 10.0f);
        }

        public override void OnPress(Vector2 pos)
        {
            base.OnPress(pos);
            RaiseFlag(Jabber.Flags.DELETE);
            Body.RaiseFlag(Jabber.Flags.DELETE);
            Body.IgnoreRayCast = true;
            Body.CollisionGroup = Fox.FOX_NONE_COLLISION_GROUP;
            Explosion exp = new Explosion(10, 300, 1.0f, 0.1f, scene.World, Position);
            exp.Initialize(Jabber.BaseGame.Get.Content);
            scene.AddNode(exp);

            List<Fox> foxes = new List<Fox>();
            for (int i = 0; i < scene.Nodes.Count; i++)
            {
                if (scene.Nodes[i] is Fox)
                {
                    Fox f = scene.Nodes[i] as Fox;
                    if (f.IsAlive)
                    {
                        foxes.Add(f);
                    }
                }
            }
            float Nearest = float.MaxValue;
            List<Vector2> nearestPos = new List<Vector2>();
            Vector2 curNearest = Vector2.Zero;
            int usingI = -1;
            for (int i = 0; i < foxes.Count; i++)
            {
                if ((Position - foxes[i].Position).Length() < Nearest)
                {
                    usingI = i;
                    Nearest = (Position - foxes[i].Position).Length();
                    curNearest = foxes[i].Position;
                }
            }
            if (usingI >= 0)
            {
                Nearest = float.MaxValue;
                foxes.RemoveAt(usingI);
                nearestPos.Add(curNearest);

                usingI = -1;
                if (foxes.Count > 0)
                {
                    for (int i = 0; i < foxes.Count; i++)
                    {
                        if ((Position - foxes[i].Position).Length() < Nearest)
                        {
                            usingI = i;
                            Nearest = (Position - foxes[i].Position).Length();
                            curNearest = foxes[i].Position;
                        }
                    }
                    if (usingI >= 0)
                    {
                        Nearest = float.MaxValue;
                        foxes.RemoveAt(usingI);
                        nearestPos.Add(curNearest);

                        usingI = -1;
                        if (foxes.Count > 0)
                        {
                            for (int i = 0; i < foxes.Count; i++)
                            {
                                if ((Position - foxes[i].Position).Length() < Nearest)
                                {
                                    usingI = i;
                                    Nearest = (Position - foxes[i].Position).Length();
                                    curNearest = foxes[i].Position;
                                }
                            }
                            if (usingI >= 0)
                            {
                                foxes.RemoveAt(usingI);
                                nearestPos.Add(curNearest);
                            }
                        }
                    }
                }
            }
            AudioQueue.PlayOnce("Sounds/Explode_Chicken");
            Deactivate();
            Vector2 bodypos = Body.Position;
            Body.PosX = -10000;
            for (int i = 0; i < nearestPos.Count; i++)
            {
                Chicken_Mini e = new Chicken_Mini(scene);
                e.Position = bodypos;
                e.Initialize(Jabber.BaseGame.Get.Content);
                scene.AddNode(e);

                Vector2 dir = (nearestPos[i] - bodypos);
                dir.Normalize();

                float dist = (nearestPos[i] - bodypos).Length();

                float ydif = Math.Abs(bodypos.Y - nearestPos[i].Y);
                float xdif = Math.Abs(bodypos.X - nearestPos[i].X);
                float curY = bodypos.Y;
                float tarX = nearestPos[i].X;

                float horizontalSpeed = (dir * dist * 0.01f).X;
                /*
                FarWorld f = new FarWorld();
                f.Initialize(World.Gravity);
                JabActor circle = f.CreateSphere(Width / 2.0f, bodypos, JabActor.BodyType.DYNAMIC);
                circle.Mass = Body.Mass;
                circle.Restitution = Body.Restitution;
                circle.LinearDamping = 0.5f;
                circle.LinearVelocity = new Vector2(horizontalSpeed, 0);

                float lastDist = float.MaxValue;
                float lastY = 0;
                while (true)
                {
                    f.Update(new GameTime(new TimeSpan(0, 0, 0, 0, 33), new TimeSpan(0, 0, 0, 0, 33)));
                    float curDist = Math.Abs(circle.PosX - tarX);
                    if (lastDist < curDist)
                    {
                        break;
                    }
                    lastY = circle.PosY;
                    lastDist = curDist;
                }

                float yAtDest = circle.PosY;
                float tarY = nearestPos[i].Y;

                float upWardRequired = (yAtDest - tarY) / 100.0f;
                */
                e.Body.LinearVelocity = dir * 20;// *0.03f;// new Vector2(horizontalSpeed, horizontalSpeed);
                e.vectorDir = dir * 20.0f;
            }
        }

        public override void ProcessEvent(Event ev)
        {
            //base.ProcessEvent(ev);

            if (ev is CollisionEvent)
            {
                JabActor other = (ev as CollisionEvent).ActorPresent(Body);
                if (other != null)
                {
                    if ((Body.LinearVelocity - other.LinearVelocity).Length() > 3.0f)
                    {
                        AudioQueue.PlayOnce("Sounds/Hit_Egg_01");
                    }
                    //hasCollided = true;
                }
            }
        }

        bool firstUpdate = true;
        public override void Update(GameTime gameTime)
        {
            
            if (firstUpdate)
            {
                firstUpdate = false;
                AsType.Animation = AccelerateAnimation;
                Vector2 dir = Body.LinearVelocity;
                dir.Normalize();

                Body.Rot = (float)Math.Atan2((float)dir.X, (float)dir.Y);
            }
            //if(!hasCollided)
              //  base.Update(gameTime);
        }
    }
}
