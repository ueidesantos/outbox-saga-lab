from motor.motor_asyncio import AsyncIOMotorClientSession
from .mongo_context import MongoContext
from ...domain.financial_entry import FinancialEntry

class MongoFinancialRepository:
    def __init__(self, context: MongoContext):
        self.context = context

    async def add(self, entry: FinancialEntry, session: AsyncIOMotorClientSession):
        await self.context.entries.insert_one(entry.__dict__, session=session)

    async def exists_for_order(self, order_id: str) -> bool:
        count = await self.context.entries.count_documents({"order_id": order_id})
        return count > 0
