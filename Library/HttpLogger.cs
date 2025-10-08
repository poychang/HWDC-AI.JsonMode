namespace Library
{
    /// <summary>
    /// Logging handler you might want to use to see the HTTP traffic sent by SK to LLMs.
    /// </summary>
    public class HttpLogger : DelegatingHandler
    {
        public static HttpClient GetHttpClient(bool isShowLogging = false)
        {
            var httpclient = isShowLogging
                ? new HttpClient(new HttpLogger(new HttpClientHandler()))
                : new HttpClient();

            httpclient.Timeout = TimeSpan.FromMinutes(5);

            return httpclient;
        }

        public HttpLogger(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("\x1b[34mRequest Body:\x1b[0m");
            if (request.Content != null)
            {
                Console.WriteLine($"\x1b[90m{request.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult().ToMinify()}\x1b[0m");
            }
            Console.WriteLine();

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine("\u001b[32mResponse Body:\x1b[0m");
            if (response.Content != null)
            {
                Console.WriteLine($"\x1b[90m{response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult().ToMinify()}\x1b[0m");
            }
            Console.WriteLine();

            return response;
        }
    }
}
