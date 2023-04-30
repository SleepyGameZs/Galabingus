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
        private List<Song> songCollection;

        public List<Song> SongCollection
        {
            get { return songCollection; }
        }

        public AudioManager()
        {
            sounds = new List<Sound>();
            songCollection = new List<Song>();
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
                    //sounds[i].Volume = sounds[i].Volume * UIManager.Instance.MasterVolume;
                    sounds[i].CreateInstance();
                    RunSound(sounds[i].Volume, sounds[i].SoundEffect, sounds[i].AudioInstance);
                }
            }
        }

        public void CallSoundOnce(string name)
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].Name == name)
                {
                    //sounds[i].Volume = sounds[i].Volume * UIManager.Instance.MasterVolume;
                    RunSound(sounds[i].Volume, sounds[i].SoundEffect, sounds[i].AudioInstance);
                }
            }
        }

        public void CallMusic(string name)
        {
            for(int i = 0;i < songCollection.Count; i++) 
            {
                if (songCollection[i].Name == name && MediaPlayer.State != MediaState.Playing)
                {
                    //MediaPlayer.Volume = UIManager.Instance.MasterVolume;
                    MediaPlayer.Play(songCollection[i]);
                }
            }
        }

        public void StopSound(string name)
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].Name == name)
                {
                    sounds[i].AudioInstance.Stop();
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
