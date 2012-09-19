using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using System.Text;

using Jabber.Util;
using Jabber.Media;
using Jabber.Scene;
using ChicksnVixens.Screens;

namespace ChicksnVixens
{
    public partial class GameCamera : Camera
    {
        public GameCamera(GameplayScreen scene)
            : base()
        {
            this.screen = scene;
            RaiseFlag(Jabber.Flags.ACCEPTINPUT);

            UniformWorldScale = 0.4f;
            EventManager.Get.RegisterListner(this);
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is CannonFireEvent)
            {
                ScaleOnFire = UniformWorldScale;
            }
        }

        float ScaleOnFire = 1.0f;
        public GameplayScreen screen = null;

        bool draggingThisFrame = false;


        public void UpMoveCamPos()
        {
            float posx = PosX;

            float targetX = targetAreaPos.X;
            float originX = originPos.X;
            if (posx < originX)
            {
                posx = originX;
            }
            else if (posx > targetX)
            {
                posx = targetX;
            }

            float distBetween = Math.Abs(targetX - originX);

            targetX -= originX;
            posx -= originX;

            posx /= targetX;


            PosY = targetPos.Y = JabMath.LinearInterpolate(originPos.Y, targetAreaPos.Y, posx);

            float lowestPoint = -512.0f;
            if (ScreenToWorld(Vector2.One).Y < lowestPoint)
            {
                PosY = targetPos.Y = PosY = PosY + (lowestPoint - ScreenToWorld(Vector2.One).Y);
            }
        }

        bool followingChickenLastFrame = false;
        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);

            List<BaseSprite> nodes = screen.scene.Nodes;
            furthestRight = float.MinValue;
            furthestLeft = float.MaxValue;
            mostLowest = float.MaxValue;
            mostHigh = float.MinValue;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is PhysicSprite && nodes[i].PosY > 0)
                {
                    if (nodes[i].PosY < mostLowest)
                    {
                        mostLowest = nodes[i].PosY;
                    }
                    if (nodes[i].PosY > mostHigh)
                    {
                        mostHigh = nodes[i].PosY;
                    }
                    if (nodes[i].PosX < furthestLeft)
                    {
                        furthestLeft = nodes[i].PosX;
                    }                                                             
                    if (nodes[i].PosX > furthestRight)
                    {
                        furthestRight = nodes[i].PosX;
                    }
                }
            }

            furthestRight += 100;
            furthestLeft -= 100;

            mostHigh += 100;
            mostLowest -= 100;

            scaleAtTarget = Math.Min(ScaleToCoverHorizontalArea(furthestLeft, furthestRight), ScaleToCoverVerticalArea(mostHigh, mostLowest));
            scaleAtOrigin = scaleAtTarget;// Math.Max(scaleAtTarget, 0.25f);// Min(ScaleToCoverHorizontalArea(furthestLeft, furthestRight), ScaleToCoverVerticalArea(mostHigh, mostLowest));
            targetAreaPos.X = (furthestRight + furthestLeft) / 2.0f;
            targetAreaPos.Y = (mostHigh + mostLowest) / 2.0f;


            float oldscale = Camera.Get.UniformWorldScale;
            Camera.Get.UniformWorldScale = scaleAtOrigin;

            leftMostScreenPos = Camera.Get.LeftMostWherePosXIs(screen.scene.startPos.X);
            Camera.Get.UniformWorldScale = scaleAtTarget;
            rightMostScreenPos = Camera.Get.RightMostWherePosXIs(targetAreaPos.X);

            maxCamScale = ScaleToCoverHorizontalArea(leftMostScreenPos, rightMostScreenPos);

            originPos = screen.scene.startPos + new Vector2(200, 0) * ScaleFactor;

            Camera.Get.UniformWorldScale = oldscale;
        }

        float leftMostScreenPos = 0;
        float rightMostScreenPos = 0;


        void DoZoom(float delta)
        {
            if (oldScale + delta <= 0)
            {
                oldScale = 0.01f;
            }
            else
            {
                oldScale += delta;
            }

            if (oldScale < maxCamScale)
            {
                oldScale = maxCamScale;
            }
            else if (oldScale > scaleAtTarget)
            {
                oldScale = scaleAtTarget;
            }
        }

#if WINDOWS
        public override void OnMouseScroll(int delta)
        {
            float val = delta / 120.0f * 0.05f;
            DoZoom(val);
            
            draggingThisFrame = true;
            base.OnMouseScroll(delta);
        }
#endif

#if WINDOWS
        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            if (screen.CameraFollowingChicken || screen.CannonBeingDragged)
            {
                return;
            }
            if (!screen.IsTopScreen)
            {
                return;
            }
            base.OnDrag(lastPos, thispos);

            UpMoveCamPos();

            lastPos.Y = thispos.Y = 0;

            draggingThisFrame = true;
            lastPos = ScreenToWorld(lastPos);
            thispos = ScreenToWorld(thispos);

            TargetPos = TargetPos + (lastPos - thispos) * 3.0f;
        }
#endif
        public override void OnRelease(Vector2 pos)
        {
            if (!screen.IsTopScreen)
            {
                return;
            }
            base.OnRelease(pos);

            draggingThisFrame = false;
        }
		float oldScale = 1.0f;

        float timer = 0;
        public override void Update(GameTime dt)
        {
            if (!screen.IsTopScreen)
            {
                return;
            }
            if (!screen.CheckStateFlag(Jabber.StateFlag.FADE_IN_COMPLETE))
            {
                return;
            }
            /*
            timer += gttf(dt);

            Position = Vector2.Zero;
            PosX = (float)Math.Sin((float)timer) * 100;
            UniformWorldScale = 0.25f;
            return;*/
           // PosX += gttf(dt) * 105;
           // return;
			
			UniformWorldScale = oldScale;
            base.Update(dt);
            UpMoveCamPos();

            if (screen.CameraFollowingChicken)
            {
                FollowChicken(dt);
            }
            else
            {
                FreeCamDrag(dt);
            }
            UpMoveCamPos();

			oldScale = UniformWorldScale;
			//UniformWorldScale *= 2.0f;
        }

        public bool FollowingChicken = false;
        public Vector2 lastPosBeforeFire = Vector2.Zero;
        public float lastScaleBeforeFire = 0.5f;
        public float scaleAtOrigin = 0.3f;
        public float scaleAtTarget = 0.1f;
        public float maxCamScale = 1.0f;
        public Vector2 targetAreaPos;
        public Vector2 originPos = Vector2.Zero;
        public Vector2 TargetPos
        {
            get { return targetPos; }
            set { targetPos = value; }
        }
        Vector2 targetPos;


        float furthestRight;
        float furthestLeft;
        float mostLowest;
        float mostHigh;
    }
}
