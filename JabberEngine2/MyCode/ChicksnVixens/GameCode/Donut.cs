using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabber.Physics;
using Jabber.Media;
using Jabber.Scene;
using Microsoft.Xna.Framework;
using Jabber.Util;
using ChicksnVixens;
using Jabber;

namespace ChicksnVixens
{
    public class Donut : Sprite
    {
        public static int DonutCollisionGroup = 255;
        public Donut(GameScene scene, DonutScore score)
            : base("misc")
        {
            Layer = SpriteLayer.UILAYER2;
            this.scene = scene;
            this.score = score;

            scene.World.MakeEntityGroup(DonutCollisionGroup);
            EventManager.Get.RegisterListner(this);

            Rot = RandomFloat * JabMath.PI * 2.0f;
        }

        public Donut(GameScene scene):base("misc")
        {
            Layer = SpriteLayer.UILAYER2;
            this.scene = scene;

            scene.World.MakeEntityGroup(DonutCollisionGroup);
            EventManager.Get.RegisterListner(this);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            box.RaiseFlag(Flags.DELETE);
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("misc_frames");
            CurrentFrame = "donut";
            ResetDimensions();

            box = scene.World.CreateBox(Dimension, Position, JabActor.BodyType.STATIC);
            box.CollisionGroup = DonutCollisionGroup;
            box.IgnoreRayCast = true;
            box.Rot = Rot;
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is CollisionEvent)
            {
                JabActor other = (ev as CollisionEvent).ActorPresent(box);
                if (other != null && (other.UserData is Chicken || other.UserData is Fox))
                {
                    if (!Chosen)
                    {
                        AudioQueue.PlayOnce("Sounds/pop");
                    }
                    Chosen = true;
                }
                if (other != null && other.UserData is Fan)
                {
                    InFan = true;
                }
            }
        }

        bool InFan = false;
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (score == null)
            {
                for (int i = 0; i < scene.Nodes.Count; i++)
                {
                    if (scene.Nodes[i] is DonutScore)
                    {
                        score = scene.Nodes[i] as DonutScore;
                    }
                }
            }

            box.Position = Position;

            if (!Chosen)
            {
                if (InFan)
                {
                    Timer += gttf(dt) * 2.0f;
                    PosY += JabMath.Sin(Timer) * WindWeakness;

                    if (Timer > 2 * JabMath.PI);
                        Timer -= 2 * JabMath.PI;
                }
                else
                {
                    Timer += gttf(dt) * 4.0f;
                    PosY += JabMath.Sin(Timer) * 0.25f;
                }
            }
            else
            {
                Position = JabMath.LinearMoveTowards(Position, score.ScaledPos, gttf(dt) * 1000.0f / Camera.Get.UniformWorldScale);
                
                UniformScale = JabMath.LinearMoveTowards(UniformScale, score.ScaledScale, gttf(dt) / Camera.Get.UniformWorldScale);

                if (Position == score.ScaledPos && !CheckFlag(Flags.DELETE))
                {
                    RaiseFlag(Flags.DELETE);
                    score.IncrementScore();
                }
            }

            InFan = false;
        }
        public override void Draw()
        {
            base.Draw();

            if (Chosen)
            {
                Colour = Color.White * 0.5f;
            }
        }

        float Timer = RandomFloat * JabMath.PI * 2.0f;
        float WindWeakness = RandomFloatInRange(0.5f, 0.6f);
        GameScene scene;
        JabActor box;

        public bool Chosen = false;
        DonutScore score;
    }

    public class DonutCase : PhysicSprite
    {
        public DonutCase(ThisGamesScene world):base()
        {
            this.world = world;
            Layer = SpriteLayer.UILAYER3;
            EventManager.Get.RegisterListner(this);
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);
            /*

            if (ev is CollisionEvent)
            {
                JabActor other = (ev as CollisionEvent).ActorPresent(Body);
                if (other != null)
                {
                    if (other.Restitution > 1)
                    {
                        Body.LinearDamping = 1.0f;
                    }
                }
            }*/
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (CheckFlag(Flags.FADE_OUT))
            {
                float a = Colour.A / 255.0f;
                float d = JabMath.MoveTowards(a, 0.0f, gttf(gameTime) * 2.0f, 0.001f);
                Colour = Color.White * d;
                if (d == 0)
                {
                    RaiseFlag(Flags.DELETE);
                }
                return;
            }
            if (PosX > world.GetRightMaxPos() + 1000 || PosX < world.startPos.X - 1000)
            {
                Rot = 0.5f * JabMath.PI;
            }
            if (Math.Abs(Rot) > 0.45f * JabMath.PI)
            {
                deathTimer += gttf(gameTime) * 5.0f;
                while (deathTimer > 1.0f && numDonutsMade < 5)
                {
                    deathTimer -= 1.0f;
                    Donut d = new Donut(world);
                    AudioQueue.PlayOnce("Sounds/pop");
                    d.Initialize(BaseGame.Get.Content);
                    d.Position = Position + new Vector2(RandomFloatInRange(-0.5f, 0.5f) * Width, RandomFloatInRange(-0.5f, 0.5f) * Height);
                    d.Chosen = true;
                    world.AddNode(d);
                    deathTimer -= 1.0f;
                    ++numDonutsMade;

                    float oldWidth = Body.Width;
                    float oldHeight = Body.Height;
                    Vector2 oldPos = Body.Position;
                    float oldRot = Body.Rot;
                    

                    oldWidth *= 0.9f;
                    oldHeight *= 0.9f;
                    Vector2 linearVelocity = Body.LinearVelocity;
                    float angularVelocity = Body.AngularVelocity;

                    Body.RaiseFlag(Flags.DELETE);
                    Body.CollisionGroup = Donut.DonutCollisionGroup;

                    Body = world.World.CreateBox(new Vector2(oldWidth, oldHeight), oldPos, JabActor.BodyType.DYNAMIC);
                    Body.Rot = oldRot;
                    Body.LinearVelocity = linearVelocity;
                    Body.AngularVelocity = angularVelocity;
                    //Sprite.Width = oldWidth;
                    //Sprite.Height = oldHeight;
                }
                if (numDonutsMade > 4)
                {
                    RaiseFlag(Flags.DELETE);
                }
                //Colour = Color.White * ((5.0f - numDonutsMade) / 5.0f);
            }

            Sprite.Width = JabMath.MoveTowards(Sprite.Width, Body.Width, gttf(gameTime) * 1.0f);
            Sprite.Height = JabMath.MoveTowards(Sprite.Height, Body.Height, gttf(gameTime) * 1.0f);

         //   Body.LinearDamping = 0.15f;
         //   Body.AngularDamping = 0.3f;
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            textureDir = "misc";
            base.Initialize(Content);
            AsType.CreateFramesFromXML("misc_frames");
            AsType.CurrentFrame = "donutcase";
            AsType.ResetDimensions();
            Body = world.World.CreateBox(AsType.Dimension * Scale, Position, JabActor.BodyType.DYNAMIC);
            Body.Mass = 1.5f;
            Body.Friction = 0.35f;
           // Body.LinearDamping = 0.2f;
            //Body.AngularDamping = 0.2f;
        }

        int numDonutsMade = 0;
        float deathTimer = 0;
        ThisGamesScene world;
    }
}
