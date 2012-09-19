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
    public class MediaTextDrawer : Sprite
    {
        public MediaTextDrawer(string dir, string xmldir)
            : base(dir)
        {
            this.xmldir = xmldir;
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            CreateFramesFromXML(xmldir);
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (!Camera.Get.IsVisible(this))
            {
                int k = 0;
            }
        }

        public override float Height
        {
            get
            {
                if (GetBaseDims)
                {
                    return base.Height;
                }
                float total = 0;
                Vector2 oldpos = Position;
                Vector2 oldDim = Dimension;
                Vector2 oldScale = Scale;
                for (int i = 0; i < 1; i++)
                {
                    CurrentFrame = Text[i].ToString();
                    Width = m_Frames[CurrentFrame].Width;
                    Height = m_Frames[CurrentFrame].Height;
                    Scale = oldScale;
                    total = m_Frames[CurrentFrame].Height * ScaleX;

                  //  PosX += Width * ScaleX;
                }

                Scale = oldScale;
                Dimension = oldDim;
                Position = oldpos;

                return total;
            }
            set
            {
                base.Height = value;
            }
        }

        public override float Width
        {
            get
            {
                if (GetBaseDims)
                {
                    return base.Width;
                }
                float total = 0;
                Vector2 oldpos = Position;
                Vector2 oldDim = Dimension;
                Vector2 oldScale = Scale;
                for (int i = 0; i < Text.Length; i++)
                {
                    CurrentFrame = Text[i].ToString();
                    Width = m_Frames[CurrentFrame].Width;
                    Height = m_Frames[CurrentFrame].Height;
                    Scale = oldScale;
                    total += m_Frames[CurrentFrame].Width * ScaleX;

                    PosX += m_Frames[CurrentFrame].Width * ScaleX;
                }

                Scale = oldScale;
                Dimension = oldDim;
                Position = oldpos;

                return total;
            }
            set
            {
                base.Width = value;
            }
        }

        string xmldir = "";
        bool GetBaseDims = false;
        public override void Draw()
        {
            Vector2 oldpos = Position;
            Vector2 oldDim = Dimension;
            Vector2 oldScale = Scale;
            SpriteHandle oldHandle = handle;
            
            Vector2 offset = GetOriginOffset(Width, Height, handle);

            offset.Y *= -1;
            Position -= offset;

            Handle = SpriteHandle.TOPLEFT;
            for (int i = 0; i < Text.Length; i++)
            {
                CurrentFrame = Text[i].ToString();
                ResetDimensions();

                Dimension *= oldScale;

                GetBaseDims = true;
                base.Draw();
                GetBaseDims = false;

                PosX += Dimension.X;// *ScaleX;
            }

            Handle = oldHandle;
            Scale = oldScale;
            Dimension = oldDim;
            Position = oldpos;
        }

        public string Text
        {
            get;
            set;
        }
    }

    public class TextDrawer : BaseSprite
    {
        public TextDrawer(string font)
            : base()
        {
            fontdir = font;
            Text = " ";
        }
        string fontdir;
        SpriteFont font;

        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);

            font = content.Load<SpriteFont>(fontdir);
        }

        public string Text { get; set; }

        public override JabRectangle GetRectangle()
        {
            Width = font.MeasureString(Text).X;
            Height = font.MeasureString(Text).Y;
            return base.GetRectangle();
        }

        public override float Height
        {
            get
            {
                Height = font.MeasureString(Text).Y;
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }


        public override float Width
        {
            get
            {
                Width = font.MeasureString(Text).X;
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (DrawIn == DrawSpace.WORLDSPACE)
            {
                Vector2 dim = font.MeasureString(Text);
                Vector2 pos = Position;
                pos.Y *= -1;
                dim = GetOriginOffset(dim.X, dim.Y, handle);

                SpriteBatch.DrawString(font, Text, pos, Colour, 0.0f, dim, UniformScale, SpriteEffects.None, 0);
            }
            else
            {
                Vector2 dim = font.MeasureString(Text);
                dim *= -1;
                dim /= 2.0f;
                SpriteBatch.DrawString(font, Text, RealScreenPosition + dim, Colour);
            }
        }
    }
}
