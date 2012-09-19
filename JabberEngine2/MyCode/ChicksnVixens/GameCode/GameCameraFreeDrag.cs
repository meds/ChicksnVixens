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
using Jabber;
namespace ChicksnVixens
{
    public partial class GameCamera : Camera
    {
        // checks to see if overflow has gone too far and caps it if it has done so
        void CapOverflow()
        {
            if (UniformWorldScale == maxCamScale)
            {
                targetPos.X = (targetAreaPos.X + originPos.X) / 2.0f;
            }
            else
            {
                float absoluteFarRightAllowed = RightMostWherePosXIs(targetAreaPos.X + 200);
                float absoluteFarLeftAllowed = LeftMostWherePosXIs(originPos.X - 200);
                if (ScreenToWorld(new Vector2(1, 0)).X > absoluteFarRightAllowed)
                {
                    float dif = Camera.Get.ScreenToWorld(new Vector2(1, 0)).X - absoluteFarRightAllowed;
                    PosX -= dif * UniformWorldScale;
                    targetPos.X = PosX;
                }
                else if (ScreenToWorld(Vector2.Zero).X < absoluteFarLeftAllowed)
                {
                    float dif = Camera.Get.ScreenToWorld(new Vector2(0, 0)).X - absoluteFarLeftAllowed;
                    PosX -= dif * UniformWorldScale;
                    targetPos.X = PosX;
                }
            }
        }

        void SetScale()
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

            if (UniformWorldScale > JabMath.LinearInterpolate(scaleAtTarget, scaleAtTarget, posx))
                UniformWorldScale = JabMath.LinearInterpolate(scaleAtTarget, scaleAtTarget, posx);
            else if (UniformWorldScale < maxCamScale)
                UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, maxCamScale, 0.05f, 0.01f);// JabMath.LinearInterpolate(UniformWorldScale, maxCamScale, posx); //maxCamScale;

            targetPos.Y = JabMath.LinearInterpolate(originPos.Y, targetPos.Y, posx);
        }

#if WINDOWS_PHONE || IPHONE
        Vector2 lastFinger1Pos = Vector2.Zero;
        Vector2 lastFinger2Pos = Vector2.Zero;
        bool Finger1ValidLastFrame = false;
        bool Finger2ValidLastFrame = false;

        bool validScrollLastFrame = false;


        bool Finger1And2ValidLastFrame = false;
        Vector2 Finger1And2CenterLastFrame = Vector2.Zero;
#endif
        public void FreeCamDrag(GameTime dt)
        {
            SetScale();
#if WINDOWS_PHONE || IPHONE
            if (InputManager.Get.Finger1Valid && InputManager.Get.Finger2Valid &&
                Finger1ValidLastFrame && Finger2ValidLastFrame)
            {
                Vector2 thisFinger1Pos = InputManager.Get.Finger1Pos;
                Vector2 thisFinger2Pos = InputManager.Get.Finger2Pos;

                float distThisPos = (thisFinger1Pos - thisFinger2Pos).Length();
                float distLastPos = (lastFinger2Pos - lastFinger1Pos).Length();

                if (validScrollLastFrame)
                {
                    DoZoom((distThisPos - distLastPos) / 2.0f);
                    UniformWorldScale = oldScale;
                }
                validScrollLastFrame = true;
            }
            else
                validScrollLastFrame = false;

            if (InputManager.Get.Finger2Valid)
            {
                //if (!Finger2ValidLastFrame)
                {
                    lastFinger2Pos = InputManager.Get.Finger2Pos;
                }

                Finger2ValidLastFrame = true;
            }
            else
            {
                Finger2ValidLastFrame = false;
            }


            if (InputManager.Get.Finger1Valid) 
            {
                if (Finger1ValidLastFrame && !(screen.CameraFollowingChicken || screen.CannonBeingDragged) && screen.IsTopScreen && !InputManager.Get.Finger2Valid)
                {
                    Vector2 thispos = InputManager.Get.Finger1Pos;

                    draggingThisFrame = true;
                    Vector2 lastPos = ScreenToWorld(lastFinger1Pos);
                    thispos = ScreenToWorld(InputManager.Get.Finger1Pos);

                    float absoluteFarRightAllowed = RightMostWherePosXIs(targetAreaPos.X);
                    float absoluteFarLeftAllowed = LeftMostWherePosXIs(originPos.X);
                    if (ScreenToWorld(new Vector2(1, 0)).X > absoluteFarRightAllowed)
                    {
                        TargetPos = TargetPos + (lastPos - thispos) * 1.0f;
                    }
                    else if (ScreenToWorld(Vector2.Zero).X < absoluteFarLeftAllowed)
                    {
                        TargetPos = TargetPos + (lastPos - thispos) * 1.0f;
                    }
                    else
                        TargetPos = TargetPos + (lastPos - thispos) * 3.0f;
                }
                else if (Finger1ValidLastFrame && Finger2ValidLastFrame)
                {
                    if (Finger1And2ValidLastFrame)
                    {
                        Vector2 Finger1And2Center = (InputManager.Get.Finger1Pos + InputManager.Get.Finger2Pos) / 2.0f;

                        Vector2 lastPos = ScreenToWorld(Finger1And2CenterLastFrame);
                        Vector2 thispos = ScreenToWorld(Finger1And2Center);

                        float absoluteFarRightAllowed = RightMostWherePosXIs(targetAreaPos.X);
                        float absoluteFarLeftAllowed = LeftMostWherePosXIs(originPos.X);
                        if (ScreenToWorld(new Vector2(1, 0)).X > absoluteFarRightAllowed)
                        {
                            TargetPos = TargetPos + (lastPos - thispos) * 1.0f;
                        }
                        else if (ScreenToWorld(Vector2.Zero).X < absoluteFarLeftAllowed)
                        {
                            TargetPos = TargetPos + (lastPos - thispos) * 1.0f;
                        }
                        else
                            TargetPos = TargetPos + (lastPos - thispos) * 10.0f;
                    }
                }
                Finger1And2ValidLastFrame = Finger1ValidLastFrame && Finger2ValidLastFrame;
                if (Finger1And2ValidLastFrame)
                {
                    Finger1And2CenterLastFrame = (lastFinger1Pos + lastFinger2Pos) / 2.0f;
                }
            }
            else
            {
                draggingThisFrame = false;
            }

            if (InputManager.Get.Finger1Valid)
            {
                Finger1ValidLastFrame = true;
                lastFinger1Pos = InputManager.Get.Finger1Pos;
            }
            else
            {
                Finger1ValidLastFrame = false;
            }
#endif

            CapOverflow();

            float farRightAllowed = RightMostWherePosXIs(targetAreaPos.X);
            float farLeftAllowed = LeftMostWherePosXIs(originPos.X);

            // manage left/right overflow..
            float rightMostPos = Camera.Get.ScreenToWorld(new Vector2(1, 0)).X;
            float leftMostPos = Camera.Get.ScreenToWorld(Vector2.Zero).X;

            if (farRightAllowed < rightMostPos && !draggingThisFrame)
            {
                float rms = PosXWhereRightMostIs(farRightAllowed);
                TargetPos = new Vector2(rms, TargetPos.Y);
            }
            // todo: fix left overflow
            else if (farLeftAllowed > leftMostPos && !draggingThisFrame)
            {
                if (PosX < originPos.X)
                {
                    TargetPos = originPos;
                }
                //float lms = PosXWhereLeftMostIs(screen.leftMostPos);
                // TargetPos = new Vector2(lms, TargetPos.Y);
            }
            //UniformWorldScale = 0.3f;

            // move camera towards target position
            if (TargetPos != Position)
            {
                PosX = JabMath.MoveTowards(PosX, TargetPos.X, gttf(dt) * 3.0f, 5);// (TargetPos.X - PosX) * gttf(dt) * 3.0f;
                PosY = JabMath.MoveTowards(PosY, TargetPos.Y, gttf(dt) * 3.0f, 5);
                //PosY += (TargetPos.Y - PosY) * gttf(dt) * 3.0f;
            }


            CapOverflow();
        }
    }
}
