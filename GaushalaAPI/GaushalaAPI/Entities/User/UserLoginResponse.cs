using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities.User
{
    public class UserLoginResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public DateTime Created { get; set; }
        public int Usertype { get; set; }
        public string Type { get; set; }
        private string PasswordHash { get; set; }
        public string AccessToken { get; set; }
        public bool IsSuccess { get; set; }
        public string Role { get; set; }
        public Dictionary<string,string> errors { get; set; }
    }
}
