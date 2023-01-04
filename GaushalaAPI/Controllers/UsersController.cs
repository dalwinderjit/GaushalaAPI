using GaushalaAPI.DBContext;
using GaushalaAPI.Models;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GaushalaAPI.Authorization;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using GaushalAPI.Entities.User;

namespace GaushalAPI.Controllers
{
    //[Authorize(new int[]{ 1,2})]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        UsersContext userContext = null;
        private readonly IConfiguration _configuration;
        private HttpContext _httpContext;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
            //_httpContext = _httpContext;
            userContext = new UsersContext(_configuration);
           // d1.SetConfiguration(_configuration);
        }
        //Filters pending
        [HttpPost]
        //#nullable enable
        public Dictionary<string, object> GetDoctors(string? username,string? name)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["data"] = userContext.GetDoctors(username, name);
            data["recordsFiltered"] = 4;
            data["recordsTotal"] = 4;
            return data;
        }
        //#nullable disable
        //#nullable enable
        [HttpPost]
        public Dictionary<string, object> GetUsers(string? username, string? name)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["data"] = userContext.GetUsers(username, name);
            data["recordsFiltered"] = 4;
            data["recordsTotal"] = 4;
            return data;
        }
        //#nullable disable
        // GET api/<UsersController>/5
        //[AllowAnonymous]
        [HttpPost]
        public UserLoginResponse Login(UserLoginRequest user)
        {
            Console.WriteLine(user.Username + " " + user.Password);
            UserLoginResponse loginResponse = new UserLoginResponse();
            if (user.ValidCredentials() == true)
            {
                UserModel user1 = new UserModel();
                loginResponse = userContext.Login(user.Username, user.Password);
                if (loginResponse.IsSuccess == true)
                {
                    this.HttpContext.Response.Headers.Add("Authorization", loginResponse.AccessToken);
                    //this.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    this.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Accept, Origin, Content-Type, Cookie");
                    //this.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin","http://localhost:4000");
                    this.HttpContext.Response.Cookies.Append("__secure_token", loginResponse.AccessToken,
                        new CookieOptions()
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                            Expires = DateTime.Now.AddMinutes(1440)
                        }
                    );
                    
                }
            }
            else
            {
                loginResponse.errors = user.GetErrors();
            }
            return loginResponse;
        }
        //[Authorize]
        [HttpGet]
        public object IsLoggedIn()
        {
            return new
            {
                isLoggedIn = true,
                message = "Authorized"
            }; 
        }
        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
