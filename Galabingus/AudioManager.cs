using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Galabingus
{
    /* The audio manager holds all the game's sound and music, handeling
     * the importation of sound files and the creation of corresponding
     * sound objects. The manager Adjusts the volume of audio by the volume 
     * slider in options and handels the pausing of sound */

    public sealed class AudioManager
    {
        #region Fields

        // The instance of the audio manager
        private static AudioManager instance = null;

        // List of all sounds
        private List<Sound> sounds;

        // List of all music
        private List<Song> songCollection;

        #endregion

        #region Properties

        /// <summary>
        /// List of all song objects in the game
        /// </summary>
        public List<Song> SongCollection
        {
            get { return songCollection; }
        }

        /// <summary>
        /// Reference to the audio manager
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioManager();
                }
                return instance;
            }

        }

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiates both the sound and music lists 
        /// </summary>
        public AudioManager()
        {
            sounds = new List<Sound>();
            songCollection = new List<Song>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Adds a sound to the sounds list
        /// </summary>
        /// <param name="name"> Name used to identify the sound </param>
        /// <param name="volume"> Base volume of the sound </param>
        /// <param name="path"> File path for the imported file </param>
        /// <param name="content"> Content manager used for importing </param>
        public void AddSound(string name, float volume, string path, ContentManager content)
        {
            sounds.Add(new Sound(name, volume, path, content));
        }

        /// <summary>
        /// Plays a sound effect from the sounds list
        /// </summary>
        /// <param name="name"> Name of the sound </param>
        public void CallSound(string name)
        {
            // Search the sounds list for the sound
            for (int i = 0; i < sounds.Count; i++) 
            {
                if (sounds[i].Name == name)
                {
                    // Create a new instance of the sound
                    sounds[i].CreateInstance();

                    // Adjust the volume of the sound instance by master volume
                    sounds[i].AudioInstance.Volume = sounds[i].Volume * UIManager.Instance.MasterVolume;

                    // Run the adjusted sound instance
                    RunSound(sounds[i].AudioInstance.Volume, sounds[i].AudioInstance);
                }
            }
        }

        /// <summary>
        /// Plays a sound effect from the sounds list
        /// Does not allow for multiple instances to be played at once
        /// </summary>
        /// <param name="name"> Name of the sound </param>
        public void CallSoundOnce(string name)
        {
            // Search the sounds list for the sound
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].Name == name)
                {
                    // Adjust the volume of the sound instance by master volume
                    sounds[i].AudioInstance.Volume = sounds[i].Volume * UIManager.Instance.MasterVolume;

                    // Run the adjusted sound instance
                    RunSound(sounds[i].AudioInstance.Volume, sounds[i].AudioInstance);
                }
            }
        }

        /// <summary>
        /// Plays a music track from the songs list
        /// </summary>
        /// <param name="name"> Name of the song </param>
        public void CallMusic(string name)
        {
            // Reset media player volume
            MediaPlayer.Volume = 1f;

            // Search the songs list for the music track 
            for(int i = 0;i < songCollection.Count; i++) 
            {
                // Check to see if music is already being played or not
                if (songCollection[i].Name == name && MediaPlayer.State != MediaState.Playing)
                {
                    // Adjust the media player's volume by master volume
                    MediaPlayer.Volume = MediaPlayer.Volume * UIManager.Instance.MasterVolume;

                    // Play the adjusted song 
                    MediaPlayer.Play(songCollection[i]);
                }
            }
        }

        /// <summary>
        /// Stops a sound from playing
        /// </summary>
        /// <param name="name"> Name of the sound </param>
        public void StopSound(string name)
        {
            // Search the sounds list for the sound  
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].Name == name)
                {
                    // Stop the playing of the instance
                    sounds[i].AudioInstance.Stop();
                }
            }
        }

        /// <summary>
        /// Helper meathod for playing sounds
        /// </summary>
        /// <param name="voulume"> Volume of the instance </param>
        /// <param name="instance"> The instance of the sound object </param>
        private void RunSound(float voulume, SoundEffectInstance instance)
        {
            instance.Volume = voulume;
            instance.Play();
        }

        #endregion
    }
}
