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

using Jabber;
using Jabber.Util;
using Jabber.Media;

namespace Jabber.J3D
{
#if WINDOWS_PHONE || WINDOWS
    public struct ShapeVertexFormat
    {
        private Vector3 position;
        private Color color;

        public ShapeVertexFormat(Vector3 position, Color color)
        {
            this.position = position;
            this.color = color;
        }

        public static VertexElement[] VertexElements =
 {
     new VertexElement(0, 0, VertexElementUsage.Position, 0),
     new VertexElement(3*sizeof(float), 0, VertexElementUsage.Color, 1)
 };
    };
#endif
    /// <summary>
    /// Custom vertex type for vertices that have just a
    /// position and a normal, without any texture coordinates.
    /// </summary>
    public struct VertexPositionNormal
#if WINDOWS || WINDOWS_PHONE
        : IVertexType
#endif
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VertexPositionNormal(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
        }

#if WINDOWS_PHONE || WINDOWS
        /// <summary>
        /// A VertexDeclaration object, which contains information about the vertex
        /// elements contained within this struct.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionNormal.VertexDeclaration; }
        }
#endif
    }

    public partial class Shape : BaseSprite
    {
        public Shape()
            : base()
        {

        }
        /*
        void AddIndex(int index)
        {
            indices.Add((ushort)index);
        }*/
        
        public void AddTriangle(Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            AddVertex(pos1);
            AddVertex(pos2);
            AddVertex(pos3);
        }

        public override JabRectangle GetRectangle()
        {
            return new JabRectangle(0, 0, 1, 1);
        }

        public void AddRectangle(Vector3 pos, Vector2 dimensions, float rot)
        {
            Vector3 vert = new Vector3(0, 0, pos.Z);

            // bottom left corner
            vert.X = -dimensions.X / 2.0f;
            vert.Y = -dimensions.Y / 2.0f;

            vert = new Vector3(JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).X, JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).Y, pos.Z);
            vert += pos;

            AddVertex(vert);

            // bottom right corner
            vert.X = dimensions.X / 2.0f;
            vert.Y = -dimensions.Y / 2.0f;
            vert = new Vector3(JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).X, JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).Y, pos.Z);
            vert += pos;

            AddVertex(vert);

            // top left corner
            vert.X = -dimensions.X / 2.0f;
            vert.Y = dimensions.Y / 2.0f;
            vert = new Vector3(JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).X, JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).Y, pos.Z);
            vert += pos;

            AddVertex(vert);


            rot += 1.0f * (float)Math.PI;

            // bottom left corner
            vert.X = -dimensions.X / 2.0f;
            vert.Y = -dimensions.Y / 2.0f;

            vert = new Vector3(JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).X, JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).Y, pos.Z);
            vert += pos;

            AddVertex(vert);

            // bottom right corner
            vert.X = dimensions.X / 2.0f;
            vert.Y = -dimensions.Y / 2.0f;
            vert = new Vector3(JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).X, JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).Y, pos.Z);
            vert += pos;

            AddVertex(vert);

            // top left corner
            vert.X = -dimensions.X / 2.0f;
            vert.Y = dimensions.Y / 2.0f;
            vert = new Vector3(JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).X, JabMath.RotateVector(new Vector2(vert.X, vert.Y), rot).Y, pos.Z);
            vert += pos;

            AddVertex(vert);

        }
        public void AddVertex(Vector3 position)
        {
            vertices.Add(new VertexPositionNormal(position, new Vector3(0, 0, -1), new Vector2(0, 0)));
        }
        public Vector2 GetLastVector()
        {
            return new Vector2(vertices[vertices.Count - 1].Position.X, vertices[vertices.Count - 1].Position.Y);
        }

        public Vector2[] GetVertices()
        {
            Vector2[] ToReturn = new Vector2[vertices.Count];

            for (int i = 0; i < ToReturn.Count<Vector2>(); i++)
            {
                ToReturn[i] = new Vector2(vertices[i].Position.X, vertices[i].Position.Y);
            }
            return ToReturn;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        
#if WINDOWS || WINDOWS_PHONE
            vertexBuffer.Dispose();
            basicEffect.Dispose();
#endif
        }

        public float TileSize
        {
            get { return tilesize; }
            set { tilesize = value; }
        }
        float tilesize = 100;
        Vector2 tileUVShift = Vector2.Zero;
        public Vector2 TileUVShift
        {
            set { tileUVShift = value; }
            get { return tileUVShift; }
        }
        public virtual void FinalizeVertices()
        {
#if WINDOWS_PHONE || WINDOWS
            FinalizeVertices_XNA();
#endif
        }
        Texture2D texture;
        public string TextureDir
        {
            set { System.Diagnostics.Debug.Assert(texture == null, "Error: Texture already loaded!"); textureDir = value; }
            get { return TextureDir; }
        }
        string textureDir = null;
        public override void Initialize(ContentManager Content)
        {
#if WINDOWS_PHONE || WINDOWS
            Initialize_XNA(Content);
#endif
        }
        public override float Rot
        {
            get { return base.Rot; }
            set { base.Rot = value; }
        }

        public override bool IsVisible()
        {
            return true;
        }

        public override void Draw()
        {
#if WINDOWS_PHONE || WINDOWS
            Draw_XNA();
#endif
        }

#if WINDOWS_PHONE || WINDOWS
        public BasicEffect basicEffect;
        VertexBuffer vertexBuffer;
#endif

        // During the process of constructing a primitive model, vertex
        // and index data is stored on the CPU in these managed lists.
        List<VertexPositionNormal> vertices = new List<VertexPositionNormal>();
       // List<ushort> indices = new List<ushort>();
    };
}