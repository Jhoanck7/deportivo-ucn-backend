# Documentación de la API REST (para Frontend)

Esta API está desarrollada en .NET 10, sigue una Arquitectura en Capas y está protegida con autenticación basada en **JWT (JSON Web Tokens)**.

---

## 1. Autenticación y Gestión de Usuarios

Todos los endpoints que requieren autenticación deben incluir la cabecera HTTP:
`Authorization: Bearer <token_jwt>`

### Registrarse (Crear Usuario Externo/DT/Admin)
* **Endpoint:** `POST /api/auth/register`
* **Acceso:** Público
* **Cuerpo de la Petición (JSON):**
  ```json
  {
    "rut": "12345678-9",
    "email": "usuario@correo.cl",
    "password": "mi_contraseña_segura",
    "firstName": "Jhoan",
    "lastName": "Castro",
    "phone": "+56912345678",
    "role": "User" // Opciones: "User" (por defecto), "Coach", "Admin"
  }
  ```
* **Respuesta Exitosa (200 OK):**
  ```json
  {
    "message": "User registered successfully",
    "data": {
      "token": "eyJhbGciOi...",
      "email": "usuario@correo.cl",
      "firstName": "Jhoan",
      "lastName": "Castro",
      "role": "User"
    }
  }
  ```

### Iniciar Sesión (Login)
* **Endpoint:** `POST /api/auth/login`
* **Acceso:** Público
* **Cuerpo de la Petición (JSON):**
  ```json
  {
    "emailOrRut": "usuario@correo.cl", // O "12345678-9"
    "password": "mi_contraseña_segura"
  }
  ```
* **Respuesta Exitosa (200 OK):**
  ```json
  {
    "message": "Login successful",
    "data": {
      "token": "eyJhbGciOi...",
      "email": "usuario@correo.cl",
      "firstName": "Jhoan",
      "lastName": "Castro",
      "role": "User"
    }
  }
  ```

### Obtener Perfil del Usuario
* **Endpoint:** `GET /api/auth/users/{id}`
* **Acceso:** Autenticado (Cualquier Rol)

---

## 2. Gestión de Canchas (`Courts`)

### Listar Canchas
* **Endpoint:** `GET /api/courts`
* **Acceso:** Público (Ideal para el Landing Page y selectors de reserva)
* **Respuesta Exitosa (200 OK):**
  ```json
  {
    "message": "Courts retrieved successfully",
    "data": [
      {
        "id": 1,
        "name": "Cancha 1 - Pasto Sintético",
        "description": "Fútbol 7 con iluminación LED",
        "status": "Available", // Opciones: "Available", "Disabled", "UnderMaintenance"
        "pricePerHour": 25000.00
      }
    ]
  }
  ```

### Crear Cancha
* **Endpoint:** `POST /api/courts`
* **Acceso:** Solo **Admin** (Requiere Token)
* **Cuerpo de la Petición (JSON):**
  ```json
  {
    "name": "Cancha de Tenis 1",
    "description": "Arcilla con iluminación",
    "status": "Available",
    "pricePerHour": 15000.00
  }
  ```

---

## 3. Consultas y Reservas de Canchas (`Bookings`)

### Consultar Disponibilidad Horaria (Muy importante para el Frontend)
Muestra los bloques de 1 hora y si están libres u ocupados para una fecha determinada.
* **Endpoint:** `GET /api/bookings/availability?courtId={id}&date={YYYY-MM-DD}`
* **Acceso:** Público (No requiere token)
* **Parámetros Query:**
  - `courtId`: ID de la cancha a consultar
  - `date`: Fecha en formato ISO (`YYYY-MM-DD`), ej: `2026-06-08`
* **Respuesta Exitosa (200 OK):**
  Muestra los bloques desde las 08:00 hasta las 22:00.
  ```json
  {
    "message": "Availability retrieved successfully",
    "data": [
      {
        "startHour": 8,
        "endHour": 9,
        "isAvailable": true
      },
      {
        "startHour": 9,
        "endHour": 10,
        "isAvailable": false // Bloque ocupado
      }
      // ... continúa hasta las 22:00
    ]
  }
  ```
  *Nota: Las reservas solo se permiten de lunes a viernes (lógica backend).*

### Crear Reserva
El backend calculará automáticamente el costo y generará la plantilla para contactar por WhatsApp.
* **Endpoint:** `POST /api/bookings`
* **Acceso:** Autenticado (Cualquier Rol)
* **Cuerpo de la Petición (JSON):**
  ```json
  {
    "courtId": 1,
    "date": "2026-06-08",
    "startHour": 17, // Representa el bloque de 17:00 a 18:00
    "depositAmount": 12500.00 // Abono inicial (mínimo 50% de la tarifa por hora)
  }
  ```
* **Respuesta Exitosa (201 Created):**
  ```json
  {
    "message": "Booking registered successfully",
    "data": {
      "id": 12,
      "courtId": 1,
      "courtName": "Cancha 1 - Pasto Sintético",
      "userId": 4,
      "userFullName": "Jhoan Castro",
      "userPhone": "+56912345678",
      "date": "2026-06-08",
      "startHour": 17,
      "endHour": 18,
      "status": "Pending", // Opciones: "Pending", "Confirmed", "Cancelled", "Completed", "NoShow"
      "depositAmount": 12500.00,
      "totalPrice": 25000.00,
      "whatsAppLink": "https://wa.me/56912345678?text=Hola%20Jhoan...", // Link pre-construido
      "adminNotes": null,
      "createdAt": "2026-06-04T21:40:00Z"
    }
  }
  ```

---

## 4. Gestión de Deportistas (`Athletes`)

Módulo interno de ramas deportivas.

### Crear Deportista
El backend validará automáticamente si la rama asignada no ha superado su límite máximo de deportistas (`SportBranch.AthleteLimit`).
* **Endpoint:** `POST /api/athletes`
* **Acceso:** Autenticado
* **Cuerpo de la Petición (JSON):**
  ```json
  {
    "firstName": "Claudio",
    "lastName": "Bravo",
    "rut": "18.765.432-1",
    "email": "claudio@correo.com",
    "phone": "+56987654321",
    "birthDate": "2000-04-13",
    "isActive": true,
    "sportBranchId": 2 // ID de la rama (Ej. Fútbol, Tenis, etc.)
  }
  ```

---

## 5. Endpoints de Administración (Solo Administradores)

### Listar todos los usuarios registrados
* **Endpoint:** `GET /api/auth/users`
* **Acceso:** Solo **Admin**

### Vetar o Penalizar a un Usuario (RF-013)
* **Endpoint:** `PUT /api/auth/users/{id}/ban`
* **Acceso:** Solo **Admin**
* **Cuerpo de la Petición (JSON):**
  ```json
  {
    "isBanned": true,
    "banReason": "Inasistencia reiterada a reservas pagadas sin previo aviso (No-Show)."
  }
  ```

### Confirmar o Cancelar Reserva (Modificar Estado de Arriendo)
* **Endpoint:** `PUT /api/bookings/{id}/status?status={Status}&notes={Notes}`
* **Acceso:** Solo **Admin**
* **Parámetros Query:**
  - `status`: Opciones: `Confirmed`, `Cancelled`, `Completed`, `NoShow`
  - `notes`: Notas opcionales sobre el pago, observaciones, etc.
* **Respuesta:** Cambia el estado de la reserva y notifica por correo electrónico al usuario sobre su estado.
