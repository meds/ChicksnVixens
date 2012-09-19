using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Phone.Tasks;
using System.Text;


using Jabber.Physics;
using Jabber.GameScreenManager;
using Jabber.Util;

#region silly namespaces to make engine compile

namespace Microsoft.Xna.Framework.GamerServices
{
}

namespace Microsoft.Xna.Framework.Input.Touch
{
}

namespace Microsoft.Phone.Shell
{
}

namespace Microsoft.Phone.Tasks
{
}

#endregion


namespace Jabber
{
    public class FadeOutEvent : Event
    {
        public FadeOutEvent(JabJect obj)
            : base()
        {
            Sender = obj;
        }

        public JabJect Sender;
    }

    [Flags]
    public enum Flags
    {
        NONE = 0x0,
        PASSUPDATE = 0x1,
        PASSRENDER = 0x2,
        DELETE = 0x4,
        ACCEPTINPUT = 0x8,
        FADE_IN = 0x10,
        FADE_OUT = 0x20,
       // FADE_OUT_COMPLETE = 0x40
    }

    public enum StateFlag
    {
        NONE = 0x0,
        FADE_OUT_COMPLETE = 0x1,
        FADE_IN_COMPLETE = 0x2
    }
    
    public class JabJect
    {
        public StateFlag StateFlag
        {
            get;set;
        }

        public virtual bool CheckStateFlag(StateFlag flag)
        {
            return this.StateFlag == flag;
        }

        public virtual void SetStateFlag(StateFlag flag)
        {
            this.StateFlag = flag;
        }

        Flags flags = Flags.NONE;
        public virtual void RaiseFlag(Flags f)
        {
            bool before = true;
            if (!CheckFlag(Flags.PASSRENDER))
            {
                before = false;
                int k = 0;
            }
            flags |= f;

            if ((CheckFlag(Flags.PASSRENDER) && !before))
            {
                int k = 0;
            }
        }
        public virtual void BurnFlags()
        {
            flags = 0;
        }
        public virtual void LowerFlag(Flags f)
        {
            flags &= (~f);
        }
        public bool CheckFlag(Flags f)
        {
            return (flags & f) == f;
        }
        public Flags SeeFlag()
        {
            return flags;
        }

        public JabJect()
        {
            StateFlag = Jabber.StateFlag.NONE;
        }
        #region positions

        protected Vector2 position = new Vector2(0, 0);
        protected float fPosZ = 0;
        protected float fRot = 0;
        virtual public float Rot
        {
            get { return fRot; }
            set { fRot = value; }
        }
        virtual public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        /*
        virtual public float PosY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }*/
        virtual public float PosY
        {
            get { return Position.Y; }
            set { Position = new Vector2(PosX, value); }
        }
        virtual public float PosX
        {
            get { return Position.X; }
            set { Position = new Vector2(value, PosY); }
        }
        virtual public float PosZ
        {
            get { return fPosZ; }
            set
            {
                if (value < 0 || value > 1)
                {
                }
                fPosZ = value;
            }
        }
        public virtual void HandleInput(InputManager input) { }
        virtual public float UniformScale
        {
            get { return (scale.X + scale.Y) / 2.0f; }
            set { scale.X = scale.Y = value; }
        }
        virtual public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        virtual public float ScaleX
        {
            get { return scale.X; }
            set { scale.X = value; }
        }
        virtual public float ScaleY
        {
            get { return scale.Y; }
            set { scale.Y = value; }
        }
        Vector2 scale = new Vector2(1, 1);


        #endregion

        #region dimensions
        private Vector2 dimensions = Vector2.Zero;
        public virtual Vector2 Dimension
        {
            set { dimensions = value; }
            get { return dimensions; }
        }

        public virtual float Width
        {
            set { dimensions.X = value; }
            get { return dimensions.X; }
        }
        public virtual float Height
        {
            set { dimensions.Y = value; }
            get { return dimensions.Y; }
        }
        #endregion

        #region colour
        protected Color colour = Color.White;
        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }
        #endregion

        public virtual void Draw()
        {
        }
        public virtual void Update(GameTime dt)
        {
        }
        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return BaseGame.Get.SpriteBatch; }
        }

        static public SpriteBatch GetSpriteBatch()
        {
            return BaseGame.Get.SpriteBatch;
        }
        static public BaseGame GetGame()
        {
            return (BaseGame)BaseGame.Get;
        }
        public virtual void Initialize(ContentManager content)
        {
        }
        public virtual void UnloadContent()
        {
            EventManager.Get.UnregisterListner(this);
        }
        static public float GameTimeToFloat(GameTime dt)
        {
            return dt.ElapsedGameTime.Milliseconds / 1000.0f;
        }
        static public float gttf(GameTime dt)
        {
            return GameTimeToFloat(dt);
        }
        static public float ScaleFactor
        {
            get { return BaseGame.Get.ScalerFactor; }
        }
        static public float RandomFloat
        {
            get { return (float)BaseGame.Random.NextDouble(); }
        }

        static public float RandomFloatInRangeGravitate(float min, float max, float gravitate)
        {
            float retval = (float)BaseGame.Random.NextDouble() * (max - min) + min;
            retval += JabMath.LinearInterpolate(0, gravitate - retval, RandomFloat);
            return retval;
        }

        public virtual void OnFadeOutComplete()
        {
            LowerFlag(Jabber.Flags.FADE_OUT);
            StateFlag = Jabber.StateFlag.FADE_OUT_COMPLETE;
            EventManager.Get.SendEvent(new FadeOutEvent(this));
        }

        public virtual void OnFadeInComplete()
        {
            LowerFlag(Jabber.Flags.FADE_IN);
            StateFlag = Jabber.StateFlag.FADE_IN_COMPLETE;
        }

        public virtual void OnDrag(Vector2 lastpos, Vector2 thispos, int fingerID)
        {
            if (fingerID == InputManager.Get.Finger1ID)
                OnDrag(lastpos, thispos);
        }

        public virtual void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
        }

#if WINDOWS
        public virtual void OnMouseScroll(int delta)
        {
        }
#endif

#if WINDOWS_PHONE || ANDROID
        public virtual void OnBackPress()
        {
        }
#endif

        public virtual void OnTap(Vector2 pos, int fingerID)
        {
            if (fingerID == InputManager.Get.Finger1ID)
                OnTap(pos);
        }
        public virtual void OnRelease(Vector2 pos, int fingerID)
        {
            if (fingerID == InputManager.Get.Finger1ID)
                OnRelease(pos);
        }
        public virtual void OnMove(Vector2 pos, int fingerID)
        {
            if (fingerID == InputManager.Get.Finger1ID)
                OnMove(pos);
        }
        public virtual void OnPress(Vector2 pos, int fingerID)
        {
            if(fingerID == InputManager.Get.Finger1ID)
                OnPress(pos);
        }

        public virtual void OnPress(Vector2 pos)
        {
        }

        public virtual void OnTap(Vector2 pos)
        {
        }

        public virtual void OnMove(Vector2 pos)
        {
        }

        public virtual void OnRelease(Vector2 pos)
        {

        }

        static public float RandomFloatInRange(float min, float max)
        {
            return (float)BaseGame.Random.NextDouble() * (max - min) + min;
        }
        public virtual void ProcessEvent(Event ev)
        {
        }

        static public float RandomFloatGravitate(float val)
        {
            float retval = RandomFloat;
            retval += JabMath.LinearInterpolate(0, val - retval, RandomFloat);
            return retval;
        }

    };
}
