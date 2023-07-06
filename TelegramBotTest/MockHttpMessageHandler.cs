using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    internal class MockHttpMessageHandler: HttpMessageHandler
    {
        private readonly string? response;

        public MockHttpMessageHandler(string? response)
        {
            this.response = response;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(response) });
        }
    }
}
