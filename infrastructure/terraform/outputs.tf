output "environment" {
  description = "Environment associated with this Terraform state."
  value       = var.environment
}

output "deployment" {
  description = "Application deployment associated with this Terraform state."
  value       = var.deployment
}

output "api_app_name" {
  description = "API App Service name."
  value       = azurerm_linux_web_app.api.name
}

output "web_app_name" {
  description = "Web App Service name."
  value       = azurerm_linux_web_app.web.name
}

output "web_url" {
  description = "HTTPS App Service URL for the Basic Auth-protected Web app."
  value       = "https://${azurerm_linux_web_app.web.default_hostname}"
}

output "resource_group_name" {
  description = "Resource group containing all resources for this deployment."
  value       = azurerm_resource_group.application.name
}
