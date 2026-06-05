# Sistema de Administración Deportiva y Arriendo de Canchas

## Objetivo General

Implementar un sistema web para la administración integral de las ramas deportivas del club, incluyendo:

- Gestión de nóminas de deportistas.
- Gestión de entrenadores (DTs).
- Gestión de horarios de entrenamiento.
- Sistema de arriendo de canchas deportivas.

---

# Alcance del Sistema

El sistema debe contemplar:

1. Módulos de gestión interna para:
   - Administradores.
   - Directores Técnicos (DT).

2. Interfaz para usuarios externos destinada al arriendo de canchas deportivas.

---

# Requerimientos Funcionales

## 1. Gestión de Ramas Deportivas

### RF-001
**Actor:** Administrador  
**Prioridad:** Alta

El sistema debe permitir crear, modificar y eliminar ramas deportivas.

### RF-002
**Actor:** Administrador  
**Prioridad:** Alta

Cada rama deportiva debe almacenar:

- Nombre.
- Horarios de entrenamiento.
- Días de entrenamiento.
- Sector de entrenamiento.
- DT asociado.
- Límite máximo de deportistas.

### RF-003
**Actor:** DT  
**Prioridad:** Alta

El DT debe poder visualizar y modificar la nómina de deportistas de su rama asignada.

### RF-004
**Actor:** Administrador  
**Prioridad:** Alta

El Administrador debe poder visualizar y modificar la nómina de deportistas de todas las ramas.

### RF-005
**Actor:** Administrador  
**Prioridad:** Alta

El Administrador debe poder crear, asignar y reasignar DTs.

## 2. Gestión de Usuarios y Accesos

### RF-006
**Actor:** Todos  
**Prioridad:** Alta

El sistema debe contar con un módulo de autenticación para Administradores y DTs.

### RF-007
**Actor:** Usuario  
**Prioridad:** Media

El sistema debe contar con un Login para usuarios que deseen arrendar canchas utilizando RUT, correo electrónico y contraseña.

## 3. Arriendo de Canchas

### RF-008
**Actor:** Usuario  
**Prioridad:** Alta

El sistema debe mostrar la disponibilidad de las 3 canchas deportivas para arriendo.

### RF-009
**Actor:** Usuario  
**Prioridad:** Alta

Las reservas deben realizarse en bloques de 1 hora de lunes a viernes.

### RF-010
**Actor:** Administrador  
**Prioridad:** Alta

El Administrador debe poder habilitar o cancelar canchas para arriendo.

### RF-011
**Actor:** Usuario  
**Prioridad:** Alta

Al realizar una reserva, el Usuario debe recibir un correo electrónico de confirmación.

### RF-012
**Actor:** Usuario  
**Prioridad:** Alta

Debe existir un mecanismo de abono o pago inicial para asegurar la reserva.

### RF-013
**Actor:** Administrador  
**Prioridad:** Media

Debe existir un mecanismo de veto o penalización para usuarios que reserven y no utilicen la cancha.

### RF-014
**Actor:** Administrador  
**Prioridad:** Alta

Tras la reserva, el Administrador debe contactar al usuario vía WhatsApp para confirmar la cancha.

## 4. Información Pública y Landing Page

### RF-015
**Actor:** Usuario  
**Prioridad:** Media

El sistema debe contar con una Landing Page pública que muestre información de las ramas deportivas, horarios de entrenamiento y eventos.

# Requerimientos No Funcionales

### RNF-001 — Seguridad
Las contraseñas deben almacenarse de forma segura mediante cifrado.

### RNF-002 — Usabilidad
La interfaz de arriendo debe ser intuitiva y fácil de usar.

### RNF-003 — Rendimiento
La visualización de disponibilidad de canchas no debe exceder los 3 segundos.

### RNF-004 — Integración
El sistema debe integrarse con servicios de correo electrónico para enviar confirmaciones.

### RNF-005 — Comunicación
El sistema debe facilitar el contacto por WhatsApp mediante enlaces o plantillas de mensajes.

# Actores

## Administrador
- Gestiona ramas deportivas.
- Gestiona DTs.
- Gestiona nóminas.
- Gestiona canchas.
- Gestiona penalizaciones.

## DT
- Gestiona la nómina de su rama.

## Usuario
- Se registra.
- Inicia sesión.
- Reserva canchas.
- Realiza pagos o abonos.

# Modelo C4

## C1 - Contexto
Pendiente.

## C2 - Contenedores
Pendiente.
