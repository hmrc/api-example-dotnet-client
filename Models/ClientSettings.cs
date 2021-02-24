public class ClientSettings
{
  // Secrets should not be stored in plain test json files !!!
  // See https://docs.microsoft.com/en-gb/aspnet/core/security/app-secrets
  public string Uri { get; set; }
  public string ServerToken { get; set; }
  public string ClientId { get; set; }
  public string ClientSecret { get; set; }
  public string OauthCallbackUri { get; set; }
}