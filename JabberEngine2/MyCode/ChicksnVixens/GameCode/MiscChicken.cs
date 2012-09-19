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
using Jabber.Physics;
using Jabber.Scene;
using Jabber;
using ChicksnVixens.Screens;
namespace ChicksnVixens
{
    public class ChickenDrawer : Sprite
    {
        public ChickenDrawer(ChicksScene scene, ThisGamesScene gamescene)
            : base("chickens")
        {
            this.scene = scene;
            this.gamescene = gamescene;
            Layer = SpriteLayer.LAYER10;
        }
        ChicksScene scene;
        ThisGamesScene gamescene;
        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            CreateFramesFromXML("chickens_frames");
            Width = Height = 10;
        }
        
        public override void Update(GameTime dt)
        {
            base.Update(dt);
        }

        public override void Draw()
        {
            Position = gamescene.CannonPos;
            Vector2 cannonPos = gamescene.CannonPos;
            List<int> tofire = new List<int>(gamescene.ToFire);
            tofire.Reverse();
            for (int i = 0; i < tofire.Count; i++)
            {
                
                if (i == 0)
                {
                    PosX -= 46 * 2.0f;
                    PosY -= 1;
                }
                else
                {
                    PosY = cannonPos.Y - 50;
                }
                switch (tofire[i])
                {
                    case 0:
                        CurrentFrame = "bird-chicken-00000";
                        break;
                    case 1:
                        CurrentFrame = "bird-raptor-base-00000";
                        break;
                    case 2:
                        CurrentFrame = "bird-egg-00000";
                        break;
                    case 3:
                        CurrentFrame = "bird-bomb-00000";
                        break;
                    case 4:
                        CurrentFrame = "egg00000";
                        break;
                }
                ResetDimensions();
                base.Draw();

                PosX -= 128.0f;
            }
        }
    }

    public class ChickenBience : BaseSprite
    {
        public ChickenBience(GameplayScreen screen)
            : base()
        {
            this.screen = screen;
        }
        GameplayScreen screen;
        public override void Update(GameTime dt)
        {
            if(screen.CheckFlag(Flags.FADE_IN))
            {
                return;
            }
            base.Update(dt);
            if (!sound.IsPlaying)
            {
                sound.Play(0.1f);
            }

            Vector2 toCamera = (Position - Camera.Get.Position);
            float dist = toCamera.Length();
            dist /= 2000.0f;
            if (dist > 1.0f || screen.withChicks.ActiveChicken != null || screen.scene.ToFire.Count == 0)
            {
                dist = 1.0f;
            }
            dist = 1.0f - dist;
            sound.Volume = dist;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            sound.RaiseFlag(Flags.DELETE);
        }
        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);

            sound = AudioManager.CreateSound("Sounds/ChickenBience");
        }

        SoundInst sound = null;
    }
}