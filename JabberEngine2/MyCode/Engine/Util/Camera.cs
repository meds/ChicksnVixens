using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Jabber.Media;

namespace Jabber.Util
{
    public class Camera : JabJect//, GameComponent
    {
        #region Singleton
        /// <summary>
        /// The singleton for this type.
        /// </summary>
        private static Camera camera = null;
        private static Camera currentCamera = null;
        #endregion

        public static Camera CurrentCamera
        {
            set { currentCamera = value; }
            get { return currentCamera; }
        }


        public Matrix CameraView
        {
            get
            {
                float screenScale = Camera.CurrentCamera.UniformWorldScale;
                Matrix offset = Matrix.CreateTranslation(new Vector3(BaseGame.Get.HalfBackBufferWidth / screenScale, BaseGame.Get.HalfBackBufferHeight / screenScale, 0));
                Matrix pos = Matrix.CreateTranslation(new Vector3(-Camera.Get.PosX, Camera.Get.PosY, 0));
                Matrix scale = Matrix.CreateScale(screenScale);
                pos = pos * offset;
                pos *= scale;

                return pos;
            }
        }

        /*
        private Vector2 position = new Vector2(0, 0);
        private float fPosZ = 0;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float PosY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public float PosX
        {
            get { return position.X; }
            set { position.X = value; }
        }
        */

        public float HorizontalCoverSpace
        {
            get
            {
                return BaseGame.Get.BackBufferWidth / worldScale.X;
            }
        }
        public float VerticalCoverSpace
        {
            get
            {
                return BaseGame.Get.BackBufferHeight / worldScale.Y;
            }
        }
        public float ScaleToCoverHorizontalArea(float left, float right)
        {
            float width = right - left;
            float horizontalCoverAt11 = Jabber.BaseGame.Get.BackBufferWidth;
            float hortScale = horizontalCoverAt11 / width;

            return hortScale;
        }

        public void SetToCoverSpace(float top, float left, float bottom, float right)
        {
            float vert = ScaleToCoverVerticalArea(top, bottom);
            float hort = ScaleToCoverHorizontalArea(left, right);

            if (vert > hort)
            {
                UniformWorldScale = hort;
            }
            else
            {
                UniformWorldScale = vert;
            }

            PosX = (left + right) / 2.0f;
            PosY = (bottom + top) / 2.0f;
        }

        public float ScaleToCoverVerticalArea(float top, float bottom)
        {
            float height = top - bottom;
            float verticalCoverAt11 = Jabber.BaseGame.Get.BackBufferHeight;
            float vertScale = verticalCoverAt11 / height;

            return vertScale;
        }

        public float PosYWhereTopIs(float top)
        {
            float verticalCover = Jabber.BaseGame.Get.BackBufferHeight / WorldScale.Y;
            return top - verticalCover / 2.0f;
        }
        public float PosYWhereBottomIs(float bottom)
        {
            float verticalCover = Jabber.BaseGame.Get.BackBufferHeight / WorldScale.Y;
            return bottom + verticalCover / 2.0f;
        }
     
        public float PosXWhereLeftMostIs(float leftmost)
        {
            float horizontalCover = Jabber.BaseGame.Get.BackBufferWidth / UniformWorldScale;
            return leftmost + horizontalCover / 2.0f;
        }

        public float PosXWhereRightMostIs(float rightMost)
        {
            float horizontalCover = Jabber.BaseGame.Get.BackBufferWidth / UniformWorldScale;
            return rightMost - horizontalCover / 2.0f;
        }

        public float RightMostWherePosXIs(float posx)
        {
            float horizontalCover = Jabber.BaseGame.Get.BackBufferWidth / UniformWorldScale;
            return posx + horizontalCover / 2.0f;
        }
        public float LeftMostWherePosXIs(float posx)
        {
            float horizontalCover = Jabber.BaseGame.Get.BackBufferWidth / UniformWorldScale;
            return posx - horizontalCover / 2.0f;
        }

        public override float PosZ
        {
            get { return fPosZ; }
            set
            {
                if (value < 0 || value > 1)
                {
                  //  System.Windows.MessageBox.Show("Error: Z depth value set is out of range [0,1] at : " + value.ToString());
                }
                fPosZ = value;
            }
        }

        public virtual bool IsVisible(BaseSprite sprite)
        {
            {
                float max = (float)Math.Max(sprite.Width * ScaleX, sprite.Height * ScaleY);
                Vector2 pos = sprite.Position;

                Vector2 bottomRight = ScreenToWorld(Vector2.One);
                Vector2 topLeft = ScreenToWorld(Vector2.Zero);


                if (pos.X < bottomRight.X && pos.X > topLeft.X)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }



            JabRectangle camView = new JabRectangle(0, 0, 1.0f, 1.0f);
            JabRectangle spriteRect = sprite.GetRectangle();



            if (!(camView.Intersects(spriteRect) || spriteRect.Intersects(camView)))
            {
                int k = 0;
            }

            return camView.Intersects(spriteRect) || spriteRect.Intersects(camView);
        }
        public Vector2 WorldToScreen(Vector2 pos)
        {
            pos *= WorldScale;

            pos -= Position * WorldScale;
            pos.Y /= -1;
            pos += new Vector2(BaseGame.Get.HalfBackBufferWidth, BaseGame.Get.HalfBackBufferHeight);


            pos /= BaseGame.Get.BackBufferDimensions;

            return pos;
        }
        public Vector2 ScreenToWorld(Vector2 pos)
        {
            pos *= BaseGame.Get.BackBufferDimensions;
            pos -= new Vector2(BaseGame.Get.HalfBackBufferWidth, BaseGame.Get.HalfBackBufferHeight);

            pos.Y *= -1;
            pos += Position * WorldScale;

            return pos / WorldScale;
        }
        public Vector2 WorldScale
        {
            set{ worldScale = value; }
            get { return worldScale; }
        }
        public float UniformWorldScale
        {
            set { System.Diagnostics.Debug.Assert(value >= 0); worldScale = new Vector2(value, value); }
            get { return (WorldScale.X + WorldScale.Y) / 2.0f; }
        }
        Vector2 worldScale = new Vector2(1, 1);
        public Vector2 GetViewPort()
        {
            return new Vector2(BaseGame.Get.HalfBackBufferWidth / WorldScale.X, BaseGame.Get.HalfBackBufferHeight / WorldScale.Y);
        }
        public Vector2 GetWorldPosition(Vector2 pos)
        {
            pos -= Get.Position;
            pos.Y *= -WorldScale.Y;
            pos.X *= WorldScale.X;
            pos.X += BaseGame.Get.HalfBackBufferWidth;// 400;
            pos.Y += BaseGame.Get.HalfBackBufferHeight;//240;

            return pos;
        }
        public Vector2 GetWorldPosition(Vector2 pos, Vector2 camScale)
        {
            pos -= Get.Position *camScale;
            pos.Y *= -WorldScale.Y;
            pos.X *= WorldScale.X;
            pos.X += BaseGame.Get.HalfBackBufferWidth;
            pos.Y += BaseGame.Get.HalfBackBufferHeight;
                        
            return pos;
        }

        static public Camera Get
        {
            get { 
                if (currentCamera != null) 
                    return currentCamera;
                else
                return camera; 
            }
        }

        public Camera()
            : base()
        {
        }

        /// <summary>
        /// Initialize the static camera functionality.
        /// </summary>
        /// <param name="game">The game that this component will be attached to.</param>
        public static void Initialize()
        {
            if (camera != null)
            {
               // System.Windows.MessageBox.Show("Error: camera already initialized!");
            }
            camera = new Camera();
            //JabJect.GetGame().Components.Add(camera);
        }
    }
}
