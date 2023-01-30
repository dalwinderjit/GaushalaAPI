using GaushalaAPI.Models;
using GaushalAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
//using GaushalaAPI.Entities;
//using StackExchange.Redis;

namespace GaushalaAPI.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        //private readonly IList<string> _roles;
        private readonly IList<int> _role;
        public IConfiguration _configuration;
        //private readonly IList<Role> _role;
        public AuthorizeAttribute(params int[] roles)
        {
            //Console.WriteLine("Authorize Attribute parama 1");
            //_role = roles ?? new Role[] { };
            _role = roles ?? new int[] { };
            //_configuration = ne
        }
        public AuthorizeAttribute()
        {
            //Console.WriteLine("Authorize Attribute 123 ");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Console.WriteLine("Gaushala API");
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
           
             if(allowAnonymous)
                return;
            int status = 0;
            // authorization
            //Console.WriteLine("hel loo aut hor izing");
            //JwtUtils jwtUtils = new JwtUtils();
            var user = (UserModel)context.HttpContext.Items["User"];
            //Console.WriteLine("user");
            //Console.WriteLine(user.ToString());
            /* if (user == null)
             {
                 Console.WriteLine("user is null");
             }
             else
             {
                 //Console.WriteLine(user.AccessToken);
                // Console.WriteLine(user.Username);
             }*/
            if (user == null)
            {
                
                object a = new { isLoggedIn = false, message = "Unauthorized" };
                // not logged in or role not authorized
                context.Result = new JsonResult(a) { StatusCode = StatusCodes.Status401Unauthorized };
            }else
            if (user!=null && user.Usertype!=null)
            {
                object a;
                if (user != null)
                {
                    if(_role!=null && _role.Any() && !_role.Contains(user.Usertype))
                    {
                        a = new
                        {
                            isLoggedIn = true,
                            message = "Unuthorized"
                        };
                        status = StatusCodes.Status401Unauthorized;
                    }
                    else {
                        a = new
                        {
                            isLoggedIn = true,
                            message = "Authorized"
                        };
                        status = StatusCodes.Status200OK;
                        return;
                    }
                }
                else
                {
                    a = new { isLoggedIn = false, message = "Unauthorized" };
                    status = StatusCodes.Status401Unauthorized;
                }
                // not logged in or role not authorized
                context.Result = new JsonResult(a) { StatusCode = status };
            }
        }
    }
}