variable "environment" {
  description = "Target deployment environment."
  type        = string

  validation {
    condition     = var.environment == "development"
    error_message = "Environment must be development."
  }
}
