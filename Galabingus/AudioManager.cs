using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Galabingus
{
    public sealed class AudioManager
    {
        private static AudioManager instance = null;

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

        private List<Sound> sounds;

        public AudioManager()
        {
            sounds = new List<Sound>();
        }

        public void AddSound(string name, float volume, string path, ContentManager content)
        {
            sounds.Add(new Sound(name, volume, path, content));
        }

        public void CallSound(string name)
        {
            for (int i = 0; i < sounds.Count; i++) 
            {
                if (sounds[i].Name == name)
                {
                    RunSound(sounds[i].Volume, sounds[i].SoundEffect, sounds[i].AudioInstance);
                }
            }
        }

        private void RunSound(float voulume, SoundEffect sound, SoundEffectInstance instance)
        {
            instance.Volume = voulume;
            instance.Play();
        }
    }
}
