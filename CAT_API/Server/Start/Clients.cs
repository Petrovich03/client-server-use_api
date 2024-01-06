using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Options
{
    public class Clients
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Messages { get; set; }

        public Clients(string username, string password)
        {
            Username = username;
            Password = password;
            Messages = new List<string>();
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }
    }
}
