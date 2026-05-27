resource "aws_msk_cluster" "lab_kafka" {
  cluster_name           = "${var.project_name}-cluster"
  kafka_version          = var.kafka_version
  number_of_broker_nodes = 2

  broker_node_group_info {
    instance_type = var.kafka_instance_type
    client_subnets = aws_subnet.msk_subnets[*].id
    security_groups = [aws_security_group.msk_sg.id]

    storage_info {
      ebs_storage_info {
        volume_size = 10
      }
    }
  }

  encryption_info {
    encryption_at_rest_kms_key_arn = aws_kms_key.msk_key.arn
    encryption_in_transit {
      client_in_transit = "TLS_PLAINTEXT" # For the lab simplification, but ARCH RECOMMENDATION: Use TLS
      in_cluster = true
    }
  }

  logging_info {
    broker_logs {
      cloudwatch_logs {
        enabled   = true
        log_group = aws_cloudwatch_log_group.msk_logs.name
      }
    }
  }

  tags = {
    Environment = var.environment
  }
}

resource "aws_kms_key" "msk_key" {
  description = "KMS key for MSK cluster encryption"
}

resource "aws_cloudwatch_log_group" "msk_logs" {
  name              = "/aws/msk/${var.project_name}-cluster"
  retention_in_days = 7
}

resource "aws_msk_configuration" "lab_config" {
  kafka_versions = [var.kafka_version]
  name           = "${var.project_name}-config"

  server_properties = <<PROPERTIES
auto.create.topics.enable = true
delete.topic.enable = true
PROPERTIES
}
