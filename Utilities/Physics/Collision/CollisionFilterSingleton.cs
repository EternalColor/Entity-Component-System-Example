using FindTheIdol.Utilities.LayerMask;
using Unity.Physics;

namespace FindTheIdol.Utilities.Physics.Collision
{
    public class CollisionFilterSingleton
    {
        private CollisionFilter belongsToNonTraversableFilter;

        private CollisionFilter collidesWithPlayerFilter;

        private CollisionFilter belongsToPlayerFilter;

        private CollisionFilter belongsToMonsterFilter;

        private CollisionFilter belongsToWeaponFilter;

        public CollisionFilter BelongsToNonTraversableFilter
        {
            get
            {
                lock(padlock)
                {
                    //Strangely enough struct will be initialized as CollisionFilter.Zero not CollisionFilter.Default as expected.
                    if(this.belongsToNonTraversableFilter.Equals(CollisionFilter.Zero))
                    {
                        this.belongsToNonTraversableFilter = new CollisionFilter      
                        {
                            BelongsTo = LayerMaskWrapper.NonTraversableMask,
                            CollidesWith = LayerMaskWrapper.PathfindingGridMask | LayerMaskWrapper.PlayerMask,
                            GroupIndex = 0
                        };
                    }

                    return this.belongsToNonTraversableFilter;
                }
            }
        }

        public CollisionFilter CollidesWithPlayerFilter
        {
            get
            {
                lock(padlock)
                {
                    //Strangely enough struct will be initialized as CollisionFilter.Zero not CollisionFilter.Default as expected.
                    if(this.collidesWithPlayerFilter.Equals(CollisionFilter.Zero))
                    {
                        this.collidesWithPlayerFilter = new CollisionFilter      
                        {
                            BelongsTo = LayerMaskWrapper.NonTraversableMask | LayerMaskWrapper.ItemsMask | LayerMaskWrapper.AltarMask,
                            CollidesWith = LayerMaskWrapper.PlayerMask,
                            GroupIndex = 0
                        };
                    }

                    return this.collidesWithPlayerFilter;
                }
            }
        }
      
        public CollisionFilter BelongsToPlayerFilter
        {
            get
            {
                lock(padlock)
                {
                    //Strangely enough struct will be initialized as CollisionFilter.Zero not CollisionFilter.Default as expected.
                    if(this.belongsToPlayerFilter.Equals(CollisionFilter.Zero))
                    {
                        this.belongsToPlayerFilter = new CollisionFilter      
                        {
                            BelongsTo = LayerMaskWrapper.PlayerMask,
                            CollidesWith = LayerMaskWrapper.NonTraversableMask | LayerMaskWrapper.ItemsMask | LayerMaskWrapper.AltarMask,
                            GroupIndex = 0
                        };
                    }

                    return this.belongsToPlayerFilter;
                }
            }
        }

        public CollisionFilter BelongsToMonsterFilter
        {
            get
            {
                lock(padlock)
                {
                    //Strangely enough struct will be initialized as CollisionFilter.Zero not CollisionFilter.Default as expected.
                    if(this.belongsToMonsterFilter.Equals(CollisionFilter.Zero))
                    {
                        this.belongsToMonsterFilter = new CollisionFilter      
                        {
                            BelongsTo = LayerMaskWrapper.Monster,
                            CollidesWith = LayerMaskWrapper.Weapon | LayerMaskWrapper.NonTraversableMask,
                            GroupIndex = 0
                        };
                    }

                    return this.belongsToMonsterFilter;
                }
            }
        }

        public CollisionFilter BelongsToWeaponFilter
        {
            get
            {
                lock(padlock)
                {
                    //Strangely enough struct will be initialized as CollisionFilter.Zero not CollisionFilter.Default as expected.
                    if(this.belongsToWeaponFilter.Equals(CollisionFilter.Zero))
                    {
                        this.belongsToWeaponFilter = new CollisionFilter      
                        {
                            BelongsTo = LayerMaskWrapper.Weapon,
                            CollidesWith = LayerMaskWrapper.Monster,
                            GroupIndex = 0
                        };
                    }

                    return this.belongsToWeaponFilter;
                }
            }
        }

        private CollisionFilterSingleton()
        {
        }

        private static readonly object padlock = new object();

        private static CollisionFilterSingleton instance = null;

        public static CollisionFilterSingleton Instance
        {
            get
            {
                lock(padlock)
                {
                    if(instance == null)
                    {
                        instance = new CollisionFilterSingleton();
                    }
                    
                    return instance;
                }
            }
        }
    }
}
