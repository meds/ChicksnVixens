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
    public partial class Shape : BaseSprite
    {
        public virtual void Draw_XNA()
        {
            try
            {
                SpriteBatch.End();

                // Culling is not important since we're drawing all the vertices on a flat surface
                BaseGame.Get.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                // We'll use the games engines 2D camera to determine x/y offset, z can remain 0
                Vector3 cameraPosition = new Vector3(0, 0, 0);
                cameraPosition.X = Camera.Get.PosX;
                cameraPosition.Y = Camera.Get.PosY;

                // Aspect ratio would typically be 800x480
                float aspect = BaseGame.Get.GraphicsDevice.Viewport.AspectRatio;


                Matrix world = Matrix.Identity;

                // Set the rotation to what we want (typically 0)
                Matrix rot = Matrix.CreateRotationZ(-Rot);

                // The view matrix is the position and look at spot of the camera, we construct it using our camera position vector
                Matrix view = Matrix.CreateLookAt(cameraPosition, cameraPosition + new Vector3(0, 0, -1), Vector3.Up);

                // The projection determines how the vertices will be drawn to the screen. To have it lign up with sprites drawn through SpriteBatch
                // we need to set the viewing area to 800x480 and we set the near clip to 0 and far clip to 1, this will ensure the drawing order is
                // consistant with the layerDepth value in the SpriteBatch.Draw() function.
                Matrix projection = Matrix.CreateOrthographic(BaseGame.Get.BackBufferWidth / Camera.Get.WorldScale.X, BaseGame.Get.BackBufferHeight / Camera.Get.WorldScale.Y, 0, 1);

                // PosZ is the equivalent of 'LayerDepth' of SpriteBatch.
                world.Translation = new Vector3(PosX, PosY, -0.5f);
                rot *= world;
                world = rot;

                Color color = Color.White;

                // Set BasicEffect parameters.
                basicEffect.World = world;
                basicEffect.View = view;
                basicEffect.Projection = projection;
                // Texture is the texture we want to draw, it must be a power of 2 texture
                basicEffect.Texture = texture;
                basicEffect.DiffuseColor = color.ToVector3();
                basicEffect.Alpha = 1.0f;

                GraphicsDevice device = basicEffect.GraphicsDevice;
                device.DepthStencilState = DepthStencilState.Default;
                basicEffect.TextureEnabled = true;

                // Set our vertex declaration, vertex buffer, and index buffer.
                device.SetVertexBuffer(vertexBuffer);

                foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    // Draws the vertex buffer set in device.SetVertexBuffer up to the number of vertices set
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Count);
                }

                base.Draw();

                BaseGame.Get.Begin();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }

        public virtual void Initialize_XNA(ContentManager Content)
        {
            try
            {
                base.Initialize(Content);

                if (textureDir == null)
                {
                    //      texture = Content.Load<Texture2D>("textures/ground/grass");
                }
                else
                {
                    texture = Content.Load<Texture2D>(textureDir);
                }
                basicEffect = new BasicEffect(BaseGame.Get.GraphicsDevice);

                basicEffect.EnableDefaultLighting();
                basicEffect.LightingEnabled = false;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }
        public virtual void FinalizeVertices_XNA()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 pos = vertices[i].Position;

                Vector2 pos2D = new Vector2(pos.X, pos.Y);
                pos2D /= tilesize;
                pos2D *= -1;
                pos2D += tileUVShift;
                vertices[i] = new VertexPositionNormal(vertices[i].Position, vertices[i].Normal, pos2D);
            }

            // Create a vertex buffer, and copy our vertex data into it.
            vertexBuffer = new VertexBuffer(BaseGame.Get.GraphicsDevice,
                                            typeof(VertexPositionNormal),
                                            vertices.Count, BufferUsage.None);

            vertexBuffer.SetData(vertices.ToArray());
        }
    }
}
