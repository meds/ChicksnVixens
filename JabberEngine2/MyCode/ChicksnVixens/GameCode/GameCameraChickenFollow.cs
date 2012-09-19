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
        void FollowChicken(GameTime dt)
        {
            if (UniformWorldScale == maxCamScale)
            {
                //    return;
            }
            List<Chicken> chicks = screen.withChicks.GetChicks();
            if (!FollowingChicken)
            {
                FollowingChicken = true;
                lastPosBeforeFire = Position;
                lastScaleBeforeFire = UniformWorldScale;
            }
            Chicken activeChicken = screen.withChicks.ActiveChicken;
            if (activeChicken == null)
            {
                //activeChicken = screen.withChicks.LastChicken;
            }
            if (activeChicken == null)
            {
                screen.ForceChickenInActive();
                return;
            }

            float maxAllowedRight = RightMostWherePosXIs(targetAreaPos.X);
            if (activeChicken.PosX > maxAllowedRight)
            {
                activeChicken.Deactivate();
                followingChickenLastFrame = false;
                return;
            }
            if (activeChicken.PosX > furthestLeft && ScaleOnFire != maxCamScale)
            {
                targetPos = targetAreaPos;
                if (activeChicken.PosY < mostHigh && activeChicken.PosY + 200 < Camera.Get.ScreenToWorld(Vector2.Zero).Y)
                {
                    UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, scaleAtTarget, gttf(dt) * 3.0f);
                }
                else
                {
                    float targetScale = ScaleToCoverVerticalArea(activeChicken.PosY + 200, ScreenToWorld(Vector2.One).Y);
                    if (targetScale < 0.1f)
                    {
                        targetScale = 0.1f;
                    }
                    else if (targetScale > scaleAtTarget)
                    {
                        targetScale = scaleAtTarget;
                    }
                    UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, targetScale, gttf(dt) * 3.0f); //targetScale;
                }
            }
            else
            {
                float targetScale = ScaleToCoverVerticalArea(activeChicken.PosY + 200, ScreenToWorld(Vector2.One).Y);
                if (targetScale < 0.1f)
                {
                    targetScale = 0.1f;
                }
                else if (targetScale > 0.5f)
                {
                    targetScale = 0.5f;
                }
                targetPos.Y = (PosY + activeChicken.PosY) / 2.0f;
                
                if( ScaleOnFire != maxCamScale )
                    targetPos.X = PosXWhereLeftMostIs(activeChicken.PosX - 100);
            }


            if (ScreenToWorld(new Vector2(0, 0.2f)).Y < activeChicken.PosY)
            {
                float targetScale = ScaleToCoverVerticalArea(activeChicken.PosY + 200, ScreenToWorld(Vector2.One).Y);
                if (targetScale < 0.1f)
                {
                    targetScale = 0.1f;
                }
                else if (targetScale > scaleAtTarget)
                {
                    targetScale = scaleAtTarget;
                }
                UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, targetScale, gttf(dt) * 3.0f);
            }

            if (PosX > targetAreaPos.X)
            {
                PosX = targetAreaPos.X;
            }

            if (TargetPos != Position)
            {
                PosX = JabMath.MoveTowards(PosX, TargetPos.X, gttf(dt) * 3.0f, 10);
                PosY = JabMath.MoveTowards(PosY, TargetPos.Y, gttf(dt) * 3.0f, 10);
            }

            followingChickenLastFrame = true;

            if (UniformWorldScale > ScaleOnFire)
            {
                UniformWorldScale = ScaleOnFire;
            }
        }
    }
}
