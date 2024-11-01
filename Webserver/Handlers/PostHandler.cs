using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webserver.Handlers
{
	internal class PostHandler
	{
		public static void HandlePost(Queue<string> path, HttpListenerRequest req, HttpListenerResponse res)
		{
			if (req.HttpMethod == "POST")
			{
				NameValueCollection inputData = ParseQueries(req.InputStream);

				foreach (var key in inputData.AllKeys)
				{
					Console.WriteLine(key + " " + inputData[key]);
				}
				res.Redirect("/");
				res.Close();
			}
		}

		public static NameValueCollection ParseQueries(Stream inputStream)
		{
			NameValueCollection tmpDataCollection = new NameValueCollection();
			string inputDataQueries = string.Empty;
			using (StreamReader sr = new StreamReader(inputStream))
			{
				while (!sr.EndOfStream)
				{
					inputDataQueries += sr.ReadLine();
				}
				sr.Close();
			}
			if (!inputDataQueries.Contains('&'))
			{
				string[] data = inputDataQueries.Split('=');
				tmpDataCollection.Add(data[0], data[1]);
			}
			else if (inputDataQueries.Contains('&'))
			{
				string[] dataQueries = inputDataQueries.Split('&');
				foreach (string keyValue in dataQueries)
				{
					string[] data = keyValue.Split('=');
					//Urldecode value
					tmpDataCollection.Add(data[0], HttpUtility.UrlDecode(data[1]));
				}
			}
			return tmpDataCollection;
		}
	}
}
