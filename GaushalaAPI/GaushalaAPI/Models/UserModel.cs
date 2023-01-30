using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GaushalaAPI.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public DateTime Created { get; set; }
        public int Usertype { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        private string PasswordHash { get; set; }
        public string AccessToken { get; set; }
        public string Role { get; set; }
        public UserModel()
        {

        }
        public UserModel(SqlDataReader rdr)
        {
            try{this.Id = Convert.ToInt32(rdr["id"]);
            }catch(Exception e){}
            try
            {
                this.Username = Convert.ToString(rdr["UserName"]);
            }
            catch (Exception e) { }try
            {
                this.Password = Convert.ToString(rdr["Password"]);
            }
            catch (Exception e) { }try
            {
                this.Name = Convert.ToString(rdr["Name"]);
            }
            catch (Exception e) { }try
            {
                this.Created = (DateTime)(rdr["Created"]);
            }
            catch (Exception e) { }try
            {
                this.Usertype  = Convert.ToInt16(rdr["UserType"]);
            }
            catch (Exception e) { }try
            {
                this.PasswordHash = Convert.ToString(rdr["PasswordHash"]);
            }
            catch (Exception e) { }
            try
            {
                this.Label = Convert.ToString(rdr["Label"]);
            }
            catch (Exception e) { }
        }
        public string HashPassword()
        {
            string salt = BCryptNet.GenerateSalt(12);
            Console.WriteLine(salt);
            salt = BCryptNet.GenerateSalt();
            Console.WriteLine(salt);
            Console.WriteLine(this.Password);
            string hash = BCryptNet.HashPassword(this.Password, salt);
            Console.WriteLine(hash);
            return hash;
        }
        public void setPasswordHash(string hash)
        {
            this.PasswordHash = hash;
        }
        public bool VerifyPassword(string password)
        {
            
            //string salt = BCryptNet.GenerateSalt();
            //Console.WriteLine(salt);
            //Console.WriteLine(this.Password);
            //string hash = BCryptNet.HashPassword(password, salt);
            Console.WriteLine("VP " + this.PasswordHash);
            Console.WriteLine(password);
            bool a = BCryptNet.Verify(password, this.PasswordHash);
            return a;
        }
        public string GetName(){
            return this.Name;
        }
        public string GetDesignatedName(){
            return $"{this.Label} {this.Name}";
        }
    }
}
