using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly IGoodbyeService _goodbyeService;

    public TestController(
        HttpClient client,
        IGoodbyeService goodbyeService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _goodbyeService = goodbyeService ?? throw new ArgumentNullException(nameof(goodbyeService));

        var list = new List<int> { 1 };
        var first = list.First();
    }

    [HttpGet]
    public string Hello()
    {
        return "Hello World!";
    }

    [HttpGet("Goodbye")]
    public string Goodbte()
    {
        return _goodbyeService.Goodbye();
    }

    [HttpGet("DIV")]
    public string DependencyInjectorValidator()
    {
        return Type.GetType("WebApp.Controllers.TestController").ToString();
    }
}
