using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GraphDatabaseSandbox
{

    [Route("")]
    public class ExampleController : Controller
    {
        // 
        // GET: /HelloWorld/

        public async Task<IActionResult> Index() => Ok(new Something("Mooo", "More info"));

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
