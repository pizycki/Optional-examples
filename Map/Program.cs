using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using Optional;
using Optional.Collections;
using Optional.Linq;
using Optional.Unsafe;

namespace Map
{
    public class HttpRequestComposer
    {
        public Option<HttpRequest> FindCourseNameById(CourseId courseId)
        {
            Func<HttpRequest, Action<HttpRequest>, HttpRequest> modify = (req, action) =>
            {
                action(req);
                return req;
            };

            Func<Socket, Option<HttpRequest>> CreateHttpRequest = sock => new HttpRequest().Some(); // Fails when socket already closed
            Func<HttpRequest, HttpRequest> AttachXApiKeyHeader = req => modify(req, r => r.Headers.Add("X-API-KEY"));
            Func<HttpRequest, HttpRequest> SetUrl = req => modify(req, r => r.Url = "http://cop.jira.bc.com");
            Func<HttpRequest, HttpRequest> SetMethod = req => modify(req, r => r.Method = "POST");
            Func<HttpRequest, HttpRequest> GzipBody = req => modify(req, r => r.Body = "DQODJ1N29DNas");

            Option<HttpRequest> request =
                CreateHttpRequest(new Socket(SocketType.Stream, ProtocolType.IPv6))
                    .Map(AttachXApiKeyHeader)
                    .Map(SetUrl)
                    .Map(SetMethod)
                    .Map(GzipBody);

            return request;
        }
    }

    public class HttpRequest
    {
        public List<string> Headers { get; } = new List<string>();
        public string Url { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
