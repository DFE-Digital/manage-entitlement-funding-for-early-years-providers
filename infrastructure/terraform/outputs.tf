output "environment" {
  description = "Environment associated with this Terraform state."
  value       = var.environment
}

output "deployment" {
  description = "Application deployment associated with this Terraform state."
  value       = var.deployment
}

output "resource_group_name" {
  description = "Resource group containing all resources for this deployment."
  value       = azurerm_resource_group.application.name
}
