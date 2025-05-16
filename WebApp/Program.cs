using WebApp.Services;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddControllersAsServices();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient<ServizioContenenteHttpClient>();
        //builder.Services.AddScoped<SecondoServizio>();
        //builder.Services.AddScoped<ServizioInterno>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        // Verifica che tutti i controller possano essere creati
        if (app.Environment.IsDevelopment())
        {
            SampleSourceGenerator.DependencyInjectionValidationResult result = SampleSourceGenerator.DependencyInjectorValidator.Validate(app);
            if (result.IsError)
            {
                throw new InvalidOperationException(result.Message);
            }
        }

        app.Run();
    }
}
