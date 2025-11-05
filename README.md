# ReservaYa 🏫: Sistema de Reserva de Aulas y Laboratorios

![GitHub Language Count](https://img.shields.io/github/languages/count/Fabio-Menjivar/ReservaYa)
![GitHub Top Language](https://img.shields.io/github/languages/top/Fabio-Menjivar/ReservaYa)
![GitHub Stars](https://img.shields.io/github/stars/Fabio-Menjivar/ReservaYa?style=social)
![GitHub Last Commit](https://img.shields.io/github/last-commit/Fabio-Menjivar/ReservaYa)

**ReservaYa** es un sistema de gestión diseñado específicamente para **optimizar la asignación y reserva de recursos académicos** como **Aulas, Laboratorios y Salas de Reuniones** dentro de una institución educativa. Permite a docentes, estudiantes y personal administrativo verificar la disponibilidad y realizar reservas de manera rápida y eficiente, evitando conflictos y maximizando el uso de las instalaciones.

## ✨ Características Específicas

* **Calendario de Disponibilidad por Espacio:** Visualización clara de los horarios reservados para cada aula o laboratorio.
* **Perfiles de Usuario (Docente/Estudiante/Administrativo):** Control de acceso y permisos basado en el rol del usuario.
* **Filtrado Inteligente:** Búsqueda de espacios por capacidad, equipamiento (ej. proyectores, computadoras, software especializado) y ubicación.
* **Gestión de Solicitudes:** Proceso de aprobación (si aplica) para reservas, especialmente para laboratorios especializados.
* **Notificaciones:** Alertas sobre confirmación, cancelación o recordatorios de reservas.

## 💻 Tecnología Utilizada

El proyecto está construido sobre una base sólida de Microsoft y tecnologías web para una gestión de datos eficiente y una interfaz de usuario interactiva.

| Área | Tecnología | Propósito |
| :--- | :--- | :--- |
| **Backend** | **C# / .NET** | Lógica de negocio, manejo de sesiones y procesamiento de reservas. |
| **ORM** | **Entity Framework (EF)** | Interacción y modelado de la base de datos (Entidades: `Aula`, `Laboratorio`, `Reserva`, `Usuario`). |
| **Frontend** | **JavaScript, HTML, CSS** | Interfaz de usuario dinámica, manejo de eventos de calendario y formularios. |
| **Base de Datos** | **SQL Server** (o similar) | Almacenamiento de la información de reservas, usuarios y recursos. |

## ⚙️ Instalación y Configuración

Sigue estos pasos para configurar y ejecutar **ReservaYa** en tu entorno local.

### Prerrequisitos

* **Visual Studio** (Recomendado para trabajar con soluciones `.sln`).
* **.NET SDK** (Asegúrate de tener la versión compatible con el proyecto).
* **SQL Server** (o cualquier base de datos compatible con Entity Framework).

### Pasos Detallados

1.  **Clonar el Repositorio:**
    ```bash
    git clone https://github.com/Fabio-Menjivar/ReservaYa.git
    cd ReservaYa
    ```

2.  **Abrir la Solución:**
    Abre el archivo `ReservaYa.sln` en Visual Studio.

3.  **Configurar la Conexión a la DB:**
    * Verifica y ajusta la cadena de conexión de la base de datos dentro del archivo de configuración (`appsettings.json` o equivalente en la capa de datos) para que apunte a tu instancia local de SQL Server.

4.  **Ejecutar Migraciones de Entity Framework:**
    * Abre la Consola del Administrador de Paquetes (Package Manager Console).
    * Ejecuta el comando para crear o actualizar la estructura de la base de datos:
        ```powershell
        Update-Database
        ```

5.  **Iniciar la Aplicación:**
    * Establece el proyecto principal de la solución como proyecto de inicio.
    * Ejecuta el proyecto (Presiona `F5` o el botón **Iniciar**).
    * El sistema debería abrirse en tu navegador predeterminado, listo para ser utilizado.

## 🤝 Contribución

Si deseas contribuir a mejorar este sistema de reservas académicas, ¡eres bienvenido! Revisa la sección de [Issues](https://github.com/Fabio-Menjivar/ReservaYa/issues) y considera los siguientes pasos:

1.  Haz un "fork" del repositorio.
2.  Crea una nueva rama (`git checkout -b feature/mejora-reportes`).
3.  Realiza tus *commits* siguiendo buenas prácticas (`git commit -m 'feat: Añadir reportes de ocupación de aulas'`).
4.  Abre un Pull Request claro y conciso.

## 📜 Licencia

[Menciona la Licencia, por ejemplo: Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE.md] para detalles.]
