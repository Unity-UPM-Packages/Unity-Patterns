namespace TheLegends.Base.Pool
{
    /// <summary>
    /// Interface for objects that are managed by an object pool.
    /// Provides lifecycle methods for when an object is retrieved from and returned to the pool.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called automatically when the object is retrieved from the pool.
        /// Use this to initialize or reset the object's state.
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// Called automatically when the object is returned to the pool.
        /// Use this to clean up or clear the object's state before it goes dormant.
        /// </summary>
        void OnDespawn();
    }
}
