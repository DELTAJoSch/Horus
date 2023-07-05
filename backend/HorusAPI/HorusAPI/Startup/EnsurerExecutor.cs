using System.Reflection;

namespace HorusAPI.Startup
{
    /// <summary>
    /// Executes all Ensurers
    /// </summary>
    public static class EnsurerExecutor
    {
        /// <summary>
        /// Execute ensurers of this application
        /// </summary>
        /// <param name="app">The app to ensure for</param>
        public static void Ensure(WebApplication app)
        {
            // Get all ensurers
            var ensurers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IEnsurer).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract);

            foreach(var ensurer in ensurers)
            {
                if(ensurer == null) continue;

                var activated = (IEnsurer?)Activator.CreateInstance(ensurer);

                if(activated != null)
                {
                    activated.Ensure(app);
                }
            }

        }
    }
}
