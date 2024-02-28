using DocumentFormat.OpenXml.Spreadsheet;
using Google.Apis.Auth;
using Google.Apis.Util;
using JWTAuthentication.NET6._0.Auth;
using JWTAuthentication.NET6._0.Helpter;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Services;
using JWTAuthentication.NET6._0.Services.Contracts;
using MailKit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthentication.NET6._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        // get url: https://accounts.google.com/o/oauth2/v2/auth/oauthchooseaccount?response_type=code&client_id=1060492576360-2014e9qibks0v8kko62h0nh051mcboi6.apps.googleusercontent.com&redirect_uri=https://localhost:44368/api/Authenticate/loginGoogle&scope=openid%20profile%20email&service=lso&o2v=2&theme=glif&flowName=GeneralOAuthFlow
        [HttpGet]
        [Route("loginGoogle")]
        public IActionResult LoginGoogle()
        {
            // Thông tin cần thiết cho yêu cầu OAuth 2.0 đến Google
            var clientId = _configuration["Authentication:Google:ClientId"];
            var redirectUri = "https://localhost:44368/api/Authenticate/google-response";
            var scope = "openid profile email";

            // Tạo URL yêu cầu OAuth 2.0 đến Google
            var oauthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";

            // Chuyển hướng người dùng đến URL yêu cầu OAuth 2.0
            return Redirect(oauthUrl);
        }

        [HttpGet]
        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse([FromQuery] string code)
        {
            // get idToken from Google
            var client = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

            var redirectUri = "https://localhost:44368/api/Authenticate/google-response";
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];

            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            });

            var tokenResponse = await client.SendAsync(tokenRequest);
            var responseContent = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                // Handle error
                return BadRequest(responseContent);
            }

            var tokenObj = JObject.Parse(responseContent);
            var idToken = tokenObj.Value<string>("id_token");

            // Validate the Google token
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] }
            });

            // Create claims for the token
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, payload.Email),
                new Claim(ClaimTypes.Name, payload.Name),
                // Add any other claims you need
            };

            var token = GetToken(authClaims); // Generate JWT token

            // Return token to the client
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        // Oauthe2 login facebook
        [HttpGet]
        [Route("loginFacebook")]
        public IActionResult LoginFacebook()
        {
            // Thông tin cần thiết cho yêu cầu OAuth 2.0 đến Facebook
            var clientId = _configuration["Authentication:Facebook:ClientId"];
            var redirectUri = "https://localhost:44368/api/Authenticate/facebook-response";
            var scope = "email";

            // Tạo URL yêu cầu OAuth 2.0 đến Facebook
            var oauthUrl = $"https://www.facebook.com/v11.0/dialog/oauth?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";

            // Chuyển hướng người dùng đến URL yêu cầu OAuth 2.0
            return Redirect(oauthUrl);
        }
        [HttpGet]
        [Route("facebook-response")]
        public async Task<IActionResult> FacebookResponse([FromQuery] string code)
        {
            // get idToken from Facebook
            var client = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Get, "https://graph.facebook.com/v11.0/oauth/access_token");

            var redirectUri = "https://localhost:44368/api/Authenticate/facebook-response";
            var clientId = _configuration["Authentication:Facebook:ClientId"];
            var clientSecret = _configuration["Authentication:Facebook:ClientSecret"];

            tokenRequest.RequestUri = new Uri($"https://graph.facebook.com/v11.0/oauth/access_token?client_id={clientId}&redirect_uri={redirectUri}&client_secret={clientSecret}&code={code}");

            var tokenResponse = await client.SendAsync(tokenRequest);
            var responseContent = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                // Handle error
                return BadRequest(responseContent);
            }

            var tokenObj = JObject.Parse(responseContent);
            var accessToken = tokenObj.Value<string>("access_token");

            // Get user info from Facebook
            var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, "https://graph.facebook.com/me?fields=id,name,email");
            userInfoRequest.Headers.Add("Authorization", $"Bearer {accessToken}");

            var userInfoResponse = await client.SendAsync(userInfoRequest);
            var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                // Handle error
                return BadRequest(userInfoContent);
            }

            var userInfoObj = JObject.Parse(userInfoContent);

            // Create claims for the token
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userInfoObj.Value<string>("email")),
                new Claim(ClaimTypes.Name, userInfoObj.Value<string>("name")),
                // Add any other claims you need
            };

            var token = GetToken(authClaims); // Generate JWT token

            // Return token to the client
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            /*if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));*/

            // set role for user
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            // set role for admin
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpPost]
        [Route("send-mail-forgot-password")]
        public async Task<IActionResult> SendMailForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not found!" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            try
            {
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = model.Email;
                mailrequest.Subject = "Send mail forgot password";
                mailrequest.Body = _emailService.GetHtmlcontent(model.ClientURI + "?token=" + token);
                await (_emailService as SendMailForgotPass)?.SendEmailAsync(mailrequest);
                return Ok(new Response { Status = "Success", Message = "Reset password link has been sent to your email." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("reset-password")]
        /*[Authorize(Roles = $"{UserRoles.User}, {UserRoles.Admin}")]*/
        public async Task<IActionResult> ResetPassword([FromHeader(Name = "Authorization")] string authorizationHeader, [FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not found!" });

            var token = authorizationHeader?.Replace("Bearer ", "");

            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (!resetPassResult.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Reset password failed!" });

            return Ok(new Response { Status = "Success", Message = "Reset password successful!" });
        }
    }
}
