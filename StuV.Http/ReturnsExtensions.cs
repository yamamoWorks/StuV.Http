using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StuV.Http
{
    public static class ReturnsExtensions
    {
        private static FileExtensionContentTypeProvider _contentTypeProvider;

        static ReturnsExtensions()
        {
            _contentTypeProvider = new FileExtensionContentTypeProvider();
            _contentTypeProvider.Mappings[".xml"] = "application/xml";
        }

        public static void ReturnString(this Returns returns, string value, string contentType = "text/plain")
            => returns.Return(() =>
            {
                var content = new StringContent(value);
                if (!string.IsNullOrEmpty(contentType))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            });

        public static void ReturnFile(this Returns returns, string filePath, string contentType = null)
            => returns.Return(() =>
            {
                var content = new StreamContent(File.OpenRead(filePath));
                if (!string.IsNullOrEmpty(contentType))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
                else if (_contentTypeProvider.TryGetContentType(filePath, out string presumedContentType))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(presumedContentType);
                }
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            });


        public static void ReturnJson(this Returns returns, object value, JsonSerializerOptions options = null)
            => returns.Return(() =>
            {
                var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(value, options));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            });
    }
}
