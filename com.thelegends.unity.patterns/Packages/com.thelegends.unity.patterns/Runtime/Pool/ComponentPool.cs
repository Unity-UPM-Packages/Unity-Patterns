using System;
using UnityEngine;

namespace TheLegends.Base.Pool
{
    /// <summary>
    /// A specialized pool for Unity Components. Automatically handles SetActive(true/false) 
    /// and safely destroys GameObjects when the pool exceeds its max size.
    /// </summary>
    /// <typeparam name="T">The type of the Component to pool.</typeparam>
    public class ComponentPool<T> : GenericPool<T> where T : Component
    {
        /// <summary>
        /// Initializes a pool using a custom creation delegate.
        /// </summary>
        public ComponentPool(Func<T> createFunc, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
            : base(createFunc, collectionCheck, defaultCapacity, maxSize)
        {
        }

        /// <summary>
        /// Initializes a pool directly from a prefab. Automatically handles instantiation.
        /// </summary>
        public ComponentPool(T prefab, Transform parent = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
            : base(() => parent != null ? UnityEngine.Object.Instantiate(prefab, parent) : UnityEngine.Object.Instantiate(prefab), collectionCheck, defaultCapacity, maxSize)
        {
        }

        protected override void OnGetItem(T item)
        {
            if (item != null && item.gameObject != null)
            {
                item.gameObject.SetActive(true);
            }

            // Still call IPoolable.OnSpawn if applicable
            base.OnGetItem(item);
        }

        protected override void OnReleaseItem(T item)
        {
            // Still call IPoolable.OnDespawn if applicable
            base.OnReleaseItem(item);

            if (item != null && item.gameObject != null)
            {
                item.gameObject.SetActive(false);
            }
        }

        protected override void OnDestroyItem(T item)
        {
            base.OnDestroyItem(item);

            if (item != null && item.gameObject != null)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
    }
}
