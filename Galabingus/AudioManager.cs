using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

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
        private SongCollection songCollection;

        public SongCollection SongCollection
        {
            get { return songCollection; }
        }

        public AudioManager()
        {
            sounds = new List<Sound>();
        }


        public void AddMusic(Song song)
        {
            songCollection.Add(song);
        }

        public void CallSound(string name)
        {
            for (int i = 0; i < sounds.Count; i++) 
            {
                if (sounds[i].Name == name)
                {
                    sounds[i].CreateInstance();
                    RunSound(sounds[i].Volume, sounds[i].SoundEffect, sounds[i].AudioInstance);
                }
            }
        }

        private void CallMusic(string name)
        {
            for(int i = 0;i < songCollection.Count; i++) 
            {
                if (songCollection[i].Name == name && MediaPlayer.State != MediaState.Playing)
                {
                    MediaPlayer.Play(songCollection[i]);
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
