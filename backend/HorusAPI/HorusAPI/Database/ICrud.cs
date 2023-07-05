namespace HorusAPI.Database
{
    using HorusAPI.Exceptions;

    /// <summary>
    /// Simple Crud interface for repositories
    /// </summary>
    /// <typeparam name="T">The entity type of the repository</typeparam>
    public interface ICrud<T>
    {
        /// <summary>
        /// Get at max batchSize elements from the repository at page pageOffset
        /// </summary>
        /// <param name="batchSize">The max number of elements to return</param>
        /// <param name="pageOffset">the page offset to look at</param>
        /// <returns>Returns an enumerable of elements</returns>
        public Task<IEnumerable<T>> Get(int batchSize, int pageOffset);

        /// <summary>
        /// Get at max batchSize elements from the repository at page pageOffset.
        /// Only return elements confirming to the non-null fields of hints
        /// </summary>
        /// <param name="batchSize">The max number of elements to return</param>
        /// <param name="pageOffset">the page offset to look at</param>
        /// <returns>Returns an enumerable of elements</returns>
        public Task<IEnumerable<T>> Get(int batchSize, int pageOffset, T hints);

        /// <summary>
        /// Get the specified element by id
        /// </summary>
        /// <param name="id">the id of the element</param>
        /// <returns>Returns the element. Should throw a NotFoundException if no matching element exists!</returns>
        /// <exception cref="NotFoundException">Should be thrown if no element could be found</exception>
        public Task<T> Get(string id);

        /// <summary>
        /// Get the specified element by hints.
        /// </summary>
        /// <param name="hints">the hints of the element</param>
        /// <returns>Returns the first matching element. Should throw a NotFoundException if no matching element exists!</returns>
        /// <exception cref="NotFoundException">Should be thrown if no element could be found</exception>
        public Task<T> Get(T hints);

        /// <summary>
        /// Updates the element specified by the id
        /// </summary>
        /// <param name="id">The id of the element</param>
        /// <param name="updated">the new values</param>
        /// <exception cref="NotFoundException">Should be thrown if no element could be found</exception>
        public Task Update(string id, T updated);

        /// <summary>
        /// Delete the specified element
        /// </summary>
        /// <param name="id">The if of the element to delete</param>
        /// <exception cref="NotFoundException">Should be thrown if no element could be found</exception>
        public Task Delete(string id);

        /// <summary>
        /// Delete the specified element
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <exception cref="NotFoundException">Should be thrown if no element could be found</exception>
        public Task Delete(T entity);

        /// <summary>
        /// Create the specified element.
        /// </summary>
        /// <param name="create">The element to create</param>
        /// <returns>Returns the id of the created element</returns>
        public Task<string> Create(T create);

        /// <summary>
        /// Gets the number of elements in the repository
        /// </summary>
        /// <returns>The number of elements in the repository</returns>
        public long Count();

        /// <summary>
        /// Gets the number of elements matching the hints in the repository
        /// </summary>
        /// <param name="hints">The hints to look for</param>
        /// <returns>Returns the number of elements</returns>
        public long Count(T hints);
    }
}
