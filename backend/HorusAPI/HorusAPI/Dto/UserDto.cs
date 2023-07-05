using HorusAPI.Entities;

namespace HorusAPI.Dto
{
    /// <summary>
    /// User account dto
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// The account name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// the account password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// the account email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// the account role
        /// </summary>
        public Role? Role { get; set; }
    }
}
