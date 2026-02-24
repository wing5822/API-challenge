using System;
using System.Net;

namespace Amazing.Application.Exceptions
{
    public class AmazingException : Exception
    {
        public HttpStatusCode Status { get; }

        public AmazingException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.Status = statusCode; //issue with assigning on itself
        }
    }
}