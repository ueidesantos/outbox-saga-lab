import asyncio
import json
import logging
import os
from ..infrastructure.persistence.mongo_context import MongoContext, MongoConfig
from ..infrastructure.persistence.financial_repository import MongoFinancialRepository
from ..infrastructure.persistence.outbox_repository import MongoOutboxRepository
from ..infrastructure.messaging.kafka_client import KafkaConsumer, KafkaProducer, KafkaConfig
from ..application.payment_handler import PaymentProcessedHandler

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

async def run_publisher(outbox_repo, kafka_producer):
    logger.info("Outbox Publisher started.")
    while True:
        try:
            messages = await outbox_repo.get_unpublished()
            for msg in messages:
                logger.info(f"Publishing event {msg['event_type']} for aggregate {msg['aggregate_id']}")

                headers = [
                    ("CorrelationId", msg['correlation_id'].encode('utf-8')),
                    ("CausationId", msg['id'].encode('utf-8'))
                ]

                kafka_producer.publish(
                    topic="financial-entry-recorded",
                    key=msg['aggregate_id'],
                    value=msg['payload'],
                    headers=headers
                )

                await outbox_repo.mark_as_published(msg['id'])
        except Exception as e:
            logger.error(f"Error in publisher: {e}")

        await asyncio.sleep(5)

async def run_consumer(kafka_consumer, handler):
    logger.info("Kafka Consumer started.")
    kafka_consumer.subscribe(["payment-processed"])

    while True:
        msg = kafka_consumer.consume(timeout=1.0)
        if msg is None:
            await asyncio.sleep(0.1)
            continue
        if msg.error():
            logger.error(f"Consumer error: {msg.error()}")
            continue

        try:
            val = json.loads(msg.value().decode('utf-8'))

            # Extract headers for tracing
            correlation_id = ""
            causation_id = ""
            headers = msg.headers()
            if headers:
                for k, v in headers:
                    if k == "CorrelationId":
                        correlation_id = v.decode('utf-8')
                    if k == "CausationId":
                        causation_id = v.decode('utf-8')

            await handler.handle(val, correlation_id, causation_id)
        except Exception as e:
            logger.error(f"Error processing message: {e}")

async def main():
    mongo_config = MongoConfig()
    mongo_context = MongoContext(mongo_config)

    financial_repo = MongoFinancialRepository(mongo_context)
    outbox_repo = MongoOutboxRepository(mongo_context)

    kafka_config = KafkaConfig()
    kafka_producer = KafkaProducer(kafka_config)
    kafka_consumer = KafkaConsumer(kafka_config)

    handler = PaymentProcessedHandler(financial_repo, outbox_repo, mongo_context)

    await asyncio.gather(
        run_consumer(kafka_consumer, handler),
        run_publisher(outbox_repo, kafka_producer)
    )

if __name__ == "__main__":
    asyncio.run(main())
