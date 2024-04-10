using Microsoft.AspNetCore.Mvc;

namespace IntroSEProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        [HttpPost("/checkout")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
