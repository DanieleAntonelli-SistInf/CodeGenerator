namespace WebApp.Services;

public class ServizioContenenteHttpClient
{
    HttpClient http;
    ServizioInterno servizioInterno;

    public ServizioContenenteHttpClient(HttpClient http, ServizioInterno servizioInterno)
    {
        this.http = http;
        this.servizioInterno = servizioInterno;
    }

    public string GetString()
    {
        return servizioInterno.Hello();
    }
}
