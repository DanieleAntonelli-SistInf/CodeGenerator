using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ServizioContenenteHttpClient secondoServizio;

    public TestController(ServizioContenenteHttpClient secondoServizio)
    {
        this.secondoServizio = secondoServizio;
    }

    [HttpGet]
    public string Test()
    {
        return secondoServizio.GetString();
    }
}
