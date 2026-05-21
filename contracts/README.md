# Event Contracts

Esta pasta guarda os contratos públicos de integração entre bounded contexts.

Eles não substituem o domínio de nenhum serviço. A ideia é documentar o que atravessa o broker, de forma neutra para qualquer stack.

No laboratório, os eventos serão publicados no **AWS MSK (Kafka)**.

## Convenção

Cada mensagem publicada no broker deve seguir um envelope comum:

```json
{
  "message_id": "uuid",
  "event_type": "payments.payment-approved",
  "event_version": 1,
  "source": "payments",
  "correlation_id": "uuid",
  "causation_id": "uuid",
  "occurred_on_utc": "2026-05-21T10:00:00Z",
  "payload": {}
}
```

## Topics

```text
orders.events.v1
payments.events.v1
finance.events.v1
shipping.events.v1
```

## Regra De Boundary

Serviços podem compartilhar contratos públicos versionados.

Serviços não devem compartilhar código de domínio, DTOs internos ou bibliotecas específicas de outro bounded context.

