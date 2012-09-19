using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

using Jabber.Util;

using Jabber.Media;
/*
namespace Jabber.J3D
{
    public class Mesh : BaseSprite
    {
        Model model = null;
        //Matrix transform = null;

        ContentManager modelContentManager = null;
        Matrix[] boneTransforms;
        string modelDir = null;
        public string ModelDir
        {
            set { System.Diagnostics.Debug.Assert(model == null, "Error: Model already loaded!"); modelDir = value; }
            get { return modelDir; }
        }

        public override void Initialize(ContentManager Content)
        {
            modelContentManager = new ContentManager(BaseGame.Get.Services);
            modelContentManager.RootDirectory = "Content";
            model = modelContentManager.Load<Model>(ModelDir);
            boneTransforms = new Matrix[model.Bones.Count];

            base.Initialize(Content);
        }
        public override void UnloadContent()
        {
            modelContentManager.Dispose();
            base.UnloadContent();
        }
        public override void Draw()
        {
            Matrix transform = Matrix.Identity;
            
            Matrix scale = new Matrix();
            scale = Matrix.CreateScale(ScaleX, ScaleY, 0.001f);
            Matrix rot = Matrix.CreateRotationZ(Rot);
            transform.Translation = new Vector3(Position.X, Position.Y, 1000000/2.0f);
            model.Root.Transform = rot * scale * transform;

            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    Vector3 cameraPosition = new Vector3(0, 0, 0);
                    cameraPosition.X = -Camera.Get.PosX;
                    cameraPosition.Y = Camera.Get.PosY;
                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraPosition + new Vector3(0, 0, 1), Vector3.Up);

                    effect.Projection = Matrix.CreateOrthographic(800 / Camera.Get.WorldScale.X, 480 / Camera.Get.WorldScale.Y, 0, 1000000);
                    
                    effect.TextureEnabled = true;
                }

                mesh.Draw();
            }
            base.Draw();
        }
    }
}
 */