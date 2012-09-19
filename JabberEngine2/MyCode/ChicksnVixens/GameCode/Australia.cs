
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
        public static string LastMusicPlayed = "";
        public static void CreateAustralia(GameScene scene, int leftmostpos, int rightmostpos)
        {
            if (content != null)
            {
                content.Dispose();
                content = null;
            }
            if (LastMusicPlayed != "uluru")
            {
                LastMusicPlayed = "uluru";
                AudioManager.PlayMusic("uluru");
            }
            content = new ContentManager(BaseGame.Get.Services);
            content.RootDirectory = "Content";

            int j = 0;
            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "underground";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;

                s.Layer = BaseSprite.SpriteLayer.LAYER8;

                s.PosX = i;// +s.Width * s.ScaleX / 1.01f;
                s.PosY = -s.Height * s.ScaleY / 2.0f;

                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;

            //for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "distantmountain";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.05f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER4;

                s.PosX = 0;// j++ * s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f;


                //i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;

            float height = 0;

            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "skycloud";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.05f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER5;

                s.PosX = i;// (j++ * 1.0f) * s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f;

                i += (int)(s.Width * s.ScaleX / 1.01f);
                height = s.Height * s.ScaleY;
                scene.AddNode(s);
            }
            {
                Sprite s = new Sprite("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "topsky";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.05f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER5;

                s.PosX = 0;// (j++ * 1.0f) * s.Width * s.ScaleX / 1.01f;
                s.PosY = height + s.Height*s.ScaleY/2.1f;// *2.0f;

                s.Width = 1000000;

                scene.AddNode(s);
            }
            
            {
                MenuObj s = new MenuObj("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "rawbg";
                s.ResetDimensions();
                s.UniformScale = 1.5f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER6;

                s.Width = BaseGame.Get.BackBufferWidth;
                s.Height = BaseGame.Get.BackBufferHeight;
                s.Position = Vector2.Zero;

                scene.AddNode(s);
            }
            /*
            {
                Sprite s = new Sprite("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "rawbg";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER6;

                s.Width = 1000000;
                s.Height = 1000000;
                s.PosY = s.Height * s.ScaleY / 2.0f + 100;

                scene.AddNode(s);
            }*/



            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                
                Sprite s = new Sprite("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "bgstripclose";
                s.ResetDimensions();
                s.UniformScale = 2.6f;
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
                
                Sprite s = new Sprite("textures/backgrounds/australia/australia");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/australia/australia_frames");
                s.CurrentFrame = "bgstripfar";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
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