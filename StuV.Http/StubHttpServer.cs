using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StuV.Http
{
    public class StubHttpServer : IDisposable
    {
        private readonly IWebHost webHost;
        private readonly IList<Returns> handlers = new List<Returns>();

        public StubHttpServer(int port) : this($"http://localhost:{port}") { }

        public StubHttpServer(string url)
        {
            webHost = WebHost.StartWith(url, app =>
            {
                app.Run(async context =>
                {
                    var requestMessage = new HttpRequestMessageFeature(context).HttpRequestMessage;
                    var responseMessage = handlers.FirstOrDefault(h => h.Expression(requestMessage))?.ValueCreator();
                    await ConvertToHttpResponseAsync(responseMessage, context.Response).ConfigureAwait(false);
                });
            });
        }

        public void Dispose() => webHost?.Dispose();

        public Returns When(Func<HttpRequestMessage, bool> expression)
        {
            var returns = new Returns(expression);
            handlers.Add(returns);
            return returns;
        }

        private async Task ConvertToHttpResponseAsync(HttpResponseMessage responseMessage, HttpResponse response)
        {
            if (responseMessage == null)
            {
                response.StatusCode = 404;
                return;
            }

            response.StatusCode = (int)responseMessage.StatusCode;

            foreach (var keyValue in responseMessage.Headers)
            {
                foreach (var value in keyValue.Value)
                {
                    response.Headers.Add(keyValue.Key, value);
                }
            }

            await responseMessage.Content.LoadIntoBufferAsync().ConfigureAwait(false);

            foreach (var keyValue in responseMessage.Content.Headers)
            {
                foreach (var value in keyValue.Value)
                {
                    response.Headers.Add(keyValue.Key, value);
                }
            }

            await responseMessage.Content.CopyToAsync(response.Body).ConfigureAwait(false);
            await response.Body.FlushAsync().ConfigureAwait(false);
        }
    }
}
