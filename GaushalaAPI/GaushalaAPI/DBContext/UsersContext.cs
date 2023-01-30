using GaushalaAPI.Authorization;
using GaushalaAPI.Models;
using GaushalAPI.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace GaushalaAPI.DBContext
{
    public class UsersContext
    {
        private readonly IConfiguration _configuration;
        
        public UsersContext(IConfiguration configuration, HttpContext _httpContext)
        {
            _configuration = configuration;
            _httpContext = _httpContext; 
        }
        public UsersContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UserLoginResponse Login(string username, string password)
        {
            UserModel user1 = null;
            UserLoginResponse loginResponse = new UserLoginResponse();
            loginResponse.errors = new Dictionary<string, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select  * from Users where UserName = @Username", conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@Username", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Username"].Value = username;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        Console.WriteLine("User found");
                        UserModel user = new UserModel();
                        user.Id = Convert.ToInt32(sqlrdr["Id"]);
                        user.Username = Convert.ToString(sqlrdr["UserName"]);
                        user.Name = Convert.ToString(sqlrdr["Name"]);
                        user.Usertype = Convert.ToInt32(sqlrdr["UserType"]);
                        user.Password = Convert.ToString(sqlrdr["Password"]);
                        user.setPasswordHash(Convert.ToString(sqlrdr["PasswordHash"]));
                        string hash = user.HashPassword();
                        Console.WriteLine(hash);
                        bool a = user.VerifyPassword(password);
                        
                        if (a == true)
                        {
                            user1 = user;
                            loginResponse.Id = user.Id;
                            loginResponse.Username = user.Username;
                            loginResponse.Name = user.Name;
                            loginResponse.Usertype = user.Usertype;
                            loginResponse.IsSuccess = true;
                            loginResponse.Role = this.GetUserTypeById(user.Usertype);
                            //generate access token
                            JwtUtils jwt = new JwtUtils(this._configuration);
                            user1.AccessToken = jwt.GenerateJwtToken(user1);
                            loginResponse.AccessToken = jwt.GenerateJwtToken(user1);
                            return loginResponse;
                        }
                        else
                        {
                            loginResponse.IsSuccess = false;
                            loginResponse.errors["error"] = "Login Failed! Invaid Username Password";
                        }
                    }
                    else
                    {
                        loginResponse.IsSuccess = false;
                        loginResponse.errors["error"] = "Login Failed! Invalid Username Password";
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return loginResponse;
                }
                catch (Exception ex)
                {
                    loginResponse.IsSuccess = false;
                    loginResponse.errors["password"] = "Login Failed";
                    return loginResponse;
                }
            }
        }
        public HttpResponseMessage Login2(string username, string password)
        {
            UserModel user1 = null;
            HttpResponseMessage response;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select  * from Users where UserName = @Username", conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@Username", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Username"].Value = username;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine("User found");
                        UserModel user = new UserModel();
                        user.Id = Convert.ToInt32(sqlrdr["Id"]);
                        user.Username = Convert.ToString(sqlrdr["UserName"]);
                        user.Name = Convert.ToString(sqlrdr["Name"]);
                        user.Usertype = Convert.ToInt32(sqlrdr["UserType"]);
                        user.Password = Convert.ToString(sqlrdr["Password"]);
                        user.setPasswordHash(Convert.ToString(sqlrdr["PasswordHash"]));
                        string hash = user.HashPassword();
                        bool a = user.VerifyPassword(password);
                        if (a == true)
                        {
                            user1 = user;
                            //generate access token
                            JwtUtils jwt = new JwtUtils(this._configuration);
                            Console.WriteLine("JWT Object Created");
                            user1.AccessToken = jwt.GenerateJwtToken(user1);
                            Console.WriteLine(user1.AccessToken);
                            Console.WriteLine("ID " + jwt.ValidateJwtToken(user1.AccessToken));
                            CookieHeaderValue cookie = new CookieHeaderValue(
                                "__secure_token",
                                user1.AccessToken);
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("hello", Encoding.Unicode);
                            var options = new CookieOptions();
                            options.Expires = DateTime.Now.AddDays(2);
                            response.Headers.Add("Authorization", user1.AccessToken);
                            //Microsoft.AspNetCore.Http.HttpResponse.Add
                            //HttpResponse.
                            //System.Net.Http.Headers.HttpResponseHeaders.(new Cookie("__secure_token", user1.AccessToken));
                            List<Claim> claims = new List<Claim>() {
                                new Claim(ClaimTypes.Email, user1.Username),
                                new Claim(ClaimTypes.PrimarySid, user1.Id.ToString()),
                                new Claim(ClaimTypes.Role, user1.Usertype.ToString())
                            };
                            var claimsIdentity = new ClaimsIdentity(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme
                                );
                            //await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ClaimsPrincipal.Current);
                            
                            /*await _httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(claimsIdentity));*/
                            return response;
                            //, new CookieOptions() { Expires = DateTime.Now.AddDays(30), Path = "/", HttpOnly = true, SameSite = true});
                        }
                    }
                    sqlrdr.Close();
                    conn.Close();
                    response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent("hello", Encoding.Unicode);
                    return response;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HELO");
                    Console.WriteLine(ex.ToString());
                    response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent("hello", Encoding.Unicode);
                    return response;
                }
            }

        }
        #nullable enable
        internal List<Dictionary<string, object>> GetDoctors(string? username, string? name)
        {
            //UserModel user = null;
            List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();
            //Dictionary<int, Dictionary<string, object>> data = new Dictionary<int, Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            Dictionary<string, object> user;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select Users.*, UserType.Label, UserType.Type from Users join UserType on Users.UserType = UserType.Id and UserType.Id = 2", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int i = 1;
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine("User found");
                        user = new Dictionary<string, object>();
                        user["sno"] = i;
                        user["id"] = Convert.ToInt32(sqlrdr["Id"]);
                        user["usertype"] = Convert.ToString(sqlrdr["Type"]);
                        switch (user["usertype"])
                        {
                            case "doctor":
                                user["name"] = "Dr. " + Convert.ToString(sqlrdr["Name"]);
                                break;
                            default:
                                user["name"] = Convert.ToString(sqlrdr["Name"]);
                                break;
                        }

                        user["username"] = Convert.ToString(sqlrdr["UserName"]);
                        user["created"] = Convert.ToString(sqlrdr["Created"]);

                        //data[Convert.ToInt32(sqlrdr["Id"])] = user;
                        users.Add(user);
                        i++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return users;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HELO");
                    Console.WriteLine(ex.ToString());
                    return users;
                }
            }
        }
        #nullable disable
        internal List<Dictionary<string, object>> GetUsers(string? username, string? name)
        {
            //UserModel user = null;
            List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();
            //Dictionary<int, Dictionary<string, object>> data = new Dictionary<int, Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            Dictionary<string, object> user;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select Users.*, UserType.Label, UserType.Type from Users join UserType on Users.UserType = UserType.Id", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int i = 1;
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine("User found");
                        user = new Dictionary<string, object>();
                        user["sno"] = i;
                        user["id"] = Convert.ToInt32(sqlrdr["Id"]);
                        user["usertype"] = Convert.ToString(sqlrdr["Type"]);
                        switch (user["usertype"])
                        {
                            case "doctor":
                                user["name"] = "Dr. " + Convert.ToString(sqlrdr["Name"]);
                                break;
                            case "admin":
                                user["name"] = "Admin " + Convert.ToString(sqlrdr["Name"]);
                                break;
                            default:
                                user["name"] = Convert.ToString(sqlrdr["Name"]);
                                break;
                        }

                        user["username"] = Convert.ToString(sqlrdr["UserName"]);
                        user["created"] = Convert.ToString(sqlrdr["Created"]);

                        //data[Convert.ToInt32(sqlrdr["Id"])] = user;
                        users.Add(user);
                        i++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return users;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HELO");
                    Console.WriteLine(ex.ToString());
                    return users;
                }
            }
        }
        internal Dictionary<long, string> GetDoctorsIdNameByIds(List<long> doctors)
        {
            string ids = "";

            Dictionary<long, string> users = new Dictionary<long, string>();
            int i = 0;
            foreach (var m in doctors)
            {
                if (ids != "")
                {
                    ids += ",";
                }
                ids += $"@id{i}";
                i++;
            }
            if (ids != "")
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand($"Select  * from Users join UserType on Users.UserType = UserType.Id and Users.Id in ({ids})", conn);
                    try
                    {
                        i = 0;
                        conn.Open();
                        long id_ = 0;
                        foreach (var m in doctors)
                        {
                            try
                            {
                                id_ = Convert.ToInt64(m.ToString());

                            }
                            catch (Exception e)
                            {
                                id_ = 0;
                            }
                            sqlcmd.Parameters.Add($"@id{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@id{i}"].Value = Convert.ToInt64(m.ToString());
                            i++;
                        }
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        i = 1;
                        while (sqlrdr.Read())
                        {
                            Console.WriteLine("User found");
                            users[Convert.ToInt32(sqlrdr["Id"])] = Convert.ToString(sqlrdr["Name"]);
                            i++;
                        }
                        sqlrdr.Close();
                        conn.Close();
                        return users;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("HELO");
                        Console.WriteLine(ex.ToString());
                        return users;
                    }
                }
            }
            return users;
        }
        public static Dictionary<long, Dictionary<string, object>> GetDoctorsDataByIds(IConfiguration _configuration, List<long> doctors)
        {
            string ids = "";
            Dictionary<long, Dictionary<string, object>> users = new Dictionary<long, Dictionary<string, object>>();
            int i = 0;
            foreach (var m in doctors)
            {
                if (ids != "")
                {
                    ids += ",";
                }
                ids += m.ToString();
                i++;
            }
            if (ids != "")
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"Select Users.*,UserType.Label from Users join UserType on Users.UserType = UserType.Id and Users.Id in ({ids})";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    try
                    {
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        i = 1;
                        while (sqlrdr.Read())
                        {
                            Console.WriteLine("User found");
                            Dictionary<string, object> user = new Dictionary<string, object>();
                            user["id"] = sqlrdr["Id"];
                            user["name"] = sqlrdr["Name"];
                            user["label"] = sqlrdr["Label"];
                            users[Convert.ToInt64(user["id"])] = user;
                        }
                        sqlrdr.Close();
                        conn.Close();
                        return users;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("HELO");
                        Console.WriteLine(ex.ToString());
                        return users;
                    }
                }
            }
            return users;
        }
        public UserModel GetUserById(long id)
        {
            UserModel user;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $"Select Users.*,UserType.Label from Users join UserType on Users.UserType = UserType.Id and Users.Id = @Id";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = id;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if(sqlrdr.Read())
                    {
                        Console.WriteLine("User found");
                        user = new UserModel(sqlrdr);
                        return user;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    user = new UserModel();
                    return user;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HELO");
                    Console.WriteLine(ex.ToString());
                    user = new UserModel();
                }
            }
            return user;
        }
        public string GetUserTypeById(long Id){
            string type = "";
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select  * from UserType where Id = @Id", conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = Id;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        type = Convert.ToString(sqlrdr["Type"]);
                    }else{
                        type ="";
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return type;
                }
                catch (Exception ex)
                {
                    return type;
                }
            }
            return type;
        }
    }
}
