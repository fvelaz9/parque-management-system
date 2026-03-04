## Main

![Build - Test - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/build-test.yml/badge.svg?branch=main&event=push)

![Code Analysis - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/code-analysis.yml/badge.svg?branch=main&event=push)

## Develop

![Build - Test - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/build-test.yml/badge.svg?branch=develop&event=push)

![Code Analysis - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/code-analysis.yml/badge.svg?branch=develop&event=push)


# 🎢 Parque Management System

Academic project developed for the course **Application Design 2**.

This project consists of a backend system designed to manage a virtual theme park, applying layered architecture principles and clean separation of concerns.

---

## 🏗 Architecture

The solution follows a layered architecture inspired by Domain-Driven Design concepts:

Obligatorio.sln

src/
├─ Parque.WebApi/          → Presentation layer (HTTP endpoints)  
├─ Parque.Aplicacion/      → Application layer (use cases & business logic)  
├─ Parque.Dominio/         → Domain layer (entities & business rules)  
└─ Parque.Infraestructura/ → Infrastructure layer (data access & external services)  

tests/
└─ Parque.Aplicacion.Tests/ → Unit tests (MSTest)

### Architectural Highlights

- Clear separation between domain, application and infrastructure layers  
- Dependency Injection  
- RESTful API design  
- Business rule validation at domain level  
- Unit testing for application layer  
- CI pipelines configured with GitHub Actions  
- Code analysis integration  

---

## 🛠 Technologies Used

- .NET  
- C#  
- ASP.NET Web API  
- MSTest  
- GitHub Actions (CI/CD)  
- Docker (Compose configuration)  

---

## 👨‍💻 My Contribution

This was a team-based academic project.  

My main contributions included:

- Implementation of application use cases  
- REST API endpoint development  
- Business logic validation  
- Writing and maintaining unit tests  
- CI pipeline configuration  
- Refactoring and architectural improvements  

This repository is published as part of my personal portfolio to showcase backend architecture design and clean code practices.

---

## 📌 Notes

This project was developed in an academic context and does not represent a production-ready system.  
It is shared for educational and portfolio purposes.
