using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Handlers
{
	internal class Handler
	{
		public static void HandleStatic(Queue<string> path, HttpListenerRequest req, HttpListenerResponse res)
		{
			if (path.TryDequeue(out string? folder))
			{
				StaticHandler.SendFile(folder, path, req, res);
			}
			else
			{
				Write404(req, res);
			}
		}

		/// <summary>
		/// Renders an html file
		/// </summary>
		/// <param name="path">Html file path</param>
		/// <param name="req">HttpListenerRequest</param>
		/// <param name="res">HttpListenerResponse</param>
		public static void RenderHtml(string path, HttpListenerRequest req, HttpListenerResponse res)
		{
			if (req.HttpMethod == "GET")
			{
				if (File.Exists(path))
				{
					byte[] buffer = File.ReadAllBytes(path);
					res.ContentLength64 = buffer.Length;
					res.ContentType = "text/html";
					res.OutputStream.WriteAsync(buffer, 0, buffer.Length).Wait();
					res.Close();
				}
				else
				{
					Write404(req, res);
				}
			}
		}

		/// <summary>
		/// Renders Dynamic Html with data interpolation
		/// </summary>
		/// <param name="path">Html File Path</param>
		/// <param name="data">Dictionary</param>
		/// <param name="req">HttpListenerRequest</param>
		/// <param name="res">HttpListenerResponse</param>
		public static void RenderDynamicHtml(string path, Dictionary<string, string> data, HttpListenerRequest req, HttpListenerResponse res)
		{
			if (File.Exists(path))
			{
				string fileContent = File.ReadAllText(path);
				foreach (var key in data.Keys)
				{
					if (fileContent.Contains(key))
					{
						fileContent = fileContent.Replace(key, data[key]);
					}
				}
				byte[] buffer = Encoding.UTF8.GetBytes(fileContent);
				res.ContentLength64 = buffer.Length;
				res.ContentType = "text/html";
				res.OutputStream.WriteAsync(buffer, 0, buffer.Length).Wait();
				res.Close();
			}
			else
			{
				Write404(req, res);
			}
		}
		public static void WriteText(string text, HttpListenerRequest req, HttpListenerResponse res)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			res.ContentLength64 = buffer.Length;
			res.ContentType = "text/plain";
			res.StatusCode = 200; // OK
			res.OutputStream.WriteAsync(buffer, 0, buffer.Length).Wait();
			res.Close();
		}

		public static void Write404(HttpListenerRequest req, HttpListenerResponse res)
		{
			byte[] buffer = Encoding.UTF8.GetBytes("404");
			res.ContentLength64 = buffer.Length;
			res.StatusCode = 404; // not found
			res.OutputStream.Write(buffer, 0, buffer.Length);
			res.Close();
		}

		public static void CreateNewAccount(HttpListenerRequest req, HttpListenerResponse res)
		{
			if (req.HttpMethod == "POST")
			{
				var inputData = PostHandler.ParseQueries(req.InputStream);
				Guid sessionId = Guid.NewGuid();
				Program.accounts.Add(new Account(
					inputData["username"],
					inputData["password"],
					sessionId
					));
				res.SetCookie(new Cookie
				{
					Name = "sessionId",
					Expires = DateTime.Now.AddDays(14),
					Value = $"{sessionId.ToString()}"
				});
				//add accounts to table
				Program.pageData["%tableData%"] += $@"
            <tr>
                <td>{req.RemoteEndPoint.Address.ToString()}</td>
                <td>{inputData["username"]}</td>
                <td>{inputData["password"]}</td>
                <td>{sessionId.ToString()}</td>
            </tr>
	";
				res.Redirect("/");
				res.Close();
			}
		}
	}
}
