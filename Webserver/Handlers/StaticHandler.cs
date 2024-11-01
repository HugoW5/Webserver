using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Handlers
{
	internal class StaticHandler
	{
		public static void SendFile(string folder, Queue<string> path, HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				if (path.TryDequeue(out string? file))
				{
					string filePath = $"static/{folder}/{file}";
					if (File.Exists(filePath))
					{
						switch (file.Split('.')[1])
						{
							case "css":
								ServeFile(filePath, "text/css", req, res);
								break;
							default:
								throw new FileNotFoundException();
						}
					}
					else
					{
						throw new FileNotFoundException();
					}
				}
			}
			catch (Exception ex)
			{
				Handler.Write404(req, res);
			}
		}
		private static void ServeFile(string filePath, string contentType, HttpListenerRequest req, HttpListenerResponse res)
		{
			byte[] fileData = File.ReadAllBytes(filePath);
			res.ContentType = contentType;
			res.ContentLength64 = fileData.Length;
			res.OutputStream.Write(fileData, 0, fileData.Length);
			res.StatusCode = 200;
			res.Close();
		}
	}
}
