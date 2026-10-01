using System.Text.Json.Serialization;

namespace EcePoc.Console.Models;

public record EligibilityData(
    string? LastName,
    DateOnly? DateOfBirth,
    DateOnly? ValidityStartDate,
    DateOnly? ValidityEndDate,
    DateOnly? GracePeriodEndDate,
    string? EligibilityCode,
    string? NationalInsuranceNumber,
    string? Status,
    [property: JsonPropertyName("created")] DateTime? CreatedAt
)
{
    public override string ToString()
    {
        return string.Format(@"Eligibility:
          Last name: {0}
          Date of birth: {1}
          Validity start date: {2}
          Validity end date: {3}
          Grace period end date: {4}
          Eligibility code: {5}
          NI number: {6}
          Status: {7}
          Created at: {8}",
        LastName,
        DateOfBirth,
        ValidityStartDate,
        ValidityEndDate,
        GracePeriodEndDate,
        EligibilityCode,
        NationalInsuranceNumber,
        Status,
        CreatedAt);
    }

};