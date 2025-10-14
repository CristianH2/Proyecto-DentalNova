### **Sistema de Gestión para Clínica Dental - DeltaNova**
Un sistema integral para la gestión de citas, expedientes clínicos y procesos administrativos de la clínica dental "DeltaNova".

Este proyecto busca optimizar la eficiencia operativa y mejorar la experiencia del paciente a través de una plataforma digital robusta, segura y accesible, compuesta por una aplicación web, una API en la nube y una aplicación móvil.

### **🎯 Objetivo del Proyecto**
Desarrollar e implementar un sistema integral, accesible y seguro para la clínica dental DeltaNova, que permita optimizar y digitalizar la gestión de citas, expedientes clínicos y procesos administrativos. El sistema busca mejorar la eficiencia operativa del personal y la experiencia de atención del paciente a través de tres componentes principales: una aplicación web administrativa, una API de servicios en la nube y una aplicación móvil para pacientes.

### **🚀 Arquitectura del Sistema**
La solución está diseñada con una arquitectura de tres componentes que trabajan en conjunto para ofrecer una experiencia fluida y centralizada.

**1. Aplicación Web Administrativa**

Es el centro de control para el personal de la clínica (administradores y odontólogos).

Administradores: Tienen acceso total para gestionar agendas, pacientes, usuarios, roles, inventario y generar reportes.

Odontólogos: Acceden a su agenda, gestionan expedientes clínicos y registran tratamientos.

**2. API de Servicios en la Nube (/api)**

Actúa como el cerebro del sistema, centralizando la lógica de negocio y la comunicación con la base de datos.

Provee endpoints seguros para que la aplicación web y la móvil consuman la misma fuente de datos.

Gestiona tareas automatizadas como el envío de notificaciones y recordatorios de citas.

**3. Aplicación Móvil para Pacientes**

Permite a los pacientes interactuar directamente con la clínica.

Pueden registrarse, iniciar sesión, ver su historial, consultar la disponibilidad de los odontólogos y solicitar, ver o cancelar sus citas.

### ** ✨ Funcionalidades Principales**
**🏥 Módulo de Gestión Clínica**
Gestión de Pacientes: Administración completa (CRUD) de los perfiles de pacientes.

Gestión de Odontólogos: Administración de perfiles y horarios de los especialistas.

Agenda Clínica: Visualización y manejo de la agenda por odontólogo.

Expediente Clínico: Registro detallado de tratamientos, historial y notas por paciente.

**💼 Módulo Administrativo y Financiero**
Registro de Pagos: Asociación de pagos a citas y tratamientos realizados.

Gestión de Inventario: Control de artículos, gestión de proveedores y registro de compras.

Generación de Reportes: Creación de informes básicos sobre la operación de la clínica.

**📱 Módulo de Portal del Paciente**
Autenticación Segura: Registro e inicio de sesión para nuevos pacientes.

Consulta de Historial: Acceso al historial de citas y tratamientos recibidos.

Solicitud de Citas: Posibilidad de agendar nuevas citas según la disponibilidad real.

**🔒 Módulo de Seguridad**
Autenticación y Autorización: Sistema basado en roles (Administrador, Odontólogo, Paciente) para un acceso seguro y restringido a la información sensible.

**🔔 Módulo de Notificaciones**
Recordatorios Automáticos: Envío de notificaciones para citas
