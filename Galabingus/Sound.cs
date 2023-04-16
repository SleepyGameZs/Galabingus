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
        private string name;
        private float volune;
        private SoundEffect sound;
        private SoundEffectInstance audioInstance;

        public string Name
        {
            get { return name; } 
        }

        public float Volume
        {
            get { return volune; } 
        }

        public SoundEffect SoundEffect
        {
            get { return sound; }
        }

        public SoundEffectInstance AudioInstance
        {
            get { return audioInstance; } 
        }

        public Sound(string name, float volume, string path, ContentManager content)
        {
            this.name = name;
            this.volune = volume;
            sound = content.Load<SoundEffect>(path);
            CreateInstance();
        }

        public virtual void CreateInstance()
        {
            audioInstance = sound.CreateInstance();
        }
    }
}
