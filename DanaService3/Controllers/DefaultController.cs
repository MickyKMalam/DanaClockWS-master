using System.Web.Http;

namespace DanaService3.Controllers
{
    public class DefaultController : ApiController
    {
        // GET api/default
        [Route("api/default")]
        [HttpGet]
        public string Default()
        {
            return "Dana Clock API ver 1.12 25/02/2020";
        }
    }
}
