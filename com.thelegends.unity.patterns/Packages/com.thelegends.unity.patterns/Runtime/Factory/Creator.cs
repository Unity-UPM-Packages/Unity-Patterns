namespace TheLegends.Base.Factory
{
    /// <summary>
    /// The base Creator class for the Factory Method pattern (Gang of Four).
    /// Subclasses must implement the FactoryMethod to return a specific product.
    /// </summary>
    /// <typeparam name="TProduct">The type of the product being created.</typeparam>
    public abstract class Creator<TProduct>
    {
        /// <summary>
        /// The Factory Method that must be implemented by concrete creators to instantiate products.
        /// </summary>
        /// <returns>A new instance of TProduct.</returns>
        protected abstract TProduct FactoryMethod();

        /// <summary>
        /// Gets the product by invoking the FactoryMethod.
        /// This can be expanded to include core business logic before or after product creation.
        /// </summary>
        /// <returns>The created product.</returns>
        public TProduct GetProduct()
        {
            TProduct product = FactoryMethod();
            
            // Core logic or centralized setup could be placed here if needed in the future.
            
            return product;
        }
    }
}
