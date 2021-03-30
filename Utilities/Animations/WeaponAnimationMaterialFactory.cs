using System.Collections.Generic;

namespace FindTheIdol.Utilities.Animations
{
    public sealed class WeaponAnimationMaterialFactory
    {
        private IDictionary<WeaponAnimationMaterialIndex, UnityEngine.Material> cachedMaterials;

        public UnityEngine.Material GetOrCreateWeaponAnimationMaterial(WeaponAnimationMaterialIndex weaponAnimationMaterialIndex)
        {
            lock(padlock)
            {
                if(this.cachedMaterials == null)
                {
                    this.cachedMaterials = new Dictionary<WeaponAnimationMaterialIndex, UnityEngine.Material>();
                }

                if(!this.cachedMaterials.ContainsKey(weaponAnimationMaterialIndex))
                {
                    switch(weaponAnimationMaterialIndex)
                    {
                        case WeaponAnimationMaterialIndex.RangedAttackMaterial:
                            this.cachedMaterials.Add(weaponAnimationMaterialIndex, UnityEngine.Resources.Load("Meshes/Weapons/Consumables/Arrow.fbm/WeaponHandleTexture", typeof(UnityEngine.Material)) as UnityEngine.Material);
                            break;

                        case WeaponAnimationMaterialIndex.MagicAttackMaterial:
                            this.cachedMaterials.Add(weaponAnimationMaterialIndex, UnityEngine.Resources.Load("Material/Weapons/MagicWeapons/Attack", typeof(UnityEngine.Material)) as UnityEngine.Material);
                            break;

                        //No default case because we compare with to a enum
                    }
                }

                return this.cachedMaterials[weaponAnimationMaterialIndex];
            }
        }

        private WeaponAnimationMaterialFactory()
        {
        }

        private static readonly object padlock = new object();

        private static WeaponAnimationMaterialFactory instance = null;

        public static WeaponAnimationMaterialFactory Instance
        {
            get
            {
                lock(padlock)
                {
                    if(instance == null)
                    {
                        instance = new WeaponAnimationMaterialFactory();
                    }
                    
                    return instance;
                }
            }
        }
    }
}