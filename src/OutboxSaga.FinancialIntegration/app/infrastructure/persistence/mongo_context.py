import os
from motor.motor_asyncio import AsyncIOMotorClient

class MongoConfig:
    def __init__(self):
        self.connection_string = os.getenv("MONGO_CONNECTION_STRING", "mongodb://localhost:27017")
        self.database_name = os.getenv("MONGO_DATABASE_NAME", "FinancialDb")

class MongoContext:
    def __init__(self, config: MongoConfig):
        self.client = AsyncIOMotorClient(config.connection_string)
        self.db = self.client[config.database_name]
        self.entries = self.db["financial_entries"]
        self.outbox = self.db["outbox_messages"]
        self.inbox = self.db["inbox_messages"] # For idempotency

    async def get_session(self):
        return await self.client.start_session()
