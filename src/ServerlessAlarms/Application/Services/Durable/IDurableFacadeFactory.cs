namespace ServerlessAlarm.Application.Services.Durable;

using Microsoft.Azure.WebJobs.Extensions.DurableTask;

public interface IDurableFacadeFactory
{

    IDurableFacade GetFacade(IDurableOrchestrationClient client);

}
