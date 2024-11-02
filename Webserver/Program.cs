using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Webserver.Handlers;

namespace Webserver
{
	internal class Program
	{
		public static List<Account> accounts = new List<Account>();

		public static Dictionary<string, string> pageData = new Dictionary<string, string>() {
							{"%data%", $"{DateTime.Now.ToString()}"},
			{"%tableData%", ""}
						};
		static async Task Main(string[] args)
		{
			//string url = "http://localhost:8080/"; // local ip
			string url = "http://*:80/"; // Bind all ips over http

			using (HttpListener listener = new HttpListener())
			{
				listener.Prefixes.Add(url);
				listener.Start();
				Console.WriteLine("Listening on " + url);
					
				while (listener.IsListening)
				{
					try
					{

						HttpListenerContext context = await listener.GetContextAsync();
						var req = context.Request;
						var res = context.Response;
						Queue<string> path = new Queue<string>(req.RawUrl.Split('/'));

						path.Dequeue(); // remove blankspace [0]
										//Path root
						string p = path.Dequeue();
						switch (p)
						{
							case "":
								Handler.RenderDynamicHtml("static/templates/index.html", pageData, req, res);
								break;
							case "signup":
								Handler.RenderHtml("static/templates/signup.html", req, res);
								break;
							case "postNewAccount":
								Handler.CreateNewAccount(req, res);
								break;
							case "static":
								Handler.HandleStatic(path, req, res);
								break;
							default:
								Handler.Write404(req, res);
								break;
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}
			}
		}
	}
}
