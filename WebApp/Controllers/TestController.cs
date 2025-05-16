using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ServizioContenenteHttpClient servizioContenenteHttpClient;

    public TestController(ServizioContenenteHttpClient secondoServizio)
    {
        this.servizioContenenteHttpClient = secondoServizio;
    }

    [HttpGet]
    public string Test()
    {
        return servizioContenenteHttpClient.GetString();
    }
}
