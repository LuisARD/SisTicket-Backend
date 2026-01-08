# SisTicket Backend - Sistema de Gestión de Solicitudes

## Descripción del Proyecto

SisTicket Backend es una API REST robusta y escalable desarrollada en .NET 10 para la gestión integral de solicitudes (tickets) en organizaciones. Implementa una arquitectura limpia (Clean Architecture) con separación clara de responsabilidades y patrones de diseño modernos.

El sistema proporciona endpoints seguros para la gestión de usuarios, solicitudes, comentarios, adjuntos y catálogos, con autenticación JWT y autorización basada en roles.

---

## Arquitectura del Proyecto

### Capas de la Aplicación

```
SisTicket.Core.Domain/          # Entidades de negocio y enums
SisTicket.Core.Application/     # Lógica de negocio y DTOs
SisTicket.Infrastructure/       # Acceso a datos y servicios externos
WebApp.SisTicket/               # API Controllers y configuración
```

### Patrones Implementados
- **Clean Architecture**: Separación de responsabilidades por capas
- **Repository Pattern**: Abstracción del acceso a datos
- **Unit of Work**: Gestión de transacciones
- **Dependency Injection**: Inversión de control
- **DTO Pattern**: Transferencia de datos optimizada
- **AutoMapper**: Mapeo automático de objetos

---

## Características Principales

### Sistema de Autenticación y Seguridad
- Autenticación JWT con cookies HTTP-Only
- Encriptación de contraseñas con BCrypt
- Protección contra XSS y CSRF
- Tokens con expiración de 1 hora
- Endpoints protegidos por rol

### Gestión de Usuarios
- CRUD completo de usuarios
- 4 roles: Solicitante, Gestor, Admin, SuperAdmin
- Activación/Desactivación de cuentas
- Validación de unicidad (email, nombre de usuario)
- Solo usuarios inactivos pueden eliminarse
- Asignación de usuarios a áreas organizacionales

### Gestión de Solicitudes
- Estados: Nueva, En Proceso, Resuelta, Rechazada, Cerrada
- Asignación automática de número de solicitud (SOL-YYYY-NNNNNN)
- Gestores pueden auto-asignarse solicitudes de su área
- Validaciones de transición de estados
- Edición permitida solo en estado Nueva sin gestor
- Filtros avanzados (estado, prioridad, área, fechas)

### Sistema de Comentarios
- Solicitantes: Solo en sus propias solicitudes
- Gestores: Solo en solicitudes de su área
- Admin/SuperAdmin: En cualquier solicitud
- Eliminación restringida al autor o administradores
- Auditoría completa con usuario y fecha

### Gestión de Adjuntos
- Máximo 5 archivos por solicitud
- Límite de 10MB por archivo
- Formatos permitidos: PDF, PNG, JPG, JPEG, GIF, DOC, DOCX, XLS, XLSX, TXT, ZIP, RAR
- Acceso controlado por permisos de usuario
- Almacenamiento seguro con rutas únicas

### Catálogos
- Áreas organizacionales
- Tipos de solicitud por área
- Prioridades con niveles
- Validación: No eliminar si tienen solicitudes activas (no Rechazadas ni Cerradas)

---

## Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|-----------|---------|----------|
| .NET | 10.0 | Framework principal |
| C# | 14.0 | Lenguaje de programación |
| Entity Framework Core | 10.0 | ORM para acceso a datos |
| SQL Server | - | Base de datos relacional |
| BCrypt.Net | - | Encriptación de contraseñas |
| AutoMapper | - | Mapeo de objetos |
| JWT Bearer | - | Autenticación con tokens |
| Swagger/OpenAPI | - | Documentación de API |

---

## Requisitos del Sistema

### Software Necesario
- .NET SDK 10.0 o superior
- SQL Server 2019 o superior (o SQL Server Express)
- Visual Studio 2022 o Visual Studio Code
- Git (opcional)

### Verificar Instalación

```sh
dotnet --version  # Debe mostrar 10.x o superior
```

---

## Instalación y Configuración

### Paso 1: Clonar el Repositorio

```sh
git clone https://github.com/LuisARD/SisTicket-Backend.git
cd SisTicket-Backend
```

### Paso 2: Configurar Base de Datos

Edita `appsettings.json` en el proyecto `WebApp.SisTicket`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SisTicketDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Paso 3: Aplicar Migraciones

```sh
cd WebApp.SisTicket
dotnet ef database update
```

Esto creará la base de datos y ejecutará el seeder inicial con datos de prueba.

### Paso 4: Ejecutar la Aplicación

```sh
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:5147`
- HTTP: `http://localhost:5146`

### Paso 5: Acceder a Swagger

Abre tu navegador en: `https://localhost:5147/swagger`

---

## Estructura del Proyecto

```
SisTicket-Backend/
├── SisTicket.Core.Domain/
│   ├── Entities/           # Entidades de dominio (Usuario, Solicitud, etc.)
│   ├── Enums/              # Enumeraciones (EstadoSolicitud, Rol)
│   ├── Interfaces/         # Interfaces de repositorios
│   └── Common/             # Clases base
│
├── SisTicket.Core.Application/
│   ├── DTOs/               # Data Transfer Objects
│   ├── Services/           # Lógica de negocio
│   ├── Exceptions/         # Excepciones personalizadas
│   └── Mappings/           # Perfiles de AutoMapper
│
├── SisTicket.Infrastructure.Persistence/
│   ├── Context/            # DbContext de EF Core
│   ├── Repositories/       # Implementaciones de repositorios
│   ├── Configurations/     # Configuraciones de entidades
│   └── Seeders/            # Datos iniciales
│
└── WebApp.SisTicket/
    ├── Controllers/        # Endpoints de API
    ├── Middlewares/        # Middleware personalizado
    ├── Services/           # Servicios de infraestructura (JWT, FileStorage)
    └── Program.cs          # Configuración de la aplicación
```

---

## Endpoints Principales

### Autenticación (`/api/auth`)
- `POST /login` - Iniciar sesión
- `POST /logout` - Cerrar sesión
- `GET /me` - Obtener usuario actual

### Usuarios (`/api/usuarios`)
- `GET /` - Listar usuarios (todos los roles)
- `GET /{id}` - Obtener usuario por ID (todos los roles)
- `GET /area/{areaId}` - Usuarios por área (todos los roles)
- `GET /gestores/area/{areaId}` - Gestores activos por área (todos los roles)
- `POST /` - Crear usuario (SuperAdmin)
- `PUT /{id}` - Actualizar usuario (SuperAdmin)
- `PATCH /{id}/estado` - Activar/Desactivar usuario (SuperAdmin)
- `DELETE /{id}` - Eliminar usuario inactivo (SuperAdmin)

### Solicitudes (`/api/solicitudes`)
- `GET /` - Listar solicitudes (filtrado por rol)
- `GET /{id}` - Obtener solicitud por ID
- `GET /filtrar` - Filtros avanzados (Admin/SuperAdmin)
- `POST /` - Crear solicitud
- `PUT /{id}` - Actualizar solicitud
- `POST /{id}/tomar-solicitud` - Auto-asignarse (Gestor)
- `POST /{id}/asignar-gestor` - Asignar gestor (Admin/SuperAdmin)
- `POST /{id}/cambiar-estado` - Cambiar estado
- `DELETE /{id}` - Eliminar solicitud (Admin/SuperAdmin)

### Comentarios (`/api/solicitudes/{id}/comentarios`)
- `GET /` - Listar comentarios de solicitud
- `POST /` - Crear comentario (según permisos)
- `DELETE /{comentarioId}` - Eliminar comentario

### Adjuntos (`/api/solicitudes/{id}/adjuntos`)
- `GET /` - Listar adjuntos
- `POST /` - Subir archivo
- `GET /descargar` - Descargar archivo
- `DELETE /{adjuntoId}` - Eliminar archivo

### Catálogos
- **Áreas** (`/api/areas`): CRUD completo
- **Prioridades** (`/api/prioridades`): CRUD completo
- **Tipos de Solicitud** (`/api/tipossolicitud`): CRUD completo

---

## Permisos por Rol

### Solicitante
- ✅ Ver sus propias solicitudes
- ✅ Crear solicitudes
- ✅ Editar solicitudes en estado Nueva sin gestor
- ✅ Comentar en sus solicitudes
- ✅ Ver usuarios (consulta)

### Gestor
- ✅ Ver solicitudes de su área (asignadas + sin asignar)
- ✅ Auto-asignarse solicitudes
- ✅ Cambiar estado de solicitudes asignadas
- ✅ Comentar en solicitudes de su área
- ✅ Gestionar adjuntos de su área
- ✅ Ver usuarios (consulta)

### Admin
- ✅ Ver todas las solicitudes
- ✅ Asignar gestores a solicitudes
- ✅ Cambiar cualquier estado
- ✅ Gestionar catálogos (Áreas, Tipos, Prioridades)
- ✅ Ver usuarios (consulta)
- ❌ No puede gestionar usuarios (crear, editar, eliminar)

### SuperAdmin
- ✅ Todas las funciones de Admin
- ✅ Gestión completa de usuarios (CRUD)
- ✅ Activar/Desactivar usuarios
- ✅ Control total del sistema

---

## Reglas de Negocio Implementadas

### Usuarios
- Solo SuperAdmin puede crear, editar o eliminar usuarios
- No se puede eliminar un usuario activo (debe desactivarse primero)
- No se puede auto-desactivar o auto-eliminar
- Email y nombre de usuario deben ser únicos
- Usuarios inactivos no pueden iniciar sesión ni ser asignados

### Solicitudes
- Solicitantes solo pueden editar solicitudes en estado Nueva sin gestor
- Gestores solo pueden auto-asignarse solicitudes de su área
- El estado solo puede cambiarse siguiendo transiciones válidas
- Al asignar gestor, el estado cambia automáticamente a "En Proceso"
- Solo se pueden tomar solicitudes en estado Nueva sin gestor

### Comentarios
- Solicitantes solo pueden comentar en sus propias solicitudes
- Gestores solo pueden comentar en solicitudes de su área
- Admin/SuperAdmin pueden comentar en cualquier solicitud
- Solo el autor o administradores pueden eliminar comentarios

### Adjuntos
- Máximo 5 archivos por solicitud
- Tamaño máximo: 10MB por archivo
- Solo formatos permitidos específicos
- Solicitantes acceden a adjuntos de sus solicitudes
- Gestores acceden a adjuntos de su área
- Admin/SuperAdmin acceden a todos

### Catálogos
- No se puede eliminar un Área/Tipo/Prioridad con solicitudes activas
- Solo Admin y SuperAdmin pueden gestionar catálogos
- Validación de unicidad de nombres

---

## Seguridad

### Autenticación
- Tokens JWT con expiración de 1 hora
- Cookies HTTP-Only (previene ataques XSS)
- SameSite=Strict (protección CSRF)
- Secure=true en producción (solo HTTPS)

### Autorización
- Atributos `[Authorize]` en controladores
- Validación de roles con `[Authorize(Roles = "...")]`
- Validaciones adicionales en servicios
- Excepciones personalizadas (UnauthorizedException, ForbiddenException)

### Contraseñas
- Encriptación con BCrypt (salt rounds configurables)
- Hash almacenado, nunca contraseña en texto plano
- Validación de longitud mínima en el frontend

### CORS
- Configuración para frontend en localhost:5173
- AllowCredentials habilitado para cookies
- Headers y métodos específicos permitidos

---

## Datos de Prueba (Seeder)

El sistema incluye datos iniciales para desarrollo:

### Usuarios
- **SuperAdmin**: `superadmin` / `Admin123!`
- **Admin**: `admin` / `Admin123!`
- **Gestor TI**: `gestorti` / `Gestor123!`
- **Solicitante**: `solicitante1` / `User123!`

### Áreas
- Tecnología
- Recursos Humanos
- Finanzas
- Administración

### Prioridades
- Baja (Nivel 1)
- Media (Nivel 2)
- Alta (Nivel 3)
- Urgente (Nivel 4)

### Tipos de Solicitud
- Acceso a Sistema
- Mantenimiento de Equipos
- Reembolso de Gastos
- Solicitud de Vacaciones
- Y más...

---

## Migraciones de Base de Datos

### Crear Nueva Migración

```sh
dotnet ef migrations add NombreMigracion --project SisTicket.Infrastructure.Persistence --startup-project WebApp.SisTicket
```

### Aplicar Migraciones

```sh
dotnet ef database update --project SisTicket.Infrastructure.Persistence --startup-project WebApp.SisTicket
```

### Revertir Última Migración

```sh
dotnet ef migrations remove --project SisTicket.Infrastructure.Persistence --startup-project WebApp.SisTicket
```

---

## Solución de Problemas

### Error de conexión a base de datos
- Verifica que SQL Server esté ejecutándose
- Comprueba la cadena de conexión en `appsettings.json`
- Asegúrate de haber ejecutado `dotnet ef database update`

### Error "Unauthorized" en Swagger
- Haz login primero usando el endpoint `/api/auth/login`
- Las cookies se manejan automáticamente en el navegador
- En Postman/Thunder Client, habilita la gestión automática de cookies

### Migraciones no se aplican
- Verifica que Entity Framework Tools esté instalado: `dotnet tool install --global dotnet-ef`
- Asegúrate de estar en el directorio raíz del proyecto
- Especifica los proyectos correctamente con `--project` y `--startup-project`

### Puerto ya en uso
- Cambia el puerto en `launchSettings.json` del proyecto WebApp.SisTicket
- O detén la aplicación que está usando el puerto 5146/5147

### Errores de CORS en frontend
- Verifica que la URL del frontend esté configurada en `Program.cs`
- Asegúrate de que `AllowCredentials()` esté habilitado
- El frontend debe usar `credentials: 'include'` en las peticiones

---

## Variables de Entorno

Configurables en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "JwtSettings": {
    "SecretKey": "clave-secreta-muy-segura-de-al-menos-32-caracteres",
    "Issuer": "SisTicket",
    "Audience": "SisTicketUsers",
    "ExpirationHours": 1
  },
  "FileStorage": {
    "BasePath": "uploads",
    "MaxFileSize": 10485760
  }
}
```

---