## Main

![Build - Test - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/build-test.yml/badge.svg?branch=main&event=push)

![Code Analysis - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/code-analysis.yml/badge.svg?branch=main&event=push)

## Develop

![Build - Test - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/build-test.yml/badge.svg?branch=develop&event=push)

![Code Analysis - Main](https://github.com/IngSoft-DA2/cardozo-pons-velazquez/actions/workflows/code-analysis.yml/badge.svg?branch=develop&event=push)


# 📘 Obligatorio – Diseño de Aplicaciones 2  

Este repositorio contiene el desarrollo del **obligatorio de Diseño de Aplicaciones 2 (DA2)**.  
El proyecto consiste en una aplicación para un parque temático virtual.  

## 🗂️ Estructura de la solución  

/Obligatorio.sln # Archivo de solución .NET

/src
/Parque.WebApi # Capa de presentación (endpoints HTTP)
/Parque.Aplicacion # Casos de uso y orquestación de lógica
/Parque.Dominio # Entidades del dominio y reglas de negocio
/Parque.Infraestructura # Acceso a datos, servicios externos, implementaciones

/tests
/Parque.Aplicacion.Tests # Proyecto de pruebas (MSTest)