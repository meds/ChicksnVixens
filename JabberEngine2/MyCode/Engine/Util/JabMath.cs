using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace Jabber.Util
{
    public class JabMath
    {
        static public double LinearInterpolate(
        double y1, double y2,
        double mu)
        {
            return (double)((y1 * (1 - mu) + y2 * mu));
        }


        static public float LinearInterpolate(
        float y1, float y2,
        float mu)
        {
            return (float)((y1 * (1 - mu) + y2 * mu));
        }

        static public Vector2 LinearInterpolate(Vector2 y1, Vector2 y2, float mu)
        {
            return ((y1 * (1 - mu) + y2 * mu));
        }

        static public Vector2 RotateVector(Vector2 vector, float rotation)
        {
            Matrix mat = new Matrix();
            mat.Translation = new Vector3(vector.X, vector.Y, 0);

            Matrix rot = new Matrix(); rot = Matrix.CreateRotationZ(rotation);

            mat *= rot;

            return new Vector2(mat.Translation.X, mat.Translation.Y);
        }
        //static public Vector2 RotateVectorAround(Vector2 anchor, Vector2 vector, float rotation)
        static public Vector2 RotateVectorAround(Vector2 Pivot, Vector2 P, float a)
        {
            P -= Pivot;
            P = Vector2.Transform(P, Matrix.CreateRotationZ(a));
            P += Pivot;
            return P;
        }

        static public float LinearMoveTowards(float origin, float target, float td)
        {
            if (origin > target)
            {
                td *= -1;
            }
            float before = origin;
            origin += td;
            if (origin > target && td > 0)
            {
                return target;
            }
            else if (origin < target && td < 0)
            {
                return target;
            }
            return origin;
        }

        static public Vector2 LinearMoveTowards(Vector2 origin, Vector2 target, float td)
        {
            if (origin == target)
            {
                return target;
            }
            Vector2 before = origin;
            Vector2 dir = target - origin;
            dir.Normalize();
            dir *= td;
            origin += dir;
            if ((target - origin).Length() < td)
            {
                origin = target;
            }

            if ((before - target).Length() < (origin - target).Length())
            {
                origin = target;
            }

            /*
            if (target.X > origin.X && td < 0)
            {
                origin = target;
            }
            if (target.X < origin.X && td > 0)
            {
                origin = target;
            }
            if (target.Y > origin.Y && td < 0)
            {
                origin = target;
            }
            if (target.Y < origin.Y && td > 0)
            {
                origin = target;
            }
            */
            return origin;
        }

        static public Vector2 MoveTowards(Vector2 origin, Vector2 target, float scale)
        {
            return MoveTowards(origin, target, scale, 0);
        }

        static public Vector2 MoveTowards(Vector2 origin, Vector2 target, float scale, float alias)
        {
            Vector2 ret = Vector2.Zero;
            ret.X = MoveTowards(origin.X, target.X, scale, alias);
            ret.Y = MoveTowards(origin.Y, target.Y, scale, alias);

            return ret;
        }
        
        static public float MoveTowards(float origin, float target, float scale, float alias)
        {
            float newTarget = target;
            if (target > origin)
                newTarget = target + alias;
            else if(target < origin)
                newTarget = target - alias;
            
            float ret = MoveTowards(origin, newTarget, scale);

            if (ret > target && origin < target)
            {
                ret = target;
            }
            else if (ret < target && origin > target)
            {
                ret = target;
            }
            /*
            if ((float)Math.Abs((target - ret)) < alias)
            {
                return target;
            }*/
            return ret;
        }

        static public float RoundToNearest(float val, int nearest)
        {
            return (float)( Math.Round((double)(val / nearest)) * nearest );
        }

        static public float MoveTowards(float origin, float target, float scale)
        {
            float ret = origin;
            ret += (target - origin) *scale;
            if (ret > target && origin < target)
            {
                ret = target;
            }
            else if (ret < target && origin > target)
            {
                ret = target;
            }
            
            return ret;
        }

        static public float PI
        {
            get { return (float)Math.PI; }
        }

        static public float Sin(float theta)
        {
            return (float)Math.Sin((float)theta);
        }


        static public float ATan2(float y, float x)
        {
            return (float)Math.Atan2((float)y, (float)x);
        }
        static public float Cos(float theta)
        {
            return (float)Math.Cos((float)theta);
        }
    }
}
