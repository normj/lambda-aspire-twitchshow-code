var builder = DistributedApplication.CreateBuilder(args);

var cdkStack = builder.AddAWSCDKStack("AppResources");

var queue = cdkStack.AddSQSQueue("TheQueue");

#pragma warning disable CA2252 // This API requires opting into preview features
builder.AddAWSLambdaFunction<Projects.LambdaOrderProcessing>("OrderFunction", "LambdaOrderProcessing::LambdaOrderProcessing.Functions_FunctionHandler_Generated::FunctionHandler")
    .WithSQSEventSource(queue);

builder.AddProject<Projects.TestPublisher>("TestPublisher")
    .WithReference(queue);


builder.Build().Run();
