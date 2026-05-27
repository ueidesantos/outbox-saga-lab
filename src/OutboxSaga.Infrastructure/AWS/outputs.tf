output "bootstrap_brokers" {
  description = "Connection string for Kafka brokers"
  value       = aws_msk_cluster.lab_kafka.bootstrap_brokers
}

output "bootstrap_brokers_tls" {
  description = "TLS connection string for Kafka brokers"
  value       = aws_msk_cluster.lab_kafka.bootstrap_brokers_tls
}

output "vpc_id" {
  value = aws_vpc.msk_vpc.id
}
