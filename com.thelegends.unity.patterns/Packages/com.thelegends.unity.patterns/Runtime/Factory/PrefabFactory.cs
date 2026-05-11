using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheLegends.Base.Factory
{
    /// <summary>
    /// A generic Unity-specific factory that instantiates GameObjects or MonoBehaviours from a given prefab.
    /// </summary>
    /// <typeparam name="T">The type of the Unity Object to instantiate (e.g., GameObject, MonoBehaviour).</typeparam>
    public class PrefabFactory<T> : IFactory<T> where T : Object
    {
        private readonly T _prefab;
        private readonly Transform _parent;

        /// <summary>
        /// Initializes a new instance of the PrefabFactory.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="parent">The optional parent transform for the instantiated object.</param>
        public PrefabFactory(T prefab, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
        }

        /// <summary>
        /// Instantiates the prefab.
        /// </summary>
        /// <returns>The newly created instance.</returns>
        public T Create()
        {
            if (_prefab == null)
            {
                Debug.LogError($"[PrefabFactory] Prefab of type {typeof(T).Name} is null!");
                return null;
            }

            T instance = _parent != null 
                ? Object.Instantiate(_prefab, _parent) 
                : Object.Instantiate(_prefab);

            return instance;
        }
    }
}
