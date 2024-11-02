using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
	internal class Account
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string SessionId { get; set; }
		public Account(string _name, string _password, Guid _sessionId)
		{
			Username = _name;
			Password = _password;
			SessionId = _sessionId.ToString();
		}
	}
}
