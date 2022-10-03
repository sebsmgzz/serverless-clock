namespace ServerlessAlarms.Application.Functions.Debugging;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class GetAllDurableFunction
{

    private readonly ILogger logger;

    public GetAllDurableFunction(
        ILogger<GetAllDurableFunction> logger)
    {
        this.logger = logger;
    }

    [FunctionName(nameof(GetAllDurableFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "get" },
            Route = "durables")]
            HttpRequest request,
        [DurableClient] IDurableOrchestrationClient durableClient)
    {
        var query = new OrchestrationStatusQueryCondition()
        {
            ShowInput = true
        };
        var result = await durableClient.ListInstancesAsync(query, default);
        return new OkObjectResult(result);
    }

}
