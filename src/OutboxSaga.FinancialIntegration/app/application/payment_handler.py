import json
import logging
from ..domain.financial_entry import FinancialEntry
from ..infrastructure.persistence.outbox_repository import OutboxMessage

logger = logging.getLogger(__name__)

class PaymentProcessedHandler:
    def __init__(self, financial_repo, outbox_repo, mongo_context):
        self.financial_repo = financial_repo
        self.outbox_repo = outbox_repo
        self.mongo_context = mongo_context

    async def handle(self, event_data: dict, correlation_id: str, causation_id: str):
        order_id = event_data.get("OrderId")
        amount = event_data.get("Amount", 0.0) # In a real fat event this would be there or looked up

        # Need to handle the fact that C# event names might differ or use different casing
        if not order_id:
            logger.error("OrderId missing in event data")
            return

        # Idempotency check
        if await self.financial_repo.exists_for_order(order_id):
            logger.info(f"Financial entry for Order {order_id} already exists. Skipping.")
            return

        entry = FinancialEntry(order_id, amount)

        async with await self.mongo_context.get_session() as session:
            async with session.start_transaction():
                await self.financial_repo.add(entry, session)

                for event in entry.domain_events:
                    outbox_msg = OutboxMessage(
                        aggregate_id=entry.id,
                        event_type=event.__class__.__name__,
                        payload=json.dumps(event.__dict__),
                        correlation_id=correlation_id,
                        causation_id=causation_id
                    )
                    await self.outbox_repo.add(outbox_msg, session)

                entry.clear_domain_events()

        logger.info(f"Financial entry {entry.id} recorded for order {order_id}")
