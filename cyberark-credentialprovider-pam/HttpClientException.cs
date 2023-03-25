using System;
using System.Net;
using System.Runtime.Serialization;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    [Serializable]
    internal class HttpClientException : Exception
    {
        private string responseMessage;
        private HttpStatusCode statusCode;

        public HttpClientException()
        {
        }

        public HttpClientException(string message) : base(message)
        {
        }

        public HttpClientException(string responseMessage, HttpStatusCode statusCode)
        {
            this.responseMessage = responseMessage;
            this.statusCode = statusCode;
        }

        public HttpClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}