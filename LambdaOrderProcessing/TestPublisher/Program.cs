using AWS.Messaging;
using AWS.Messaging.Publishers.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessing.Models;

var hostBuilder = new HostApplicationBuilder();

var queueUrl = Environment.GetEnvironmentVariable("AWS__Resources__TheQueue__QueueUrl");


hostBuilder.Services.AddAWSMessageBus(configure =>
{
    configure.AddSQSPublisher<ProductFeedback>(queueUrl);
});

var host = hostBuilder.Build();

var publisher = host.Services.GetRequiredService<ISQSPublisher>();

for(int i = 1; true; i++)
{
    var feedback = new ProductFeedback
    {
        ProductId = i.ToString(),
        Feedback = "This is the best set of headphones I have ever used"
    };

    await publisher.SendAsync(feedback, new SQSOptions
    {
        MessageAttributes = new Dictionary<string, MessageAttributeValue> { { "AppId", new MessageAttributeValue { DataType = "String", StringValue = "TestApp" } } }
    });

    Console.WriteLine("Submitted feedback: " + feedback.Feedback);
    await Task.Delay(5000);
}


