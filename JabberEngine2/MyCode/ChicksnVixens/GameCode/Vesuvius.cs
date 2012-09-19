
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
namespace ChicksnVixens
{
    public static partial class WorldLocation
    {
        //todo: add left/right extreme values for create[location] functions!
        public static void CreateVesuvius(GameScene scene, int leftmostpos, int rightmostpos)
        {
            if (content != null)
            {
                content.Dispose();
                content = null;
            }
            if (LastMusicPlayed != "vesuvius")
            {
                LastMusicPlayed = "vesuvius";
                AudioManager.PlayMusic("vesuvius");
            }

            content = new ContentManager(BaseGame.Get.Services);
            content.RootDirectory = "Content";

            int j = 0;
            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/vesuvius/vesuvius");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/vesuvius/vesuvius_frames");
                s.CurrentFrame = "underground";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;

                s.Layer = BaseSprite.SpriteLayer.LAYER8;

                s.PosX = i;
                s.PosY = -s.Height * s.ScaleY / 2.0f;

                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;

            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/vesuvius/vesuvius");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/vesuvius/vesuvius_frames");
                s.CurrentFrame = "closehill";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.4f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER2;

                s.PosX = i;
                s.PosY = s.Height * s.ScaleY / 2.0f;


                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;



            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/vesuvius/vesuvius");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/vesuvius/vesuvius_frames");
                s.CurrentFrame = "cloud";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.1f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER2;

                s.PosX = i;
                s.PosY = s.Height * s.ScaleY / 2.0f + 400;


                i += (int)((s.Width * s.ScaleX / 1.01f * 1.8f) * JabJect.RandomFloatInRange(1.5f, 2.0f));

                scene.AddNode(s);
            }
            j = 0;



            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/vesuvius/vesuvius");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/vesuvius/vesuvius_frames");
                s.CurrentFrame = "hill";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.3f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER3;

                s.PosX = i;
                s.PosY = s.Height * s.ScaleY / 2.0f;


                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;



            for (int i = leftmostpos - 5000; i < rightmostpos + 5000; )
            {
                Sprite s = new Sprite("textures/backgrounds/vesuvius/vesuvius");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/vesuvius/vesuvius_frames");
                s.CurrentFrame = "midmountain";
                s.ResetDimensions();
                s.UniformScale = 4.0f;
                s.Handle = BaseSprite.SpriteHandle.CENTER;
                s.CamPosScale = new Vector2(0.3f, 1.0f);
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER4;

                s.PosX = i;
                s.PosY = s.Height * s.ScaleY / 2.0f;


                i += (int)(s.Width * s.ScaleX / 1.01f);

                scene.AddNode(s);
            }
            j = 0;


            {
                Sprite s = new Jabber.Util.UI.MenuObj("textures/backgrounds/vesuvius/vesuvius");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/vesuvius/vesuvius_frames");
                s.CurrentFrame = "sky";
                s.ResetDimensions();
                s.UniformScale = 2.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER6;

                s.Width = BaseGame.Get.BackBufferWidth;
                s.Height = BaseGame.Get.BackBufferHeight;
                s.Position = Vector2.Zero;

                scene.AddNode(s);
            }
            {
                Sprite s = new Sprite("textures/backgrounds/vesuvius/vesuvius");
                s.Initialize(content);
                s.CreateFramesFromXML("textures/backgrounds/vesuvius/vesuvius_frames");
                s.CurrentFrame = "vesuvius";
                s.ResetDimensions();
                s.UniformScale = 7.0f;
                s.Layer = BaseSprite.SpriteLayer.BACKGROUND_LAYER5;

                s.PosY = s.Height * s.ScaleY / 2.0f;
                s.CamPosScale = new Vector2(0.2f, 1.0f);

                scene.AddNode(s);
            }
        }
    }
}
