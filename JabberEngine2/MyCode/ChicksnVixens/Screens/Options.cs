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
using System.Xml.Linq;

using Jabber.Physics;

using Jabber.GameScreenManager;
using ChicksnVixens;
using Jabber.Util.UI;

namespace ChicksnVixens.Screens
{
    public class Options : Screen
    {
        public Options()
            : base()
        {
            EventManager.Get.RegisterListner(this);
        }
#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();
            blank.RaiseFlag(Flags.FADE_OUT);
        }
#endif
        Button arrowToggle;
        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is VolumeEvent)
            {
                if (((ev as VolumeEvent).sender as VolumeControl).Name == "MusicVol")
                {
                    AudioManager.MusicVolume = ((ev as VolumeEvent).sender as VolumeControl).CurrentVolume;
                }
                else if (((ev as VolumeEvent).sender as VolumeControl).Name == "SoundVol")
                {
                    AudioManager.SoundVolume = ((ev as VolumeEvent).sender as VolumeControl).CurrentVolume;
                }
            }
            else if (ev is MenuEvent)
            {
                if ((ev as MenuEvent).sender is Button && (ev as MenuEvent).Type == MenuEvent.EventType.RELEASE)
                {
                    try
                    {
                        if (((ev as MenuEvent).sender as Button).CurrentFrame == "quit")
                        {
                            blank.RaiseFlag(Flags.FADE_OUT);
                        }
                        else if (((ev as MenuEvent).sender as Button).Text.Text == "Trajectory")
                        {
                            arrowToggle.Text.Text = "Arrow";
                            ChicksnVixensGame.Get.UseTrajectory = false;
                        }
                        else if (((ev as MenuEvent).sender as Button).Text.Text == "Arrow")
                        {
                            arrowToggle.Text.Text = "Trajectory";
                            ChicksnVixensGame.Get.UseTrajectory = true;
                        }
                    }
                    catch (Exception excep)
                    {
                    }
                }
            }
        }

        public override void Draw()
        {
          //  base.Draw();

            for (int i = 0; i < Components.Count; i++)
            {
                if (!(Components[i] is BlankNess))
                {
                    Color oldcol = Components[i].Colour;
                    Components[i].Colour *= blank.fadeInTimer;
                    Components[i].Draw();
                    Components[i].Colour = oldcol;
                }
                else
                    Components[i].Draw();
            }
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

#if WINDOWS_PHONE
            AdSystem.TargetTop = false;
#endif

            if (blank.CheckFlag(Flags.FADE_OUT))
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].RaiseFlag(Flags.FADE_OUT);
                    //if (!(Components[i] is BlankNess))
                    {
                     //   Components[i].Colour = Color.White * blank.fadeInTimer;
                    }
                }
                if (blank.StateFlag == Jabber.StateFlag.FADE_OUT_COMPLETE)
                {
                    RaiseFlag(Flags.DELETE);
                }
            }
            if (blank.StateFlag == Jabber.StateFlag.FADE_OUT_COMPLETE)
            {
                RaiseFlag(Flags.DELETE);
            }
        }
        BlankNess blank = null;
        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);

            blank = new BlankNess();
            blank.Initialize(Content);
            blank.RaiseFlag(Flags.FADE_IN);
            blank.fullBlankity = 0.85f;
            blank.fadeSpeed = 4.0f;
            Components.Add(blank);

            VolumeControl v = new VolumeControl(AudioManager.MusicVolume);
            v.Initialize(Content);
            v.Name = "MusicVol";
            v.CurrentVolume = AudioManager.MusicVolume;
            v.PosY = 100.0f * ScaleFactor;
            Components.Add(v);


            TextDrawer text = new TextDrawer("ui/LevelFont");
            text.Initialize(Content);
            text.Text = "Music Volume";
            text.Colour = Color.Gold;

            if (!AudioManager.IsMusicAllowed)
            {
                text.Text = "Music Disabled";
                text.Colour = Color.Red;
                v.Disabled = true;
                v.CurrentVolume = 0;
            }
            
            text.Position = v.Position;
            text.UniformScale = ScaleFactor;
            Components.Add(text);


            v = new VolumeControl(AudioManager.SoundVolume);
            v.Initialize(Content);
            v.Name = "SoundVol";
            v.CurrentVolume = AudioManager.SoundVolume;
            v.PosY = 0 * ScaleFactor;
            v.PlayTickWhenDone = true;
            Components.Add(v);


            text = new TextDrawer("ui/LevelFont");
            text.Initialize(Content);
            text.Text = "Sound Volume";
            text.Colour = Color.Gold;
            text.Position = v.Position;
            text.UniformScale = ScaleFactor;
            Components.Add(text);

            Button back = new Button("ui/ui");
            back.Initialize(Content);
            back.CreateFramesFromXML("ui/ui_frames");
            back.CurrentFrame = "quit";
            back.ResetDimensions();
            back.UniformScale = ScaleFactor * 0.001f;
            back.RegularScale = ScaleFactor * 0.5f;
            back.ScaleOnHover = ScaleFactor * 0.55f;
            back.Colour = Color.Red;
            back.Effect = BaseSprite.SpriteEffect.FLIPHORIZONTAL;
            back.PosX = 0.40f * BaseGame.Get.BackBufferWidth;
            back.PosY = 0.40f * BaseGame.Get.BackBufferHeight;
            back.PlaySFXOnRelease = "Sounds/PlayStateSelect";
            Components.Add(back);


            arrowToggle = new Button("ui/ui");
            arrowToggle.CreateFramesFromXML("ui/ui_frames");
            arrowToggle.Initialize(Content);
            arrowToggle.CurrentFrame = "levelbutton";
            arrowToggle.ResetDimensions();
            arrowToggle.RegularScale = ScaleFactor * 1.5f;
            arrowToggle.ScaleOnHover = arrowToggle.RegularScale * 1.1f;
            arrowToggle.PosX = 0 * BaseGame.Get.BackBufferWidth;
            arrowToggle.PosY = -0.45f * BaseGame.Get.HalfBackBufferHeight;

            if(ChicksnVixensGame.Get.UseTrajectory)
                arrowToggle.SetText("Trajectory", "ui/Play");
            else
                arrowToggle.SetText("Arrow", "ui/Play");

            arrowToggle.Text.Colour = Color.Black;
            arrowToggle.PlaySFXOnRelease = "Sounds/PlayStateSelect";
            arrowToggle.TextScaler = 0.65f;
            arrowToggle.UniformScale = arrowToggle.RegularScale;
            Components.Add(arrowToggle);
        }
    }
}