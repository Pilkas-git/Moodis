namespace Moodis.Network.Requests
{
    class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsDoctor { get; set; }

        private RegisterRequest() { }

        public RegisterRequest(string username, string password, bool isDoctor = true)
        {
            Username = username;
            Password = password;
            IsDoctor = isDoctor;
        }
    }
}