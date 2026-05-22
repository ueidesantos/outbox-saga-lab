import os
import json
import logging
from confluent_kafka import Producer, Consumer, KafkaException
from tenacity import retry, wait_exponential, stop_after_attempt

logger = logging.getLogger(__name__)

class KafkaConfig:
    def __init__(self):
        self.bootstrap_servers = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
        self.group_id = os.getenv("KAFKA_GROUP_ID", "financial-integration-group")
        self.auto_offset_reset = "earliest"

class KafkaProducer:
    def __init__(self, config: KafkaConfig):
        producer_config = {
            'bootstrap.servers': config.bootstrap_servers,
            'acks': 'all',
            'enable.idempotence': True
        }
        self.producer = Producer(producer_config)

    @retry(wait=wait_exponential(multiplier=1, min=2, max=10), stop=stop_after_attempt(5))
    def publish(self, topic: str, key: str, value: str, headers: list):
        try:
            self.producer.produce(topic, key=key, value=value, headers=headers)
            self.producer.flush()
        except KafkaException as e:
            logger.error(f"Failed to publish to Kafka: {e}")
            raise

class KafkaConsumer:
    def __init__(self, config: KafkaConfig):
        consumer_config = {
            'bootstrap.servers': config.bootstrap_servers,
            'group.id': config.group_id,
            'auto.offset.reset': config.auto_offset_reset
        }
        self.consumer = Consumer(consumer_config)

    def subscribe(self, topics: list):
        self.consumer.subscribe(topics)

    def consume(self, timeout=1.0):
        return self.consumer.poll(timeout)

    def close(self):
        self.consumer.close()
