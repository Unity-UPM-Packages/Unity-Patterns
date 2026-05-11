using System;
using UnityEngine.Pool;

namespace TheLegends.Base.Pool
{
    /// <summary>
    /// A generic object pool wrapper that utilizes UnityEngine.Pool.ObjectPool under the hood.
    /// Uses a delegate for object creation, keeping it completely decoupled from any specific Factory pattern.
    /// </summary>
    /// <typeparam name="T">The type of object to pool. Must be a class.</typeparam>
    public class GenericPool<T> where T : class
    {
        private readonly ObjectPool<T> _pool;
        private readonly Func<T> _createFunc;

        /// <summary>
        /// Gets the count of active items.
        /// </summary>
        public int CountActive => _pool.CountActive;

        /// <summary>
        /// Gets the count of inactive items waiting in the pool.
        /// </summary>
        public int CountInactive => _pool.CountInactive;

        /// <summary>
        /// Initializes a new instance of GenericPool.
        /// </summary>
        /// <param name="createFunc">A delegate that creates a new instance of T when the pool is empty.</param>
        /// <param name="collectionCheck">Collection checks will throw errors if you try to release an item that is already in the pool.</param>
        /// <param name="defaultCapacity">The default capacity the stack will be created with.</param>
        /// <param name="maxSize">The maximum size of the pool. When the pool reaches the max size then any further instances returned to the pool will be ignored and can be garbage collected.</param>
        public GenericPool(Func<T> createFunc, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _pool = new ObjectPool<T>(
                createFunc: CreateItem,
                actionOnGet: OnGetItem,
                actionOnRelease: OnReleaseItem,
                actionOnDestroy: OnDestroyItem,
                collectionCheck: collectionCheck,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        /// <summary>
        /// Gets an instance from the pool. If the pool is empty, a new instance is created.
        /// </summary>
        /// <returns>An instance of T.</returns>
        public virtual T Get()
        {
            return _pool.Get();
        }

        /// <summary>
        /// Returns an instance to the pool.
        /// </summary>
        /// <param name="item">The item to return.</param>
        public virtual void Release(T item)
        {
            _pool.Release(item);
        }

        /// <summary>
        /// Clears the pool, releasing all inactive instances.
        /// </summary>
        public virtual void Clear()
        {
            _pool.Clear();
        }

        protected virtual T CreateItem()
        {
            return _createFunc.Invoke();
        }

        protected virtual void OnGetItem(T item)
        {
            if (item is IPoolable poolable)
            {
                poolable.OnSpawn();
            }
        }

        protected virtual void OnReleaseItem(T item)
        {
            if (item is IPoolable poolable)
            {
                poolable.OnDespawn();
            }
        }

        protected virtual void OnDestroyItem(T item)
        {
            // Unity's ObjectPool destroys surplus items here. 
            // For pure C# classes, garbage collection handles this naturally.
        }
    }
}
