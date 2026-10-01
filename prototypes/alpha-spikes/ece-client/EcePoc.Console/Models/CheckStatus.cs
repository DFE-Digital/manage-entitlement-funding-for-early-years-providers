namespace EcePoc.Console.Models;

public record CheckStatus(
  string? Status,
  string? Tier,
  string? ErrorCode);