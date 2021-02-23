using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace testWebApp
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
      services.Configure<ClientSettings>(Configuration);

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "HMRC";
      })
      .AddCookie()
      .AddOAuth("HMRC", options =>
      {
        options.ClientId = Configuration["clientId"];
        options.ClientSecret = Configuration["clientSecret"];
        options.CallbackPath = new PathString("/oauth20/callback");
        options.Scope.Add("hello");

        options.AuthorizationEndpoint = Configuration["uri"] + "/oauth/authorize";
        options.TokenEndpoint = Configuration["uri"] + "/oauth/token";
      });

      services.AddRazorPages(options =>
      {
        options.Conventions.AddPageRoute("/HelloWorld/Index", "");
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
      }

      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseAuthentication();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();

        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=HelloWorld}/{action=Index}");
      });
    }
  }
}