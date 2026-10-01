namespace EcePoc.Console.Configuration;

public sealed class EceOptions
{
    public string BaseUrl { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string EmailAddress { get; set; } = "ecsUi@education.gov.uk";
}