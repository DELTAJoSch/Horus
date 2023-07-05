namespace HorusAPI.Startup
{
    /// <summary>
    /// Specifies an ensurers external view
    /// </summary>
    public interface IEnsurer
    {
        /// <summary>
        /// Ensure the property of the ensurer
        /// </summary>
        /// <param name="application">The application builder to ensure for</param>
        public void Ensure(WebApplication application);
    }
}
