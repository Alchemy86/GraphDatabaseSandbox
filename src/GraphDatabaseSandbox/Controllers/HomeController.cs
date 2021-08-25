using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GraphDatabaseSandbox
{

    [Route("")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index() => Ok(new Something("Mooo", "More info"));

        [HttpGet("private")]
        public IActionResult Private() {
            var user = new { User.Identity.Name, User.Identity.IsAuthenticated, User.Identity.AuthenticationType };
            return Ok(user);
        }

        private class Something {
            public string Name { get ;}
            public string MegaDetails {get; }
            public Something(string name, string details){
                this.Name = name;
                this.MegaDetails = details;
            }
        }
    }
}
