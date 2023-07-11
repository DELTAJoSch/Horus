using HorusAPI.Dto;
using HorusAPI.Entities;
using HorusAPI.Exceptions;
using HorusAPI.Helpers;
using HorusAPI.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Net.Mime;
using System.Security.Claims;

namespace HorusAPI.Controllers
{
    /// <summary>
    /// User Endpoint
    /// </summary>
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService userService;

        /// <summary>
        /// Default constructor for user controller
        /// </summary>
        /// <param name="userService">the user service to use</param>
        public UserController(IUserService userService) 
        {
            this.userService = userService;
        }

        /// <summary>
        /// Get the users at the selected page and with the selected batch size
        /// </summary>
        /// <param name="page">the page to get the users at</param>
        /// <param name="size">the number of users to get</param>
        /// <param name="email">email to search for</param>
        /// <param name="name">name to search for</param>
        /// <param name="role">role to search for</param>
        /// <returns>Returns a list of user dtos</returns>
        [HttpGet("users")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int page = 0, [FromQuery] int size = 25, [FromQuery] string? name = null, [FromQuery] string? email = null, [FromQuery] Role? role = null)
        {
            var search = new Builder<UserDto>()
                    .With(name, nameof(UserDto.Name))
                    .With(email, nameof(UserDto.Email))
                    .With(role, nameof(UserDto.Role))
                    .Build();
            var count = userService.Count(search);
            HttpContext.Response.Headers.Add("X-Search-Matches", count.ToString());

            var res = await userService.Get(page, size, search);
            return Ok(res);
        }

        /// <summary>
        /// Get a single user
        /// </summary>
        /// <param name="username">the user to get</param>
        /// <returns>Returns the user</returns>
        [HttpGet("users/{username}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string username)
        {
            try
            {
                var res = await userService.Get(username);
                return Ok(res);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Returns the currently logged in account or a 401 Unauthorized if not logged in
        /// </summary>
        /// <returns>Returns the currently logged in account</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLoggedInAccount()
        {
            try
            {
                var identity = HttpContext.User.Identity;
                if (identity == null || identity.Name == null)
                {
                    return Unauthorized("User not logged in!");
                }

                var user = await userService.Get(identity.Name);
                return Ok(user);
            }
            catch (NotFoundException e)
            {
                return NotFound("The user does not exist any more");
            }
        }

        /// <summary>
        /// Login to the service
        /// </summary>
        /// <returns>Returns Ok if successful, otherwise an error is returned</returns>
        [HttpPost("session")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var ok = await userService.Login(login);

                if (ok)
                {
                    var user = await userService.Get(login.Name!);

                    if (user.Email == null || user.Role == null)
                    {
                        throw new InternalServerException("Data is inconsitent - missing role or email");
                    }

                    var claims = new List<Claim>
                {
                    new Claim(type: ClaimTypes.Role, value: user.Role.Value.ToString()),
                    new Claim(type: ClaimTypes.Name, value: login.Name!),
                    new Claim(type: ClaimTypes.Email, value: user.Email)
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14),
                        });

                    return Ok();
                }

                return Unauthorized("Wrong login data");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionException e)
            {
                return Unauthorized(e.Message);
            }
        }

        /// <summary>
        /// Logout from the current session
        /// </summary>
        /// <returns>Returns 200 OK</returns>
        [HttpGet("session")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// create a new user
        /// </summary>
        /// <param name="user">the user data</param>
        [HttpPost("users")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] UserDto user)
        {
            try
            {
                var identity = HttpContext.User.Identity;
                if (identity == null || identity.Name == null)
                {
                    return Unauthorized("Identity not known");
                }

                var name = await userService.Create(user, identity.Name);
                return Ok(name);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (PermissionException e)
            {
                return Unauthorized(e.Message);
            }
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="username">the user to update</param>
        /// <param name="user">the new user data</param>
        [HttpPatch("users/{username}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(string username, [FromBody] UserDto user)
        {
            try
            {
                var identity = HttpContext.User.Identity;
                if (identity == null || identity.Name == null)
                {
                    return Unauthorized("Identity not known");
                }

                await userService.Update(user, username, identity.Name);
                return Ok(user.Name);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionException e)
            {
                return Unauthorized(e.Message);
            }
        }

        /// <summary>
        /// Delete the specified user
        /// </summary>
        /// <param name="username">the user to delete's id</param>
        [HttpDelete("users/{username}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(string username)
        {
            try
            {
                var identity = HttpContext.User.Identity;
                if(identity == null || identity.Name == null)
                {
                    return Unauthorized("Identity not known");
                }

                await userService.Delete(username, identity.Name);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionException e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}
