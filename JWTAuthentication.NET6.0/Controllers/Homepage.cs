using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication.NET6._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =UserRoles.User)]
    public class Homepage : ControllerBase
    {
        [HttpGet]
        public string Index() {

            return "đây là trang user";
        }
    }
}
