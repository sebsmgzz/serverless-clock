namespace ServerlessAlarm.Application.Services.Durable;

using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

public class DurableFacadeFactory : IDurableFacadeFactory
{

    private readonly ILoggerFactory loggerFactory;

    public DurableFacadeFactory(ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
    }

    public IDurableFacade GetFacade(IDurableOrchestrationClient client)
    {
        var logger = loggerFactory.CreateLogger<IDurableFacade>();
        return new DurableFacade(client, logger);
    }

}
