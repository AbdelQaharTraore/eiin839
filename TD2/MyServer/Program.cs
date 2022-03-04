using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
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
            Exo1(args);     // request link = "http://localhost:8080/ce_que_je_veux/MyMethod?param1=Benjamin&custom=Jean-Yves"
            //Exo2(args);     //link = "http://localhost:8080/ce_que_je_veux/?name=toto"
            //Exo3(args);     //link = "http://localhost:8080/ce_que_je_veux/?number=5"
            Console.ReadLine();
        }

        static void Exo1(string[] args)
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
            Console.CancelKeyPress += delegate
            {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                //MyMethods myMethods = new MyMethods();
                //myMethods.ExternalCall("Abdel-Qahar");

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
                Console.WriteLine("=====" + method_string + "=====");
                if (method_string != "favicon.ico")
                {
                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;

                    // Construct a response.
                    Type type = typeof(MyMethods);
                    MethodInfo method = type.GetMethod(method_string);
                    MyMethods instance = new MyMethods();
                    object[] par = new object[] { param1, custom };
                    string result = (string)method.Invoke(instance, par);
                    Console.WriteLine(result);

                    string responseString = result;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();
                }
                //Console.ReadLine();
                Console.WriteLine("end");
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }
        static void Exo2(string[] args, string method="reflect")
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

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate
            {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string name = HttpUtility.ParseQueryString(request.Url.Query).Get("name");

                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                // Construct a response.
                MyMethods myMethods = new MyMethods();
                string result = string.Empty;
                if (method.Equals("python"))
                {
                    result = myMethods.ExternalCall(name, @"C:\Users\user\AppData\Local\Microsoft\WindowsApps\python.exe");
                    Console.WriteLine(result);
                } else
                {
                    result = myMethods.ExternalCall(name, @"C:\data\SI4\S8-data\soc\eiin839\TD2\ExecTest\bin\Debug\ExecTest.exe");
                    Console.WriteLine(result);
                }

                string responseString = result;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
        }
        static void Exo3(string[] args)
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

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate
            {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string method_string = request.Url.Segments[request.Url.Segments.Length - 1].ToString();
                Console.WriteLine("=====" + method_string + "=====");
                if (method_string != "favicon.ico")
                {
                    string name = HttpUtility.ParseQueryString(request.Url.Query).Get("number");

                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;

                    // Construct a response.
                    MyMethods myMethods = new MyMethods();
                    string got = myMethods.Incr(name);
                    // from : https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                    string result_str = string.Empty;
                    for (int i = 0; i < got.Length; i++)
                    {
                        if (Char.IsDigit(got[i]))
                            result_str += got[i];
                    }

                    int result = 0;
                    if (result_str.Length > 0)
                        result = int.Parse(result_str);
                    Console.WriteLine(result);

                    string responseString = result.ToString();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();
                }
            }
        }
    }

    public class MyMethods
    {
        public string MyMethod(string param1, string param2)
        {
            return "<HTML> <BODY> <h1> Hello " + param1 + " and " + param2 + " ! </h1> </BODY> </HTML>"; ;
        }
        public string MyMethodCustom(string param1, string param2)
        {
            return "<HTML> <BODY> <h1> Bonjour " + param1 + " et " + param2 + " ! </h1> </BODY> </HTML>"; ;
        }
        public string ExternalCall(string param, string exec)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            //start.FileName = @"C:\data\SI4\S8-data\soc\eiin839\TD2\ExecTest\bin\Debug\ExecTest.exe"; // Specify exe name.
            start.FileName = exec; // Specify exe name.
            start.Arguments = param; // Specify arguments.
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            //
            // Start the process.
            //
            using (Process process = Process.Start(start))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                    return result;
                }
            }
        }

        public string Incr(string param)
        {
            return "incr OK val=" + (int.Parse(param) + 1);
        }
    }
}
