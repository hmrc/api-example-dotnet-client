using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.AspNetCore;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;

using Serilog;

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

      services.Configure<CookiePolicyOptions>(options =>
      {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddSession(options =>
      {
          options.IdleTimeout = TimeSpan.FromSeconds(10);
          options.Cookie.HttpOnly = true;
          options.Cookie.IsEssential = true;
      });

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
        options.CallbackPath = new PathString(Configuration["callbackPath"]);
        options.Scope.Add("hello");
        options.SaveTokens = true;

        options.AuthorizationEndpoint = Configuration["uri"] + "/oauth/authorize";
        options.TokenEndpoint = Configuration["uri"] + "/oauth/token";
      });

      // Add App Restricted Endpoint Support
      services.AddAccessTokenManagement(options =>
      {
        options.Client.Clients.Add("hmrcAppRestrictedTokenMgmt", new ClientCredentialsTokenRequest
        {
          Address = Configuration["uri"] + "/oauth/token",
          ClientId = Configuration["clientId"],
          ClientSecret = Configuration["clientSecret"],
          Scope = "hello",
          GrantType = "client_credentials"
        });
      });

      services.AddClientAccessTokenHttpClient("hmrcAppRestrictedClient", configureClient: client =>
      {
          client.BaseAddress = new Uri(Configuration["uri"]);
          client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
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
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthorization();
      app.UseAuthentication();
      app.UseSession();
      app.UseCookiePolicy();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();

        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=HelloWorld}/{action=Index}"
        );
      });
    }
  }
}
