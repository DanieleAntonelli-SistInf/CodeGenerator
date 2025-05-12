using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Text;
using WebApp.Controllers;
using WebApp.Services;

namespace Test;

public class Tests
{
    private List<(Type ServiceType, Type? ImplType, ServiceLifetime Lifetime)>
        _descriptors = new()
        {
            (typeof(TestController), null, ServiceLifetime.Transient),
            (typeof(IGoodbyeService), typeof(GoodbyeService), ServiceLifetime.Scoped)
        };

    [TestCase]
    public void RegistrationValidation()
    {
        var app = new WebApplicationFactory<IApiMarker>();
        app = app
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(serviceCollection =>
                {
                    var services = serviceCollection.ToList();
                    var result = ValidateServices(services);
                    if (!result.Success)
                    {
                        Assert.Fail(result.Message);
                    }
                    Assert.Pass();
                })
            );
        app.CreateClient();
    }

    private DependencyAssertionResult ValidateServices(List<ServiceDescriptor> services)
    {
        var searchFailed = false;
        var failedText = new StringBuilder();
        foreach (var descriptor in _descriptors)
        {
            var match = services.SingleOrDefault(x =>
                x.ServiceType == descriptor.ServiceType &&
                x.ImplementationType == descriptor.ImplType &&
                x.Lifetime == descriptor.Lifetime
            );

            if (match != null)
            {
                continue;
            }
            if (!searchFailed)
            {
                failedText.AppendLine("Failed to find registered service for:");
                searchFailed = true;
            }

            failedText.AppendLine($"{descriptor.ServiceType.Name}|{descriptor.ImplType?.Name}|{descriptor.Lifetime}");
        }

        return new DependencyAssertionResult
        {
            Success = !searchFailed,
            Message = failedText.ToString()
        };
    }

    private sealed class DependencyAssertionResult
    {
        public required bool Success { get; init; }
        public required string Message { get; init; }
    }
}