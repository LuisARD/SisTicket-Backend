# ?? SisTicket API - Guía de Prueba

## ? Parte 1 Completada

### **Características Implementadas:**
- ? 7 Controllers REST API completos
- ? Middleware de excepciones global
- ? Swagger/OpenAPI documentación
- ? CORS configurado
- ? Manejo de errores centralizado

---

## ?? **Endpoints Disponibles**

### **?? Autenticación (Temporal)**
```
POST   /api/auth/login
POST   /api/auth/validate-token
```

### **?? Usuarios (Solo SuperAdmin)**
```
GET    /api/usuarios
GET    /api/usuarios/{id}
GET    /api/usuarios/area/{areaId}
GET    /api/usuarios/gestores/area/{areaId}
POST   /api/usuarios
PUT    /api/usuarios/{id}
DELETE /api/usuarios/{id}
```

### **?? Áreas**
```
GET    /api/areas
GET    /api/areas/{id}
POST   /api/areas
PUT    /api/areas/{id}
DELETE /api/areas/{id}
```

### **? Prioridades**
```
GET    /api/prioridades
GET    /api/prioridades/{id}
POST   /api/prioridades
PUT    /api/prioridades/{id}
DELETE /api/prioridades/{id}
```

### **?? Tipos de Solicitud**
```
GET    /api/tipossolicitud
GET    /api/tipossolicitud/{id}
POST   /api/tipossolicitud
PUT    /api/tipossolicitud/{id}
DELETE /api/tipossolicitud/{id}
```

### **?? Solicitudes**
```
GET    /api/solicitudes
GET    /api/solicitudes/{id}
GET    /api/solicitudes/solicitante/{solicitanteId}
GET    /api/solicitudes/gestor/{gestorId}
GET    /api/solicitudes/filtrar?estado=1&prioridadId=1
POST   /api/solicitudes
PUT    /api/solicitudes/{id}
POST   /api/solicitudes/{id}/asignar-gestor
POST   /api/solicitudes/{id}/cambiar-estado
DELETE /api/solicitudes/{id}
```

### **?? Comentarios**
```
GET    /api/comentarios/solicitud/{solicitudId}
POST   /api/comentarios
DELETE /api/comentarios/{id}
```

---

## ?? **Cómo Ejecutar la API**

### **1. Ejecutar el Proyecto**
```bash
cd WebApp.SisTicket
dotnet run
```

### **2. Abrir Swagger**
Abre tu navegador en: **http://localhost:5000** o **https://localhost:5001**

Verás la interfaz de Swagger con todos los endpoints documentados.

---

## ?? **Pruebas de Ejemplo**

### **1. Login (Temporal)**
```http
POST /api/auth/login
Content-Type: application/json

{
  "nombreUsuario": "cesar",
  "password": "cesar"
}
```

**Respuesta:**
```json
{
  "id": 1,
  "nombreUsuario": "cesar",
  "nombre": "Cesar",
  "apellido": "Motos",
  "email": "cesar@gmail.com",
  "rol": "SuperAdmin",
  "area": "Tecnología de la Información",
  "token": "TEMP_TOKEN_1_SuperAdmin_..."
}
```

### **2. Obtener Todas las Áreas**
```http
GET /api/areas
```

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Tecnología de la Información",
    "descripcion": "Área encargada de soporte técnico...",
    "fechaCreacion": "2024-01-01T00:00:00Z",
    "activo": true
  },
  ...
]
```

### **3. Crear una Solicitud**
```http
POST /api/solicitudes
Content-Type: application/json

{
  "titulo": "Problema con impresora",
  "descripcion": "La impresora del 2do piso no funciona",
  "tipoSolicitudId": 1,
  "prioridadId": 2,
  "areaId": 1
}
```

**Respuesta:**
```json
{
  "id": 1,
  "numeroSolicitud": "REQ-2024-001",
  "titulo": "Problema con impresora",
  "estado": "Nueva",
  "solicitanteNombre": "Cesar Motos",
  "gestorAsignadoNombre": null,
  ...
}
```

### **4. Asignar Gestor (Solo Admin/SuperAdmin)**
```http
POST /api/solicitudes/1/asignar-gestor
Content-Type: application/json

{
  "gestorId": 3
}
```

### **5. Agregar Comentario**
```http
POST /api/comentarios
Content-Type: application/json

{
  "texto": "Revisaré el problema mañana a primera hora",
  "solicitudId": 1
}
```

---

## ?? **Datos de Prueba Disponibles**

Los siguientes datos ya están en la base de datos (seeds):

### **Usuarios:**
| Usuario | Password | Rol | Área |
|---------|----------|-----|------|
| cesar | cesar | SuperAdmin | TI |
| admin | password | Admin | TI |
| gestor | password | Gestor | TI |
| usuario | password | Solicitante | RRHH |

### **Áreas:**
- Tecnología de la Información
- Recursos Humanos
- Finanzas
- Operaciones
- Administración

### **Prioridades:**
- Baja (Nivel 1)
- Media (Nivel 2)
- Alta (Nivel 3)
- Crítica (Nivel 4)

### **Tipos de Solicitud:**
- Soporte Técnico
- Mantenimiento
- Nuevo Requerimiento
- Incidencia
- Consulta
- Acceso y Permisos

---

## ?? **Respuestas de Error**

El middleware de excepciones devuelve respuestas consistentes:

### **404 Not Found:**
```json
{
  "statusCode": 404,
  "message": "La entidad 'Usuario' con id (999) no fue encontrada."
}
```

### **400 Bad Request:**
```json
{
  "statusCode": 400,
  "message": "Ya existe un usuario con el email 'test@example.com'"
}
```

### **401 Unauthorized:**
```json
{
  "statusCode": 401,
  "message": "Solo el SuperAdmin puede crear usuarios"
}
```

---

## ?? **Notas Importantes**

1. **Autenticación Temporal:** 
   - El sistema usa un token temporal
   - En la Parte 2 se implementará JWT real

2. **Usuario Actual:**
   - Por ahora todos los endpoints usan el usuario con ID=1 (SuperAdmin "cesar")
   - En la Parte 2 se obtendrá del token JWT

3. **Passwords:**
   - Los passwords se hashean con un método temporal
   - En la Parte 2 se implementará BCrypt

---

## ?? **Próxima Fase: Parte 2**

- ? Implementación de JWT real
- ? Autenticación completa
- ? Autorización por roles con `[Authorize]`
- ? Hash de passwords con BCrypt
- ? Refresh tokens

---

## ?? **Troubleshooting**

### **Error de compilación:**
```bash
dotnet build
```

### **Error de base de datos:**
```bash
dotnet ef database update --project SisTicket.Infrastructure.Persistence --startup-project WebApp.SisTicket
```

### **Ver logs:**
- Los logs aparecen en la consola durante la ejecución
- Los errores 500+ se registran como ERROR
- Los errores 400 se registran como WARNING

---

## ? **¡API Lista para Probar!**

Abre **http://localhost:5000** y explora los endpoints con Swagger.
