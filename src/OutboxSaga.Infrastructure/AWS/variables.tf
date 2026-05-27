variable "aws_region" {
  description = "AWS Region to deploy resources"
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Project name for resource tagging"
  type        = string
  default     = "outbox-saga-lab"
}

variable "environment" {
  description = "Environment name"
  type        = string
  default     = "dev"
}

variable "kafka_instance_type" {
  description = "Instance type for MSK broker nodes"
  type        = string
  default     = "kafka.t3.small"
}

variable "kafka_version" {
  description = "Kafka version for MSK"
  type        = string
  default     = "3.6.0"
}
