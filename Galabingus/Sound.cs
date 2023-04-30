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

namespace Galabingus
{
    internal class Sound
    {
        #region Fields
        
        // Name used to identify sound
        private string name;

        private float volume;
        private SoundEffect sound;
        private SoundEffectInstance audioInstance;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; } 
        }

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public SoundEffect SoundEffect
        {
            get { return sound; }
        }

        public SoundEffectInstance AudioInstance
        {
            get { return audioInstance; } 
        }
        #endregion

        #region Constructor
        public Sound(string name, float volume, string path, ContentManager content)
        {
            this.name = name;
            this.volume = volume;
            sound = content.Load<SoundEffect>(path);
            CreateInstance();
        }
        #endregion

        #region Methods
        public virtual void CreateInstance()
        {
            audioInstance = sound.CreateInstance();
        }
        #endregion
    }
}
