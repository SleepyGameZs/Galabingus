using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Sound Class
 * Creates sound objects using imported sound files Monogame's SoundEffect Class
 * Sounds have a volume that can changed and can be player ontop of one another */ 
  
namespace Galabingus
{
    internal class Sound
    {
        #region Fields
        
        // Name used to identify sound
        private string name;

        // Volume of the sound
        private float volume;

        // The sound that plays and its instance
        private SoundEffect sound;
        private SoundEffectInstance audioInstance;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the sound object
        /// </summary>
        public string Name
        {
            get { return name; } 
        }

        /// <summary>
        /// Volume of the base sound object
        /// </summary>
        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        /// <summary>
        /// The sound that plays
        /// </summary>
        public SoundEffect SoundEffect
        {
            get { return sound; }
        }

        /// <summary>
        /// The specfic instance of the sound object
        /// </summary>
        public SoundEffectInstance AudioInstance
        {
            get { return audioInstance; } 
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a sound object with a base instance containing its name and volume 
        /// </summary>
        /// <param name="name"> Name of the sound </param>
        /// <param name="volume"> Volume of the sound </param>
        /// <param name="path"> File path for sound file </param>
        /// <param name="content"> Content manager used for importing </param>
        public Sound(string name, float volume, string path, ContentManager content)
        {
            this.name = name;
            this.volume = volume;

            // Importing of sound effect
            sound = content.Load<SoundEffect>(path);

            // Creation of the base instance
            CreateInstance();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of a sound so that multiple instaces can run at once
        /// </summary>
        public virtual void CreateInstance()
        {
            audioInstance = sound.CreateInstance();
        }
        #endregion
    }
}
