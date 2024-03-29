using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Controllers
{
  public class HelloWorldController : Controller
  {
    private const string api = "hello (v1.0)";
    private const string unrestrictedEndpoint = "/world";
    private const string appRestrictedEndpoint = "/application";
    private const string userRestrictedEndpoint = "/user";

    private readonly ClientSettings _clientSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public HelloWorldController(IOptions<ClientSettings> clientSettingsOptions, IHttpClientFactory httpClientFactory)
    {
      _clientSettings = clientSettingsOptions.Value;
      _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
      ViewData["service"] = api;
      ViewData["unrestrictedEndpoint"] = unrestrictedEndpoint;
      ViewData["appRestrictedEndpoint"] = appRestrictedEndpoint;
      ViewData["userRestrictedEndpoint"] = userRestrictedEndpoint;
      return View();
    }

    public async Task<ContentResult> UnrestrictedCall()
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(_clientSettings.Uri);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));

        HttpResponseMessage response = await client.GetAsync("hello/world");

        String resp = await response.Content.ReadAsStringAsync();
        return Content(resp, "application/json");
      }
    }

    public async Task<ContentResult> AppRestrictedCall()
    {
      using(var client = _httpClientFactory.CreateClient("hmrcAppRestrictedClient"))
      {
        var response = await client.GetStringAsync("/hello/application");
        return Content(response, "application/json");
      }
    }

    public async Task<IActionResult> UserRestrictedCall()
    {
      string accessToken = await HttpContext.GetTokenAsync("access_token");
      
      if(accessToken != null)
      {
        using (var client = new HttpClient())
        {
          client.BaseAddress = new Uri(_clientSettings.Uri);
          client.DefaultRequestHeaders.Accept.Clear();
          client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

          HttpResponseMessage response = await client.GetAsync("hello/user");

          String resp = await response.Content.ReadAsStringAsync();
          return Content(resp, "application/json");
        }
      }
      else
      {
        return Challenge(new AuthenticationProperties() { RedirectUri = "/HelloWorld/UserRestrictedCall" }, "HMRC");
      }
    }
  }
}