using HorusAPI.Dto;
using HorusAPI.Exceptions;

namespace HorusAPI.Service
{
    /// <summary>
    /// This service is used in conjunction with users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Checks whether the login details match
        /// </summary>
        /// <param name="loginDto">The login data</param>
        /// <returns>Returns true if the passwords match</returns>
        /// <exception cref="ArgumentException">Thrown if the login data does not contain all required data</exception>
        /// <exception cref="NotFoundException">Thrown if the user does not exist</exception>
        public Task<bool> Login(LoginDto loginDto);

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="user">The user data to create</param>
        /// <param name="issuer">The issuing user</param>
        /// <returns>Returns the new user's name</returns>
        /// <exception cref="ArgumentException">Thrown if the user does not contain all required data</exception>
        /// <exception cref="PermissionException">Thrown if the issuer does not have the correct permissions</exception>
        public Task<string> Create(UserDto user, string issuer);

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="updated">The user data to update</param>
        /// <param name="issuer">The issuing user</param>
        /// <param name="id">The id of the user to update</param>
        /// <exception cref="ArgumentException">Thrown if the user does not contain all required data</exception>
        /// <exception cref="NotFoundException">Thrown if the user does not exist</exception>
        /// <exception cref="PermissionException">Thrown if the issuer does not have the correct permissions</exception>
        public Task Update(UserDto updated, string id, string issuer);

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="username">The user to delete</param>
        /// <param name="issuer">The issuing user</param>
        /// <exception cref="NotFoundException">Thrown if the user does not exist</exception>
        /// <exception cref="PermissionException">Thrown if the issuer does not have the correct permissions</exception>
        public Task Delete(string username, string issuer);

        /// <summary>
        /// Get a user's data
        /// </summary>
        /// <param name="username">The user to query</param>
        /// <exception cref="NotFoundException">Thrown if the user does not exist</exception>
        public Task<UserDto> Get(string username);
        
        /// <summary>
        /// Get users
        /// </summary>
        /// <param name="page">the page to get the useres at</param>
        /// <param name="size">the number of users to get</param>
        /// <param name="search">the search params</param>
        /// <returns>Returns a list of users conforming to the batch size and the page matching the search params</returns>
        public Task<IEnumerable<UserDto>> Get(int page, int size, UserDto search);

        /// <summary>
        /// Get the number of users matching the search
        /// </summary>
        /// <param name="search">the search params</param>
        public long Count(UserDto search);
    }
}
