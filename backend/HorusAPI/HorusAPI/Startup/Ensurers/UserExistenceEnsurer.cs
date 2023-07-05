using HorusAPI.Database;
using HorusAPI.Entities;
using HorusAPI.Helpers;

namespace HorusAPI.Startup.Ensurers
{
    /// <summary>
    /// Ensures the existence of at least one account on app startup. If no account is present, a default account is inserted
    /// </summary>
    public class UserExistenceEnsurer : IEnsurer
    {
        public void Ensure(WebApplication application)
        {
            var userDb = application.Services.GetService(typeof(ICrud<User>));

            if (userDb == null)
            {
                throw new Exception("Cannot ensure user existence!");
            }

            var userCrud = (ICrud<User>)userDb;

            if(userCrud.Count() == 0)
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var pw = BCrypt.Net.BCrypt.HashPassword("admin", salt);

                var defaultUser = new Builder<User>()
                    .With("admin", nameof(User.Name))
                    .With("admin@example.com", nameof(User.Email))
                    .With(Role.Admin, nameof(User.Role))
                    .With(pw, nameof(User.Password))
                    .With(salt, nameof(User.Salt))
                    .Build();

                userCrud.Create(defaultUser);
            }
        }
    }
}
