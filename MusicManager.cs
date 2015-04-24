using Android.Content;
using Android.Media;
using System;
using System.Collections.Generic;

namespace com.axxies.blockstar
{
    public static class MusicManager
    {
        /// <summary>The types of music that can be played.</summary>
        public enum MusicType 
        {
            Menu = 1,
            Level = 2
        }

        private static Dictionary<MusicType, List<int>> m_soundPool;
        private static AudioManager m_audioManager;
        private static Context m_context;
        private static MediaPlayer m_mediaPlayer;
        private static EventHandler m_mediaPlayerPreparedEvent;
        private static float m_volume;

        /// <summary>Whether the music manager has been initialized or not.</summary>
        public static bool Initialized { get; private set; }

        /// <summary>Static initialization.</summary>
        static MusicManager()
        {
            Initialized = false;
            m_mediaPlayer = null;
            m_audioManager = null;
            m_mediaPlayerPreparedEvent = null;
            m_context = null;
            m_soundPool = null;
            m_volume = 100;
        }

        /// <summary>Allow directly initializing the sound manager so that we can decide when we want to load up all the resources.</summary>
        public static void Initialize(Context context, int volumePercent)
        {
            // Don't re-initialize if initialization has happened already.
            if (Initialized)
            {
                return;
            }

            // Make sure arguments are valid.
            if (null == context)
            {
                throw new ArgumentNullException("context");
            }
            else if (volumePercent < 0 || volumePercent > 100)
            {
                throw new ArgumentOutOfRangeException("volumePercent", "Volume must be between 0 and 100 percent.");
            }

            // Set member variables.
            m_context = context;
            m_soundPool = new Dictionary<MusicType, List<int>>();
            m_audioManager = m_context.GetSystemService(Context.AudioService) as AudioManager;
            m_volume = GetMediaPlayerVolume(m_audioManager, volumePercent);

            // Add the level music to our sound pool.
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_1);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_2);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_3);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_4);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_5);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_6);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_7);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_8);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_9);
            m_soundPool.CreateOrAdd(MusicType.Level, Resource.Raw.level_music_10);

            // Add the menu music to our sound pool.
            m_soundPool.CreateOrAdd(MusicType.Menu, Resource.Raw.menu_music);

            // We are now initialized.
            Initialized = true;
        }

        /// <summary>Plays the given music for the given type.  Use MusicManager.GetMusicCountForType() to determine the number of music of each type there is.</summary>
        public static int GetMusicCountForType(MusicType musicType)
        {
            ThrowIfNotInitialized();

            // Check parameters.
            if (!m_soundPool.ContainsKey(musicType))
            {
                throw new ArgumentOutOfRangeException("musictype", "The given music type is not invalid...");
            }

            // Return the count of music for the given type.
            return m_soundPool[musicType].Count;
        }

        /// <summary>Sets the volume of the music.</summary>
        public static void SetVolumePercent(int volumePercent)
        {
            ThrowIfNotInitialized();

            // Check for a valid percent.
            if (volumePercent < 0 || volumePercent > 100)
            {
                throw new ArgumentOutOfRangeException("volumePercent", "Volume must be between 0 and 100 percent.");
            }

            // Set the new volume.
            m_volume = GetMediaPlayerVolume(m_audioManager, volumePercent);

            // We need to adjust the current volume if the media player is playing.
            if (null != m_mediaPlayer)
            {
                m_mediaPlayer.SetVolume(m_volume, m_volume);
            }
        }

        /// <summary>Plays a random music number of the given type.</summary>
        public static void Play(MusicType musicType)
        {
            ThrowIfNotInitialized();

            // Make sure the given parameters are valid.
            if (!m_soundPool.ContainsKey(musicType))
            {
                throw new ArgumentOutOfRangeException("musictype", "The given music type is not invalid...");
            }

            // Play something random.
            Random rand = new Random();
            Play(musicType, rand.Next(m_soundPool[musicType].Count - 1));
        }

        /// <summary>Plays the given music for the given type.  Use MusicManager.GetMusicCountForType() to determine the number of music of each type there is.</summary>
        public static void Play(MusicType musicType, int musicNumber)
        {
            ThrowIfNotInitialized();

            // Make sure the given parameters are valid.
            if (!m_soundPool.ContainsKey(musicType))
            {
                throw new ArgumentOutOfRangeException("musictype", "The given music type is not invalid...");
            }
            else if (m_soundPool[musicType].Count <= musicNumber)
            {
                throw new ArgumentOutOfRangeException("musicNumber", "The given music number is invalid.  Make sure to call GetMusicCountForType()");
            }

            // Stop and cleanup previous MediaPlayer.
            Stop();

            // Create new MediaPlayer with the given music ID.
            m_mediaPlayer = MediaPlayer.Create(m_context, m_soundPool[musicType][musicNumber]);
            m_mediaPlayer.Looping = true;
            m_mediaPlayer.SetVolume(m_volume, m_volume);
            m_mediaPlayerPreparedEvent = delegate { m_mediaPlayer.Start(); };
            m_mediaPlayer.Prepared += m_mediaPlayerPreparedEvent;
        }

        /// <summary>Resumes playback.  Can only be called after MusicManager.Pause().</summary>
        public static void Resume()
        {
            ThrowIfNotInitialized();

            // Make sure that the media player exists...otherwise Play() was never called.
            if (null == m_mediaPlayer)
            {
                throw new Exception("MusicManager.Resume() can't be called before Play() and Pause() has been called.");
            }

            // Don't bother if the player is not already playing.
            if (!m_mediaPlayer.IsPlaying)
            {
                m_mediaPlayer.Start();
            }
        }

        /// <summary>Resumes playback.  Can only be called after MusicManager.Play().</summary>
        public static void Pause()
        {
            ThrowIfNotInitialized();

            // Make sure that the media player exists...otherwise Play() was never called.
            if (null == m_mediaPlayer)
            {
                throw new Exception("MusicManager.Pause() can't be called before Play() has been called.");
            }

            // Don't bother if the player is already playing.
            if (m_mediaPlayer.IsPlaying)
            {
                m_mediaPlayer.Pause();
            }
        }

        /// <summary>Destroys and uninitializes the MusicManager.</summary>
        public static void Destroy()
        {
            Stop();
            m_audioManager.UnloadSoundEffects();
            m_context = null;
            m_soundPool = null;
            m_audioManager = null;
            Initialized = false;
        }

        /// <summary>Stops the MusicManager.  This invalidates the MediaPlayer.</summary>
        private static void Stop()
        {
            ThrowIfNotInitialized();

            // Don't try to stop the media player if it doesn't exist.
            if (null != m_mediaPlayer)
            {
                // Remove the prepared event and stop and release the media player before setting it to null.
                System.Diagnostics.Debug.Assert(null != m_mediaPlayerPreparedEvent, "The media player prepared event should not be null if the media player is not null!");
                m_mediaPlayer.Prepared -= m_mediaPlayerPreparedEvent;
                m_mediaPlayer.Stop();
                m_mediaPlayer.Release();
                m_mediaPlayerPreparedEvent = null;
                m_mediaPlayer = null;
            }
        }

        /// <summary>Throw an exception if initialize wasn't called before attempting to use the music manager.</summary>
        private static void ThrowIfNotInitialized()
        {
            if (!Initialized)
            {
                throw new Exception("MusicManager.Initialize() has to be called before using MusicManager.");
            }
        }

        /// <summary>Calculates and returns the media player volume to use.</summary>
        private static float GetMediaPlayerVolume(AudioManager audioManager, int volumePercent)
        {
            // Check parameters.
            if (null == audioManager)
            {
                throw new ArgumentNullException("audioManager");
            }
            else if (volumePercent < 0 || volumePercent > 100)
            {
                throw new ArgumentOutOfRangeException("volumePercent", "Volume must be between 0 and 100 percent.");
            }

            // Get the max stream volume.
            float maxVolume = m_audioManager.GetStreamMaxVolume(Stream.Music);

            // Return the max stream volume reduced by the given volume percent.
            return maxVolume * (volumePercent / 100);
        }

        /// <summary>Create the given key/list pair in the dictionary or add to the list if it already exists.</summary>
        private static void CreateOrAdd<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(value);
            }
            else
            {
                dictionary.Add(key, new List<TValue>() { value });
            }
        }
    }
}
