# Sistema de Gestión de Citas Médicas - Especificación

## 1. Project Overview

- **Project Name**: SistemaMedico
- **Type**: Web Application (ASP.NET Core MVC)
- **Core Functionality**: Sistema de gestión de citas médicas con módulos de pacientes, citas y autenticación
- **Target Users**: Recepcionistas y Médicos de una clínica

## 2. Technology Stack

- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server
- **Architecture**: Clean Architecture (4 capas)
- **Authentication**: Cookie-based con Roles

## 3. Database Design

### Tablas

#### patients
| Column | Type | Constraints |
|--------|------|-------------|
| IdPaciente | INT | PK, Identity |
| TipoDocumento | VARCHAR(20) | NOT NULL |
| NumeroDocumento | VARCHAR(20) | UNIQUE, NOT NULL |
| Nombres | VARCHAR(100) | NOT NULL |
| Apellidos | VARCHAR(100) | NOT NULL |
| FechaNacimiento | DATE | NOT NULL |
| Sexo | VARCHAR(10) | NOT NULL |
| Telefono | VARCHAR(20) | NOT NULL |
| Email | VARCHAR(100) | NOT NULL |
| Direccion | VARCHAR(200) | NULL |
| Observaciones | VARCHAR(500) | NULL |
| Estado | BIT | DEFAULT 1 |
| FechaCreacion | DATETIME | DEFAULT GETDATE() |

#### medicos
| Column | Type | Constraints |
|--------|------|-------------|
| IdMedico | INT | PK, Identity |
| Nombre | VARCHAR(150) | NOT NULL |
| Especialidad | VARCHAR(100) | NOT NULL |
| Estado | BIT | DEFAULT 1 |

#### citas
| Column | Type | Constraints |
|--------|------|-------------|
| IdCita | INT | PK, Identity |
| IdPaciente | INT | FK -> patients |
| IdMedico | INT | FK -> medicos |
| Fecha | DATE | NOT NULL |
| Hora | TIME | NOT NULL |
| Motivo | VARCHAR(500) | NOT NULL |
| Observaciones | VARCHAR(500) | NULL |
| Estado | VARCHAR(20) | NOT NULL (Programada, Atendida, Cancelada) |
| FechaCreacion | DATETIME | DEFAULT GETDATE() |

#### usuarios
| Column | Type | Constraints |
|--------|------|-------------|
| IdUsuario | INT | PK, Identity |
| Username | VARCHAR(50) | UNIQUE, NOT NULL |
| Password | VARCHAR(255) | NOT NULL |
| Rol | VARCHAR(20) | NOT NULL (Recepcionista, Medico) |
| IdMedico | INT | FK -> medicos (nullable) |
| Estado | BIT | DEFAULT 1 |

## 4. User Stories

### 4.1 Gestión de Pacientes
- [ ] Registrar paciente
- [ ] Listar pacientes
- [ ] Editar paciente
- [ ] Eliminar paciente
- [ ] Validar documento único
- [ ] Validar formato teléfono y email

### 4.2 Registro de Citas Médicas
- [ ] Registrar cita médica
- [ ] Mostrar disponibilidad de médicos
- [ ] Validar no superposición

### 4.3 Consulta de Citas (Médico)
- [ ] Consultar citas con filtros
- [ ] Filtrar por fecha, rango fechas, médico, estado

## 5. Architecture

### Capas
```
SistemaMedico/
├── SistemaMedico.Domain/        # Entidades
├── SistemaMedico.Application/  # Interfaces, Servicios, DTOs
├── SistemaMedico.Infrastructure/ # Data, Repositories
└── SistemaMedico.Web/        # MVC App
```

### Roles
- **Recepcionista**: CRUD Pacientes, Registro Citas
- **Médico**: Consulta Citas

## 6. Acceptance Criteria

1. Sistema compila sin errores
2. Login funciona correctamente
3. CRUD Pacientes funciona con validaciones
4. Registro de citas valida disponibilidad
5. Médico puede consultar sus citas
6. Navegación por menú según rol