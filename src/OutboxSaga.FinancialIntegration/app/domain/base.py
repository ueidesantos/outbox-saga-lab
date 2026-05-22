from abc import ABC
from datetime import datetime, timezone
from typing import List, Any
import uuid

class DomainEvent(ABC):
    def __init__(self):
        self.event_id = str(uuid.uuid4())
        self.occurred_on_utc = datetime.now(timezone.utc).isoformat()

class AggregateRoot:
    def __init__(self):
        self._domain_events: List[DomainEvent] = []

    def add_domain_event(self, event: DomainEvent):
        self._domain_events.append(event)

    def clear_domain_events(self):
        self._domain_events = []

    @property
    def domain_events(self) -> List[DomainEvent]:
        return self._domain_events
