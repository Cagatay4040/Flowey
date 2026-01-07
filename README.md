# Flowey - Project Management System
Flowey is a Jira-like project management REST API built to demonstrate Clean Architecture and Solid Principles. It manages projects, tasks, and steps with a secure, granular permission system.

Key Technical Features:

- Architecture: N-Layer / Onion Architecture (Strict separation of concerns).

- Security: JWT Authentication & Custom Role-Based Access Control (RBAC).

- AOP Implementation: Custom ActionFilters for dynamic permission checking using Reflection to inspect DTOs and Routes without cluttering business logic.

- Data Access: Entity Framework Core with Repository Pattern.

- Best Practices: Dependency Injection, AutoMapper, Custom Exception Handling, and Centralized Constant Management.

## 🚧 Status & Roadmap
The backend logic is **90% complete**.
- [x] Database Design & Migrations
- [x] CRUD Operations for Tasks/Boards
- [x] User Authentication (JWT)
- [ ] Unit Tests (In Progress)
- [ ] Frontend Integration (Planned)
