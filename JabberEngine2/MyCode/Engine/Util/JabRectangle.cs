using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.ComponentModel;

using Microsoft.Xna.Framework;

namespace Jabber.Util
{
    public struct JabRectangle : IEquatable<JabRectangle>
    {
        #region Private Fields

        private static JabRectangle emptyRectangle = new JabRectangle();

        #endregion Private Fields


        #region Public Fields

        public float X;
        public float Y;
        public float Width;
        public float Height;

        #endregion Public Fields


        #region Public Properties

        public static JabRectangle Empty
        {
            get { return emptyRectangle; }
        }

        public float Left
        {
            get { return this.X; }
        }

        public float Right
        {
            get { return (this.X + this.Width); }
        }

        public float Top
        {
            get { return this.Y; }
        }

        public float Bottom
        {
            get { return (this.Y + this.Height); }
        }
        /*
        public Vector2 Center
        {
            get 
            { 
                return new Vector2((this.Right - (this.Width / 2.0)), (this.Bottom - (this.Height / 2.0f))); 
            }
        }
         * */
        #endregion Public Properties


        #region Constructors

        public JabRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        #endregion Constructors


        #region Public Methods

        public static bool operator ==(JabRectangle a, JabRectangle b)
        {
            return ((a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height));
        }

        public static bool operator !=(JabRectangle a, JabRectangle b)
        {
            return !(a == b);
        }

        public void Offset(Vector2 offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Inflate(float horizontalValue, float verticalValue)
        {
            X -= horizontalValue;
            Y -= verticalValue;
            Width += horizontalValue * 2;
            Height += verticalValue * 2;
        }

        public bool Equals(JabRectangle other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return (obj is JabRectangle) ? this == ((JabRectangle)obj) : false;
        }

        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1} Width:{2} Height:{3}}}", X, Y, Width, Height);
        }
        /*
        public override float GetHashCode()
        {
            return (this.X ^ this.Y ^ this.Width ^ this.Height);
        }
        */
        public bool Intersects(JabRectangle r2)
        {
            return !(r2.Left > Right
                     || r2.Right < Left
                     || r2.Top > Bottom
                     || r2.Bottom < Top
                    );

        }


        public void floatersects(ref JabRectangle value, out bool result)
        {
            result = !(value.Left > Right
                     || value.Right < Left
                     || value.Top > Bottom
                     || value.Bottom < Top
                    );

        }

        public bool Contains(float x, float y)
        {
            return (this.Left <= x && this.Right >= x &&
                    this.Top <= y && this.Bottom >= y);
        }

        public bool Contains(Vector2 value)
        {
            return (this.Left <= value.X && this.Right >= value.X &&
                    this.Top <= value.Y && this.Bottom >= value.Y);
        }

        public void Contains(ref Vector2 value, out bool result)
        {
            result = (this.Left <= value.X && this.Right >= value.X &&
                      this.Top <= value.Y && this.Bottom >= value.Y);
        }

        public bool Contains(JabRectangle value)
        {
            return (this.Left <= value.Left && this.Right >= value.Right &&
                    this.Top <= value.Top && this.Bottom >= value.Bottom);
        }

        public void Contains(ref JabRectangle value, out bool result)
        {
            result = (this.Left <= value.Left && this.Right >= value.Right &&
                      this.Top <= value.Top && this.Bottom >= value.Bottom);
        }


        public Rectangle ToRectangle
        {
            get
            {
                Rectangle r = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
                return r;
            }
        }

        #endregion Public Methods
    }
}
