using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }


            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine(str);
                }

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);

                //parse params in url
                string param1 = HttpUtility.ParseQueryString(request.Url.Query).Get("param1");
                string param2 = HttpUtility.ParseQueryString(request.Url.Query).Get("param2");
                string param3 = HttpUtility.ParseQueryString(request.Url.Query).Get("param3");
                string param4 = HttpUtility.ParseQueryString(request.Url.Query).Get("param4");
                string custom = HttpUtility.ParseQueryString(request.Url.Query).Get("custom");

                Console.WriteLine("param1 = " + param1);
                Console.WriteLine("param2 = " + param2);
                Console.WriteLine("param3 = " + param3);
                Console.WriteLine("param4 = " + param4);
                Console.WriteLine("custom = " + custom);
                //
                Console.WriteLine(documentContents);
                // request method
                string method_string = request.Url.Segments[request.Url.Segments.Length - 1].ToString();

                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                // request link = "http://localhost:8080/ce_que_je_veux/MyMethod?param1=Abdel-Qahar%20Traore&custom=TAQ%201234"
                // Construct a response.
                Type type = typeof(MyServerReflectionClass);
                MethodInfo method = type.GetMethod(method_string);
                string result = (string) method.Invoke(new MyServerReflectionClass(), new object[] { param1, custom });
                Console.WriteLine(result);

                string responseString = result;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
                Console.ReadLine();
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();

        }
    }

    public class MyServerReflectionClass
    {
        public string MyMethod(string param1, string param2)
        {
            return "<HTML> <BODY> <h1> Hello " + param1 + " and " + param2 + " ! </h1> </BODY> </HTML>"; ;
        }
        public string MyMethodCustom(string param1, string param2)
        {
            return "<HTML> <BODY> <h1> Bonjour " + param1 + " et " + param2 + " ! </h1> </BODY> </HTML>"; ;
        }
    }
}
