### Version 1.0.0

- New Feature: NServiceBus integration module supporting transport configuration, message handler generation, recoverability policies, SQL Persistence transactional outbox, and multi-broker co-existence.
- Improvement: UnitOfWork/TransactionScope co-existence logic moved to `Intent.EntityFrameworkCore` (v5.0.45+); install that module alongside this one for full SQL Persistence outbox support.
