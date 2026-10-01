namespace EcePoc.Console.Models;

public record ApiEnvelope<T>(
  T Data,
  Links? Links);