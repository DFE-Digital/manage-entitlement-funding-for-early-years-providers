using System.Xml.XPath;
using EcePoc.Console.Configuration;
using EcePoc.Console.Models;
using EcePoc.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddUserSecrets<Program>(optional: false);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<EceOptions>(context.Configuration.GetSection("Ece"));

                services.AddTransient<AuthHandler>();

                services.AddHttpClient<AuthenticationService>((sp, client) =>
                 {
                     var options = context.Configuration.GetSection("Ece").Get<EceOptions>()!;
                     client.BaseAddress = new Uri(options.BaseUrl);
                 });

                services.AddHttpClient<EligibilityCheckingClient>((sp, c) =>
                 {
                     var options = context.Configuration.GetSection("Ece").Get<EceOptions>()!;
                     c.BaseAddress = new Uri(options.BaseUrl);
                 })
                 .AddHttpMessageHandler<AuthHandler>();
            }).Build();

if (args.Length < 4)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  wf <eligibilityCode> <dob> <lastName> <nin>");
    Console.WriteLine("  2y <dob> <lastName> <nin>");
    return;
}

var command = args[0].ToLowerInvariant();

var client = host.Services.GetRequiredService<EligibilityCheckingClient>();

EligibilityData? result = null;

switch (command)
{
    case "wf" when args.Length >= 5:
        {
            var wfRequest = new WorkingFamiliesData(
                EligibilityCode: args[1],
                DateOfBirth: args[2],
                LastName: args[3],
                NationalInsuranceNumber: args[4]
            );

            result = await client.CheckWorkingFamiliesAsync(wfRequest);
            break;
        }
    case "2y" when args.Length >= 4:
        {
            var ey2Request = new TwoYearOfferData(
                DateOfBirth: args[1],
                LastName: args[2],
                NationalInsuranceNumber: args[3]
            );

            result = await client.CheckTwoYearOfferAsync(ey2Request);
            break;
        }
    default:
        Console.WriteLine("Invalid command or insufficient arguments");
        return;
}


Console.WriteLine(result);
