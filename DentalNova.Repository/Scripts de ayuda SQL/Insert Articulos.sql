USE [DentalNovaBD_test2]
GO

INSERT INTO [dbo].[Articulos]
           ([Categoria]
           ,[Nombre]
           ,[Descripcion]
           ,[Codigo]
           ,[Reutilizable]
           ,[Stock]
           ,[Activo])
     VALUES
-- Insumos Desechables (Categoría 1)
           (1
           ,'Guantes de Nitrilo (Caja 100u)'
           ,'Guantes de examen de nitrilo sin polvo, talla M.'
           ,'GL-NIT-M-100'
           ,0 -- No reutilizable
           ,50
           ,1),
           
           (1
           ,'Mascarillas Quirúrgicas (Caja 50u)'
           ,'Cubrebocas tricapa con filtro bacteriano.'
           ,'MASK-TRI-50'
           ,0 -- No reutilizable
           ,100
           ,1),
           
           (1
           ,'Eyectores de Saliva (Bolsa 100u)'
           ,'Cánulas de aspiración desechables, punta suave.'
           ,'EYEC-SAL-100'
           ,0 -- No reutilizable
           ,200
           ,1),
           
           (1
           ,'Lidocaína 2% con Epinefrina (Caja 50u)'
           ,'Cartuchos de anestésico local para uso dental.'
           ,'ANE-LID-EPI-50'
           ,0 -- No reutilizable
           ,25
           ,1),

-- Instrumental (Categoría 2)
           (2
           ,'Espejo Dental #5'
           ,'Espejo bucal de acero inoxidable, rosca estándar.'
           ,'INST-ESP-05'
           ,1 -- Reutilizable (se esteriliza)
           ,20
           ,1),
           
           (2
           ,'Explorador Dental #23 (Sonda)'
           ,'Sonda de exploración de acero inoxidable, doble extremo.'
           ,'INST-EXP-23'
           ,1 -- Reutilizable (se esteriliza)
           ,20
           ,1),
           
           (2
           ,'Fresa de Diamante (Bola, Grano Fino)'
           ,'Fresa para turbina (FG), forma de bola, grano fino.'
           ,'INST-FRE-D01'
           ,1 -- Reutilizable (se esteriliza)
           ,50
           ,1),

-- Materiales de Restauración (Categoría 3)
           (3
           ,'Resina Compuesta A2 (Jeringa 4g)'
           ,'Composite universal nanohíbrido, color A2.'
           ,'MAT-RES-A2-4G'
           ,0 -- Es un material, no se reutiliza
           ,30
           ,1),
           
           (3
           ,'Alginato para Impresiones (Bolsa 450g)'
           ,'Material de impresión de fraguado rápido.'
           ,'MAT-ALG-450G'
           ,0 -- Es un material, no se reutiliza
           ,15
           ,1),

-- Esterilización (Categoría 4)
           (4
           ,'Bolsa Esterilización (Caja 200u)'
           ,'Bolsas autosellantes para autoclave, 90x260mm.'
           ,'EST-BOL-90260'
           ,0 -- No reutilizable
           ,10
           ,1)
GO