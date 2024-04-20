using Microsoft.AspNetCore.Mvc;

namespace Layer.Presentation.Controllers
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
