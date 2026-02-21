
### **Sistema de Gestión para Clínica Dental - DentalNova**
Un sistema integral para la gestión de citas, expedientes clínicos y procesos administrativos de la clínica dental "DentalNova".

Este proyecto busca optimizar la eficiencia operativa y mejorar la experiencia del paciente a través de una plataforma digital robusta, segura y accesible, compuesta por una aplicación web, una API en la nube y una aplicación móvil.

### **🎯 Objetivo del Proyecto**
Desarrollar e implementar un sistema integral, accesible y seguro para la clínica dental DentalNova, que permita optimizar y digitalizar la gestión de citas, expedientes clínicos y procesos administrativos. El sistema busca mejorar la eficiencia operativa del personal y la experiencia de atención del paciente a través de tres componentes principales: una aplicación web administrativa, una API de servicios en la nube y una aplicación móvil para pacientes.

### **🚀 Arquitectura del Sistema**
La solución está diseñada con una arquitectura de tres componentes que trabajan en conjunto para ofrecer una experiencia fluida y centralizada.

**1. Aplicación Web Administrativa (MVC)**
Es el centro de control para el personal de la clínica
   (administradores y odontólogos).

- Administradores: Tienen acceso total para gestionar agendas, pacientes, usuarios, roles e inventario.

- Odontólogos: Acceden a su agenda, gestionan expedientes clínicos y atienden citas.

**2. API de Servicios en la Nube**

Actúa como el cerebro del sistema, centralizando la lógica de negocio y la comunicación con la base de datos.

Provee endpoints seguros para que la aplicación web y la móvil consuman la misma fuente de datos.

**3. Aplicación Móvil para Pacientes**

Permite a los pacientes interactuar directamente con la clínica.

Pueden registrarse, iniciar sesión, ver su historial, consultar la disponibilidad de los odontólogos y solicitar, ver o cancelar sus citas.

### ✨ Funcionalidades Principales
**🏥 Módulo de Gestión Clínica**

Gestión de Pacientes: Administración completa (CRUD) de los perfiles de pacientes.

Gestión de Odontólogos: Administración de perfiles y horarios de los especialistas.

Agenda Clínica: Visualización y manejo de la agenda por odontólogo.

Expediente Clínico: Registro detallado de tratamientos, historial y notas por paciente.

**💼 Módulo Administrativo**

Registro de Pagos: Asociación de pagos a citas y tratamientos realizados.

Gestión de Inventario: Control de artículos e insumos.

Generación de Reportes: Creación de informes básicos sobre la operación de la clínica.

**📱 Módulo de Portal del Paciente**

Autenticación Segura: Registro e inicio de sesión para nuevos pacientes.

Consulta de Historial: Acceso al historial de citas y tratamientos recibidos.

Solicitud de Citas: Posibilidad de agendar nuevas citas según la disponibilidad real.

**🔒 Módulo de Seguridad**

Autenticación y Autorización: Sistema basado en roles (Administrador, Odontólogo, Paciente) para un acceso seguro y restringido a la información sensible.

**🔔 Módulo de Notificaciones**

Recordatorios: Envío de notificaciones para citas.

## Configuración del entorno Local

### Configurar base de datos
1. Abrir la Consola: Herramientas > Administrador de paquetes NuGet > Consola del Administrador de paquetes.

2. Crea la Migración: Add-Migration "InitialCreate"

3. Aplica la Migración: Update-Database


O ejecutar el script DentalNova_DB.sql con las tablas ya creadas.

### Cargar datos requeridos
1. Ejecutar de la carpeta 'DentalNova.Repository/Scripts de ayuda SQL' el script DentalNova_Datos.sql para rellenar información escencial.

### Iniciar sesión:
- Usuario (Admin): admin@correo.com
- Contraseña (Admin): Admin123.

  
- Usuario (Odontólogo): rebeca@gmail.com
- Contraseña (Odontólogo): User123.

  
- Usuario (Paciente): juan@gmail.com
- Contraseña (Paciente): User123.


## Arquitectura del Sistema

```mermaid
flowchart TD
 subgraph subGraph0["Actores (Usuarios)"]
    direction LR
        Paciente("Paciente")
        Odontologo("Odontólogo")
        Admin("Administrador")
 end

 subgraph subGraph1["Plataformas Cliente"]
    direction LR
        AppMovil["**Aplicación Móvil**<br>*(Pacientes)*"]
        Navegador["<b>Navegador Web</b><br>*(Admin / Odontólogo)*"]
 end

 subgraph subGraphLogic["Lógica Compartida"]
        EFCore["<b>Entity Framework Core</b><br><i>(Acceso a Datos)</i>"]
 end

 subgraph subGraph2["<b>Servidor ASP.NET Core</b>"]
    direction TB
        MVC["<b>Capa MVC</b><br><i>(Vistas Razor / UI Web)</i>"]
        API["<b>API REST</b><br><i>(Servicios para Web y App Móvil)</i>"]
        subGraphLogic
 end

 subgraph subGraph3["Infraestructura Externa"]
    direction TB
        DB[("<b>Base de Datos</b><br><i>SQL Server</i>")]
        Notif["<b>Servicio de Notificaciones</b><br><i>(Recordatorios)</i>"]
 end

    %% Relaciones de usuarios
    Paciente -- Usa --> AppMovil
    Odontologo -- Usa --> Navegador
    Admin -- Usa --> Navegador

    %% Flujo Web
    Navegador -- Interacción UI --> MVC
    MVC -- Consume API --> API

    %% Flujo Móvil
    AppMovil -- Peticiones API (JSON) --> API

    %% Lógica y datos
    API -- Usa --> EFCore
    EFCore <-- CRUD --> DB

    %% Servicios externos
    API -- Solicita Envío --> Notif

