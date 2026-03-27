# Plataforma de Gestión "El Edén" - San Cristóbal

##  Propósito
Proyecto de Tesis 2026 para la optimización administrativa del Albergue El Edén.

---

##  Descripción del Proyecto
El Albergue "El Edén" en San Cristóbal enfrenta retos operativos por el manejo manual de datos.  

Esta solución es una plataforma web integral que centraliza el control de población, movimientos entre zonas, historiales médicos e inventario inteligente de alimentos, garantizando seguridad mediante roles y auditoría de datos.

---

## 🛠️ Tecnologías Utilizadas
- **Framework:** .NET 9.0 (ASP.NET Core)  
- **Base de Datos:** MySQL (vía Pomelo.EntityFrameworkCore.MySql)  
- **ORM:** Entity Framework Core 9.0  
- **Documentación:** Swagger / OpenAPI 10.1.5  
- **Control de Versiones:** Git & GitHub  

---

##  Metodología y Gestión

- **Metodología:** Scrumban (Sprints + Kanban visual)  
- **Control de Versiones:** Git con estrategia Git Flow (main, develop, feature/)  

- **Calidad:**
  - Unit Testing con xUnit  
  - Pruebas manuales de flujo de usuario en frontend  

---

## 🧩 Módulos Principales

- **Gestión de Animales:** Registro completo, fotos e historial de movimientos  
- **Zonas del Albergue:** Control de ubicación (Cuarentena, Adopción, Médica)  
- **Gestión Médica:** Historial de tratamientos, dosis y alertas de medicación  
- **Inventario:** Control de stock de alimentos con alertas de stock bajo y vencimiento  
- **Dashboard:** Estadísticas en tiempo real y generación de reportes PDF  



##  Estructura del Código
```plaintext
Api_Eden/
├── Controllers/           # Definición de Endpoints (Verbos HTTP)
│   ├── AnimalController.cs
│   └── AuthController.cs  (Gestión de Tokens JWT)
├── Data/                  # Capa de Acceso a Datos
│   ├── AppDbContext.cs    (Mapeo de Tablas MySQL)
│   └── Migrations/        # Historial de cambios en la DB
├── Models/                # Entidades del Dominio (Tablas)
│   ├── Animal.cs
│   ├── Usuario.cs
│   └── HistorialMedico.cs
├── DTOs/                  # Objetos de Transferencia de Datos
│   ├── AnimalDTO.cs       (Filtra lo que se envía al Frontend)
│   └── LoginDTO.cs        (Estructura para autenticación)
├── Services/              # Lógica de Negocio y Seguridad
│   ├── IAuthService.cs    (Interfaz de autenticación)
│   └── AuthService.cs     (Implementación de JWT y BCrypt)
└── Program.cs             # Configuración y Middlewares
```

##  Stack Tecnológico y Dependencias

### Backend (ASP.NET Core 9.0)
- Base de Datos: MySQL Server  
- ORM: Entity Framework Core (EF Core)  

### 📚 Paquetes NuGet Cruciales
- Pomelo.EntityFrameworkCore.MySql  
- Microsoft.EntityFrameworkCore.Design  
- Microsoft.AspNetCore.Authentication.JwtBearer  
- BCrypt.Net-Next  
- QuestPDF  

---

##  Seguridad e Integridad
- **Autenticación:** Basada en JWT  
  `Authorization: Bearer <token>`

- **Cifrado:** Contraseñas protegidas con BCrypt  

- **Integridad:** Uso de transacciones para consistencia de datos  

---

## 🔗 Endpoints Principales (Resumen)

| Método | Endpoint           | Descripción |
|--------|-------------------|-------------|
| POST   | /api/auth/login   | Autentica usuario y devuelve Token JWT |
| GET    | /api/animal       | Lista todos los animales registrados |
| POST   | /api/animal       | Registra un nuevo ejemplar |
| GET    | /api/animal/{id}  | Obtiene historial completo |
| PUT    | /api/animal/mover | Cambia de zona |


---

##  Módulos Principales
- Gestión de Animales  
- Zonas del Albergue  
- Gestión Médica  
- Inventario  
- Dashboard  

---

## Despliegue
Desplegable en Railway o Render con CI/CD conectado a GitHub.

La arquitectura está diseñada para ser alojada en la nube mediante servicios de despliegue continuo (CI/CD) como Railway o Render, conectando directamente con el repositorio de GitHub.
