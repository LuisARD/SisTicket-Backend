# Seeds - Datos Iniciales del Sistema

Esta carpeta contiene las clases de seed que insertan datos iniciales en la base de datos del sistema SisTicket.

## ?? Archivos de Seed

### 1. **AreaSeed.cs**
Crea 5 áreas organizacionales iniciales:
- Tecnología de la Información
- Recursos Humanos
- Finanzas
- Operaciones
- Administración

### 2. **PrioridadSeed.cs**
Define 4 niveles de prioridad:
- **Baja** (Nivel 1) - Sin urgencia
- **Media** (Nivel 2) - Prioridad normal
- **Alta** (Nivel 3) - Urgente
- **Crítica** (Nivel 4) - Inmediata

### 3. **TipoSolicitudSeed.cs**
Crea 6 tipos de solicitudes:
- Soporte Técnico
- Mantenimiento
- Nuevo Requerimiento
- Incidencia
- Consulta
- Acceso y Permisos

### 4. **UsuarioSeed.cs**
Crea 4 usuarios de ejemplo con diferentes roles:

| Usuario | Email | Rol | Área | Password (temporal) |
|---------|-------|-----|------|---------------------|
| superadmin | superadmin@sisticket.com | SuperAdmin | TI | Admin@123 |
| admin.ti | admin.ti@sisticket.com | Admin | TI | Admin@123 |
| gestor.ti | gestor.ti@sisticket.com | Gestor | TI | Gestor@123 |
| usuario.demo | usuario.demo@sisticket.com | Solicitante | RRHH | User@123 |

## ?? IMPORTANTE - Seguridad

Los passwords en los seeds son **TEMPORALES Y DE DEMOSTRACIÓN**.

### ? En Producción:
1. **NUNCA uses estos passwords**
2. Los PasswordHash deben ser generados con un algoritmo de hash seguro (BCrypt, Argon2, etc.)
3. Los passwords temporales deben cambiarse en el primer inicio de sesión
4. Considera eliminar los usuarios de ejemplo en producción

### ?? Recomendaciones:
- Implementar política de contraseñas fuertes
- Forzar cambio de contraseña en primer login
- Usar algoritmos de hashing seguros (ej: BCrypt con salt)
- No versionar passwords reales en Git

## ?? Aplicación de Seeds

Los seeds se aplican automáticamente al:
1. Crear una nueva migración con `dotnet ef migrations add [Nombre]`
2. Actualizar la base de datos con `dotnet ef database update`

Entity Framework Core detecta automáticamente todas las clases que implementan `IEntityTypeConfiguration<T>` gracias a:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
```

## ?? Cómo Agregar Nuevos Seeds

1. Crear una nueva clase que implemente `IEntityTypeConfiguration<TEntidad>`
2. Usar el método `builder.HasData()` para definir los datos
3. Crear una nueva migración
4. Aplicar la migración

Ejemplo:
```csharp
public class MiEntidadSeed : IEntityTypeConfiguration<MiEntidad>
{
    public void Configure(EntityTypeBuilder<MiEntidad> builder)
    {
        builder.HasData(
            new MiEntidad { Id = 1, Nombre = "Ejemplo", ... }
        );
    }
}
```

## ??? Fechas

Todas las fechas de creación están establecidas a `2024-01-01 00:00:00 UTC` para consistencia.

## ? Estado Activo

Todos los registros se crean con `Activo = true` por defecto.
