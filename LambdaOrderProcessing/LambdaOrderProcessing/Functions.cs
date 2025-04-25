using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.SQSEvents;
using AWS.Messaging.Lambda;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaOrderProcessing;

public class Functions
{

    [LambdaFunction(Policies = "AWSLambdaSQSQueueExecutionRole,ComprehendReadOnly,AmazonDynamoDBFullAccess")]
    public async Task<SQSBatchResponse> FunctionHandler([FromServices] ILambdaMessaging messaging, SQSEvent evnt, ILambdaContext context)
    {
        return await messaging.ProcessLambdaEventWithBatchResponseAsync(evnt, context);
    }
}
