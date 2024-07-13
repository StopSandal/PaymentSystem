Payments System Web API

This API supports CRUD operations for managing cards. Creating, confirmation, canceling and returning transactions.

Remeber, that api is just stub for main project EventSeller.

The solution consists of 4 projects:

1. Main Layer (Presentation) - [EventSeller](https://github.com/StopSandal/EventSeller)
2. Service Layer (Service) - [EventSeller.Services](https://github.com/StopSandal/EventSeller.Services)
3. Data Layer (Data) - [EventSeller.DataLayer](https://github.com/StopSandal/EventSeller.DataLayer)
4. Stub of Payment System (EXternal api) - [PaymentSystem](https://github.com/StopSandal/PaymentSystem)

To start project:

1. Create a database for your data on your server.
2. Update the connection string in `appsettings.json` to point to your database.
3. Finally, use Entity Framework tools to update your database schema.  Ensure that your project is configured to work with Entity Framework, specifically targeting the [PaymentSystem.DataLayer] project.
4. Set up runnable projects PaymentSystem 
