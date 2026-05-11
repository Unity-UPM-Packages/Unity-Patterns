namespace TheLegends.Base.Factory
{
    /// <summary>
    /// Represents a generic factory capable of creating instances of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    public interface IFactory<out T>
    {
        T Create();
    }

    /// <summary>
    /// Represents a generic factory capable of creating instances of type TResult with a specific TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter required for creation.</typeparam>
    /// <typeparam name="TResult">The type of the object to create.</typeparam>
    public interface IFactory<in TParam, out TResult>
    {
        TResult Create(TParam param);
    }
}
