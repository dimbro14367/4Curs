using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Owin;
using InsuranceAPI.Services;

[assembly: OwinStartup(typeof(InsuranceAPI.App_Start.Startup))]

namespace InsuranceAPI.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = JwtService.GetIssuer(),
                    ValidateAudience = true,
                    ValidAudience = JwtService.GetAudience(),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JwtService.GetSecurityKey(),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }
            });
        }
    }
}