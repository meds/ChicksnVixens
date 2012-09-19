
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
using Jabber.Util.UI;
using Jabber;

using Jabber.Physics;
namespace ChicksnVixens
{
    public static partial class WorldLocation
    {
        //todo: add left/right extreme values for create[location] functions!
        public static void CreateParis(GameScene scene, int leftmostpos, int rightmostpos)
        {
            if (content != null)
            {
                content.Dispose();
                content = null;
            }
            if (LastMusicPlayed != "paris")
            {
                LastMusicPlayed = "paris";
                AudioManager.PlayMusic("paris");
            }
            content = new ContentManager(BaseGame.Get.Services);
            content.RootDirectory = "Content";

            int j = 0;
            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/paris/paris");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
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
            

            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/paris/paris");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
                s.CurrentFrame = "distantbg";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.05f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER3;

                s.PosX = i;// j++ * s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f;


                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;


            {
                MenuObj s = new MenuObj("textures/backgrounds/paris/paris");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
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
           
            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/paris/paris");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
                s.CurrentFrame = "cloud1";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.05f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER4;

                s.PosX = (j++ * 1.5f) * s.Width * s.ScaleX / 1.01f;
                s.PosY = s.Height * s.ScaleY / 2.0f + 150;

                i += (int)(s.Width * s.ScaleX);

                scene.AddNode(s);
            }*/

            
           /* {
                Sprite s = new Sprite("textures/backgrounds/paris/paris");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
                s.CurrentFrame = "rawbg";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER5;

                s.Width = 1000000;
                s.PosY = s.Height * s.ScaleY / 2.0f;

                scene.AddNode(s);
            }*/
            /*
            {
                Sprite s = new Jabber.Util.UI.MenuObj("textures/backgrounds/paris/paris");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
                s.CurrentFrame = "skycore";
                s.ResetDimensions();
                s.UniformScale = 1.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER6;

                s.Width = BaseGame.Get.BackBufferWidth;
                s.Height = BaseGame.Get.BackBufferHeight;
                s.Position = Vector2.Zero;

                scene.AddNode(s);
            }*/
            {
                Sprite s = new Sprite("textures/backgrounds/paris/paris");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
                s.CurrentFrame = "eiffel";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER2;

                s.PosY = s.Height * s.ScaleY / 2.0f;
                s.CamPosScale = new Vector2(0.8f, 1.0f);

                scene.AddNode(s);
            }

            /*
            for (int i = -100; i < 100; i++)
            {
                Sprite s = new Sprite("parisbg");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/paris/paris_frames");
                s.CurrentFrame = "closebuildings";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER2;

                s.PosX = i * s.Width * s.ScaleX * 0.999f;
                s.PosY = s.Height * s.ScaleY / 2.0f;

                s.CamPosScale = new Vector2(0.4f, 1.0f);

                scene.AddNode(s);
            }*/
            /*
            {
                Sprite s = new Sprite("parisbg");
                s.Initialize(content);
                s.CreateFramesFromXML("Content/paris_frames.xml");
                s.CurrentFrame = "SkyGradient";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER3;

                s.Width = 1000000;
                s.PosY = s.Height * s.ScaleY / 2.0f;

                scene.AddNode(s);
            }
            {
                Sprite s = new Sprite("parisbg");
                s.Initialize(content);
                s.CreateFramesFromXML("Content/paris_frames.xml");
                s.CurrentFrame = "Sky";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER5;

                s.Width = 1000000;
                s.Height = 1000000;
                s.PosY = s.Height * s.ScaleY / 2.0f + 100;

                scene.AddNode(s);
            }*/
        }
    }
}
