using HorusAPI.Database;
using HorusAPI.Dto;
using HorusAPI.Entities;
using HorusAPI.Exceptions;
using HorusAPI.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;

namespace HorusAPI.Service.Implementations
{
    /// <summary>
    /// Default User Service
    /// </summary>
    public class UserService : IUserService
    {
        private ICrud<User> userRepository;

        /// <summary>
        /// Default Constructor for UserService
        /// </summary>
        /// <param name="userRepository">The user repository to use</param>
        public UserService(ICrud<User> userRepository) {
            this.userRepository = userRepository;
        }

        public long Count(UserDto search)
        {
            var hints = new Builder<User>()
                .With(search.Name, nameof(User.Name))
                .With(search.Email, nameof(User.Email))
                .With(search.Role, nameof(User.Role))
            .Build();

            return userRepository.Count(hints);
        }

        public async Task<string> Create(UserDto user, string issuer)
        {
            if(user.Password == null || user.Email == null || user.Name == null)
            {
                throw new ArgumentException("Elements needed for creation were null!");
            }

            var hints = new Builder<User>()
                .With(issuer, nameof(User.Name))
                .Build();
            var issuingUser = await userRepository.Get(hints);

            if(issuingUser == null)
            {
                throw new NotFoundException("Issuing user not found!");
            }

            if(issuingUser.Role == null || issuingUser.Role != Role.Admin)
            {
                throw new PermissionException("Issuer role not correct!");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashed = hash(user.Password, salt);

            var userData = new Builder<User>()
                .With(hashed, nameof(User.Password))
                .With(user.Name, nameof(User.Name))
                .With(user.Email, nameof(User.Email))
                .With(salt, nameof(User.Salt))
                .With(user.Role, nameof(User.Role))
                .Build();

            await userRepository.Create(userData);
            return user.Name;
        }

        public async Task Delete(string username, string issuer)
        {
            var hints = new Builder<User>()
                .With(issuer, nameof(User.Name))
                .Build();
            var issuingUser = await userRepository.Get(hints);

            if (issuingUser == null)
            {
                throw new ArgumentException("Issuing user not found!");
            }

            if (issuingUser.Role == null || issuingUser.Role != Role.Admin)
            {
                throw new PermissionException("Issuer role not correct!");
            }

            hints = new Builder<User>()
                .With(username, nameof(User.Name))
                .Build();
            var delete = await userRepository.Get(hints);

            if(delete != null)
            {
                await userRepository.Delete(delete);
            }
        }

        public async Task<UserDto> Get(string username)
        {
            var hints = new Builder<User>()
                .With(username, nameof(User.Name))
                .Build();
            var user = await userRepository.Get(hints);

            if(user == null)
            {
                throw new NotFoundException("User not found!");
            }

            return new Builder<UserDto>()
                .With(user.Role, nameof(UserDto.Role))
                .With(user.Email, nameof(UserDto.Email))
                .With(user.Name, nameof(UserDto.Name))
                .Build();
        }

        public async Task<IEnumerable<UserDto>> Get(int page, int size, UserDto search)
        {
            var hints = new Builder<User>()
                .With(search.Name, nameof(User.Name))
                .With(search.Email, nameof(User.Email))
                .With(search.Role, nameof(User.Role))
                .Build();

            var users = await userRepository.Get(size, page, hints);

            var res = new List<UserDto>();

            foreach(var user in users)
            {
                res.Add(new Builder<UserDto>()
                        .With(user.Role, nameof(UserDto.Role))
                        .With(user.Name, nameof(UserDto.Name))
                        .With(user.Email, nameof(UserDto.Email))
                        .Build()
                        );
            }

            return res;
        }

        public async Task<bool> Login(LoginDto loginDto)
        {
            if (loginDto.Password == null || loginDto.Name == null)
            {
                throw new ArgumentException("The password or name was null");
            }

            var hints = new User();
            hints.Name = loginDto.Name;

            var found = await userRepository.Get(hints);

            return found.Password != null && found.Salt != null && (found.Password == hash(loginDto.Password, found.Salt));
        }

        public async Task Update(UserDto updated, string username, string issuer)
        {
            var hints = new Builder<User>()
                .With(issuer, nameof(User.Name))
                .Build();
            var issuingUser = await userRepository.Get(hints);

            if (issuingUser == null)
            {
                throw new ArgumentException("Issuing user not found!");
            }

            if (issuingUser.Role == null || issuingUser.Role != Role.Admin)
            {
                throw new PermissionException("Issuer role not correct!");
            }

            if(updated.Password == null)
            {
                throw new ArgumentException("Updated Password was null");
            }

            hints = new Builder<User>()
                .With(username, nameof(User.Name))
                .Build();
            var user = await userRepository.Get(hints);

            if(user == null || user.Id == null)
            {
                throw new NotFoundException("Could not find user to update!");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashed = hash(updated.Password, salt);

            var userData = new Builder<User>()
                .With(hashed, nameof(User.Password))
                .With(updated.Name, nameof(User.Name))
                .With(updated.Email, nameof(User.Email))
                .With(salt, nameof(User.Salt))
                .With(updated.Role, nameof(User.Role))
                .Build();

            await userRepository.Update(user.Id, userData);
        }

        /// <summary>
        /// Salts and hashes the password
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="salt">The salt associated with the password</param>
        /// <returns>Returns the hash</returns>
        private string hash(string password, string salt)
        {
            try
            {
                return BCrypt.Net.BCrypt.HashPassword(password, salt);
            }
            catch (Exception e)
            {
                throw new InternalServerException("An exception occured during hashing", e);
            }
        }
    }
}
