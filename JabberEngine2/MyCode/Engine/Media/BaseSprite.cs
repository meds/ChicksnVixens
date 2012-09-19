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
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

using Jabber.Util;

namespace Jabber.Media
{
    public class BaseSprite : JabJect, IComparable
    {
        public enum SpriteHandle
        {
            CENTER,
            BOTTOMCENTER,
            TOPLEFT,
            TOPCENTER,
            TOPRIGHT,
            BOTTOMLEFT,
            BOTTOMRIGHT,
            CENTERLEFT,
            CENTERRIGHT,
            CUSTOM
        };

        public virtual bool IsVisible()
        {
            return Camera.Get.IsVisible(this);
        }

        public int CompareTo(object o)
        {
            if (!(o is BaseSprite))
            {
                return 0;
            }
            BaseSprite other = o as BaseSprite;
            if (other.Layer == Layer)
            {
                return 0;
            }
            else
            {
                return other.Layer - Layer;
            }
        }

        public enum SpriteLayer
        {
            UILAYER0 = 0,
            UILAYER1,
            UILAYER2,
            UILAYER3,
            LAYER0,
            LAYER1,
            LAYER2,
            LAYER3,
            LAYER4,
            LAYER5,
            LAYER6,
            LAYER7,
            LAYER8,
            LAYER9,
            LAYER10,
            BACKGROUND_LAYER0,
            BACKGROUND_LAYER1,
            BACKGROUND_LAYER2,
            BACKGROUND_LAYER3,
            BACKGROUND_LAYER4,
            BACKGROUND_LAYER5,
            BACKGROUND_LAYER6
        };
        public BaseSprite()
        {
            Layer = SpriteLayer.LAYER3;
        }
        public SpriteLayer Layer { get; set; }

        Vector2 customOrigin = Vector2.Zero;
        public virtual Vector2 CustomOrigin
        {
            set { customOrigin = value; }
            get { return customOrigin; }
        }

        Vector2 camScale = Vector2.One;
        public Vector2 CamPosScale
        {
            set { camScale = value; }
            get { return camScale; }
        }

        protected SpriteHandle handle = SpriteHandle.CENTER;
        public virtual SpriteHandle Handle
        {
            set
            {
                handle = value;

            }
            get { return handle; }
        }

        [Flags]
        public enum SpriteEffect
        {
            NONE,
            FLIPHORIZONTAL,
            FLIPVERTICAL
        }

        SpriteEffect spriteEffect;
        public SpriteEffect Effect
        {
            set { spriteEffect = value; }
            get { return spriteEffect; }
        }

        public enum DrawSpace
        {
            WORLDSPACE,
            SCREENSPACE
        }
        DrawSpace drawSpace = DrawSpace.WORLDSPACE;
        public DrawSpace DrawIn
        {
            get { return drawSpace; }
            set { drawSpace = value; }
        }

        public virtual JabRectangle GetRectangle()
        {
            JabRectangle rec;

            if (DrawIn == DrawSpace.SCREENSPACE)
            {
                rec = new JabRectangle((float)TopLeftPosition().X, (float)TopLeftPosition().Y, (float)(Width * ScaleX),
                    (float)(Height * ScaleY));
            }
            else
            {
                Vector2 topleftpos = TopLeftPosition();
                rec = new JabRectangle(topleftpos.X, topleftpos.Y, (float)(Width * ScaleX * Camera.Get.WorldScale.X), (float)(Height * ScaleY * Camera.Get.WorldScale.Y));
            }

            Vector2 topleft = Vector2.Zero;
            topleft.X = rec.Left;
            topleft.Y = rec.Top;

            Vector2 bottomRight = Vector2.Zero;
            bottomRight.X = rec.Right;
            bottomRight.Y = rec.Bottom;

            Vector2 topright = Vector2.Zero;
            topright.X = rec.Right;
            topright.Y = rec.Top;

            Vector2 bottomleft = Vector2.Zero;
            bottomleft.X = rec.Left;
            bottomleft.Y = rec.Bottom;

            topleft = JabMath.RotateVectorAround(HandleAdjustedPosition(), topleft, Rot);
            bottomRight = JabMath.RotateVectorAround(HandleAdjustedPosition(), bottomRight, Rot);
            bottomleft = JabMath.RotateVectorAround(HandleAdjustedPosition(), bottomleft, Rot);
            topright = JabMath.RotateVectorAround(HandleAdjustedPosition(), topright, Rot);

            Vector2 newTopLeft = Vector2.Zero;
            Vector2 newBottomRight = Vector2.Zero;

            if (DrawIn == DrawSpace.WORLDSPACE)
            {
                newTopLeft = new Vector2(Math.Min(Math.Min(topleft.X, bottomRight.X), Math.Min(bottomleft.X, topright.X)),
                    Math.Min(Math.Min(topleft.Y, bottomRight.Y), Math.Min(bottomleft.Y, topright.Y)));

                newBottomRight = new Vector2(
                    Math.Max(Math.Max(topleft.X, bottomRight.X), Math.Max(bottomleft.X, topright.X)),
                    Math.Max(Math.Max(topleft.Y, bottomRight.Y), Math.Max(bottomleft.Y, topright.Y)));
            }
            else
            {
                newTopLeft = new Vector2(Math.Min(Math.Min(topleft.X, bottomRight.X), Math.Min(bottomleft.X, topright.X)),
                    Math.Min(Math.Min(topleft.Y, bottomRight.Y), Math.Min(bottomleft.Y, topright.Y)));

                newBottomRight = new Vector2(
                    Math.Max(Math.Max(topleft.X, bottomRight.X), Math.Max(bottomleft.X, topright.X)),
                    Math.Max(Math.Max(topleft.Y, bottomRight.Y), Math.Max(bottomleft.Y, topright.Y)));
            }

            float rectangleWidth = (float)Math.Abs((float)newTopLeft.X - newBottomRight.X);
            float rectangleHeight = (float)Math.Abs((float)newTopLeft.Y - newBottomRight.Y);

            Vector2 pos = bottomRight - (new Vector2(rectangleWidth, rectangleHeight));
            if (DrawIn == DrawSpace.SCREENSPACE)
            {
                rec = new JabRectangle(newTopLeft.X,
                                        newTopLeft.Y,
                                        rectangleWidth, rectangleHeight);
            }
            else
            {
                newTopLeft /= BaseGame.Get.BackBufferDimensions;
                rec = new JabRectangle(newTopLeft.X, newTopLeft.Y, rectangleWidth / BaseGame.Get.BackBufferWidth, rectangleHeight / BaseGame.Get.BackBufferHeight);
            }
            return rec;
        }

        public virtual bool Contains(float posx, float posy)
        {
            JabRectangle rect = GetRectangle();

            if (rect.Left <= posx)
            {
                if (rect.Right >= posx)
                {
                    if (rect.Top <= posy)
                    {
                        if (rect.Bottom >= posy)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the center of the sprite
        /// </summary>
        public virtual Vector2 GetAbsoluteCenter()
        {
            Vector2 newpos = TopLeftPosition();

            if (DrawIn == DrawSpace.WORLDSPACE)
            {
                newpos.X += 0.5f * Width * ScaleX;
                newpos.Y += 0.5f * Height * ScaleY;
            }
            else if (DrawIn == DrawSpace.SCREENSPACE)
            {
                newpos.X += 0.5f * Width * ScaleX;
                newpos.Y += 0.5f * Height * ScaleY;
            }

            return newpos;
        }

        public virtual Vector2 TopLeftPosition()
        {
            Vector2 newpos = Position;
            if (DrawIn == DrawSpace.WORLDSPACE)
            {
                newpos = Camera.Get.GetWorldPosition(newpos, CamPosScale);
                switch (Handle)
                {
                    case SpriteHandle.CENTER:
                        newpos.X -= Width * ScaleX * Camera.Get.WorldScale.X / 2.0f;
                        newpos.Y -= Height * ScaleY * Camera.Get.WorldScale.Y / 2.0f;
                        break;
                    case SpriteHandle.BOTTOMCENTER:
                        newpos.X -= Width * ScaleX * Camera.Get.WorldScale.X / 2.0f;
                        newpos.Y -= Height * ScaleY * Camera.Get.WorldScale.Y;
                        break;
                    case SpriteHandle.CENTERLEFT:
                        newpos.Y -= Height * ScaleY * Camera.Get.WorldScale.Y / 2.0f;
                        break;
                    case SpriteHandle.BOTTOMLEFT:
                        newpos.Y -= Height * ScaleY * Camera.Get.WorldScale.Y;
                        break;
                    case SpriteHandle.BOTTOMRIGHT:
                        newpos.Y -= Height * ScaleY * Camera.Get.WorldScale.Y;
                        break;
                    case SpriteHandle.CENTERRIGHT:
                        newpos.X -= Width * ScaleX * Camera.Get.WorldScale.X;
                        newpos.Y -= Height * ScaleY * Camera.Get.WorldScale.Y / 2.0f;
                        break;
                    case SpriteHandle.TOPLEFT:
                        break;
                    case SpriteHandle.TOPRIGHT:
                        newpos.X += Width * ScaleX * Camera.Get.WorldScale.X / 2.0f;
                        break;
                    case SpriteHandle.CUSTOM:
                        newpos.X -= customOrigin.X * Camera.Get.WorldScale.X * ScaleX;
                        newpos.Y -= customOrigin.Y * Camera.Get.WorldScale.Y * ScaleY;
                        break;
                }
            }
            else if (DrawIn == DrawSpace.SCREENSPACE)
            {
                switch (Handle)
                {
                    case SpriteHandle.CENTER:
                        newpos.X -= Width * ScaleX / 2.0f;
                        newpos.Y -= Height * ScaleY / 2.0f;
                        break;
                    case SpriteHandle.BOTTOMCENTER:
                        newpos.X -= Width * ScaleX / 2.0f / BaseGame.Get.BackBufferWidth;
                        newpos.Y -= Height * ScaleY / BaseGame.Get.BackBufferHeight;
                        break;
                    case SpriteHandle.CENTERLEFT:
                        newpos.Y -= Height / 2.0f / BaseGame.Get.BackBufferHeight;
                        break;
                    case SpriteHandle.CUSTOM:
                        newpos.X -= customOrigin.X / BaseGame.Get.BackBufferWidth;
                        newpos.Y -= customOrigin.Y / BaseGame.Get.BackBufferHeight;
                        break;
                }
            }

            return newpos;
        }

        protected virtual Vector2 HandleAdjustedPosition()
        {
            Vector2 newpos = TopLeftPosition();
            if (DrawIn == DrawSpace.WORLDSPACE)
            {
                switch (Handle)
                {
                    case SpriteHandle.CENTER:
                        newpos.X += (Width * ScaleX * Camera.Get.WorldScale.X) / 2.0f;
                        newpos.Y += (Height * ScaleY * Camera.Get.WorldScale.Y) / 2.0f;
                        break;
                    case SpriteHandle.BOTTOMCENTER:
                        newpos.X += (Width * ScaleX * Camera.Get.WorldScale.X) / 2.0f;
                        newpos.Y += (Height * ScaleY * Camera.Get.WorldScale.Y);
                        break;
                    case SpriteHandle.CENTERLEFT:
                        newpos.Y += Height * ScaleY * Camera.Get.WorldScale.Y / 2.0f;
                        break;
                    case SpriteHandle.CUSTOM:
                        newpos.X += CustomOrigin.X * ScaleX * Camera.Get.WorldScale.X;
                        newpos.Y += CustomOrigin.Y * ScaleY * Camera.Get.WorldScale.Y;
                        break;
                }
            }
            else if (DrawIn == DrawSpace.SCREENSPACE)
            {
                switch (Handle)
                {
                    case SpriteHandle.CENTER:
                        newpos.X -= Width * ScaleX * 2.0f;
                        newpos.Y -= Height * ScaleY * 2.0f;
                        break;
                    case SpriteHandle.BOTTOMCENTER:
                        newpos.X += Width * ScaleX / 2.0f;
                        newpos.Y += Height * ScaleY / 2.0f;
                        break;
                    case SpriteHandle.CENTERLEFT:
                        newpos.Y += Height * ScaleY / 2.0f;
                        break;
                    case SpriteHandle.CUSTOM:
                        newpos.X += CustomOrigin.X;
                        newpos.Y += CustomOrigin.Y;
                        break;
                }
            }

            return newpos;
        }

        // The texture dimensions set after being de-normalized
        public virtual Vector2 RealTextureDim
        {
            get
            {
                if (DrawIn == DrawSpace.SCREENSPACE)
                {
                    return new Vector2(Width * BaseGame.Get.BackBufferWidth, Height * BaseGame.Get.BackBufferHeight) * Scale;
                }
                else
                {
                    return new Vector2(Width, Height) * Scale;
                }
            }
        }

        protected Vector2 RealScreenPosition
        {
            get
            {
                if (DrawIn == DrawSpace.SCREENSPACE)
                {
                    return new Vector2(BaseGame.Get.BackBufferWidth, BaseGame.Get.BackBufferHeight) * Position;
                }
                else
                {
                    return position;
                }
            }
        }

        // The scale needed to be set so that the texture will draw in the intended width/height on screen
        public virtual Vector2 GetDrawScaleToFitDimensions()
        {
            return RealTextureDim / ActualTextureDim;
        }

        // The texture dimensions in normalized form
        public virtual Vector2 TextureDimensionsWithScale
        {
            get
            {
                return new Vector2(Width * Scale.X, Height * Scale.Y);
            }
        }

        // todo: fix if broken
        public Vector2 GetPositionAtHandle(SpriteHandle handle)
        {
            return Position + GetOriginOffset(Width, Height, handle);
        }

        public static Vector2 GetOriginOffset(float width, float height, SpriteHandle handle)
        {
            Vector2 Origin = Vector2.Zero;
            switch (handle)
            {
                case SpriteHandle.TOPLEFT:
                    Origin = Vector2.Zero;
                    break;
                case SpriteHandle.CENTER:
                    Origin.X += width / 2.0f;
                    Origin.Y += height / 2.0f;
                    break;
                case SpriteHandle.CENTERLEFT:
                    Origin.Y += height / 2.0f;
                    break;
                case SpriteHandle.CENTERRIGHT:
                    Origin.X += width;
                    Origin.Y += height / 2.0f;
                    break;
                case SpriteHandle.BOTTOMCENTER:
                    Origin.X += width / 2.0f;
                    Origin.Y += height;
                    break;
                case SpriteHandle.TOPCENTER:
                    Origin.X += width / 2.0f;
                    break;
            }

            return Origin;
        }

        // the textures dimensions as they are without any scaling
        public virtual Vector2 ActualTextureDim
        {
            get { return Vector2.Zero; }
        }
    }
}