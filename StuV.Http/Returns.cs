using System;
using System.Net.Http;

namespace StuV.Http
{
    public class Returns
    {
        internal Returns(Func<HttpRequestMessage, bool> expression)
        {
            Expression = expression;
        }

        public Func<HttpRequestMessage, bool> Expression { get; private set; }

        public Func<HttpResponseMessage> ValueCreator { get; private set; }

        public void Return(Func<HttpResponseMessage> valueCreator)
        {
            ValueCreator = valueCreator;
        }
    }
}
