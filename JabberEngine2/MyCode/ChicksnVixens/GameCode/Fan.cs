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
using ChicksnVixens.Screens;

using Jabber.GameScreenManager;

namespace ChicksnVixens
{
    public class Fan : AnimSprite
    {
        SoundInst FanSound = null;
       // static int NumFans = 0;

        public static int FanEntitySpaceGroup = 15;
        public Fan(JabWorld world)
            : base("misc")
        {
            this.world = world;

            Rot = 0.24f * JabMath.PI;
            Layer = SpriteLayer.LAYER8;

            world.MakeEntityGroup(FanEntitySpaceGroup);
            EventManager.Get.RegisterListner(this);

            //++NumFans;
          //  if (FanSound == null)
            {
                FanSound = AudioManager.CreateSound("Sounds/Fan_Blowing_2", true);
                FanSound.Volume = 0.0f;
            }
            FanSound.Play(1.0f);
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is CollisionEvent)
            {
                CollisionEvent e = ev as CollisionEvent;
                if (e.ActorPresent(box) != null)
                {
                    if (e.ActorPresent(box).UserData is Chicken || e.ActorPresent(box).UserData is Fox || (e.ActorPresent(box).BodyState == JabActor.BodyType.DYNAMIC))
                    {
                        JabActor actor = e.ActorPresent(box);

                        float dist = (actor.Position - Position).Length();

                        float maxdist = (float)Math.Sqrt((float)(Height * Height + Width * Width));
                        float prop = dist / maxdist;
                        prop = 1.0f - prop;
                        if (prop <= 0)
                        {
                        }
                        else
                        {
                            actor.AddForce((prop * Direction * 100.0f));
                        }
                        if (actor.UserData is Chicken)
                        {
                            int k = 0;
                        }
                    }
                    
                }
            }
        }

        Vector2 Direction
        {
            get
            {
                return new Vector2(JabMath.Sin(Rot), JabMath.Cos(Rot));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
           // Rot = 0;// -0.25f * JabMath.PI;// 1.0f * JabMath.PI;// gttf(gameTime);
            /*
            Vector2 toCamera = (Position - Camera.Get.Position);
            float dist = toCamera.Length();
            dist /= 2000.0f;
            if (dist > 1.0f || !Camera.Get.IsVisible(this))
            {
                dist = 1.0f;
            }
            dist = 1.0f - dist;*/

            float closestChicken = float.MaxValue;
            for (int i = 0; i < world.Actors.Count; i++)
            {
                if (world.Actors[i].UserData is Chicken)
                {
                    if ((Position - world.Actors[i].Position).Length() < closestChicken && world.Actors[i].LinearVelocity.Length() > 5.0f)
                    {
                        closestChicken = (Position - world.Actors[i].Position).Length();
                    }
                }
            }
            if (closestChicken < 500 && closestChicken > 0)
            {
                FanSound.Volume = (500 - closestChicken) / 500.0f;
            }
            else
                FanSound.Volume = 0.0f;

            placeholder.Position = Position;
            placeholder.Rot = Rot;

            box.Rot = placeholder.Rot;
            Vector2 airDir = new Vector2(JabMath.Sin(Rot), JabMath.Cos(Rot));
            box.Position = (placeholder.Position + airDir * box.Height / 2.0f) - (100 * airDir);
            for (int i = 0; i < world.Actors.Count; i++)
            {
            }
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("misc_frames");
            Animation = "FanSpin";
            ResetDimensions();
            Width *= 2.0f;
            Height *= 2.0f;
            placeholder = new Sprite("ui/ui");
            placeholder.Initialize(Content);
            placeholder.CreateFramesFromXML("ui/ui_frames");
            placeholder.CurrentFrame = "whitecore";
            placeholder.Width = Width;
            placeholder.Height = 500;
            placeholder.Handle = SpriteHandle.BOTTOMCENTER;

            box = world.CreateBox(placeholder.Dimension + new Vector2(0, 100), placeholder.GetAbsoluteCenter(), JabActor.BodyType.STATIC);
            box.CollisionGroup = FanEntitySpaceGroup;
            box.IgnoreRayCast = true;
            box.UserData = this;

            m_Animations[Animation].curFrame = Jabber.BaseGame.Random.Next(0, m_Animations[Animation].animFrames.Count);
        }


        public override void Draw()
        {
            base.Draw();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
           // --NumFans;
            //if (NumFans <= 0)
            {
                FanSound.Stop();
                FanSound.RaiseFlag(Jabber.Flags.DELETE);
            }
            box.RaiseFlag(Jabber.Flags.DELETE);
        }


        Sprite placeholder;
        JabWorld world;
        JabActor box;
    }
}
