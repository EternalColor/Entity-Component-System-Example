using System;
using FindTheIdol.Components.Music;

namespace FindTheIdol.Utilities.Music
{
    public sealed class AudioClipCache 
    {
        private UnityEngine.AudioClip cachedFanfareAudioClip;

        private UnityEngine.AudioClip cachedMonsterGrowlAudioClip;

        private UnityEngine.AudioClip cachedMonsterTakeDamageAudioClip;
        
        private UnityEngine.AudioClip cachedPositiveAudioClip;
        
        private UnityEngine.AudioClip cachedNegativeAudioClip;
        
        private UnityEngine.AudioClip cachedSmashAudioClip;

        public UnityEngine.AudioClip GetOrCreateSFX(SFXType sfxType)
        {
            lock(padlock)
            {
                switch(sfxType)
                {
                    case SFXType.Fanfare:
                        if(this.cachedFanfareAudioClip == null)
                        {
                            this.cachedFanfareAudioClip = this.LoadResourceWrapper("Music/SFX/fanfare");
                        }
                        return this.cachedFanfareAudioClip;

                    case SFXType.MonsterGrowl:
                        if(this.cachedMonsterGrowlAudioClip == null)
                        {
                            this.cachedMonsterGrowlAudioClip = this.LoadResourceWrapper("Music/SFX/monstergrowl");
                        }
                        return this.cachedMonsterGrowlAudioClip;

                    case SFXType.MonsterTakeDamage:
                        if(this.cachedMonsterTakeDamageAudioClip == null)
                        {
                            this.cachedMonsterTakeDamageAudioClip = this.LoadResourceWrapper("Music/SFX/monsteroof");
                        }
                        return this.cachedMonsterTakeDamageAudioClip;

                    case SFXType.Negative:
                        if(this.cachedNegativeAudioClip == null)
                        {
                            this.cachedNegativeAudioClip = this.LoadResourceWrapper("Music/SFX/negative");
                        }
                        return this.cachedNegativeAudioClip;

                    case SFXType.Positive:
                        if(this.cachedPositiveAudioClip == null)
                        {
                            this.cachedPositiveAudioClip = this.LoadResourceWrapper("Music/SFX/positive");
                        }
                        return this.cachedPositiveAudioClip;

                    case SFXType.Smash:
                         if(this.cachedSmashAudioClip == null)
                        {
                            this.cachedSmashAudioClip = this.LoadResourceWrapper("Music/SFX/smash");
                        }
                        return this.cachedSmashAudioClip;

                    //No default case, because we compare to enum
                }

                throw new Exception("Could not find required AudioClip by SFX type");
            }
        }
        
        private UnityEngine.AudioClip LoadResourceWrapper(string location)
        {
            return UnityEngine.Resources.Load(location, typeof(UnityEngine.AudioClip)) as UnityEngine.AudioClip;
        }

        private AudioClipCache()
        {
        }

        private static readonly object padlock = new object();

        private static AudioClipCache instance = null;

        public static AudioClipCache Instance
        {
            get
            {
                lock(padlock)
                {
                    if(instance == null)
                    {
                        instance = new AudioClipCache();
                    }
                    
                    return instance;
                }
            }
        }
    }
}