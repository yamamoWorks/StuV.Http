using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace StuV.Http
{
    public static class StubHttpServerExtensions
    {
        public static Returns WhenGet(this StubHttpServer stubHttpServer, string pathAndQuery)
            => stubHttpServer.When(
                request => request.Method == HttpMethod.Get
                        && request.RequestUri.PathAndQuery == pathAndQuery);

        public static Returns WhenPost(this StubHttpServer stubHttpServer, string pathAndQuery, Func<string, bool> bodyExpression)
            => stubHttpServer.When(
                request => request.Method == HttpMethod.Post
                        && request.RequestUri.PathAndQuery == pathAndQuery
                        && bodyExpression(request.Content.ReadAsStringAsync().Result));

        public static Returns WhenPut(this StubHttpServer stubHttpServer, string pathAndQuery, Func<string, bool> bodyExpression)
            => stubHttpServer.When(
                request => request.Method == HttpMethod.Put
                        && request.RequestUri.PathAndQuery == pathAndQuery
                        && bodyExpression(request.Content.ReadAsStringAsync().Result));

        public static Returns WhenDelete(this StubHttpServer stubHttpServer, string pathAndQuery)
            => stubHttpServer.When(
                request => request.Method == HttpMethod.Delete
                        && request.RequestUri.PathAndQuery == pathAndQuery);
    }
}
