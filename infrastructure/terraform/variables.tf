variable "environment" {
  description = "Target deployment environment."
  type        = string

  validation {
    condition     = contains(["development", "staging", "production"], var.environment)
    error_message = "Environment must be development, staging or production."
  }
}

variable "deployment" {
  description = "Persistent main deployment or an ephemeral PR deployment."
  type        = string

  validation {
    condition     = can(regex("^(main|pr-[1-9][0-9]*)$", var.deployment))
    error_message = "Deployment must be main or pr-<positive PR number>."
  }
}

variable "resource_group_name" {
  description = "Dedicated application resource group, separate from the backend."
  type        = string
}

variable "app_service_sku" {
  description = "App Service plan SKU shared by the API and Web apps."
  type        = string
  default     = "F1"
}

variable "development_basic_auth_password" {
  description = "Shared password for the development Web app access gate."
  type        = string
  sensitive   = true
  default     = null

  validation {
    condition = var.environment != "development" || (
      var.development_basic_auth_password == null ? false : length(trimspace(var.development_basic_auth_password)) >= 32
    )
    error_message = "Development deployments require a Basic Auth password of at least 32 characters."
  }
}

variable "location" {
  description = "Azure region for application resources."
  type        = string
  default     = "uksouth"
}
