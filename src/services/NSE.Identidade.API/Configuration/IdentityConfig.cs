using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using System.Text;

namespace NSE.Identidade.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            #region Configuracao entity framework context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion

            #region Configuracao Identity
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            #endregion


            #region Configuracao Autenticacao token JWT
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSessings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSessings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.RequireHttpsMetadata = true;
                bearerOptions.SaveToken = true;
                bearerOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = appSessings.ValidoEm,
                    ValidIssuer = appSessings.Emissor,
                };
            });
            #endregion

            return services;
        }

        public static WebApplication UserIdentityConfiguration(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
