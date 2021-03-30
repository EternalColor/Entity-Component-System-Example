using FindTheIdol.Components.Deletion;
using FindTheIdol.Components.Music;
using FindTheIdol.Utilities.Music;
using Unity.Burst;
using Unity.Entities;

namespace FindTheIdol.Systems.Music.SFX
{
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))] 
    public class PlaySoundEffectsSystem : ComponentSystem
    {
        private UnityEngine.AudioSource audioSourceGameObject;

        protected override void OnCreate()
        {
            //Assuming there is only 1 audio source in the whole game
            this.audioSourceGameObject = UnityEngine.Object.FindObjectOfType<UnityEngine.AudioSource>();
        }

        protected override void OnUpdate()
        {
            //Prevent this from being null when scene is loaded from another scene (Something about the lifecycle of ECS not behaving well with normal gameobject instantiating -> i can not always guarantee that oncreate receives the gameobject)
            if(this.audioSourceGameObject == null)
            {
                this.audioSourceGameObject = UnityEngine.Object.FindObjectOfType<UnityEngine.AudioSource>();
            }

            this.Entities.ForEach((Entity entity, ref SFXComponent sfxComponent) => 
            {
                UnityEngine.AudioClip clipToPlay = AudioClipCache.Instance.GetOrCreateSFX(sfxComponent.SFXType);
                
                this.audioSourceGameObject.PlayOneShot(clipToPlay);

                this.PostUpdateCommands.AddComponent<DeletionTag>(entity);
            });
        }
    }
}
