# MatchupCompanion
Un proyecto web full-stack construido con **.NET 8 (o 7)** que permite a los jugadores de League of Legends consultar, crear y compartir estrategias para enfrentamientos (matchups) específicos.

Este proyecto fue creado para demostrar habilidades en el ecosistema .NET, incluyendo **ASP.NET Core Web API**, **Blazor WebAssembly** y **Entity Framework Core**.

---

## 📖 Descripción del Proyecto

Esta aplicación web resuelve un problema común para los jugadores de LoL: "¿Cómo juego este matchup?". La aplicación permite a los usuarios:

1.  **Seleccionar** dos campeones (tu campeón y el campeón enemigo).
2.  **Ver** instantáneamente los consejos, nivel de dificultad y estrategias enviadas por otros usuarios para ese enfrentamiento.
3.  **Contribuir** añadiendo sus propios consejos para un matchup que aún no existe o que quieran complementar.

Todo esto se gestiona a través de una interfaz de usuario reactiva construida con Blazor que consume una API de backend de ASP.NET Core.

---

## 🛠️ Stack de Tecnologías

Este proyecto utiliza una arquitectura de Aplicación Blazor WebAssembly Hospedada en ASP.NET Core, lo que permite un desarrollo full-stack cohesivo.

### Backend (`.Server`)
* **ASP.NET Core Web API (.NET 8 / 7)**: Para construir los endpoints RESTful que gestionan los datos.
* **Entity Framework Core 8 / 7**: Para el ORM (mapeo objeto-relacional) y la comunicación con la base de datos.
* **SQL Server** (o `[Tu Base de Datos, ej: PostgreSQL, SQLite]`): Como motor de la base de datos.

### Frontend (`.Client`)
* **Blazor WebAssembly**: Para construir una SPA (Single Page Application) interactiva y de alto rendimiento que se ejecuta en el navegador.
* **C#**: Lógica del cliente escrita en C# en lugar de JavaScript.
* **CSS / Bootstrap**: Para el diseño y la interfaz responsiva.

### Compartido (`.Shared`)
* Modelos de datos y DTOs (Data Transfer Objects) compartidos entre el cliente y el servidor para asegurar consistencia.

---

## ✨ Características Principales

* **API RESTful Completa**: Endpoints para operaciones CRUD (Crear, Leer, Actualizar, Borrar) sobre Campeones y Matchups.
* **Interfaz Reactiva**: Componentes de Blazor que reaccionan a la selección del usuario sin recargar la página.
* **Persistencia de Datos**: Uso de Entity Framework Core para almacenar y recuperar matchups de forma eficiente.
* **Validación de Formularios**: Manejo de la entrada del usuario tanto en el cliente (Blazor) como en el servidor (API).

---

## 📈 Posibles Mejoras Futuras

Este proyecto tiene una base sólida y puede expandirse con nuevas características:

* **Autenticación de Usuarios**: Implementar **ASP.NET Core Identity** para que los usuarios se registren y puedan editar/eliminar *sus propios* consejos.
* **Sistema de Votación**: Permitir a los usuarios votar (upvote/downvote) los consejos más útiles.
* **Integración con la API de Riot**: Poblar la base de datos de campeones automáticamente usando la [API oficial de Riot Games](https://developer.riotgames.com/).
* **Estadísticas Avanzadas**: Calcular *win rates* basados en los datos de la API de Riot.

---

*Proyecto creado por `Alejandro Burciaga Calzadillas` *
