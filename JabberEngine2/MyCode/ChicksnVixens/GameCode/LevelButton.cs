using Jabber.Media;
using Jabber.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

using Jabber.Util;
using Jabber;

using Jabber.Physics;

using Jabber.GameScreenManager;
using Jabber.Util.UI;

namespace ChicksnVixens
{
    public class LevelButton : Sprite
    {
        public LevelButton(int x, int y, int overallrow, string location)
            : base("ui/ui")
        {
            RaiseFlag(Flags.ACCEPTINPUT);
            float xpos = 0;
            float ypos = 0;
            this.location = location;
            float xoffset = BaseGame.Get.BackBufferWidth * overallrow / 1.5f;
            switch (x)
            {
                case 0:
                    xpos = 100 * ScaleFactor + xoffset;
                    break;
                case 1:
                    xpos = 200 * ScaleFactor + xoffset;
                    break;
                case 2:
                    xpos = 300 * ScaleFactor + xoffset;
                    break;
                case 3:
                    xpos = 400 * ScaleFactor + xoffset;
                    break;
                case 4:
                    xpos = 500 * ScaleFactor + xoffset;
                    break;
                default:
                    int k = 0;
                    break;
            }
            switch (y)
            {
                case 0:
                    ypos = 200 * ScaleFactor;
                    break;
                case 1:
                    ypos = 100 * ScaleFactor;
                    break;
                case 2:
                    ypos = 0 * ScaleFactor;
                    break;
                case 3:
                    ypos = -100 * ScaleFactor;
                    break;
                default:
                    int k = 0;
                    break;
            }

            xpos -= BaseGame.Get.HalfBackBufferWidth * 0.75f;

            float maxpossiblewidth = 500 * ScaleFactor - 100 * ScaleFactor;
            float firstx = 100 * ScaleFactor;

            Vector2 targetPos = Vector2.Zero;
            targetPos.X = xpos * 1.5f;
            targetPos.Y = ypos - 30 * ScaleFactor;

            OriginPos = new Vector2(xpos, ypos) * 7.5f * ScaleFactor;
            if (OriginPos == Vector2.Zero)
            {
                OriginPos = new Vector2(0, -350) * ScaleFactor;
            }
            OriginPos = OriginPos + new Vector2(0, -30);
            Position = OriginPos;

            Position = targetPos;
            InitialPosition = targetPos;
            InitialOriginPos = OriginPos;
            row = y;
            column = x;
            higherColumn = overallrow;
        }
        string location = "";
        int row = 0;
        int column = 0;
        int higherColumn;
        Vector2 InitialPosition;
        Vector2 InitialOriginPos;
        public int Level
        {
            get
            {
                return (column + 1) + ((row + 1) * 5) - 5 + higherColumn * 20;
            }
        }
        public float xOffset = 0;
        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("ui/ui_frames");

            CurrentFrame = "levelbutton";
            ResetDimensions();
            UniformScale = 0.8f * ScaleFactor / Camera.Get.UniformWorldScale;

            Colour = new Color(1, 1, 1, 0);

            text = new TextDrawer("ui/LevelFont");
            text.Initialize(Content);
            text.Text = Level.ToString();
            if (!ChicksnVixensGame.Get.LevelUnlocked(location, Level))
            {
                text.Text = "X";

                padLock.Initialize(Content);
                padLock.CreateFramesFromXML("ui/ui_frames");
                padLock.CurrentFrame = "lock";
                padLock.ResetDimensions();
                padLock.UniformScale = ScaleFactor;
            }

            stars = new Sprite("ui/ui");
            stars.Initialize(Content);
            stars.CreateFramesFromXML("ui/ui_frames");
            stars.CurrentFrame = "star";

            stars.ResetDimensions();
            stars.UniformScale = ScaleFactor * 0.3f;
        }
        Vector2 OriginPos = Vector2.Zero;
        float lastAlpha = 0;
        bool firstUpdate = true;
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (!(CheckFlag(Flags.FADE_IN) || CheckFlag(Flags.FADE_OUT)) && CheckStateFlag(StateFlag.FADE_IN_COMPLETE))
            {
                if (IsPressing)
                {
                    UniformScale = JabMath.LinearMoveTowards(UniformScale, 0.8f * ScaleFactor / Camera.Get.UniformWorldScale * 1.15f, gttf(dt) * 0.8f);
                }
                else
                {
                    float target = 0.8f * ScaleFactor / Camera.Get.UniformWorldScale;
                    UniformScale = JabMath.LinearMoveTowards(UniformScale, target, gttf(dt));
                }
            }
            else
            {
                UniformScale = 0.8f * ScaleFactor / Camera.Get.UniformWorldScale;
            }

            if (!(CheckFlag(Flags.FADE_IN) || CheckFlag(Flags.FADE_OUT)) && CheckStateFlag(StateFlag.FADE_IN_COMPLETE))
                text.UniformScale = ScaleFactor / Camera.Get.UniformWorldScale * UniformScale * 1.2f;
            else
                text.UniformScale = ScaleFactor / Camera.Get.UniformWorldScale;

            float alpha = JabMath.LinearInterpolate(1.0f, 0.0f, (Camera.Get.UniformWorldScale - 1) / 2.0f);

            if (lastAlpha != alpha)
            {
                if (alpha == 0)
                {
                    OnFadeOutComplete();
                }
                else if (alpha == 1)
                {
                    OnFadeInComplete();
                }
            }

            lastAlpha = alpha;
            Colour = new Color(alpha, alpha, alpha, alpha);
            text.Colour = Color.Red * alpha;
        }
        Sprite padLock = new Sprite("ui/ui");

        public override void OnPress(Vector2 pos)
        {
            base.OnPress(pos);
            IsPressing = false;
            if (!(CheckFlag(Flags.FADE_IN) || CheckFlag(Flags.FADE_OUT)) && CheckStateFlag(StateFlag.FADE_IN_COMPLETE))
            {
                if (Contains(pos.X, pos.Y))
                {
                    IsPressing = true;
                }
            }
        }
        float curDragLength = 0;
        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            base.OnDrag(lastPos, thispos);
            if (IsPressing)
            {
                curDragLength += Math.Abs((thispos.Length() - lastPos.Length()));
                if (curDragLength > 0.005f)
                {
                    IsPressing = false;
                    curDragLength = 0;
                }
            }
        }
        public override void OnRelease(Vector2 pos)
        {
            base.OnRelease(pos);

            if (IsPressing && Contains(pos.X, pos.Y))
            {
                EventManager.Get.SendEvent(new Jabber.Util.UI.MenuEvent(this, MenuEvent.EventType.TAP));
            }
        }

        bool IsPressing = false;
        /*public override void OnTap(Vector2 pos)
        {
            if (text.Text == "X")
            {
                return;
            }
            base.OnTap(pos);

            if (!(CheckFlag(Flags.FADE_IN) || CheckFlag(Flags.FADE_OUT)) && CheckStateFlag(StateFlag.FADE_IN_COMPLETE))
            {
                if (Contains(pos.X, pos.Y))
                {
                    EventManager.Get.SendEvent(new Jabber.Util.UI.MenuEvent(this, MenuEvent.EventType.TAP));
                }
            }
        }*/

        Sprite stars;

        public override void Draw()
        {
            PosX += xOffset;
            base.Draw();
            if (text.Text == "X")
            {
                padLock.Position = Position;
                padLock.Colour = Colour;
                padLock.UniformScale = UniformScale;
                padLock.Draw();
               // text.Position = Position;
               // text.Draw();
                return;
            }
            int numstars = ChicksnVixensGame.Get.GetLevelState(location, int.Parse(text.Text, System.Globalization.CultureInfo.InvariantCulture)).NumStars;

            stars.ResetDimensions();
            stars.UniformScale = ScaleFactor * 0.35f / Camera.Get.UniformWorldScale * UniformScale;
            Vector2 PosOffset = new Vector2(0, 5) * ScaleFactor / Camera.Get.UniformWorldScale;
            stars.Colour = Color.Gold * lastAlpha;
            switch (numstars)
            {
                case 0:
                    {
                        //stars.Position = Position + PosOffset;
                        //stars.Draw();
                    }
                    break;
                case 1:
                    {
                        stars.Position = Position + PosOffset;
                        stars.Draw();
                    }
                    break;
                case 2:
                    {
                        stars.Position = Position + PosOffset;
                        stars.PosX -= stars.Width / 6.0f * ScaleFactor / Camera.Get.UniformWorldScale;
                        stars.Draw();

                        stars.Position = Position + PosOffset;
                        stars.PosX += stars.Width / 6.0f * ScaleFactor / Camera.Get.UniformWorldScale;
                        stars.Draw();
                    }
                    break;
                case 3:
                    {
                        stars.Position = Position + PosOffset;
                        stars.PosX -= stars.Width / 5.0f * ScaleFactor / Camera.Get.UniformWorldScale;
                        stars.Draw();

                        stars.Position = Position + PosOffset;
                        stars.PosX += stars.Width / 5.0f * ScaleFactor / Camera.Get.UniformWorldScale;
                        stars.Draw();


                        stars.Position = Position + PosOffset + new Vector2(0, 5) * ScaleFactor / Camera.Get.UniformWorldScale;
                        stars.Draw();
                    }
                    break;
            }



            text.Position = Position;
            text.Draw();

            PosX -= xOffset;
        }

        TextDrawer text;
    }
}
