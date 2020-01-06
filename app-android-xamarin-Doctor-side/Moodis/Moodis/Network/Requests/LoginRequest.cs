using System;
namespace Moodis.Network.Requests
{
    class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private LoginRequest() { }

        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}