using System.Text.Json.Serialization;

namespace EcePoc.Console.Models;

public record Links(
    [property: JsonPropertyName("get_EligibilityCheck")] string? GetEligibilityCheck,
    [property: JsonPropertyName("put_EligibilityCheckProcess")] string? PutEligibilityCheckProcess,
    [property: JsonPropertyName("get_EligibilityCheckStatus")] string? GetEligibilityCheckStatus
);