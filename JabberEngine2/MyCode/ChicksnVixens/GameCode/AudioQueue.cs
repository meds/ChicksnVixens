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
    public class AudioPlayEvent : Event
    {
        public AudioPlayEvent(string dir, float vol)
        {
            this.Dir = dir;
            this.volume = vol;
        }

        public string Dir = "";
        public float volume = 1.0f;
    }
    public class PlaySlide : Event
    {
        public PlaySlide(bool play)
            : base()
        {
            this.Play = play;
        }

        public bool Play = false;
    }

    public class AudioQueue : BaseSprite
    {
        public AudioQueue(GameplayScreen screen)
            : base()
        {
            this.screen = screen;
            EventManager.Get.RegisterListner(this);
        }
        GameplayScreen screen;
        public override void ProcessEvent(Event ev)
        {
            if (screen.CheckFlag(Flags.FADE_IN) || screen.feathers.fadeInTimer > 0.1f)
            {
                return;
            }
            base.ProcessEvent(ev);

            if (ev is AudioPlayEvent)
            {
                PlayListInst i = new PlayListInst();
                i.Dir = (ev as AudioPlayEvent).Dir;
                i.Volume = (ev as AudioPlayEvent).volume;
                ToPlay.Add(i);
            }
            else if (ev is PlaySlide)
            {
                PlaySlide i = ev as PlaySlide;
                if (i.Play)
                {
                    PlaySlide();
                }
                else
                {
                    StopPlaySlide();
                }
            }
        }

        public class PlayListInst
        {
            public string Dir;
            public float Volume = 1.0f;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            slide.RaiseFlag(Flags.DELETE);
        }

        List<PlayListInst> ToPlay = new List<PlayListInst>();
        public override void Update(GameTime dt)
        {
            if (NumSlide > 0)
            {
                slide.Volume = JabMath.MoveTowards(slide.Volume, 1.0f, gttf(dt)*2.0f);
            }
            else
            {
                slide.Volume = JabMath.MoveTowards(slide.Volume, 0.0f, gttf(dt) * 2.0f);
            }
            base.Update(dt);
            for (int i = 0; i < breakSounds.Count; i++)
            {
                breakSounds[i].lastPlayed -= gttf(dt);
                if (breakSounds[i].lastPlayed < 0)
                {
                    breakSounds.RemoveAt(i); --i;
                }
            }
            for (int i = 0; i < hitSounds.Count; i++)
            {
                hitSounds[i].lastPlayed -= gttf(dt);
                if (hitSounds[i].lastPlayed < 0)
                {
                    hitSounds.RemoveAt(i); --i;
                }
            }
            breakSounds.Clear();
            hitSounds.Clear();
            // if an explosion sfx is playing don't play any other sfx!
            bool AnyExplosion = false;
            for (int i = 0; i < ToPlay.Count; i++)
            {
                String dir = ToPlay[i].Dir;
                if (dir == "Sounds/Explode_Chicken" || dir == "Sounds/Explosion")
                {
                    SoundInst inst = GetInst(dir);
                    if (!inst.IsPlaying)
                    {
                        inst.Play(ToPlay[i].Volume);
                    }
                    else
                        AudioManager.PlayOnce(dir);
                    AnyExplosion = true;
                    break;
                }
            }

            if (!AnyExplosion)
            {
                for (int i = 0; i < ToPlay.Count; i++)
                {
                    String dir = ToPlay[i].Dir;

                    if (ShouldSoundPlay(dir))
                    {
                        SoundInst inst = GetInst(dir);
                        if (!inst.IsPlaying)
                        {
                            inst.Play(ToPlay[i].Volume);
                        }
                    }
                }
            }
            
            ToPlay.Clear();
        }


        SoundInst GetInst(string dir)
        {
            for (int i = 0; i < soundInstances.Count; i++)
            {
                if (soundInstances[i].SoundDir == dir)
                {
                    return soundInstances[i];
                }
            }

            SoundInst s = AudioManager.CreateSound(dir);
            soundInstances.Add(s);
            return s;
        }

        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);
            slide = AudioManager.CreateSound("Sounds/slide", true);
            slide.Play(0);
        }

        List<SoundInst> soundInstances = new List<SoundInst>();

        public static void PlaySlide(bool val)
        {
            EventManager.Get.SendEvent(new PlaySlide(val));
        }

        public static void PlayOnce(string sound)
        {
            PlayOnce(sound, 1.0f);
        }

        public static void PlayOnce(string sound, float vol)
        {
            EventManager.Get.SendEvent(new AudioPlayEvent(sound, vol));
        }

        class BreakSound
        {
            public BreakableBody.BodyMaterial material;
            public float lastPlayed = 0;
            SoundInst sfx = null;
        }
        class HitSound
        {
            public BreakableBody.BodyMaterial material;
            public float lastPlayed = 0.0001f;
            SoundInst sfx = null;
        }

        public void PlaySlide()
        {
            ++NumSlide;
        }

        public void StopPlaySlide()
        {
            --NumSlide;
        }

        int NumSlide = 0;
        SoundInst slide;

        bool ShouldSoundPlay(string dir)
        {
            if (!IsBreakOrHitSound(dir))
            {
                return true;
            }
            BreakableBody.BodyMaterial material = MaterialFromDir(dir);

            if (IsBreakSound(dir))
            {
                for (int i = 0; i < breakSounds.Count; i++)
                {
                    if (breakSounds[i].material == material)
                    {
                        return false;
                    }
                }

                BreakSound s = new BreakSound();
                s.material = material;
                breakSounds.Add(s);

                return true;
            }
            
            if(IsHitSound(dir))
            {
                for (int i = 0; i < hitSounds.Count; i++)
                {
                    if (hitSounds[i].material == material)
                    {
                        return false;
                    }
                }

                HitSound s = new HitSound();
                s.material = material;
                hitSounds.Add(s);

                return true;
            }

            return true;
        }


        bool BreakSoundPlaying(BreakableBody.BodyMaterial material)
        {
            for (int i = 0; i < breakSounds.Count; i++)
            {
                if (breakSounds[i].material == material)
                {
                    return true;
                }
            }

            return false;
        }

        bool HitSoundPlaying(BreakableBody.BodyMaterial material)
        {
            for (int i = 0; i < hitSounds.Count; i++)
            {
                if (hitSounds[i].material == material)
                {
                    return true;
                }
            }

            return false;
        }

        BreakableBody.BodyMaterial MaterialFromDir(string dir)
        {
            if (dir.Contains("Wood"))
            {
                return BreakableBody.BodyMaterial.WOOD;
            }
            else if (dir.Contains("Concrete"))
            {
                return BreakableBody.BodyMaterial.CONCRETE;
            }
            else if (dir.Contains("Glass"))
            {
                return BreakableBody.BodyMaterial.GLASS;
            }
            return BreakableBody.BodyMaterial.GLASS;
        }

        bool IsBreakOrHitSound(string dir)
        {
            return IsBreakSound(dir) || IsHitSound(dir);
        }

        bool IsBreakSound(string dir)
        {
            return dir.Contains("Break");// == "Sounds/Break";
        }

        bool IsHitSound(string dir)
        {
            return dir.Contains("Hit") && !dir.Contains("Chicken");// Substring(10) == "Sounds/Hit";
        }

        List<BreakSound> breakSounds = new List<BreakSound>();
        List<HitSound> hitSounds = new List<HitSound>();
    }
}