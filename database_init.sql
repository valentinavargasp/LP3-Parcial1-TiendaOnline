-- ========================================================
-- ISSD - Laboratorio de Programación 3
-- Parcial 1: Sistema para Tienda Online (TechStore)
-- Script de Creación de Base de Datos, Tablas y Registros
-- ========================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'tienda_db')
BEGIN
    CREATE DATABASE tienda_db;
END;
GO

USE tienda_db;
GO

-- Eliminación previa para pruebas limpias
IF OBJECT_ID('productos', 'U') IS NOT NULL DROP TABLE productos;
IF OBJECT_ID('categorias', 'U') IS NOT NULL DROP TABLE categorias;
GO

-- Tabla categorias
-- Campos requeridos: idCategoria (Clave Primaria), descripción
CREATE TABLE categorias (
    idCategoria INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(50) NOT NULL
);
GO

-- Tabla productos
-- Campos requeridos: idProducto (Clave Primaria), nombre, precio, categoría
CREATE TABLE productos (
    idProducto INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    precio DECIMAL(18,2) NOT NULL,
    idCategoria INT NOT NULL CONSTRAINT FK_productos_categorias FOREIGN KEY REFERENCES categorias(idCategoria),
    -- Alias computado para permitir consultas tanto por 'categoria' como por 'idCategoria'
    categoria AS idCategoria
);
GO

-- Carga inicial de categorías de muestra
INSERT INTO categorias (descripcion) VALUES 
('Notebooks y Computadoras'),
('Smartphones y Celulares'),
('Audio y Auriculares'),
('Periféricos y Accesorios'),
('Monitores y Video');
GO

-- Al menos 5 registros de productos cargados manualmente para demostración 
INSERT INTO productos (nombre, precio, idCategoria) VALUES
('Notebook Lenovo IdeaPad 3 15.6', 850000.00, 1),
('Smartphone Samsung Galaxy S23 256GB', 980000.00, 2),
('Auriculares Sony WH-1000XM5 Bluetooth', 420000.00, 3),
('Teclado Mecánico Redragon Kumara RGB', 65000.00, 4),
('Monitor Samsung Curvo 24 FHD 75Hz', 195000.00, 5),
('Mouse Inalámbrico Logitech MX Master 3S', 125000.00, 4),
('Notebook Dell Inspiron 15 Core i5', 920000.00, 1);
GO

-- Verificación de inserciones con JOIN
SELECT 
    p.idProducto,
    p.nombre,
    p.precio,
    c.descripcion AS categoria
FROM productos p
INNER JOIN categorias c ON p.idCategoria = c.idCategoria;
GO
