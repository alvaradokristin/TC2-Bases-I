-- # --------------- ELIMINAR BASE DE DATOS SI EXISTE --------------- #
DROP DATABASE IF EXISTS tallermecanicot2;

-- #----------------------------#
-- #   CREAR LA BASE DE DATOS   #
-- #----------------------------#
CREATE DATABASE tallermecanicot2;

-- #-------------------------------#
-- #  SELECCIONA LA BASE DE DATOS  #
-- #-------------------------------#
USE tallermecanicot2;

-- #--------------------------------#
-- #        CREAR CATALOGOS         #
-- #--------------------------------#
-- Se usa en Modelo
CREATE TABLE Marca (
	nombre varchar(20) NOT NULL PRIMARY KEY
);

-- Se usa en Vehiculo
CREATE TABLE Modelo (
	marca varchar(20) NOT NULL,
	nombre varchar(20) NOT NULL,
    
    -- PRIMARY KEYS
    PRIMARY KEY (marca, nombre),
    
    -- FOREIGN KEYS
    FOREIGN KEY (marca) REFERENCES Marca(nombre)
);

-- Se usa en Vehiculo
CREATE TABLE Color (
	nombre varchar(12) NOT NULL PRIMARY KEY
);

-- Se usa en MecanicoXReparacion
CREATE TABLE Rol (
	nombre varchar(12) NOT NULL PRIMARY KEY,
    
    -- CHECK
    CONSTRAINT CHK_Rol CHECK (nombre = 'Lider' OR nombre = 'Soporte' OR nombre = 'Otro')
);

-- #--------------------------------#
-- #        CREAR LAS TABLAS        #
-- #--------------------------------#
CREATE TABLE TallerMecanico (
	cedulaJuridica varchar(10) NOT NULL PRIMARY KEY,
	nombre varchar(15) NOT NULL,
	telefono varchar(10) NOT NULL
);

CREATE TABLE Vehiculo (
	placa varchar(6) NOT NULL PRIMARY KEY,
	marca varchar(20) NOT NULL,
	modelo varchar(20) NOT NULL,
    anno varchar(4) NOT NULL,
    color varchar(12) NOT NULL,
    
    -- FOREIGN KEYS
    FOREIGN KEY (marca, modelo) REFERENCES Modelo(marca, nombre),
    FOREIGN KEY (color) REFERENCES Color(nombre)
);

CREATE TABLE Cliente (
	cedula varchar(12) NOT NULL PRIMARY KEY,
	nombre varchar(15) NOT NULL,
	apellido1 varchar(15) NOT NULL,
	apellido2 varchar(15) NOT NULL,
    direccion varchar(65) NOT NULL
);

CREATE TABLE TelefonoClientes (
	cedulaCliente varchar(12) NOT NULL,
    telefonoCliente varchar(10) NOT NULL,
    
    -- PRIMARY KEYS
    PRIMARY KEY (cedulaCliente, telefonoCliente),
    
    -- FOREIGN KEYS
	FOREIGN KEY (cedulaCliente) REFERENCES Cliente(cedula)
);

CREATE TABLE Mecanico (
	cedula varchar(12) NOT NULL PRIMARY KEY,
	nombre varchar(15) NOT NULL,
	apellido1 varchar(15) NOT NULL,
	apellido2 varchar(15) NOT NULL
);

CREATE TABLE Repuesto (
	codigo varchar(8) NOT NULL PRIMARY KEY,
	nombre varchar(15) NOT NULL,
	precio decimal(8,2) NOT NULL,
    
    -- CHECK
    CONSTRAINT CHK_Repuesto CHECK (precio > 100 AND precio <= 9999999)
);

CREATE TABLE Reparacion (
	consecutivo varchar(8) NOT NULL PRIMARY KEY,
	cliente varchar(12) NOT NULL,
    tallerMecanico varchar(10) NOT NULL,
	vehiculo varchar(6) NOT NULL,
    mecanicoPrincipal varchar(12) NOT NULL,
    fechaReparacion date NOT NULL,
    
    -- FOREIGN KEYS
    FOREIGN KEY (cliente) REFERENCES Cliente(cedula),
    FOREIGN KEY (tallerMecanico) REFERENCES TallerMecanico(cedulaJuridica),
    FOREIGN KEY (vehiculo) REFERENCES Vehiculo(placa),
    FOREIGN KEY (mecanicoPrincipal) REFERENCES Mecanico(cedula)
);

CREATE TABLE Factura (
	consecutivo varchar(8) NOT NULL,
	anno varchar(4) NOT NULL,
    nombre varchar(15) NOT NULL,
    precio decimal(10,2) NOT NULL,
    consecutivoReparacion varchar(8) NOT NULL,
	
    -- PRIMARY KEYS
    PRIMARY KEY (consecutivo, anno),
    
    -- FOREIGN KEYS
	FOREIGN KEY (consecutivoReparacion) REFERENCES Reparacion(consecutivo),
    
    -- CHECK
    CONSTRAINT CHK_Factura CHECK (precio > 1000 AND precio <= 99999999)
);

CREATE TABLE LineaFactura (
	consecutivFactura varchar(8) NOT NULL,
    annoFactura varchar(4) NOT NULL,
    codigoRMO varchar(8) NOT NULL,
    precioReal decimal(10,2) NOT NULL,
    descuento decimal(4,2) NOT NULL,
    
    -- PRIMARY KEYS
    PRIMARY KEY (consecutivFactura, annoFactura, codigoRMO),
    
    -- FOREIGN KEYS
	FOREIGN KEY (consecutivFactura, annoFactura) REFERENCES Factura(consecutivo, anno),
    
    -- CHECK
    CONSTRAINT CHK_LineaFactura CHECK (precioReal > 1000 AND precioReal <= 99999999 AND descuento <= 70)
);

CREATE TABLE MecanicoXReparacion (
	consecutivRepa varchar(8) NOT NULL,
    cedulaMecanico varchar(12) NOT NULL,
    mecanicoRol varchar(12) NOT NULL,
	
    -- PRIMARY KEYS
    PRIMARY KEY (consecutivRepa, cedulaMecanico),
    
    -- FOREIGN KEYS
	FOREIGN KEY (consecutivRepa) REFERENCES Reparacion(consecutivo),
    FOREIGN KEY (cedulaMecanico) REFERENCES Mecanico(cedula),
    FOREIGN KEY (mecanicoRol) REFERENCES Rol(nombre)
);

CREATE TABLE RepuestoXReparacion (
	consecutivRepa varchar(8) NOT NULL,
    codigoRepuesto varchar(8) NOT NULL,
    cantidad tinyint NOT NULL,
    
    -- PRIMARY KEYS
    PRIMARY KEY (consecutivRepa, codigoRepuesto),
	
    -- FOREIGN KEYS
	FOREIGN KEY (consecutivRepa) REFERENCES Reparacion(consecutivo),
    FOREIGN KEY (codigoRepuesto) REFERENCES Repuesto(codigo),
    
    -- CHECK
    CONSTRAINT CHK_RepuestoXReparacion CHECK (cantidad > 0 AND cantidad < 128)
);

CREATE TABLE ActividadManoObra (
	codigo varchar(8) NOT NULL PRIMARY KEY,
    nombre varchar(15) NOT NULL,
	precio decimal(8,2) NOT NULL
);

CREATE TABLE ManoObraXReparacion (
	consecutivRepa varchar(8) NOT NULL,
    codigoActividadManoObra varchar(8) NOT NULL,
    
    -- PRIMARY KEYS
    PRIMARY KEY (consecutivRepa, codigoActividadManoObra),
    
    -- FOREIGN KEYS
	FOREIGN KEY (consecutivRepa) REFERENCES Reparacion(consecutivo),
    FOREIGN KEY (codigoActividadManoObra) REFERENCES ActividadManoObra(codigo)
);

-- #--------------------------------------#
-- #        AGREGAR DATOS A TABLAS        #
-- #--------------------------------------#
INSERT INTO TallerMecanico (cedulaJuridica, nombre, telefono)
VALUES 
('J11111001', 'T. Gonzalez', '8888-8801'),
('J11111002', 'T. Alvarado', '8888-8802'),
('J11111003', 'T. Rojas', '8888-8803');

INSERT INTO Rol (nombre)
VALUES
('Lider'),
('Soporte'),
('Otro');

INSERT INTO Marca (nombre)
VALUES 
('Toyota'),
('Audi'),
('Honda');

INSERT INTO Modelo (marca, nombre)
VALUES
('Toyota', 'M0'),
('Toyota', 'M1'),
('Audi', 'M2'),
('Audi', 'M3'),
('Audi', 'M4'),
('Honda', 'M4');

INSERT INTO Color (nombre)
VALUES
('Rojo'),
('Negro'),
('Blanco'),
('Azul'),
('Plateado'),
('Cobre');

INSERT INTO Vehiculo (placa, marca, modelo, anno, color)
VALUES 
('PLC101', 'Toyota', 'M0', '2020', 'Cobre'),
('PLC102', 'Toyota', 'M1', '2021', 'Azul'),
('PLC103', 'Audi', 'M2', '2022', 'Negro');

INSERT INTO Cliente (cedula, nombre, apellido1, apellido2, direccion)
VALUES 
('111111110', 'Kristin', 'Alvarado', 'Gonzalez', 'Barrio Los Angeles, San Jose'),
('111111111', 'Ana', 'Gonzalez', 'Aguilera', 'Barrio Los Angeles, San Jose'),
('111111112', 'Leda', 'Gonzalez', 'Aguilera', 'Barrio Los Angeles, San Jose'),
('111111113', 'Tatiana', 'Gutierrez', 'Rojas', 'Hatillo Centro, San Jose');

INSERT INTO Mecanico (cedula, nombre, apellido1, apellido2)
VALUES
('222222220', 'Carmen', 'Sandiego', 'Rojas'),
('222222221', 'Diego', 'Primo', 'Dora'),
('222222222', 'Vannesa', 'Guerra', 'Gonzalez'),
('222222223', 'Sarah', 'Lantigua', 'Guerra'),
('222222224', 'Omar', 'Slazar', 'Calderon'),
('222222225', 'Alejandro', 'Figueres', 'Bui');

INSERT INTO Reparacion (consecutivo, cliente, tallerMecanico, vehiculo, mecanicoPrincipal, fechaReparacion)
VALUES
('RCO101', '111111110', 'J11111001', 'PLC101', '222222220', '2022-10-08'),
('RCO102', '111111111', 'J11111001', 'PLC102', '222222221', '2022-09-08');

INSERT INTO MecanicoXReparacion (consecutivRepa, cedulaMecanico, mecanicoRol)
VALUES
('RCO101', '222222220', 'Lider'),
('RCO102', '222222221', 'Lider');

INSERT INTO ActividadManoObra (codigo, nombre, precio)
VALUES
('ACT901', 'Activ 91', 15500.00),
('ACT902', 'Activ 92', 25500.00),
('ACT903', 'Activ 93', 35500.00),
('ACT904', 'Activ 94', 45500.00);

INSERT INTO Repuesto (codigo, nombre, precio)
VALUES
('REP101', 'Repuesto 01', 30000.00),
('REP102', 'Repuesto 02', 60000.00),
('REP103', 'Repuesto 03', 90000.00),
('REP104', 'Repuesto 04', 100000.00);

INSERT INTO ManoObraXReparacion (consecutivRepa, codigoActividadManoObra)
VALUES
('RCO101', 'ACT901'),
('RCO102', 'ACT902'),
('RCO101', 'ACT903'),
('RCO102', 'ACT904');

INSERT INTO RepuestoXReparacion (consecutivRepa, codigoRepuesto, cantidad)
VALUES
('RCO101', 'REP101', 2),
('RCO102', 'REP102', 3),
('RCO101', 'REP103', 1),
('RCO102', 'REP104', 2);

-- #----------------------------#
-- #        CREAR VISTAS        #
-- #----------------------------#
-- Vista que suma el precio de todas las actividades de mano de obra por reparacion
CREATE VIEW sumaManoObra AS (
SELECT
	r.consecutivo,
	SUM(amo.precio)  AS precioManoObra
FROM Reparacion AS r
JOIN ManoObraXReparacion AS mor ON mor.consecutivRepa = r.consecutivo
JOIN ActividadManoObra AS amo ON amo.codigo = mor.codigoActividadManoObra
GROUP BY r.consecutivo);

-- Vista que suma el precio de todos los repuestos usados por reparacion
CREATE VIEW sumaRepuestos AS (
SELECT
	r.consecutivo,
    SUM(repu.precio) * rr.cantidad AS precioRepuestos
FROM Reparacion AS r
JOIN RepuestoXReparacion AS rr ON rr.consecutivRepa = r.consecutivo
JOIN Repuesto AS repu ON repu.codigo = rr.codigoRepuesto
GROUP BY r.consecutivo);

-- Vista para el calculo del subtotal y total de la factura
CREATE VIEW preciosFactura AS (
SELECT 
	r.consecutivo,
	smo.precioManoObra,
    sr.precioRepuestos,
    smo.precioManoObra + sr.precioRepuestos AS subtotal,
    (smo.precioManoObra + (smo.precioManoObra * 0.05)) + (sr.precioRepuestos + (sr.precioRepuestos * 0.15)) AS total
FROM Reparacion AS r
LEFT JOIN sumaManoObra AS smo ON smo.consecutivo = r.consecutivo
LEFT JOIN sumaRepuestos AS sr ON sr.consecutivo = r.consecutivo);

-- Vista para obtener la informacion del cliente y el vehiculo de una reparacion
CREATE VIEW clienteVehiculoXReparacion AS (
SELECT
	r.consecutivo AS reparacion,
	c.cedula,
    CONCAT(`nombre`, ' ', `apellido1`, ' ', `apellido2`) AS nombreCompletoC,
    c.direccion,
    v.placa,
    v.marca,
    v.modelo,
    v.anno,
    v.color
FROM Reparacion AS r
JOIN Cliente AS c ON c.cedula = r.cliente
JOIN Vehiculo AS v ON v.placa = r.vehiculo);

-- Vista para obtener los repuestos usados en una reparacion
CREATE VIEW repuestosInfoXReparacion AS (
SELECT
	r.consecutivo AS reparacion,
    rep.codigo,
    rep.nombre,
    rep.precio,
    rxr.cantidad
FROM Reparacion AS r
JOIN RepuestoXReparacion AS rxr ON rxr.consecutivRepa = r.consecutivo
JOIN Repuesto AS rep ON rep.codigo = rxr.codigoRepuesto);

-- Vista para obtener la informacion de todos los mecanicos de una reparacion
CREATE VIEW mecanicosInfoXReparacion AS (
SELECT
	r.consecutivo AS reparacion,
    m.cedula,
    CONCAT(`nombre`, ' ', `apellido1`, ' ', `apellido2`) AS nombreCompletoM,
    mxr.mecanicoRol
FROM Reparacion AS r
JOIN MecanicoXReparacion AS mxr ON mxr.consecutivRepa = r.consecutivo
JOIN Mecanico AS m ON m.cedula = mxr.cedulaMecanico);

-- Vista para obtener la informacion las actividades de mano de obra usadas en una reparacion
CREATE VIEW manoObraInfoXReparacion AS (
SELECT
	r.consecutivo AS reparacion,
    amo.codigo,
    amo.nombre,
    amo.precio
FROM Reparacion AS r
JOIN ManoObraXReparacion AS mor ON mor.consecutivRepa = r.consecutivo
JOIN ActividadManoObra AS amo ON amo.codigo = mor.codigoActividadManoObra);

-- Vista para obtener la informacion de una reparacion, pero sustituir los ids (de cliente y mecanico) por el nombre
CREATE VIEW basicInfoXReparacion AS (
SELECT 
	r.consecutivo,
    cvxr.nombreCompletoC AS cliente,
    cvxr.placa AS vehiculo,
    tm.nombre AS taller,
    mixr.nombreCompletoM AS mecanicoPrincipal,
    r.fechaReparacion AS fecha
FROM Reparacion AS r
JOIN clienteVehiculoXReparacion AS cvxr ON cvxr.reparacion = r.consecutivo
JOIN TallerMecanico AS tm ON tm.cedulaJuridica = r.tallerMecanico
JOIN mecanicosInfoXReparacion AS mixr ON mixr.reparacion = r.consecutivo
WHERE mixr.mecanicoRol = 'Lider');

-- Vista para obtener la informacion que se debe mostrar para facturar
-- Agregar un WHERE al SELECT que la use, para tomar la informacion de una sola reparacion
CREATE VIEW facturaBasicXReparacion AS (
SELECT 
	pf.consecutivo,
	cvxr.cedula,
    cvxr.nombreCompletoC,
    cvxr.direccion,
    cvxr.placa,
    cvxr.marca,
    cvxr.modelo,
    cvxr.anno,
    cvxr.color,
    pf.precioManoObra,
    pf.precioRepuestos,
    pf.subtotal,
    pf.total
FROM clienteVehiculoXReparacion AS cvxr
JOIN preciosFactura AS pf ON pf.consecutivo = cvxr.reparacion);

-- #-----------------------------------#
-- #        MOSTRAR INFORMACION        #
-- #-----------------------------------#
-- SELECT * FROM Cliente;

-- SELECT * FROM Vehiculo;

-- SELECT * FROM Mecanico;

-- SELECT * FROM Repuesto;

-- SELECT * FROM ActividadManoObra;

SELECT cedula, CONCAT(`nombre`, ' ', `apellido1`, ' ', `apellido2`) AS nombreCompleto FROM Mecanico;

-- SELECT * FROM Reparacion;

-- SELECT * FROM sumaManoObra;

-- SELECT * FROM sumaRepuestos;

-- SELECT * FROM preciosFactura;

SELECT total FROM preciosFactura WHERE consecutivo = 'RCO101';

-- SELECT * FROM Factura;

SELECT * FROM MecanicoXReparacion;

SELECT * FROM RepuestoXReparacion;

SELECT * FROM ManoObraXReparacion;

SELECT *
FROM basicInfoXReparacion
WHERE consecutivo NOT IN (
SELECT DISTINCT consecutivoReparacion
FROM Factura);