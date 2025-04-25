using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using AWS.Messaging;
using OrderProcessing.Models;

namespace OrderProcessing.Handlers
{
    public class ProductFeedbackHandler : IMessageHandler<ProductFeedback>
    {
        private ILambdaContext _lambdaContext;
        private IDynamoDBContext _ddbContext;
        private IAmazonComprehend _comprehendClient;

        public ProductFeedbackHandler(ILambdaContext lambdaContext, IDynamoDBContext ddbContext, IAmazonComprehend comprehendClient) 
        {
            _lambdaContext = lambdaContext;
            _ddbContext = ddbContext;
            _comprehendClient = comprehendClient;
        }

        public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<ProductFeedback> messageEnvelope, CancellationToken token = default)
        {
            var productFeedback = messageEnvelope.Message;
            _lambdaContext.Logger.LogInformation("Processing feedback for {product}", productFeedback.ProductId);

            var detectRequest = new DetectSentimentRequest
            {
                Text = messageEnvelope.Message.Feedback,
                LanguageCode = "en"
            };

            var detectResponse = await _comprehendClient.DetectSentimentAsync(detectRequest);

            _lambdaContext.Logger.LogInformation("Sentiment of feedback is {sentiment}", detectResponse.Sentiment);

            var analysis = new ProductFeedbackAnalysis
            {
                AnalysisId = Guid.NewGuid().ToString(),
                Created = DateTime.UtcNow,
                ProductId = productFeedback.ProductId,
                Feedback = productFeedback.Feedback,
                Sentiment = detectResponse.Sentiment
            };

            await _ddbContext.SaveAsync(analysis);

            return MessageProcessStatus.Success();
        }

        [DynamoDBTable("FeedbackAnalysis")]
        public class ProductFeedbackAnalysis
        {
            [DynamoDBHashKey]
            public string? ProductId { get; set; }

            [DynamoDBRangeKey]
            public string? AnalysisId { get; set; }

            public DateTime Created {  get; set; }

            public string? Feedback { get; set; }

            public string? Sentiment { get; set; }
        }
    }
}
