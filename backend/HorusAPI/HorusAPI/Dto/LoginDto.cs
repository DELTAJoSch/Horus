namespace HorusAPI.Dto
{
    /// <summary>
    /// The account login info
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// The account name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// the account password
        /// </summary>
        public string? Password { get; set; }
    }
}
