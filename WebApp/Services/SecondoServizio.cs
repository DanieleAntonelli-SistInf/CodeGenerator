namespace WebApp.Services;

public class SecondoServizio
{
    HttpClient http;
    ServizioInterno servizioInterno;

    public SecondoServizio(HttpClient http, ServizioInterno servizioInterno)
    {
        this.http = http;
        this.servizioInterno = servizioInterno;
    }
}
