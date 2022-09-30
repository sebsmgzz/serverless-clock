
[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection
    .FunctionsStartup(typeof(ServerlessAlarm.Application.Startup))]
namespace ServerlessAlarm.Application;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Domain.Aggregators.Alarms;
using Infrastructure.Repositories;
using MediatR;
using System.Reflection;

public class Startup : FunctionsStartup
{

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<IAlarmRepository, AlarmRepository>();
        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    }

}
