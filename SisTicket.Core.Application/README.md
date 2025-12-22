# Application Layer - SisTicket

Esta capa contiene la lógica de aplicación, casos de uso, DTOs y validaciones.

## ?? Estructura de Carpetas

```
SisTicket.Core.Application/
??? DTOs/
?   ??? Auth/
?   ?   ??? LoginRequest.cs
?   ?   ??? LoginResponse.cs
?   ??? Usuario/
?   ?   ??? UsuarioRequest.cs
?   ?   ??? UsuarioResponse.cs
?   ?   ??? CrearUsuarioRequest.cs
?   ?   ??? ActualizarUsuarioRequest.cs
?   ??? Solicitud/
?   ?   ??? SolicitudRequest.cs
?   ?   ??? SolicitudResponse.cs
?   ?   ??? CrearSolicitudRequest.cs
?   ?   ??? ActualizarSolicitudRequest.cs
?   ?   ??? AsignarGestorRequest.cs
?   ?   ??? CambiarEstadoSolicitudRequest.cs
?   ??? Comentario/
?   ?   ??? ComentarioRequest.cs
?   ?   ??? ComentarioResponse.cs
?   ??? Area/
?   ?   ??? AreaRequest.cs
?   ?   ??? AreaResponse.cs
?   ??? Prioridad/
?   ?   ??? PrioridadRequest.cs
?   ?   ??? PrioridadResponse.cs
?   ??? TipoSolicitud/
?       ??? TipoSolicitudRequest.cs
?       ??? TipoSolicitudResponse.cs
??? Services/
?   ??? Interfaces/
?   ?   ??? IAuthService.cs
?   ?   ??? IUsuarioService.cs
?   ?   ??? ISolicitudService.cs
?   ?   ??? IComentarioService.cs
?   ?   ??? IAreaService.cs
?   ?   ??? IPrioridadService.cs
?   ?   ??? ITipoSolicitudService.cs
?   ??? Implementations/
?       ??? AuthService.cs
?       ??? UsuarioService.cs
?       ??? SolicitudService.cs
?       ??? ComentarioService.cs
?       ??? AreaService.cs
?       ??? PrioridadService.cs
?       ??? TipoSolicitudService.cs
??? Validators/
?   ??? UsuarioValidator.cs
?   ??? SolicitudValidator.cs
?   ??? ComentarioValidator.cs
?   ??? AreaValidator.cs
?   ??? PrioridadValidator.cs
?   ??? TipoSolicitudValidator.cs
??? Mappings/
?   ??? MappingProfile.cs
??? Exceptions/
?   ??? ApplicationException.cs
?   ??? ValidationException.cs
?   ??? NotFoundException.cs
?   ??? UnauthorizedException.cs
??? DependencyInjection.cs
```

## ?? Descripción de Carpetas

### **DTOs/**
Data Transfer Objects organizados por entidad. Separa Request y Response para cada caso de uso.

### **Services/**
- **Interfaces/**: Contratos de los servicios
- **Implementations/**: Implementaciones de los servicios con lógica de negocio

### **Validators/**
Validaciones de DTOs usando FluentValidation.

### **Mappings/**
Configuración de AutoMapper para mapear entre Entidades y DTOs.

### **Exceptions/**
Excepciones personalizadas de la capa de aplicación.

## ?? Responsabilidades

- ? DTOs (Request/Response)
- ? Casos de uso (Application Services)
- ? Validaciones de entrada
- ? Reglas de negocio de aplicación
- ? Mapeo entre entidades y DTOs
- ? Control de permisos por rol
- ? Orquestación de repositorios

## ?? No debe contener

- ? Lógica de acceso a datos (eso es Infrastructure)
- ? Lógica de presentación (eso es WebApp)
- ? Reglas de dominio puras (eso es Domain)
