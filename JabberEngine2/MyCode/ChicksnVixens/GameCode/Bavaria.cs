﻿
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
using Jabber.Util.UI;
using Jabber.Physics;
namespace ChicksnVixens
{
    public static partial class WorldLocation
    {
        //todo: add left/right extreme values for create[location] functions!
        public static void CreateBavaria(GameScene scene, int leftmostpos, int rightmostpos)
        {
            
            if (content != null)
            {
                content.Dispose();
                content = null;
            }
            if (LastMusicPlayed != "bavaria")
            {
                LastMusicPlayed = "bavaria";
                AudioManager.PlayMusic("bavaria");
            }

            content = new ContentManager(BaseGame.Get.Services);
            content.RootDirectory = "Content";

            int j = 0;
            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/bavaria/bavaria");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/bavaria/bavaria_frames");
                s.CurrentFrame = "underground";
                s.ResetDimensions();
                s.UniformScale = 4.5f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;

                s.Layer = BaseSprite.SpriteLayer.LAYER8;

                s.PosX = i;// +s.Width * s.ScaleX / 1.01f;
                s.PosY = -s.Height * s.ScaleY / 2.0f;

                i += (int)(s.Width * s.ScaleX / 1.001f);

                scene.AddNode(s);
            }
            j = 0;
            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/bavaria/bavaria");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/bavaria/bavaria_frames");
                s.CurrentFrame = "mountain";
                s.ResetDimensions();
                s.UniformScale = 6.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.05f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER4;

                s.PosX = i;// j++ * s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f;


                i += (int)(s.Width * s.ScaleX / 1.001f);

                scene.AddNode(s);
            }
            j = 0;
            /*

            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/bavaria/bavaria");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/bavaria/bavaria_frames");
                s.CurrentFrame = "sky";
                s.ResetDimensions();
                s.UniformScale = 3.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.05f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER5;

                s.PosX = i;// (j++ * 1.0f) * s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f;

                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }*/
            {
                MenuObj s = new MenuObj("textures/backgrounds/bavaria/bavaria");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/bavaria/bavaria_frames");
                s.CurrentFrame = "rawbg";
                s.ResetDimensions();
                s.UniformScale = 1.4f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER6;

                s.Dimension = BaseGame.Get.BackBufferDimensions;
                s.Position = Vector2.Zero;
                scene.AddNode(s);
            }


            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/bavaria/bavaria");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/bavaria/bavaria_frames");
                s.CurrentFrame = "bgstripclose";
                s.ResetDimensions();
                s.UniformScale = 2.6f * 2.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;

                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER2;

                s.PosX = i;// +s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f;
                s.CamPosScale = new Vector2(0.3f, 1.0f);
                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;


            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/bavaria/bavaria");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/bavaria/bavaria_frames");
                s.CurrentFrame = "bgstripfar";
                s.ResetDimensions();
                s.UniformScale = 2.0f * 2.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;

                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER3;
                s.CamPosScale = new Vector2(0.1f, 1.0f);

                s.PosX = i;// +s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f;

                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;
        }
    }
}
