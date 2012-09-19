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
    public class CannonFireEvent : Event
    {
        public CannonFireEvent(Vector2 pos, Vector2 dir)
        {
            Dir = dir;
            Position = pos;
        }

        public Vector2 Dir;
    }
    public class Cannon : AnimSprite
    {
        public Cannon(GameplayScreen screen)
            : base("chickens")
        {
            this.screen = screen;
            Layer = SpriteLayer.LAYER9;

            RaiseFlag(Jabber.Flags.ACCEPTINPUT);

            projectileWorld = new FarWorld();
            projectileWorld.Initialize(new Vector2(0, -15.0f));
        }
        FarWorld projectileWorld;
        GameplayScreen screen;
        Sprite baseBarrel;
        public override void Initialize(ContentManager _Content)
        {
            base.Initialize(_Content);
            ResetDimensions();

            CreateFramesFromXML("Chickens_Frames");

            //Animation = "Fire";
            ResetDimensions();
            baseBarrel = new Sprite("chickens");
            baseBarrel.Initialize(_Content);
            baseBarrel.CreateFramesFromXML("Chickens_Frames");
            baseBarrel.UniformScale = 2.0f;

            baseBarrel.CurrentFrame = "cannon-base00000";
            Animation = "Barrel";
            CurrentFrame = "cannon-seq-00000";


            baseBarrel.ResetDimensions();
            ResetDimensions();
            baseBarrel.UniformScale = 2.0f;
            UniformScale = 2.0f;
            
            RayCastHit ah = screen.scene.World.RayCast(Position, new Vector2(0, -500) + Position);
            if (ah.actor != null)
            {
                Vector2 worldPos = ah.worldImpact + new Vector2(0, Height * UniformScale) / 2.7f;
                Position = worldPos;
            }

            Handle = SpriteHandle.CUSTOM;
            CustomOrigin = new Vector2(Width / 2.0f, Height / 2.0f + 7);

            FrameChanges += OnFrameChange;

            smoke = new AnimSprite("chickens");
            smoke.Initialize(_Content);
            smoke.CreateFramesFromXML("Chickens_Frames");
            smoke.Animation = "Barrel";
            smoke.UniformScale = UniformScale;
            smoke.Width = Width;
            smoke.Height = Height;
            smoke.Handle = SpriteHandle.CENTERLEFT;
            smoke.RaiseFlag(Jabber.Flags.PASSRENDER);

            smoke.FrameChanges += OnSmokeFrameChange;


            trajectory = new AnimSprite("chickens");
            trajectory.Initialize(_Content);
            trajectory.CreateFramesFromXML("Chickens_Frames");
            trajectory.CurrentFrame = "feather-00000";
            trajectory.ResetDimensions();
            trajectory.UniformScale = 6.0f;
            trajectory.Width = 10;
            trajectory.Height = 10;
            trajectory.Handle = SpriteHandle.CENTER;


            arrow = new AnimSprite("ui/ui");
            arrow.Initialize(_Content);
            arrow.CreateFramesFromXML("ui/ui_frames");
            arrow.CurrentFrame = "arrow_dir";
            arrow.ResetDimensions();
            arrow.UniformScale = 1.0f;
            arrow.Width = 10;
            arrow.Height = 10;
            arrow.Handle = SpriteHandle.CENTER;
        }

        public override void OnPress(Vector2 pos)
        {
            if (Camera.Get.IsVisible(this))
            {
                int k = 0;
            }
            if (screen.scene.ToFire.Count == 0)
            {
                return;
            }
            base.OnPress(pos);
            if (Contains(pos.X, pos.Y))
            {
                fingerDraggingOnMe = true;
            }
        }
        Vector2 lastFingerPos = Vector2.Zero;
        public bool fingerDraggingOnMe = false;
        bool FirstDrag = true;
        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            if (screen.scene.ToFire.Count == 0)
            {
                return;
            }
            thispos = Camera.Get.ScreenToWorld(thispos);
            Vector2 dir = Position - thispos;
            if (fingerDraggingOnMe)
            {
                lastFingerPos = thispos;
                Rot = (float)Math.Atan2((float)dir.X, (float)dir.Y) - 0.5f * (float)Math.PI;


                currentFireDir = Position - lastFingerPos;
                currentFireDir /= 20.0f;
                if (currentFireDir.Length() > 22)
                {
                    currentFireDir.Normalize();
                    currentFireDir *= 22.0f;
                }
                if (currentFireDir.Length() > 1 && FirstDrag)
                {
                    FirstDrag = false;
                    AudioManager.PlayOnce("Sounds/Cannon_Buildup_01");
                }
            }
        }
        Vector2 currentFireDir = Vector2.Zero;
        public override void OnRelease(Vector2 pos)
        {
            if (screen.scene.ToFire.Count == 0)
            {
                return;
            }
            FirstDrag = true;
            base.OnRelease(pos);

            if (fingerDraggingOnMe)
            {
                Vector2 dir = Position - lastFingerPos;
                dir /= 20.0f;
                if (dir.Length() > 22)
                {
                    dir.Normalize();
                    dir *= 22.0f;
                }

                Animation = "Fire";
                //EventManager.Get.SendEvent(new CannonFireEvent(Position, dir));

                FireDir = dir;
            }

            fingerDraggingOnMe = false;
        }

        void OnSmokeFrameChange(int prevframe, int curframe)
        {
            if (curframe == 0)
            {
                smoke.RaiseFlag(Jabber.Flags.PASSRENDER);
            }
        }

        void OnFrameChange(int prevFrame, int curFrame)
        {
            if (curFrame == 0 && Animation == "Fire")
            {
                EventManager.Get.SendEvent(new CannonFireEvent(Position, FireDir));
                Animation = "Barrel";
                smoke.Animation = "Smoke";
                smoke.LowerFlag(Jabber.Flags.PASSRENDER);

                Vector2 dir = FireDir;
                dir.Normalize();
                smoke.Position = Position + dir * Width / 2.0f;
                smoke.Rot = Rot;

                AudioQueue.PlayOnce("Sounds/Cannon_Fire_1");// + Jabber.BaseGame.Random.Next(1, 3));
            }
        }


        AnimSprite smoke = null;
        Vector2 FireDir = Vector2.Zero;
        Vector2 LastDir = Vector2.Zero;
        void UpdateTrajectory()
        {
            if (currentFireDir == LastDir)
                return;

            LastDir = currentFireDir;
            trajectorypositions.Clear();

            if (currentFireDir.Length() > 0)
            {
                JabActor circle = projectileWorld.CreateSphere(75, Position, JabActor.BodyType.DYNAMIC);
                circle.LinearVelocity = (currentFireDir);
                
                Vector2 LastPos = Vector2.Zero;
                for (int i = 0; i < 200; i++)
                {
                    if(i > 0 && i % 4 == 0)
                    {
                        RayCastHit hit = screen.withChicks.World.RayCast(LastPos, circle.Position);
                        if (hit.actor != null)
                        {
                            trajectorypositions.Add(hit.worldImpact);
                            break;
                        }
                    }
                    if (circle.PosY < 0)
                    {
                        break;
                    }
                    if (i % 4 == 0)
                    {
                        trajectorypositions.Add(circle.Position);
                        LastPos = circle.Position;
                    }
					
                    projectileWorld.Update(new GameTime(new TimeSpan(0, 0, 0, 0, 33), new TimeSpan(0, 0, 0, 0, 33)));
                }
                circle.RaiseFlag(Jabber.Flags.DELETE);
            }
        }
        JabRectangle cannonRect;
        List<Vector2> trajectorypositions = new List<Vector2>();
        public override void Update(GameTime gameTime)
        {
            UpdateTrajectory();
            base.Update(gameTime);
            //Camera.Get.Position = Position;// +new Vector2(400, -400);

            if(cannonRect == null)
                cannonRect = GetRectangle();
            Vector2 topleft = TopLeftPosition();

            if (!smoke.CheckFlag(Jabber.Flags.PASSRENDER))
            {
                smoke.Update(gameTime);
            }
        }
        Sprite trajectory;
        Sprite arrow;
        public override void Draw()
        {
            if (ChicksnVixensGame.Get.UseTrajectory)
            {
                for (int i = 0; i < trajectorypositions.Count; i++)
                {
                    trajectory.Position = trajectorypositions[i];
                    trajectory.Draw();
                }
            }

            baseBarrel.Position = position + new Vector2(0, 10);
            baseBarrel.Draw();

            base.Draw();

            if (!ChicksnVixensGame.Get.UseTrajectory)
            {
                if (fingerDraggingOnMe && currentFireDir.Length() != 0)
                {
                    arrow.Handle = SpriteHandle.CENTERLEFT;
                    arrow.Position = position;
                    Vector2 dir = currentFireDir;
                    dir.Normalize();
                    float angle = -JabMath.ATan2(dir.Y, dir.X);
                    arrow.Rot = angle;
                    arrow.Width = currentFireDir.Length() * 10;
                    arrow.Height = 100;
                    arrow.Draw();
                }
            }



            

            if (!smoke.CheckFlag(Jabber.Flags.PASSRENDER))
            {
                //Vector2 oldpos = smoke.Position;
                //smoke.Position = Camera.Get.Position;
                smoke.Draw();


                //smoke.Position = baseBarrel.Position + FireDir * baseBarrel.Width / 2.0f;
                //smoke.Draw();
            }

            //DrawRect();
        }
    }
}
