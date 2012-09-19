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
using Jabber.Util.UI;

namespace ChicksnVixens
{
    public class BreakableDestroyed : Event
    {
        public BreakableDestroyed(BreakableBody.BodyMaterial sender)
        {
            Broken = sender;
        }
        public BreakableBody.BodyMaterial Broken;
    }

    public class BreakableHit : BreakableDestroyed
    {
        public BreakableHit(BreakableBody.BodyMaterial sender)
            : base(sender)
        {
        }
    }
    public class BreakableBody : PhysicSprite
    {
        public static int BodyNoneCollisionGroup = 11;
        public enum BodyMaterial
        {
            WOOD,
            METAL,
            GLASS,
            CONCRETE
        };
        protected BodyMaterial material;
        GameScene scene;
        static float sfxtimer = 0.0f;
        public static void PlayHitSFXForMaterial(BodyMaterial material)
        {
            switch (material)
            {
                case BodyMaterial.WOOD:
                    AudioQueue.PlayOnce("Sounds/Hit_Wood_0" + BaseGame.Random.Next(1, 3));
                    break;
                case BodyMaterial.GLASS:
                    AudioQueue.PlayOnce("Sounds/Hit_Glass_0" + BaseGame.Random.Next(1, 3));
                    break;
                case BodyMaterial.CONCRETE:
                    AudioQueue.PlayOnce("Sounds/Hit_Concrete_0" + BaseGame.Random.Next(1, 5));
                    break;
            }
        }
        public static void PlaySFXBreakForMaterial(BodyMaterial material)
        {
            switch (material)
            {
                case BodyMaterial.WOOD:
                    AudioQueue.PlayOnce("Sounds/Break_Wood_01");// + BaseGame.Random.Next(1, 3));
                    break;
                case BodyMaterial.GLASS:
                    AudioQueue.PlayOnce("Sounds/Break_Glass_01");
                    break;
                case BodyMaterial.CONCRETE:
                    AudioQueue.PlayOnce("Sounds/Break_Concrete_01");
                    break;
            }
        }

        public BodyMaterial Material
        {
            get
            {
                return material;
            }
        }

        public BreakableBody(BodyMaterial mat, GameScene scene)
            : base()
        {
            material = mat;
            this.scene = scene;

            Layer = SpriteLayer.LAYER1;

            EventManager.Get.RegisterListner(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Body.UserData = this;

            if (health <= 0)
            {
                health = 0;
                if (!SplintersCreated)
                {
                    CreateSplinters();
                    SplintersCreated = true;
                }

                RaiseFlag(Jabber.Flags.DELETE);
                Body.CollisionGroup = BodyNoneCollisionGroup;
                Body.IgnoreRayCast = true;
            }

            if (InFan)
            {
                Body.LinearVelocity = new Vector2(Body.LinearVelocity.X, 0);
                if (!InBentFan)
                {
                    Body.LinearDamping = 150.0f;
                }
                
                if (fanTimer == 0)
                {
                    fanTimer = RandomFloat * 2.0f * JabMath.PI;
                }
                fanTimer += gttf(gameTime) * 2.0f;
                Body.PosY += JabMath.Sin(fanTimer) / 3.0f;
            }
            else
            {
                Body.BodyState = JabActor.BodyType.DYNAMIC;
                //Body.AngularDamping = Body.LinearDamping = 0.5f;
                Body.LinearDamping = Body.AngularDamping = JabMath.MoveTowards(Body.LinearDamping, 0.75f, gttf(gameTime) * 3.0f);
            }
            if(InFan || InBentFan)
            {
                Body.AngularDamping = 20.0f;
            }
            InFan = false;
            InBentFan = false;
        }
        float fanTimer = 0;
        void AfterContact(Collision col)
        {
            JabActor other = col.HasActor(Body);
            if (other != null && health > 0)
            {
                Vector2 relativeForce = other.LinearVelocity * other.Mass - Body.LinearVelocity * Body.Mass;
               
                if (health <= 0)
                {
                }
                else if (other.UserData is Chicken && health > 0)
                {
                    (other.UserData as Chicken).AfterBreakableHit(this);
                }
            }
        }
        
        public void TakeHit(float val)
        {
            health -= val;

            if (health <= 0)
            {
                health = 0;
                if (!SplintersCreated)
                {
                    CreateSplinters();
                    SplintersCreated = true;
                }

                RaiseFlag(Jabber.Flags.DELETE);
                Body.CollisionGroup = BodyNoneCollisionGroup;
                Body.IgnoreRayCast = true;

                Event e = new BreakableDestroyed(material);
                e.Position = Body.Position;
                EventManager.Get.SendEvent(e);
            }
            else
            {
                Event e = new BreakableHit(material);
                e.Position = Body.Position;
                EventManager.Get.SendEvent(e);
            }
        }

        void OnContact(Collision col)
        {
            JabActor other = col.HasActor(Body);
            if (other != null && health > 0 && !World.IsEntityGroup(other.CollisionGroup))
            {
                Vector2 relativeForce = other.LinearVelocity * other.Mass - Body.LinearVelocity * Body.Mass;

                if (other.UserData is BreakableBody)// || other.UserData is Fox)
                {
                    relativeForce /= 30.0f; //reduced once because of uluru 9, concrete falling on glass failed to break it
                }
                else if (other.UserData is Fox)
                {
                    relativeForce /= 10.0f;
                }
                else
                {
                    relativeForce /= 6.0f;
                }
                if (relativeForce.Length() > 0.1f)
                {
                    health -= relativeForce.Length();
                    Event e = new BreakableHit(material);
                    e.Position = Body.Position;
                    EventManager.Get.SendEvent(e);
                    PlayHitSFXForMaterial(material);
                }
                if (health <= 0)
                {
                    Event e = new BreakableDestroyed(material);
                    e.Position = Body.Position;
                    EventManager.Get.SendEvent(e);
                    PlaySFXBreakForMaterial(material);
                    health = 0;
                    if (!SplintersCreated)
                    {
                        CreateSplinters();
                        SplintersCreated = true;
                    }
                    if (other.UserData is Chicken)
                    {
                        (other.UserData as Chicken).OnBreakableDestroyed(this);
                    }
                    else if (other.UserData is Fox)
                    {
                        other.LinearVelocity *= 0.5f;
                    }

                    RaiseFlag(Jabber.Flags.DELETE);
                    Body.CollisionGroup = BodyNoneCollisionGroup;
                    Body.IgnoreRayCast = true;
                }
                else if (other.UserData is Chicken && health > 0)
                {
                    (other.UserData as Chicken).OnBreakableHit(this);
                }
            }
            else if (other != null && World.IsEntityGroup(other.CollisionGroup) && other.UserData is Fan)
            {
                if (other.Rot == 0.0f)
                {
                    InFan = true;
                }
                else
                {
                    InBentFan = true;
                }
            }
        }
        bool InFan = false;
        bool InBentFan = false;
        public virtual void CreateSplinters()
        {
            float x = (float)Math.Sin((float)Rot);
            float y = (float)Math.Cos((float)Rot);
            Vector2 rotDir = new Vector2(x, y);
            rotDir.Normalize();

            /*
            for (float f = Height * ScaleY / 2.0f; f > -Height * ScaleY / 2.0f; f -= 40)//Height * ScaleY / 5.0f)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (BaseGame.Random.Next(1, 5) == 2)
                    {
                        Vector2 fuzz = new Vector2(RandomFloatInRange(-Width / 2.0f, Width / 2.0f), RandomFloatInRange(-Width / 2.0f, Width / 2.0f));
                        Vector2 firstPos = Position + f * rotDir + fuzz;

                        Splinter s = new Splinter(material);
                        s.Initialize(BaseGame.Get.Content);
                        s.Position = firstPos;
                        scene.AddNode(s);
                    }
                }
            }*/
            int cur = 0;
          
            for (float f = Height * ScaleY / 2.0f; f > -Height * ScaleY / 2.0f; f -= 80)
            {
                for (float j = Width * ScaleY / 2.0f; j > -Width * ScaleY / 2.0f; j -= 80)
                {
                    Vector2 fuzz = new Vector2(RandomFloatInRange(-Width / 2.0f, Width / 2.0f), RandomFloatInRange(-Width / 2.0f, Width / 2.0f));
                    Vector2 firstPos = Position + f * rotDir + fuzz;

                    preMadeSplinters[cur].Position = firstPos;
                    scene.AddNode(preMadeSplinters[cur]);
                    ++cur;
                    if (cur >= preMadeSplinters.Count)
                    {
                        break;
                    }
                }
                if (cur >= preMadeSplinters.Count)
                {
                    break;
                }
            }
            preMadeSplinters.Clear();
        }
        bool SplintersCreated = false;

        List<Splinter> preMadeSplinters = new List<Splinter>();


        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

           /* if (ev is CollisionEvent)
            {
                CollisionEvent e = ev as CollisionEvent;
                if(e.ActorPresent(Body) != null)
                {
                    JabActor other = e.ActorPresent(Body);
                    Vector2 relativeForce = other.LinearVelocity * other.Mass - Body.LinearVelocity * Body.Mass;
                    relativeForce /= 20.0f;
                    if (relativeForce.Length() > 1.0f)
                    {
                        health -= relativeForce.Length();
                    }
                }
            }*/
        }

        public override void Draw()
        {
          //  AsType.Dimension = Dimension;
           // UniformScale = 1.0f;

            base.Draw();

            
            float healthProp = 1.0f - health / maxHealth;
            Colour = Color.White * healthProp;
            
            string oldFrame = AsType.CurrentFrame;
            AsType.CurrentFrame = oldFrame + "_broken";
            base.Draw();

            AsType.CurrentFrame = oldFrame;
            Colour = Color.White;
        }
        float maxHealth = 0;
        public float health = 0;

        public override void UnloadContent()
        {
            base.UnloadContent();

            scene.World.BeginContact -= OnContact;
            scene.World.EndContact -= AfterContact;
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);
            scene.World.BeginContact += OnContact;
            scene.World.EndContact += AfterContact;
            AsType.CreateFramesFromXML("break_frames");
            switch (material)
            {
                case BodyMaterial.WOOD:
                    AsType.CurrentFrame = "wood";
                    Body.Mass = 0.5f;
                    Body.Friction = 3;
                    Body.Restitution = 0.1f;
                    health = 1.5f;
                    break;
                case BodyMaterial.GLASS:
                    AsType.CurrentFrame = "glass";
                    Body.Mass = 0.8f;
                    Body.Friction = 2;
                    Body.Restitution = 0.1f;
                    health = 0.9f;
                    break;
                case BodyMaterial.CONCRETE:
                    AsType.CurrentFrame = "cement";
                    Body.Mass = 1.5f;
                    Body.Friction = 6;
                    Body.Restitution = 0.01f;
                    health = 4.0f;
                    break;
            }

            maxHealth = health;

            Width = Body.Width;// *AsType.ScaleX;
            Height = Body.Height;// *AsType.ScaleY;
            UniformScale = 1.0f;

            if (Math.Abs(Height / Width - 1) < 0.3f && Height < 80)
            {
                AsType.CurrentFrame = AsType.CurrentFrame + "_block";
            }
            else if (Math.Abs(Height / Width - 1) < 0.3f)
            {
                AsType.CurrentFrame = AsType.CurrentFrame + "_square";
            }
            else if (Height / Width > 5)
            {
                AsType.CurrentFrame = AsType.CurrentFrame + "_long";
            }

            AsType.UpdateOrigin();

            for (float f = Height * ScaleY / 2.0f; f > -Height * ScaleY / 2.0f; f -= 80)//Height * ScaleY / 5.0f)
            {
                for (float j = Height * ScaleY / 2.0f; j > -Height * ScaleY / 2.0f; j -= 80)
                {
                   // for (int i = 0; i < 4; i++)
                    {
                        //if (BaseGame.Random.Next(1, 5) == 2)
                        {
                            Vector2 fuzz = new Vector2(RandomFloatInRange(-Width / 2.0f, Width / 2.0f), RandomFloatInRange(-Width / 2.0f, Width / 2.0f));
                            // Vector2 firstPos = Position + f * rotDir + fuzz;

                            Splinter s = new Splinter(material);
                            s.ScaleMultiplier = (Width * Height) / 10000.0f;
                            s.Initialize(BaseGame.Get.Content);

                            //s.Position = firstPos;
                            //scene.AddNode(s);
                            preMadeSplinters.Add(s);
                        }
                    }
                }
            }
        }
    }
}