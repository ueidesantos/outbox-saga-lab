from datetime import datetime, timezone
import uuid
from .base import AggregateRoot, DomainEvent

class FinancialEntryRecorded(DomainEvent):
    def __init__(self, entry_id: str, order_id: str, amount: float):
        super().__init__()
        self.entry_id = entry_id
        self.order_id = order_id
        self.amount = amount

class FinancialEntry(AggregateRoot):
    def __init__(self, order_id: str, amount: float):
        super().__init__()
        self.id = str(uuid.uuid4())
        self.order_id = order_id
        self.amount = amount
        self.status = "RECORDED"
        self.created_at_utc = datetime.now(timezone.utc).isoformat()

        self.add_domain_event(FinancialEntryRecorded(self.id, self.order_id, self.amount))

    @staticmethod
    def rehydrate(id: str, order_id: str, amount: float, status: str, created_at_utc: str):
        entry = FinancialEntry.__new__(FinancialEntry)
        super(FinancialEntry, entry).__init__()
        entry.id = id
        entry.order_id = order_id
        entry.amount = amount
        entry.status = status
        entry.created_at_utc = created_at_utc
        return entry
