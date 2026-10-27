using Microsoft.AspNetCore.Mvc;

namespace DonationTask.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "Healthy" });
    }
}