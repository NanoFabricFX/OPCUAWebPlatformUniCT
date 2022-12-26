using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPlatform.Auth;
using WebPlatform.Models.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebPlatform.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private IAuth _auth;
        private ITokenManager _jwtManager;

        public AuthController(ITokenManager jwtManager, IAuth auth) 
        {
            _jwtManager = jwtManager;
            _auth = auth;
        }

        // GET: api/values
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Get([FromForm]AuthCredentials authCreds)
        {
            if (!authCreds.isValid) { return BadRequest("Inserisci username e password"); }

            if(_auth.AuthenticateWithCredentials(authCreds.Username, authCreds.Password))
            {
                
                return Ok(new { token = _jwtManager.GenerateTokenForUser(authCreds.Username) });
            }

            return Unauthorized();
        }

    }
}
