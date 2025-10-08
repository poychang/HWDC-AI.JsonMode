namespace Server
{
    public class Secrets
    {
        public AzureOpenAIModel AzureOpenAI { get; set; } = new AzureOpenAIModel();
        public OpenAIModel OpenAI { get; set; } = new OpenAIModel();
        public class AzureOpenAIModel
        {
            public string Endpoint { get; set; } = string.Empty;
            public string APIKey { get; set; } = string.Empty;
            public string DeploymentName { get; set; } = string.Empty;
            public string ImageModel { get; set; } = string.Empty;
        }
        public class OpenAIModel
        {
            public string APIKey { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string ImageModel { get; set; } = string.Empty;
        }
    }
}
