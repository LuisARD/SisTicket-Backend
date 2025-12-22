# ?? SisTicket API - Guía de Prueba (Versión Minimalista)

## ? Parte 1 Completada

### **Características Implementadas:**
- ? 4 Controllers REST API (diseño minimalista)
- ? Middleware de excepciones global
- ? Swagger/OpenAPI documentación
- ? CORS configurado
- ? Manejo de errores centralizado

---

## ?? **Endpoints Disponibles (Minimalistas)**

### **?? Autenticación** `/api/auth`
```
POST   /api/auth/login
POST   /api/auth/validate-token
```

### **?? Usuarios** `/api/usuarios` (Solo SuperAdmin)
```
GET    /api/usuarios
GET    /api/usuarios/{id}
GET    /api/usuarios/area/{areaId}
GET    /api/usuarios/gestores/area/{areaId}
POST   /api/usuarios
PUT    /api/usuarios/{id}
DELETE /api/usuarios/{id}
```

### **?? Catálogos** `/api/catalogos` (Áreas, Prioridades, Tipos)
```
# Áreas
GET    /api/catalogos/areas
GET    /api/catalogos/areas/{id}
POST   /api/catalogos/areas
PUT    /api/catalogos/areas/{id}
DELETE /api/catalogos/areas/{id}

# Prioridades
GET    /api/catalogos/prioridades
GET    /api/catalogos/prioridades/{id}
POST   /api/catalogos/prioridades
PUT    /api/catalogos/prioridades/{id}
DELETE /api/catalogos/prioridades/{id}

# Tipos de Solicitud
GET    /api/catalogos/tipos-solicitud
GET    /api/catalogos/tipos-solicitud/{id}
POST   /api/catalogos/tipos-solicitud
PUT    /api/catalogos/tipos-solicitud/{id}
DELETE /api/catalogos/tipos-solicitud/{id}
```

### **?? Solicitudes y Comentarios** `/api/solicitudes`
```
# Solicitudes
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

# Comentarios (dentro de solicitudes)
GET    /api/solicitudes/{solicitudId}/comentarios
POST   /api/solicitudes/{solicitudId}/comentarios
DELETE /api/solicitudes/{solicitudId}/comentarios/{comentarioId}
```

---

## ?? **Resumen de Endpoints**

| Controller | Endpoints | Descripción |
|------------|-----------|-------------|
| **AuthController** | 2 | Login y validación |
| **UsuariosController** | 7 | CRUD de usuarios |
| **CatalogosController** | 15 | Áreas, Prioridades, Tipos |
| **SolicitudesController** | 13 | Solicitudes + Comentarios |
| **Total** | **37** | |

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

### **1. Login**
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
  "token": "TEMP_TOKEN_..."
}
```

### **2. Obtener Catálogos**

**Áreas:**
```http
GET /api/catalogos/areas
```

**Prioridades:**
```http
GET /api/catalogos/prioridades
```

**Tipos de Solicitud:**
```http
GET /api/catalogos/tipos-solicitud
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
  ...
}
```

### **4. Agregar Comentario a una Solicitud**
```http
POST /api/solicitudes/1/comentarios
Content-Type: application/json

{
  "texto": "Revisaré el problema mañana a primera hora"
}
```

### **5. Ver Comentarios de una Solicitud**
```http
GET /api/solicitudes/1/comentarios
```

---

## ?? **Datos de Prueba Disponibles**

### **Usuarios:**
| Usuario | Password | Rol | Área |
|---------|----------|-----|------|
| cesar | cesar | SuperAdmin | TI |
| admin | password | Admin | TI |
| gestor | password | Gestor | TI |
| usuario | password | Solicitante | RRHH |

### **Catálogos (ya cargados en BD):**
- **5 Áreas:** TI, RRHH, Finanzas, Operaciones, Administración
- **4 Prioridades:** Baja, Media, Alta, Crítica
- **6 Tipos:** Soporte, Mantenimiento, Requerimiento, Incidencia, Consulta, Acceso

---

## ?? **Ventajas del Diseño Minimalista**

? **Endpoints lógicamente agrupados**  
? **Menos rutas = más fácil de mantener**  
? **Comentarios dentro del contexto de solicitudes**  
? **Catálogos consolidados en un solo controller**  
? **API más intuitiva y RESTful**  

---

## ?? **Estructura de Rutas**

```
/api/auth              ? Autenticación
/api/usuarios          ? Gestión de usuarios
/api/catalogos         ? Todos los catálogos del sistema
  ?? /areas
  ?? /prioridades
  ?? /tipos-solicitud
/api/solicitudes       ? Solicitudes y sus comentarios
  ?? /{id}/comentarios
```

---

## ?? **Notas Importantes**

1. **Autenticación Temporal:** En la Parte 2 se implementará JWT real
2. **Usuario Actual:** Por ahora usa el usuario con ID=1 (SuperAdmin)
3. **Passwords:** Hash temporal, se implementará BCrypt en Parte 2

---

## ?? **¡API Minimalista Lista!**

**Solo 4 controllers principales con endpoints bien organizados.**

Abre **http://localhost:5000** y explora los endpoints con Swagger.
