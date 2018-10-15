using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NotetasticApi.Common;
using NotetasticApi.Users;

namespace NotetasticApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var jwtSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"]));
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateAudience = false,
						ValidateIssuer = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = jwtSigningKey,
						ClockSkew = TimeSpan.FromSeconds(30)
					};
				});
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


			var mongoClient = new MongoClient(Configuration["mongo:connection_string"]);
			var db = mongoClient.GetDatabase(Configuration["mongo:db_name"]);
			
			services.AddSingleton<IMongoCollection<User>>(db.GetCollection<User>("Users"));
			services.AddSingleton<IMongoCollection<RefreshToken>>(db.GetCollection<RefreshToken>("RefreshTokens"));

			services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
			services.AddSingleton<IUserRepository, UserRepository>();

			services.AddSingleton<ITokenService>(new TokenService(jwtSigningKey));
			services.AddSingleton<IValidationService, ValidationService>();
			services.AddSingleton<IPasswordService, PasswordService>();

			services.AddSingleton<IUserService, UserService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseMvc();
		}
	}
}
