using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class SecondoController : ControllerBase
{
    private readonly SecondoServizio secondoServizio;

    public SecondoController(SecondoServizio secondoServizio)
    {
        this.secondoServizio = secondoServizio;
    }

    [HttpGet]
    public string Test()
    {
        return secondoServizio.GetString();
    }
}
