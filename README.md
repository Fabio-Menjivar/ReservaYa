## 🏫 ReservaYa: Academic Classroom and Laboratory Reservation System

[![GitHub Language Count](https://img.shields.io/github/languages/count/Fabio-Menjivar/ReservaYa)](https://github.com/Fabio-Menjivar/ReservaYa)
[![GitHub Top Language](https://img.shields.io/github/languages/top/Fabio-Menjivar/ReservaYa)](https://github.com/Fabio-Menjivar/ReservaYa)
[![GitHub Stars](https://img.shields.io/github/stars/Fabio-Menjivar/ReservaYa?style=social)](https://github.com/Fabio-Menjivar/ReservaYa/stargazers)
[![GitHub Last Commit](https://img.shields.io/github/last-commit/Fabio-Menjivar/ReservaYa)](https://github.com/Fabio-Menjivar/ReservaYa/commits)

**ReservaYa** is a management system specifically designed to **optimize the assignment and reservation of academic resources** such as **Classrooms, Laboratories, and Meeting Rooms** within an educational institution. It allows faculty, students, and administrative staff to check **availability** and make reservations quickly and efficiently, preventing conflicts and maximizing the use of facilities.

---

## ✨ Specific Features

* **Space Availability Calendar:** Clear visualization of reserved times for each classroom or laboratory.
* **User Profiles (Faculty/Student/Administrative):** Access control and permissions based on the user's role.
* **Intelligent Filtering:** Search for spaces by capacity, equipment (e.g., projectors, computers, specialized software), and location.
* **Request Management:** Approval process (if applicable) for reservations, especially for specialized laboratories.
* **Notifications:** Alerts for confirmation, cancellation, or reservation reminders.

---

## 💻 Technologies Used

The project is built on a solid foundation of Microsoft and web technologies for efficient data management and an interactive user interface.

| Area | Technology | Purpose |
| :--- | :--- | :--- |
| **Backend** | **C# / .NET** | Business logic, session handling, and reservation processing. |
| **ORM** | **Entity Framework (EF)** | Database interaction and modeling (Entities: `Classroom`, `Laboratory`, `Reservation`, `User`). |
| **Frontend** | **JavaScript, HTML, CSS** | Dynamic user interface, handling of calendar events and forms. |
| **Database** | **SQL Server** (or similar) | Storage of reservation, user, and resource information. |

---

## ⚙️ Installation and Configuration

Follow these steps to set up and run **ReservaYa** in your local environment.

### Prerequisites

* **Visual Studio** (Recommended for working with `.sln` solutions).
* **.NET SDK** (Ensure you have the compatible version for the project).
* **SQL Server** (or any database compatible with Entity Framework).

### Detailed Steps

1.  **Clone the Repository:**
    ```bash
    git clone https://github.com/Fabio-Menjivar/ReservaYa.git
    cd ReservaYa
    ```

2.  **Open the Solution:**
    Open the file `ReservaYa.sln` in Visual Studio.

3.  **Configure the DB Connection:**
    * Verify and adjust the database connection string within the configuration file (`appsettings.json` or equivalent in the data layer) to point to your local SQL Server instance.

4.  **Run Entity Framework Migrations:**
    * Open the Package Manager Console.
    * Execute the command to create or update the database structure:
        ```powershell
        Update-Database
        ```

5.  **Start the Application:**
    * Set the main project in the solution as the startup project.
    * Run the project (Press `F5` or the **Start** button).
    * The system should open in your default browser, ready for use.
