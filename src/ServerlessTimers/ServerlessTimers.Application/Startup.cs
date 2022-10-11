
[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection
    .FunctionsStartup(typeof(ServerlessTimers.Application.Startup))]
namespace ServerlessTimers.Application;

using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.Options;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerlessTimers.Domain.Aggregators.Timers;
using ServerlessTimers.Infrastructure.Repositories;
using System.Reflection;
using ServerlessTimers.Application.Services.Durables;
using ServerlessTimers.Domain.Services;

public class Startup : FunctionsStartup
{

    public override void Configure(IFunctionsHostBuilder builder)
    {

        // Application services
        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
        builder.Services.AddScoped<IDurableFacade, DurableFacade>();
        builder.Services.AddScoped<IDurableClient>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var factory = provider.GetRequiredService<IDurableClientFactory>();
            return factory.CreateClient(new DurableClientOptions
            {
                TaskHub = "testhubname"
            });
        });

        // Infrastructure services
        builder.Services.AddScoped<ITimerRepository, TimerRepository>();
        
        // Domain services
        builder.Services.AddScoped<ITimerCalculatorFactory, TimerCalculatorFactory>();
    
    }

}
