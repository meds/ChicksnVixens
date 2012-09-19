using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

using Jabber.Physics;

using Microsoft.Xna.Framework.Content;

namespace Jabber.Media
{
    public class PhysicSprite : BaseSprite
    {
        JabActor body;
        protected BaseSprite sprite;
        
        // Should the position/rotation be handled by the physics object?
        bool DoRotation = true;
        bool DoPosition = true;
        public bool DoDimensions { get; set; }
        public bool PhysicsRotate
        {
            get { return DoRotation; }
            set {
                if (!value && DoRotation && Sprite != null && Body != null)
                {
                    Sprite.Rot = Body.Rot;
                }
                DoRotation = value; 
            }
        }
        public bool DoHandle
        {
            get;set;
        }
        public bool PhysicsPosition
        {
            get { return DoPosition; }
            set {
                if (!value && DoPosition && Sprite != null && Body != null)
                {
                    Sprite.Position = Body.Position;
                }
                DoPosition = value; 
            }
        }

        public virtual BaseSprite Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        public Sprite AsType
        {
            get { return sprite as Sprite; }
        }



        protected string textureDir;

        public string TextureDir
        {
            set { Debug.Assert(sprite == null, "Error: Sprite already initialized!"); textureDir = value; }
            get { return textureDir; }
        }
        public override void Initialize(ContentManager Content)
        {
            if (sprite != null)
            {
                return;
            }
            sprite = new Sprite(textureDir);
            sprite.Initialize(Content);
            base.Initialize(Content);
        }


        public PhysicSprite()
        {
        }


        public PhysicSprite(Vector2 dim, Vector2 pos, bool dynamic, JabWorld world, string imagedir)
            : base()
        {
            TextureDir = imagedir;
            Width = dim.X;
            Height = dim.Y;
            JabActor.BodyType type = JabActor.BodyType.DYNAMIC;
            if (!dynamic)
            {
                type = JabActor.BodyType.STATIC;
            }
            body = world.CreateBox(dim, pos, type);
            body.UserData = this;
            this.world = world;
            DoDimensions = true;
            DoHandle = true;
        }
        public PhysicSprite(float radius, Vector2 pos, bool dynamic, JabWorld world, string imagedir)
            : base()
        {
            TextureDir = imagedir;
            Width = radius;
            Height = radius;
            JabActor.BodyType type = JabActor.BodyType.DYNAMIC;
            if (!dynamic)
            {
                type = JabActor.BodyType.STATIC;
            }
            body = world.CreateSphere(radius/2.0f, pos, type);
            body.UserData = this;
            this.world = world;

           // Initialize();
        }
        JabWorld world;
        public JabWorld World
        {
            get { return world; }
            set { world = value; }
        }
        public JabActor Body
        {
            get { return body; }
            set { body = value; }
        }
        ~PhysicSprite()
        {
            //Debug.Assert(body == null);
        }
        public override void UnloadContent()
        {
            body.RaiseFlag(Flags.DELETE);
            body = null;
            base.UnloadContent();
        }
        public override void Draw()
        {
            if (DoPosition)
            {
                sprite.Position = body.Position;
            }
            if (DoRotation)
            {
                sprite.Rot = body.Rot;
            }
            if (DoDimensions)
            {
                sprite.Width = Width;
                sprite.Height = Height;
                sprite.Scale = Scale;
            }
            if (DoHandle)
            {
                sprite.Handle = this.Handle;
            }
            sprite.PosZ = PosZ;
            sprite.Colour = colour;

            sprite.Draw();
            base.Draw();
        }

        public override bool IsVisible()
        {
            return AsType.IsVisible();
        }

        public override void Update(GameTime gameTime)
        {
            body.Update(gameTime);
            sprite.Update(gameTime);
            base.Update(gameTime);
        }

        override public float Rot
        {
            get {
                if (PhysicsRotate)
                {
                    return body.Rot;
                }
                else
                {
                    return Sprite.Rot;
                }
            }
            set {
                if (PhysicsRotate)
                {
                    body.Rot = value;
                }
                else
                {
                    Sprite.Rot = Rot;
                }
            
            }
        }
        override public Vector2 Position
        {
            get
            {
                if (PhysicsPosition)
                {
                    if (body == null)
                    {
                        return Vector2.Zero;
                    }
                    return body.Position;
                }
                else
                {
                    return Sprite.Position;// base.Position;
                }
            }
            set {
                if (PhysicsPosition)
                {
                    body.Position = value;
                }
                else
                {
                    Sprite.Position = value;// base.Position;
                }
                //Position = value; 
            }
        }
        override public float PosY
        {
            get { return body.PosY; }
            set { 
                body.PosY = value; 
            }
        }
        override public float PosX
        {
            get { return body.PosX; }
            set {
                body.PosX = value; }
        }
    }
    public class PhysicAnimSprite : PhysicSprite
    {
        public PhysicAnimSprite(Vector2 dim, Vector2 pos, bool dynamic, JabWorld world, string imagedir)
            :base(dim, pos, dynamic, world, imagedir)
        {
        }
        public PhysicAnimSprite(float radius, Vector2 pos, bool dynamic, JabWorld world, string imagedir)
            : base(radius, pos, dynamic, world, imagedir)
        {
        }
        public AnimSprite AnimSprite
        {
            get { return sprite as AnimSprite; }
        }
        public override void Initialize(ContentManager content)
        {
            sprite = new AnimSprite(textureDir);
            sprite.Initialize(content);
        }


        public AnimSprite AsType
        {
            get { return sprite as AnimSprite; }
        }
    }
}
