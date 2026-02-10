# AI Collaboration Instructions for ProdSight Project

## Introduction
This file contains instructions for AI agents (e.g., Copilot, GitHub Copilot) on collaborating with the ProdSight (.NET) project. ProdSight is a modular application for visualizing production data related to factory floors and the operation of machines/production lines. Modules are typically implemented using Onion Architecture. Identity provider is Keycloak.

## General Rules
- Analyze only existing code in the repo (e.g., [ProdSight.sln](ProdSight.sln), IDE configuration files in `.idea/`).
- Do not modify IDE configuration files (e.g., [workspace.xml](.idea/.idea.ProdSight/.idea/workspace.xml)) without permission.
- If the project expands, follow security guidelines: do not generate code with sensitive data, use only approved models.
- All generated files, unless otherwise specified, must be in English. This includes comments and commit messages.
- Implement new modules using Onion Architecture (Domain, Application, Infrastructure, Presentation layers).
- Integrate Keycloak for identity and access management in authentication-related code.
- Use Minimal API for defining endpoints instead of controllers.

## Example Task
1. Analyze [ProdSight.sln](ProdSight.sln) and suggest project structure (.csproj, code) following modular Onion Architecture.
2. Generate a new module for production data visualization, including Domain, Application, Infrastructure, and Presentation layers.
3. Integrate Keycloak authentication in the module.
4. Define Minimal API endpoints for the module instead of controllers.
5. Update this file with new instructions after adding files.

## Security
- Do not store keys in the repo; use `.env` in the future.
- If output contains errors, report in comments.

## Contact
For questions, consult the repo owner.
