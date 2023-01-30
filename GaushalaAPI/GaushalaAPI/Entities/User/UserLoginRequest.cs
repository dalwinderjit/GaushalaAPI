using GaushalaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GaushalAPI.Entities.User
{
    public class UserLoginRequest
    {
        private Dictionary<string, string> errors = new Dictionary<string, string>();
        public string Username { get; set; }
        public string Password { get; set; }
        public Dictionary<string,string> GetErrors()
        {
            return this.errors;
        }
        internal bool ValidCredentials()
        {
            bool valid = true;
            Regex re;
            re = new Regex(Helper.UsernameRegEx);
            if (this.Username.Trim().Length == 0)
            {
                errors.Add("username", "Please Provide the Username"); 
                valid = false;
            }
            else if (re.IsMatch(this.Username) == false)
            {
                errors.Add("username", "Invalid Username (only alphabet, digits, . are allowed.(length 3 to 25)"); 
                valid = false;
            }
            re = new Regex(Helper.PasswordRegEx);
            if(this.Password.Trim().Length == 0)
            {
                errors.Add("password", "Please Provide the Password");
                valid = false;
            }else if (re.IsMatch(this.Password)==false)
            {
                errors.Add("password", "Invalid Password Entered.(Length 8-20)");
                valid = false;
            }
            return valid;
        }
    }
}
