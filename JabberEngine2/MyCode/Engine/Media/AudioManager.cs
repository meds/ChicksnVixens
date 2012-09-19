// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------
#define PLAY_SOUND

#region File Description
//-----------------------------------------------------------------------------
// AudioManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
#endregion



namespace Jabber.Media
{
    public class SoundInst : JabJect
    {
        public SoundInst(string dir, bool loop)
        {
            soundDir = dir;
#if PLAY_SOUND
            LoadSFX();
            sound.IsLooped = loop;
#endif
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            #if PLAY_SOUND
            sound.Dispose();
            #endif
        }
#if PLAY_SOUND
        SoundEffectInstance sound;
#endif
        public void Play(float vol)
        {
            #if PLAY_SOUND
            //LoadSFX();
            sound.Volume = vol * AudioManager.SoundVolume;
            sound.Play();
            #endif
        }
        public void Stop()
        {
            #if PLAY_SOUND
            sound.Stop();
            #endif
        }
        public void Resume()
        {
#if PLAY_SOUND
            sound.Resume();
            #endif
        }
        public void Pause()
        {
#if PLAY_SOUND
            sound.Pause();
            #endif
        }

        public bool IsPlaying
        {
            get
            {
#if PLAY_SOUND
                return sound.State == SoundState.Playing;
                #endif
                return false;
            }
        }

        public float Volume
        {
#if PLAY_SOUND
            get
            {
                if (AudioManager.SoundVolume == 0)
                {
                    return 0;
                }
                return sound.Volume / AudioManager.SoundVolume;
            }
            set
            {
                sound.Volume = value * AudioManager.SoundVolume;
            }
#else
            get;set;
#endif
        }

        void LoadSFX()
        {
            #if PLAY_SOUND
            if (sound != null)
            {
                sound.Stop();
                sound.Dispose();
            }
            sound = BaseGame.Get.Content.Load<SoundEffect>(soundDir).CreateInstance();
#endif
        }

        public string SoundDir
        {
            get
            {
                return soundDir;
            }
        }

        string soundDir = "";
    }
    /// <summary>
    /// Component that manages audio playback for all sounds.
    /// </summary>
    public class AudioManager : GameComponent
    {
        #region Singleton
        /// <summary>
        /// The singleton for this type.
        /// </summary>
        private static AudioManager audioManager = null;
        #endregion

        #region Audio Data
        private SoundEffectInstance musicSound;
        private Dictionary<string, SoundEffectInstance> soundBank;
        //private string[,] soundNames;
        #endregion

        #region Initialization Methods

        private AudioManager(Game game)
            : base(game)
		{
            //MediaPlayer.ActiveSongChanged += SongChanged;
            //MediaPlayer.MediaStateChanged += MediaStateChanged;
        }
        static void SongChanged(object sender, EventArgs e)
        {
        }
        static void MediaStateChanged(object sender, EventArgs e)
        {
        }
        static public void CacheSound(string dir)
        {
            audioManager.Game.Content.Load<SoundEffect>("Sounds/" + dir);
        }
        /// <summary>
        /// Initialize the static AudioManager functionality.
        /// </summary>
        /// <param name="game">The game that this component will be attached to.</param>
        public static void Initialize(Game game)
        {
            audioManager = new AudioManager(game);

            if (game != null)
            {
                game.Components.Add(audioManager);
            }
        }

        #endregion

        #region Loading Methodes
        /// <summary>
        /// Loads a sounds and organizes them for future usage
        /// </summary>
        public static void LoadSounds()
        {
            // string soundLocation = "Sounds/";
            /*audioManager.soundNames = new string[,] { 
                            {"CatapultExplosion", "catapultExplosion"}, 
                            {"Lose", "gameOver_Lose"},
                            {"Win", "gameOver_Win"},
                            {"BoulderHit", "boulderHit"},
                            {"CatapultFire", "catapultFire"},
                            {"RopeStretch", "ropeStretch"}};*/

            audioManager.soundBank = new Dictionary<string, SoundEffectInstance>();

            /* for (int i = 0; i < audioManager.soundNames.GetLength(0); i++)
             {
                 SoundEffect se = audioManager.Game.Content.Load<SoundEffect>(
                     soundLocation + audioManager.soundNames[i, 0]);
                 audioManager.soundBank.Add(
                     audioManager.soundNames[i, 1], se.CreateInstance());
             }*/
        }
        #endregion
        	
        public override void Update(GameTime time)
        {
            base.Update(time);
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].CheckFlag(Flags.DELETE))
                {
                    sounds[i].UnloadContent();
                    sounds.RemoveAt(i); --i;
                }
            }
        }
        static List<SoundInst> sounds = new List<SoundInst>();
        public static SoundInst CreateSound(string dir, bool looped)
        {
            SoundInst s = new SoundInst(dir, looped);
            sounds.Add(s);
            return s;
        }

        public static SoundInst CreateSound(string dir)
        {
            return CreateSound(dir, false);
        }

        public static bool IsMusicAllowed
        {
            get
            {
                return MediaPlayer.GameHasControl && !DoNotPlayMusic;
            }
        }

        public static void PlayOnce(string dir, float volume)
        {
#if PLAY_SOUND
            SoundEffect se = audioManager.Game.Content.Load<SoundEffect>(dir);
            se.Play(volume * SoundVolume, 0.0f, 0.0f);
#endif
        }
        public static void PlayOnce(string dir)
        {
#if PLAY_SOUND
            SoundEffect se = audioManager.Game.Content.Load<SoundEffect>(dir);
            se.Play(0.4f * SoundVolume, 0.0f, 0.0f);
#endif
        }

        #region Sound Methods
        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play</param>
        public static void PlaySound(string soundName)
        {
            // If the sound exists, start it
            if (audioManager.soundBank.ContainsKey(soundName))
                audioManager.soundBank[soundName].Play();
        }

        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play</param>
        /// <param name="isLooped">Indicates if the sound should loop</param>
        public static void PlaySound(string soundName, bool isLooped)
        {
            // If the sound exists, start it
            if (audioManager.soundBank.ContainsKey(soundName))
            {
                if (audioManager.soundBank[soundName].IsLooped != isLooped)
                    audioManager.soundBank[soundName].IsLooped = isLooped;

                audioManager.soundBank[soundName].Play();
            }
        }


        /// <summary>
        /// Stops a sound mid-play. If the sound is not playing, this
        /// method does nothing.
        /// </summary>
        /// <param name="soundName">The name of the sound to stop</param>
        public static void StopSound(string soundName)
        {
            // If the sound exists, stop it
            if (audioManager.soundBank.ContainsKey(soundName))
                audioManager.soundBank[soundName].Stop();
        }

        /// <summary>
        /// Stops a sound mid-play. If the sound is not playing, this
        /// method does nothing.
        /// </summary>
        /// <param name="soundName">The name of the sound to stop</param>
        public static void StopSounds()
        {
            var soundEffectInstances = from sound in audioManager.soundBank.Values
                                       where sound.State != SoundState.Stopped
                                       select sound;

            foreach (var soundeffectInstance in soundEffectInstances)
                soundeffectInstance.Stop();
        }

        /// <summary>
        /// Pause or Resume all sounds to support pause screen
        /// </summary>
        /// <param name="isPause">Should pause or resume?</param>
        public static void PauseResumeSounds(bool isPause)
        {
            SoundState state = isPause ? SoundState.Paused : SoundState.Playing;

            var soundEffectInstances = from sound in audioManager.soundBank.Values
                                       where sound.State == state
                                       select sound;

            foreach (var soundeffectInstance in soundEffectInstances)
            {
                if (isPause)
                    soundeffectInstance.Play();
                else
                    soundeffectInstance.Pause();
            }
        }

        string songPlayingWhenStopped = "";
        public static bool MusicPlaying
        {
            get { return MediaPlayer.State == MediaState.Playing; }
            set
            {
                if (value)
                {
                    if (!MusicPlaying)
                    {
                        MediaPlayer.Resume();
                    }
                }
                else
                {
                    if (MusicPlaying)
                    {
                        MediaPlayer.Pause();
                    }
                }
            }
        }
        static float fSoundVolume = 0.5f;
        public static float SoundVolume
        {
            set
            {
                fSoundVolume = value;
            }
            get
            {
                return fSoundVolume;
            }
        }
        public static float MusicVolume
        {
            get
            {
                if (!MusicPlaying)
                {
                    return 0;
                }
                return MediaPlayer.Volume;
            }
            set
            {
                if (value == 0)
                {
                    MediaPlayer.Volume = 0.0000001f;
                }
                else
                {
                    MediaPlayer.Volume = value;
                }
            }
        }
        static string currentSong = "";
        static public void StopTheMusic()
        {
            MediaPlayer.Stop();
            currentSong = "";
        }
        static public string CurrentSong
        {
            get
            {
                return currentSong;
            }
        }

        public static double PlayingMusicPosition
        {
            get
            {
                return MediaPlayer.PlayPosition.TotalSeconds;
            }
        }

        public static bool DoNotPlayMusic
        {
            get;
            private set;
        }
        /// <summary>
        /// Play music by sound name.
        /// </summary>
        /// <param name="musicSoundName">The name of the music sound</param>
        public static void PlayMusic(string musicSoundName)
        {
            if (DoNotPlayMusic)
            {
                return;
            }
            try
            {
#if WINDOWS_PHONE
                if (!MediaPlayer.GameHasControl)
                    return;
#endif
                if (currentSong != musicSoundName)
                {
                    currentSong = musicSoundName;
                    audioManager.song = audioManager.Game.Content.Load<Song>("Sounds/" + musicSoundName);
                    MediaPlayer.Play(audioManager.song);


                    if (currentSong.ToLowerInvariant() != musicSoundName.ToLowerInvariant())
                    {
                        DoNotPlayMusic = true;
                    }
                }
            }
            catch (Exception excep)
            {
                DoNotPlayMusic = true;
            }

            /*if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(audioManager.song);
                MediaPlayer.IsRepeating = true;
            }*/
            /*
            // Stop the old music sound
            if (audioManager.musicSound != null)
                audioManager.musicSound.Stop(true);
            

            // If the music sound exists
            if (audioManager.soundBank.ContainsKey(musicSoundName))
            {
                // Get the instance and start it
                audioManager.musicSound = audioManager.soundBank[musicSoundName];
                if (!audioManager.musicSound.IsLooped)
                    audioManager.musicSound.IsLooped = true;
                audioManager.musicSound.Play();
            }
            */
        }
        #endregion
        Microsoft.Xna.Framework.Media.Song song = null;
        #region Instance Disposal Methods
        /// <summary>
        /// Clean up the component when it is disposing.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                /*if (disposing)
                {
                    foreach (var item in soundBank)
                    {
                        item.Value.Dispose();
                    }
                    soundBank.Clear();
                    soundBank = null;
                }*/
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}