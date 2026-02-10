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

- `src/`: Source code for modules and main application.
- `tests/`: Unit and integration tests.
- `.github/`: CI/CD and AI instructions.
- `ProdSight.sln`: Solution file.

## Contributing

- Follow the guidelines in [.github/copilot/instructions.md](.github/copilot/instructions.md).
- Use English for all code, comments, and commits.
- Implement new modules using Onion Architecture.

## License

[Specify license if applicable]