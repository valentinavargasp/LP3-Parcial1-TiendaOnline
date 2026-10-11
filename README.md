# Sistema de Gestión para Tienda Online (TechStore)
### Primer Examen Parcial — Laboratorio de Programación 3 (ISSD 2026)

**Docente:** Prof. Mansilla Christian Eduardo  
**Tecnología:** ASP.NET Core Razor Pages (.NET 9) · C# · SQL Server · ADO.NET (`Microsoft.Data.SqlClient`)

---

## 📌 Descripción del Proyecto
Sistema web administrativo desarrollado para la gestión integral de una tienda online de productos de tecnología (*TechStore*). El proyecto implementa un esquema relacional de dos tablas y cubre todas las operaciones CRUD (Altas, Bajas, Modificaciones y Consultas con JOIN).

---

## 🗄️ Esquema Relacional de Base de Datos
El proyecto incluye el script completo de inicialización en el archivo `database_init.sql`.

### Tablas Implementadas:
1. **`categorias`:**
   * `idCategoria` (INT, IDENTITY, Clave Primaria)
   * `descripcion` (VARCHAR(50), NOT NULL)

2. **`productos`:**
   * `idProducto` (INT, IDENTITY, Clave Primaria)
   * `nombre` (VARCHAR(100), NOT NULL)
   * `precio` (DECIMAL(18,2), NOT NULL)
   * `idCategoria` (INT, NOT NULL, Clave Foránea `REFERENCES categorias(idCategoria)`)
   * `categoria` (Alias computado para compatibilidad exacta de consultas)

---

## 💻 Módulos y Funcionalidades (CRUD)

1. **Panel Principal (`/Index`):**
   * Dashboard con hipervínculos de navegación hacia todos los formularios.
   * Monitoreo en tiempo real del estado de conexión con SQL Server y métricas de inventario.

2. **Formulario de Alta (`/AltaProducto`):**
   * Carga de nombre y precio mediante controles TextBox.
   * DropDownList de categorías poblado dinámicamente desde SQL Server.
   * Inserción parametrizada segura en base de datos.
   * Hipervínculo para retornar a la página principal.

3. **Formulario de Consulta (`/ConsultaProducto`):**
   * Consulta relacional utilizando **`INNER JOIN`** entre `productos` y `categorias`.
   * Muestra la denominación del rubro en lugar de códigos numéricos.
   * Buscador interactivo por ID de producto o nombre.
   * Hipervínculo para retornar a la página principal.

4. **Formulario de Modificación (`/ModificacionProducto`):**
   * Búsqueda por código ID y carga de campos editables mediante data-binding (`asp-for`).
   * Permite actualizar datos del producto y reasignar categoría.
   * **Validación de negocio:** Verifica en el servidor que al menos un campo se haya modificado antes de ejecutar la actualización.
   * Hipervínculo para retornar a la página principal.

5. **Formulario de Baja (`/BajaProducto`):**
   * Búsqueda por ID con vista previa de confirmación.
   * Confirmación interactiva mediante mensaje al usuario antes de proceder.
   * Eliminación enfocada en la tabla `productos`, **manteniendo intacta la tabla `categorias`** para preservar la integridad referencial.
   * Hipervínculo para retornar a la página principal.

6. **Estilos y Diseño (Puntos Extra):**
   * Hoja de estilos externa personalizada en `wwwroot/css/site.css` con diseño moderno, ergonómico y profesional.

---

## 🚀 Instrucciones para Ejecutar Localmente

### Requisitos:
* .NET SDK 9.0 o superior (compatible con Visual Studio Community 2022).
* SQL Server Express LocalDB (`(localdb)\MSSQLLocalDB`).

### Pasos:
1. **Inicializar la Base de Datos:**
   Ejecutar el script `database_init.sql` desde SQL Server Management Studio, Visual Studio o la terminal:
   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -i database_init.sql
   ```

2. **Ejecutar la Aplicación Web:**
   ```powershell
   dotnet run
   ```

3. **Abrir en el Navegador:**
   Navegar a `http://localhost:5153` (o el puerto indicado en la consola).


