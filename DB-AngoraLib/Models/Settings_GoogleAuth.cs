using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Settings_GoogleAuth
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public static class Settings_GoogleAuthExtensions
    {
        public static IServiceCollection AddGoogleAuth(this IServiceCollection services, Settings_GoogleAuth settings)
        {
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
            });

            return services;
        }
    }
}
