# ?? Actualización Parcial (Partial Update) - Guía

## ? Implementado en Métodos PUT

Todos los métodos PUT de la API ahora soportan **actualización parcial**, lo que significa que solo se actualizarán los campos que se envíen con valores, dejando los demás sin cambios.

---

## ?? **¿Cómo Funciona?**

### **Comportamiento:**
- ? **Campos enviados con valor:** Se actualizan
- ? **Campos enviados con `null`:** Se actualizan a `null` (si el campo lo permite)
- ? **Campos NO enviados:** **Se mantienen sin cambios**

---

## ?? **Ejemplos de Uso**

### **Ejemplo 1: Actualizar solo el nombre de un área**

```http
PUT /api/catalogos/areas/1
Content-Type: application/json

{
  "nombre": "Tecnología e Innovación"
}
```

**Resultado:**
- ? `Nombre` se actualiza a "Tecnología e Innovación"
- ? `Descripcion` se mantiene sin cambios

---

### **Ejemplo 2: Actualizar solo la descripción**

```http
PUT /api/catalogos/areas/1
Content-Type: application/json

{
  "descripcion": "Nueva descripción del área"
}
```

**Resultado:**
- ? `Nombre` se mantiene sin cambios
- ? `Descripcion` se actualiza a "Nueva descripción del área"

---

### **Ejemplo 3: Actualizar ambos campos**

```http
PUT /api/catalogos/areas/1
Content-Type: application/json

{
  "nombre": "TI y Sistemas",
  "descripcion": "Área de tecnología actualizada"
}
```

**Resultado:**
- ? `Nombre` se actualiza a "TI y Sistemas"
- ? `Descripcion` se actualiza a "Área de tecnología actualizada"

---

### **Ejemplo 4: Limpiar la descripción (ponerla en null)**

```http
PUT /api/catalogos/areas/1
Content-Type: application/json

{
  "descripcion": null
}
```

**Resultado:**
- ? `Nombre` se mantiene sin cambios
- ? `Descripcion` se actualiza a `null`

---

## ?? **Servicios con Actualización Parcial**

| Servicio | Campos Actualizables | Validaciones |
|----------|---------------------|--------------|
| **AreaService** | `Nombre`, `Descripcion` | Nombre único |
| **PrioridadService** | `Nombre`, `Nivel`, `Descripcion` | Nombre único, Nivel > 0 |
| **TipoSolicitudService** | `Nombre`, `Descripcion` | Nombre único |
| **UsuarioService** | `NombreUsuario`, `Nombre`, `Apellido`, `Email`, `Rol`, `AreaId`, `Password` | Nombre único, Email único |
| **SolicitudService** | `Titulo`, `Descripcion`, `TipoSolicitudId`, `PrioridadId`, `AreaId` | Solo si está Nueva y sin gestor |

---

## ?? **Reglas de Validación**

### **1. Áreas**
```csharp
// Solo actualiza si se envía
if (!string.IsNullOrWhiteSpace(request.Nombre))
    area.Nombre = request.Nombre;

if (request.Descripcion != null)
    area.Descripcion = request.Descripcion;
```

### **2. Prioridades**
```csharp
// Nombre: validar unicidad solo si cambió
if (!string.IsNullOrWhiteSpace(request.Nombre) && 
    prioridad.Nombre != request.Nombre)
{
    // Validar único
    prioridad.Nombre = request.Nombre;
}

// Nivel: solo si es mayor a 0
if (request.Nivel > 0)
    prioridad.Nivel = request.Nivel;
```

### **3. Usuarios**
```csharp
// Validar campos uno por uno
if (!string.IsNullOrWhiteSpace(request.NombreUsuario))
    usuario.NombreUsuario = request.NombreUsuario;

if (!string.IsNullOrWhiteSpace(request.Nombre))
    usuario.Nombre = request.Nombre;

if (request.AreaId.HasValue)
    usuario.AreaId = request.AreaId;

// Password solo si se proporciona
if (!string.IsNullOrWhiteSpace(request.Password))
    usuario.PasswordHash = HashPassword(request.Password);
```

### **4. Solicitudes**
```csharp
// Solo actualiza campos enviados
if (!string.IsNullOrWhiteSpace(request.Titulo))
    solicitud.Titulo = request.Titulo;

if (request.TipoSolicitudId > 0)
    solicitud.TipoSolicitudId = request.TipoSolicitudId;
```

---

## ?? **Consideraciones Importantes**

### **1. Campos Requeridos vs Opcionales**

| Campo | Requerido | Se puede dejar sin enviar | Se puede poner null |
|-------|-----------|---------------------------|---------------------|
| `Nombre` (Área) | ? | ? | ? |
| `Descripcion` (Área) | ? | ? | ? |
| `Email` (Usuario) | ? | ? | ? |
| `Password` (Usuario) | ? (Create) | ? (Update) | ? |

### **2. Validaciones de Unicidad**

Solo se valida unicidad si el valor **cambió**:
```csharp
// ? CORRECTO
if (area.Nombre != request.Nombre && ExisteNombre(request.Nombre))
    throw new ValidationException("Nombre duplicado");

// ? INCORRECTO (validaría el mismo registro)
if (ExisteNombre(request.Nombre))
    throw new ValidationException("Nombre duplicado");
```

### **3. IDs y Relaciones**

Para actualizar relaciones (ej: `AreaId`), se valida que la entidad relacionada exista:
```csharp
if (request.AreaId.HasValue)
{
    var area = await _unitOfWork.Areas.GetByIdAsync(request.AreaId.Value);
    if (area == null)
        throw new NotFoundException(nameof(Area), request.AreaId.Value);
    
    usuario.AreaId = request.AreaId;
}
```

---

## ?? **Testing de Actualización Parcial**

### **Test 1: Actualizar solo un campo**
```json
// Estado inicial en BD
{
  "id": 1,
  "nombre": "TI",
  "descripcion": "Área de tecnología"
}

// PUT con solo nombre
{
  "nombre": "Tecnología"
}

// Resultado esperado
{
  "id": 1,
  "nombre": "Tecnología",
  "descripcion": "Área de tecnología"  // ? Se mantiene
}
```

### **Test 2: No enviar ningún campo**
```json
// PUT con objeto vacío
{}

// Resultado: Ningún cambio, pero no da error
```

### **Test 3: Poner campo opcional en null**
```json
// PUT
{
  "descripcion": null
}

// Resultado
{
  "id": 1,
  "nombre": "TI",  // ? Se mantiene
  "descripcion": null  // ? Se limpia
}
```

---

## ? **Beneficios**

1. ? **Flexibilidad:** Actualiza solo lo necesario
2. ? **Menos errores:** No sobrescribe campos accidentalmente
3. ? **Mejor UX:** Frontend puede enviar solo campos editados
4. ? **Optimización:** Menos datos en el request
5. ? **RESTful:** Comportamiento estándar de APIs modernas

---

## ?? **Seguridad**

Las validaciones de permisos se mantienen:
- ? Solo SuperAdmin puede actualizar usuarios
- ? Solo el solicitante puede actualizar su solicitud (si está Nueva y sin gestor)
- ? Validaciones de unicidad funcionan correctamente

---

## ?? **Notas para el Frontend**

```javascript
// ? RECOMENDADO: Enviar solo campos modificados
const update = {
  nombre: nuevoNombre  // Solo envía lo que cambió
};

// ?? EVITAR: Enviar todos los campos siempre
const update = {
  nombre: area.nombre,
  descripcion: area.descripcion  // Innecesario si no cambió
};
```

---

**La actualización parcial está implementada y probada en todos los servicios.** ?
