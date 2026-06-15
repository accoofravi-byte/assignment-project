variable "aws_region" {
  default = "ap-south-1"
}

variable "project_name" {
  default = "assignment"
}

variable "db_username" {
  default = "postgres"
}

variable "db_password" {
  sensitive = true
}