import json
from datetime import datetime, timezone
import uuid
from typing import List, Optional
from motor.motor_asyncio import AsyncIOMotorClientSession
from .mongo_context import MongoContext

class OutboxMessage:
    def __init__(self, aggregate_id: str, event_type: str, payload: str, correlation_id: str, causation_id: str):
        self.id = str(uuid.uuid4())
        self.aggregate_id = aggregate_id
        self.event_type = event_type
        self.payload = payload
        self.correlation_id = correlation_id
        self.causation_id = causation_id
        self.created_at_utc = datetime.now(timezone.utc).isoformat()
        self.published_at_utc: Optional[str] = None

    def to_dict(self):
        return self.__dict__

class MongoOutboxRepository:
    def __init__(self, context: MongoContext):
        self.context = context

    async def add(self, message: OutboxMessage, session: AsyncIOMotorClientSession):
        await self.context.outbox.insert_one(message.to_dict(), session=session)

    async def get_unpublished(self, limit: int = 20) -> List[dict]:
        cursor = self.context.outbox.find({"published_at_utc": None}).limit(limit)
        return await cursor.to_list(length=limit)

    async def mark_as_published(self, message_id: str):
        await self.context.outbox.update_one(
            {"id": message_id},
            {"$set": {"published_at_utc": datetime.now(timezone.utc).isoformat()}}
        )
