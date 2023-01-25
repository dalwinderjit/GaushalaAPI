using GaushalaAPI.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
//using AuthApi.Helpers;
//using AuthApi.Services;

namespace GaushalaAPI.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            Console.Write("JWt MIDDLEWARE");
            _next = next;
        }
        /*public async Task Invoke(HttpContext context, IConfiguration iconfiguration, IJwtUtils jwtUtils)
        {
            Console.WriteLine("HELLO INVOke");
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Console.WriteLine(token);
            var userId = jwtUtils.ValidateJwtToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                UsersContext usersContext = new UsersContext(iconfiguration);
                context.Items["User"] = usersContext.GetUserById(userId.Value);
            }
            await _next(context);
        }*/
        public async Task Invoke(HttpContext context, IConfiguration iconfiguration, IJwtUtils jwtUtils)
        {
            try
            {
                Console.WriteLine("HELLO INVOke");
                //context.Response.Cookies.Append("__secure_token", "LKJLk");
                string token = null;
                context.Request.Cookies.TryGetValue("__secure_token", out token);
                Console.WriteLine("TOKEN Cookie" + token);
                //token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                //Console.WriteLine(token);
                var userId = jwtUtils.ValidateJwtToken(token);
                if (userId != null)
                {
                    // attach user to context on successful jwt validation
                    UsersContext usersContext = new UsersContext(iconfiguration);
                    context.Items["User"] = usersContext.GetUserById(userId.Value);
                }
                await _next(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}