﻿
[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection
    .FunctionsStartup(typeof(ServerlessAlarms.Application.Startup))]
namespace ServerlessAlarms.Application;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Domain.Aggregators.Alarms;
using Infrastructure.Repositories;
using MediatR;
using System.Reflection;
using ServerlessAlarms.Application.Services.Durables;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.Options;

public class Startup : FunctionsStartup
{

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<IDurableClient>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var factory = provider.GetRequiredService<IDurableClientFactory>();
            return factory.CreateClient(new DurableClientOptions
            {
                TaskHub = "testhubname"
            });
        });
        builder.Services.AddScoped<IDurableFacade, DurableFacade>();
        builder.Services.AddScoped<IAlarmRepository, AlarmRepository>();
        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    }

}
