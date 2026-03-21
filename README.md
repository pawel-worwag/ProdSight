# ProdSight

ProdSight is a modular application for visualizing production data related to factory floors and the operation of machines/production lines. The project is built using .NET and follows modular architecture principles, with modules typically implemented using Onion Architecture. Identity provider is Keycloak.

## Architecture

- **Modular Design**: The application is divided into independent modules for scalability and maintainability.
- **Onion Architecture**: Each module follows Onion Architecture (Domain, Application, Infrastructure, Presentation layers).
- **Identity**: Keycloak is used as the identity and access management provider.

## Technologies

- .NET (target framework: .NET 10)
- Keycloak for authentication and authorization

## Project Structure

- `src/ProdSight.Api/`: Main API application with endpoints, middleware, and module registration.
- `src/ProdSight.Api.Modules/`: Modular components.
  - `IdentityModule/`: Handles identity and authentication (Domain, Application, Infrastructure, Presentation layers).
  - `MeasurementModule/`: Manages production data and measurements (Domain, Application, Infrastructure, Presentation layers).
- `src/ProdSight.Api.Shared/`: Shared utilities, exceptions, and module interfaces.
- `src/ProdSight.Api.Shared.DTOs/`: Data Transfer Objects for API responses.
- `src/ProdSight.Frontend/`: Frontend application (Blazor WebAssembly).
- `.github/`: CI/CD and AI instructions.
- `ProdSight.sln`: Solution file.

## Reverse Proxy Note (Keycloak + Chrome)

This setting fixes issues with access token handling in Chrome (token not available after navigation between pages).

Set this on the Nginx location that proxies to Keycloak:

```nginx 
location / {

  ...

  # IMPORTANT
  proxy_cookie_flags ~ secure samesite=none;

  ...

}
```

## Contributing

- Follow the guidelines in [.github/copilot/instructions.md](.github/copilot/instructions.md).
- Use English for all code, comments, and commits.
- Implement new modules using Onion Architecture.