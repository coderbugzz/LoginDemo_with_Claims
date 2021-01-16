using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginDemo.Models
{
    public class Response<T>
    {
        public T Data { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }
}