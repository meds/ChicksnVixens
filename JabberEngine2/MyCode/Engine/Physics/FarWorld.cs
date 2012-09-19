using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Phone.Tasks;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using Jabber.Util;

using FarseerPhysics.Collision;
using System.Diagnostics;
using Jabber.Media;
using Jabber;

#if WINDOWS || WINDOWS_PHONE
using FarseerPhysics.DebugViews;
#endif


namespace Jabber.Physics
{
    class FarActor : JabActor
    {
        public FarActor()
            : base()
        {
        }
        public override float Density
        {
            get { return body.FixtureList[0].Shape.Density; }
            set
            {

                for (int i = 0; i < body.FixtureList.Count; i++)
                {
                    body.FixtureList[i].Shape.Density = value;
                }
            }
        }
        public override float Mass
        {
            get { return body.Mass; }
            set { body.Mass = value; }
        }
        public override float LinearDamping
        {
            get { return body.LinearDamping; }
            set { body.LinearDamping = value; }
        }
        public override float AngularDamping
        {
            get { return body.AngularDamping; }
            set { body.AngularDamping = value; }
        }
        public override bool HeedGravity
        {
            get { return !body.IgnoreGravity; }
            set { body.IgnoreGravity = !value; }
        }
        public override float Friction
        {
            get { return body.FixtureList[0].Friction; }
            set
            {
                for (int i = 0; i < body.FixtureList.Count; i++)
                {
                    body.FixtureList[i].Friction = value;
                }
            }
        }
        public override float Restitution
        {
            get
            {
                return body.FixtureList[0].Restitution;
            }
            set
            {
                for (int i = 0; i < body.FixtureList.Count; i++)
                {
                    body.FixtureList[i].Restitution = value;
                }
            }
        }

        public override void AddLinearImpulse(Vector2 dir)
        {
            body.ApplyLinearImpulse(ref dir);
        }
        public override void AddForce(Vector2 dir)
        {
            body.ApplyForce(ref dir);
        }
        public override void AddForce(Vector2 dir, Vector2 worldpos)
        {
            worldpos /= 100.0f;
            body.ApplyForce(dir, worldpos);
        }
        public override void AddLinearImpulse(Vector2 dir, Vector2 worldpos)
        {
            worldpos /= 100.0f;
            body.ApplyLinearImpulse(ref dir, ref worldpos);
        }
        public override Vector2 LinearVelocity
        {
            get {
                return LastGoodVelocity;
             //   return body.LinearVelocity;
            }
            set { 
                body.LinearVelocity = value;
                LastGoodVelocity = body.LinearVelocity;            
            }
        }
        public override float AngularVelocity
        {
            get { return body.AngularVelocity; }
            set { body.AngularVelocity = value; }
        }

        public override bool Awake
        {
            get
            {
                return body.Awake;
            }
            set
            {
                body.Awake = value;
            }
        }
        
        public override bool DoContinuous
        {
            get { return body.IsBullet; }
            set { body.IsBullet = value; }
        }

        public override Vector2 Position
        {
            get
            {
                //return LastGoodPosition * 100;
                return body.Position * 100.0f;
            }
            set
            {
                body.Position = value / 100.0f;
                LastGoodPosition = body.Position;
            }
        }

        public override float PosX
        {
            get
            {
                return Position.X;// *100.0f;
            }
            set
            {
                body.Position = new Vector2(value / 100.0f, body.Position.Y);
                LastGoodPosition.X = body.Position.X;
            }
        }
        
        public override float PosY
        {
            get
            {
                return Position.Y;
                //return body.Position.Y * 100;
            }
            set
            {
                body.Position = new Vector2(body.Position.X, value / 100.0f);
                LastGoodPosition.Y = body.Position.Y;
            }
        }
        FarseerPhysics.Dynamics.BodyType GetBodyType(JabActor.BodyType type)
        {
            switch (type)
            {
                case BodyType.DYNAMIC:
                    return FarseerPhysics.Dynamics.BodyType.Dynamic;
                case BodyType.STATIC:
                    return FarseerPhysics.Dynamics.BodyType.Kinematic;
            }
            //System.Windows.MessageBox.Show("ERROR: Body type provided invalid for FarWorld");

            return FarseerPhysics.Dynamics.BodyType.Static;
        }
        override public JabActor.BodyType BodyState
        {
            set
            {
                body.BodyType = GetBodyType(value);
                this.bodyType = value;
            }
            get
            {
                return this.bodyType;
            }
        }
        public override float Rot
        {
            get
            {
                return -body.Rotation;
            }
            set
            {
                body.Rotation = -value;
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public Body body;

        public Vector2 LastGoodPosition = Vector2.Zero;
        public Vector2 LastGoodVelocity = Vector2.Zero;
    };
    public class FarWorld : JabWorld
    {
        public FarWorld()
            : base()
        {
        }
        World world;
        public bool PhysicsThreadStepping = false;

        bool OnContact(Contact contact)
        {
            bool collisionAllowed = AllowCollisionsBetweenGroups(((FarActor)contact.FixtureA.Body.UserData).CollisionGroup, ((FarActor)contact.FixtureB.Body.UserData).CollisionGroup);
            bool eitherEntity = IsEntityGroup(((FarActor)contact.FixtureA.Body.UserData).CollisionGroup) || IsEntityGroup(((FarActor)contact.FixtureB.Body.UserData).CollisionGroup);

            if (!collisionAllowed && !eitherEntity)
            {
                return false;
            }

            if (null != BeginContact)
            {
                Collision collision = new Collision();
                collision.actor1 = (contact.FixtureA.Body.UserData as JabActor);
                collision.actor2 = (contact.FixtureB.Body.UserData as JabActor);

                BeginContact.Invoke(collision);
            }


            CollisionEvent ev = new CollisionEvent();
            ev.contactType = CollisionEvent.ContactType.ONCONTACT;
            ev._Actor1 = (contact.FixtureA.Body.UserData as JabActor);
            ev._Actor2 = (contact.FixtureB.Body.UserData as JabActor);
            
            Jabber.Util.EventManager.Get.SendEvent(ev);

            collisionAllowed = AllowCollisionsBetweenGroups(((FarActor)contact.FixtureA.Body.UserData).CollisionGroup, ((FarActor)contact.FixtureB.Body.UserData).CollisionGroup);
            if (eitherEntity || !collisionAllowed)
            {
                return false;
            }
            return true;
        }

        void ContactOver(Contact contact)
        {
            if (null != EndContact)
            {
                Collision collision = new Collision();
                collision.actor1 = (contact.FixtureA.Body.UserData as JabActor);
                collision.actor2 = (contact.FixtureB.Body.UserData as JabActor);
                base.EndContact.Invoke(collision);
                /*
                CollisionEvent ev = new CollisionEvent();
                ev.contactType = CollisionEvent.ContactType.AFTERCONTACT;
                ev._Actor1 = collision.actor1;
                ev._Actor2 = collision.actor2;

                Jabber.Util.EventManager.Get.SendEvent(ev);*/
            }


            CollisionEvent ev = new CollisionEvent();
            ev.contactType = CollisionEvent.ContactType.AFTERCONTACT;
            ev._Actor1 = (contact.FixtureA.Body.UserData as JabActor);
            ev._Actor2 = (contact.FixtureB.Body.UserData as JabActor);

            Jabber.Util.EventManager.Get.SendEvent(ev);
        }
        public bool PausePhysicsUpdate = false;
        public override void Initialize(Vector2 gravity)
        {
            world = new World(gravity);
            world.ContactManager.BeginContact += OnContact;
            world.ContactManager.EndContact += ContactOver;

            FarseerPhysics.Settings.VelocityIterations = 3;
            FarseerPhysics.Settings.PositionIterations = 1;

            FarseerPhysics.Settings.ContinuousPhysics = false;
            FarseerPhysics.Settings.EnableDiagnostics = false;

            FarseerPhysics.Settings.MaxPolygonVertices = 32;
#if WINDOWS || WINDOWS_PHONE
            _debugView = new DebugViewXNA(world);
            _debugView.LoadContent(BaseGame.Get.GraphicsDevice, BaseGame.Get.Content);
#endif
        }

        void CleanUpDeletedActors()
        {
            bool somethingdeleted = false;
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i].CheckFlag(Flags.DELETE))
                {
                    world.RemoveBody((actors[i] as FarActor).body);
                    actors.Remove(actors[i]);
                    somethingdeleted = true;
                    --i;
                }
                else
                {

                }
            }

            if (somethingdeleted)
            {
                for (int i = 0; i < actors.Count; i++)
                {
                    (actors[i] as FarActor).body.Awake = true;
                }
            }
        }
        public override Vector2 Gravity
        {
            get{return world.Gravity;}
            set { world.Gravity = value; }
        }
        public override bool DestroyActor(JabActor actor)
        {
            FarActor act = (FarActor)(actor);

            world.RemoveBody(act.body);
            act.body = null;
            actors.Remove(act);
            return true;
		}
		
#if IPHONE || WINDOWS// || WINDOWS_PHONE
		bool everyOther = false;
#endif
        public override void Update(GameTime gameTime)
        {
#if IPHONE || WINDOWS//|| WINDOWS_PHONE
            if (!everyOther)		
			{
				everyOther = true;
			//	return;
			}
			everyOther = false;
#endif
#if IPHONE || WINDOWS
			float updatestep = Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * SimulationSpeedFactor,
                                        (1f / 60f) * SimulationSpeedFactor);
#else
            double d = 1.0D / 30.0D;
			float updatestep = Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * SimulationSpeedFactor,
                                       0.0333333f);
            updatestep /= 2.0f;
#endif
            CleanUpDeletedActors();

            world.Step(updatestep);


            //for (int i = 0; i < 5; i++)
              //  world.Step((1f / 60.0f) * SimulationSpeedFactor);

#if WINDOWS_PHONE
            world.Step(updatestep);
#endif
            CleanUpDeletedActors();



            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i].LinearVelocity.Length() > 100)
                {
                    actors[i].DoContinuous = true;
                }
                else
                {
                    actors[i].DoContinuous = false;
                }
                (actors[i] as FarActor).LastGoodPosition = (actors[i] as FarActor).body.Position;
                (actors[i] as FarActor).LastGoodVelocity = (actors[i] as FarActor).body.LinearVelocity;
            }

            base.Update(gameTime);
        }
#if WINDOWS || WINDOWS_PHONE
        private DebugViewXNA _debugView;
#endif
        public override void Draw()
        {
            return;
#if WINDOWS || WINDOWS_PHONE
            base.Draw();

            Matrix proj = Matrix.CreateOrthographic(BaseGame.Get.BackBufferWidth / Camera.Get.WorldScale.X / 100.0f, BaseGame.Get.BackBufferHeight / Camera.Get.WorldScale.Y / 100.0f, 0, 1000000);
            Vector3 campos = new Vector3();
            campos.X = -Camera.Get.Position.X / 100.0f;
            campos.Y = Camera.Get.Position.Y / -100.0f;
            campos.Z = 0;
            Matrix tran = Matrix.Identity;
            tran.Translation = campos;
            Matrix view = tran;


            SpriteBatch.End();

            _debugView.RenderDebugData(ref proj, ref view);



            float screenScale = Camera.CurrentCamera.UniformWorldScale;
            Matrix offset = Matrix.CreateTranslation(new Vector3(BaseGame.Get.HalfBackBufferWidth / screenScale, BaseGame.Get.HalfBackBufferHeight / screenScale, 0));
            Matrix pos = Matrix.CreateTranslation(new Vector3(-Camera.Get.PosX, Camera.Get.PosY, 0));
            Matrix scale = Matrix.CreateScale(screenScale);
            pos = pos * offset;
            pos *= scale;


           BaseGame.Get.Begin();
#endif
        }
        public override JabActor CreateFromTriangles(List<Vector2> triangles, Vector2 pos, JabActor.BodyType bodytype)
        {
            try
            {
                FarActor actor = new FarActor();
                actor.body = new Body(world);// world.CreateBody();
                actor.body.UserData = actor;

                //FarseerPhysics.Settings.MaxPolygonVertices = 33;

                for (int i = 0; i < triangles.Count; i++)
                {
                    Vector2[] tri = new Vector2[3];
                    tri[0] = triangles[i] / 100.0f;
                    ++i;
                    tri[1] = triangles[i] / 100.0f;
                    ++i;
                    tri[2] = triangles[i] / 100.0f;

                    Vertices vertices = new Vertices(tri);
                    PolygonShape shape = new PolygonShape(vertices, 1.0f);
                    actor.body.CreateFixture(shape, 1.0f);
                }

                actor.BodyState = bodytype;
                actor.Position = pos;

                actors.Add(actor);
                return actor;

            }
            catch (Exception e)
            {
                //System.Windows.MessageBox.Show(e.ToString());
            }

            return null;
        }
        public override JabActor CreateSphere(float radius, Vector2 pos, JabActor.BodyType bodytype)
        {
            try
            {
                radius /= 100;
                FarActor actor = new FarActor();
                actor.body = new Body(world);// world.CreateBody();
                actor.body.UserData = actor;

                Vector2 center = new Vector2(-radius, -radius) / 2.0f;
              //  FarseerPhysics.Settings.MaxPolygonVertices = 33;
                Vertices sphere = PolygonTools.CreateCircle(radius, 32);

                PolygonShape shape = new PolygonShape(sphere, 1.0f);
                actor.BodyState = bodytype;
                actor.body.CreateFixture(shape, 1.0f);
                actor.Position = pos;
                actor.Width = radius;
                actor.Height = radius;

                actors.Add(actor);
                return actor;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public override JabActor CreateBox(Vector2 dim, Vector2 pos, JabActor.BodyType bodytype)
        {
            try
            {
                dim /= 100;
                FarActor actor = new FarActor();
                actor.body = new Body(world);// world.CreateBody();
                actor.body.UserData = actor;
                Vector2 center = new Vector2(-dim.X, -dim.Y) / 2.0f;
                Vertices box = PolygonTools.CreateRectangle(dim.X / 2.0f, dim.Y / 2.0f, center, 0);
                for (int i = 0; i < box.Count(); i++)
                {
                    Vector2 vert = box[i];
                    vert.X += dim.X / 2.0f;
                    vert.Y += dim.Y / 2.0f;
                    box[i] = vert;
                    vert = box[i];
                }
                PolygonShape shape = new PolygonShape(box, 1.0f);
                actor.BodyState = bodytype;
                actor.body.CreateFixture(shape, 1.0f);
                actor.Position = pos;
                actor.Width = dim.X * 100;
                actor.Height = dim.Y * 100;

                actors.Add(actor);
                return actor;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public override RayCastHit RayCast(Vector2 origin, Vector2 destination)
        {
            if (destination == origin)
            {
                return null;
            }
            origin /= 100.0f;
            destination /= 100.0f;

            rayOrigin = origin;
            rayCurBest = float.MaxValue;
            rayActorHit = null;

            try
            {
                world.RayCast(Nearest, origin, destination);
            }
            catch (Exception e)
            {
                return new RayCastHit();
            }
            RayCastHit hit = new RayCastHit();
            hit.actor = rayActorHit;
            hit.worldImpact = hitPosition * 100.0f;
            hit.worldNormal = hitNormal;

            hit.Distance = (hit.worldImpact - (origin*100)).Length();

            return hit;
        }



        static Vector2 rayOrigin = Vector2.Zero;
        static float rayCurBest = float.MaxValue;
        static Vector2 hitPosition = Vector2.Zero;
        static Vector2 hitNormal = Vector2.Zero;
        static JabActor rayActorHit = null;
        static float Nearest(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if (Vector2.Distance(point, rayOrigin) < rayCurBest && !((JabActor)fixture.Body.UserData).IgnoreRayCast)
            {
                hitPosition = point;
                hitNormal = normal;
                rayCurBest = Vector2.Distance(point, rayOrigin);
                rayActorHit = (JabActor)fixture.Body.UserData;
            }
            return 1;
        }
    };
}