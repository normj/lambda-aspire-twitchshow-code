using Amazon.Comprehend;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessing.Handlers;
using OrderProcessing.Models;

namespace LambdaOrderProcessing;

[LambdaStartup]
public class Startup
{

    public HostApplicationBuilder ConfigureHostBuilder()
    {
        var hostBuilder = new HostApplicationBuilder();

        hostBuilder.Services.AddAWSService<IAmazonDynamoDB>();
        hostBuilder.Services.AddAWSService<IAmazonComprehend>();

        hostBuilder.Services.AddSingleton<IDynamoDBContext>(sp =>
        {
            var client = sp.GetRequiredService<IAmazonDynamoDB>();
            var config = new DynamoDBContextConfig { DisableFetchingTableMetadata = true };
            return new DynamoDBContext(client, config);
        });

        hostBuilder.Services.AddAWSMessageBus(builder =>
        {
            builder.AddMessageHandler<ProductFeedbackHandler, ProductFeedback>();

            builder.AddLambdaMessageProcessor();
        });

        return hostBuilder;
    }
}
