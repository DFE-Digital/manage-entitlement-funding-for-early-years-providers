variable "environment" {
  description = "Target deployment environment."
  type        = string

  validation {
    condition     = var.environment == "development"
    error_message = "Environment must be development."
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

variable "location" {
  description = "Azure region for application resources."
  type        = string
  default     = "uksouth"
}
