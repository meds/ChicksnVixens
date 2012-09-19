using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using Jabber.Physics;

namespace Jabber.J3D
{
    public class PhysicShape : Shape
    {
        public PhysicShape(JabWorld world, JabActor.BodyType bodytype)
            : base()
        {
            this.bodyType = bodytype;
            this.world = world;
        }
        public override void FinalizeVertices()
        {
            base.FinalizeVertices();

            //body = world.CreateFromTriangles(GetVertices().ToList<Vector2>(), position, bodyType);
        }
        ~PhysicShape()
        {
            //System.Diagnostics.Debug.Assert(body == null);
        }
        public override void UnloadContent()
        {
            //body.RaiseFlag(Flags.DELETE);
            //body = null;
            base.UnloadContent();
        }
        public override void Draw()
        {
            //this.position = body.Position;
            //this.Rot = body.Rot;
            base.Draw();
        }
     /*   public JabActor Body
        {
            get { return body; }
        }*/
      //  JabActor body;
        JabActor.BodyType bodyType;
        protected JabWorld world;
    }
}
