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


namespace Jabber.Physics
{
    public class JabActor : JabJect
    {
        public enum BodyType
        {
            DYNAMIC,
            STATIC
        };
        protected BodyType bodyType;
        virtual public BodyType BodyState
        {
            set{bodyType = value;}
            get{return bodyType;}
        }

        public virtual bool Awake
        {
            get;
            set;
        }

        public virtual float Mass
        {
            get { return 0; }
            set { }
        }
        public JabActor()
            : base()
        {
        }
        public virtual float Density
        {
            get { return 0; }
            set { }
        }
        protected int collisionGroup = 0;

        public int CollisionGroup
        {
            get { return collisionGroup; }
            set { collisionGroup = value; }
        }

        public virtual float LinearDamping
        {
            get { return 0; }
            set { }
        }
        public virtual float AngularDamping
        {
            get { return 0; }
            set { }
        }

        public virtual bool DoContinuous
        {
            get { return false; }
            set { }
        }
        public virtual float Restitution
        {
            get { return 0; }
            set { }
        }
        public virtual float Friction
        {
            get { return 0; }
            set { }
        }
        public virtual bool HeedGravity
        {
            get { return true; }
            set { }
        }
        public virtual bool IgnoreRayCast
        {
            get { return bIgnoreRayCast; }
            set { bIgnoreRayCast = value; }
        }
        public virtual void AddForce(Vector2 dir)
        {
            throw new NotImplementedException();
        }
        public virtual void AddForce(Vector2 dir, Vector2 worldpos)
        {
            throw new NotImplementedException();
        }
        public virtual void AddLinearImpulse(Vector2 dir, Vector2 worldpos)
        {
            throw new NotImplementedException();
        }
        public virtual void AddLinearImpulse(Vector2 dir)
        {
            throw new NotImplementedException();
        }

        public virtual float AngularVelocity
        {
            get { return 0; }
            set { }
        }

        public virtual Vector2 LinearVelocity
        {
            get { return new Vector2(0, 0); }
            set { }
        }


        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        public object UserData = null;

        string name = "ACTOR";
        bool bIgnoreRayCast = false;
    };

    public class RayCastHit
    {
        public Vector2 worldImpact;
        public Vector2 worldNormal;
        public JabActor actor = null;
        public float Distance = 0;
    };
    public class Collision
    {
        public JabActor actor1;
        public JabActor actor2;

        public JabActor HasActor(JabActor actor)
        {
            if (actor == actor1)
            {
                return actor2;
            }
            else if (actor == actor2)
            {
                return actor1;
            }
            else
                return null;
        }
    };

    public class CollisionEvent : Jabber.Util.Event
    {
        public CollisionEvent()
            : base()
        {
        }

        // If the passed in actor is either actor1 or 2 this class returns the other actor,
        // if it's neither this function returns null
        public JabActor ActorPresent(JabActor actor)
        {
            if (actor == _Actor1)
            {
                return _Actor2;
            }
            else if (actor == _Actor2)
            {
                return _Actor1;
            }
            else
            {
                return null;
            }
        }

        public enum ContactType
        {
            ONCONTACT,
            AFTERCONTACT
        }
        public ContactType contactType = ContactType.ONCONTACT;

        public JabActor _Actor1;
        public JabActor _Actor2;
    }
    public abstract class JabWorld : JabJect
    {
        public JabWorld()
            : base()
        {
            SimulationSpeedFactor = 1.0f;
        }

        public delegate void OnContactDelegate(Collision contact);
        public delegate void EndContactDelegate(Collision contact);


        public OnContactDelegate BeginContact = null;
        public EndContactDelegate EndContact = null;

        public abstract Vector2 Gravity{get;set;}
        public abstract JabActor CreateBox(Vector2 dim, Vector2 pos, JabActor.BodyType bodytype);
        public abstract JabActor CreateSphere(float radius, Vector2 pos, JabActor.BodyType bodytype);
        public abstract JabActor CreateFromTriangles(List<Vector2> triangles, Vector2 pos, JabActor.BodyType bodytype);
        public abstract void Initialize(Vector2 gravity);
        public abstract bool DestroyActor(JabActor actor);
        public abstract RayCastHit RayCast(Vector2 origin, Vector2 destination);

        public JabActor GetActor(int i)
        {
            return actors[i];
        }
        public List<JabActor> Actors
        {
            get { return actors; }
        }
        protected List<JabActor> actors = new List<JabActor>();

        public bool IsEntityGroup(int collisionGroup)
        {
            for (int i = 0; i < collisionRelations.Count; i++)
            {
                if (collisionRelations[i].colGroup1 == collisionGroup && collisionRelations[i].colGroup2 < 0 && collisionRelations[i].IsEntity)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AllowCollisionsBetweenGroups(int collisionGroupA, int collisionGroupB)
        {
            for (int i = 0; i < collisionRelations.Count; i++)
            {
                if (collisionRelations[i].colGroup1 == collisionGroupA && collisionRelations[i].colGroup2 == collisionGroupB ||
                    collisionRelations[i].colGroup1 == collisionGroupB && collisionRelations[i].colGroup2 == collisionGroupA
                    )
                {
                    return collisionRelations[i].AllowCollisions;
                }
                if (collisionRelations[i].colGroup2 < 0 && collisionRelations[i].colGroup1 == collisionGroupA ||
                    collisionRelations[i].colGroup1 < 0 && collisionRelations[i].colGroup2 == collisionGroupA || 
                    collisionRelations[i].colGroup2 < 0 && collisionRelations[i].colGroup1 == collisionGroupB ||
                    collisionRelations[i].colGroup1 < 0 && collisionRelations[i].colGroup2 == collisionGroupB
                    )
                {
                    return collisionRelations[i].AllowCollisions;
                }
            }

            return true;
        }

        public enum CollisionRelations
        {
            ALLOWCOLLISION,
            DISABLECOLLISION,
            ISENTITY
        }
        class CollisionGroupRelations
        {
            public int colGroup1;
            public int colGroup2;


            public CollisionRelations relations = CollisionRelations.DISABLECOLLISION;


            public bool IsEntity { get { return relations == CollisionRelations.ISENTITY; } }
            public bool AllowCollisions { get{return relations ==  CollisionRelations.ALLOWCOLLISION;} }// = false;
        }
        List<CollisionGroupRelations> collisionRelations = new List<CollisionGroupRelations>();

        public void RemoveCollisionForAll(int collisionGroup)
        {
            for (int i = 0; i < collisionRelations.Count; i++)
            {
                if (collisionRelations[i].colGroup1 == collisionGroup && collisionRelations[i].colGroup2 < 0)
                {
                    collisionRelations.RemoveAt(i); --i;
                }
            }
        }
        public void MakeEntityGroup(int collisionGroup)
        {
            for (int i = 0; i < collisionRelations.Count; i++)
            {
                if (collisionRelations[i].colGroup1 == collisionGroup || collisionRelations[i].colGroup2 == collisionGroup)
                {
                    collisionRelations.RemoveAt(i); --i;
                }
            }

            CollisionGroupRelations relation = new CollisionGroupRelations();
            relation.colGroup1 = collisionGroup;
            relation.colGroup2 = -1;
            relation.relations = CollisionRelations.ISENTITY;
            collisionRelations.Add(relation);
        }

        public void SetCollisionForAll(int collisionGroup, bool collisionEnabled)
        {
            for (int i = 0; i < collisionRelations.Count; i++)
            {
                if (collisionRelations[i].colGroup1 == collisionGroup || collisionRelations[i].colGroup2 == collisionGroup)
                {
                    collisionRelations.RemoveAt(i);--i;
                }
            }

            CollisionGroupRelations relation = new CollisionGroupRelations();
            relation.colGroup1 = collisionGroup;
            relation.colGroup2 = -1;
            if (!collisionEnabled)
            {
                relation.relations = CollisionRelations.DISABLECOLLISION;
            }
            else
            {
                relation.relations = CollisionRelations.ALLOWCOLLISION;
            }
            collisionRelations.Add(relation);
        }

        public void SetCollisions(int collisionGroupA, int collisionGroupB, bool collisionEnabled)
        {
            RemoveCollisionForAll(collisionGroupA);
            RemoveCollisionForAll(collisionGroupB);

            for (int i = 0; i < collisionRelations.Count; i++)
            {
                if (collisionRelations[i].colGroup1 == collisionGroupA && collisionRelations[i].colGroup2 == collisionGroupB ||
                    collisionRelations[i].colGroup1 == collisionGroupB && collisionRelations[i].colGroup2 == collisionGroupA
                    )
                {
                    if (!collisionEnabled)
                    {
                        collisionRelations[i].relations = CollisionRelations.DISABLECOLLISION;
                    }
                    else
                    {
                        collisionRelations[i].relations = CollisionRelations.ALLOWCOLLISION;
                    }

                    return;
                }
            }
            CollisionGroupRelations rel = new CollisionGroupRelations();
            rel.colGroup1 = collisionGroupA;
            rel.colGroup2 = collisionGroupB;
            if (!collisionEnabled)
            {
                rel.relations = CollisionRelations.DISABLECOLLISION;
            }
            else
            {
                rel.relations = CollisionRelations.ALLOWCOLLISION;
            }
           // rel.AllowCollisions = collisionEnabled;

            collisionRelations.Add(rel);
        }

        public float SimulationSpeedFactor { get; set; }
    }
}
