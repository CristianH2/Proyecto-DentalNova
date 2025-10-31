USE [DentalNovaBD_test2]
GO

INSERT INTO [dbo].[Tratamientos]
           ([Nombre]
           ,[Descripcion]
           ,[Costo]
           ,[DuracionDias]
           ,[Activo])
     VALUES

-- ########## TRATAMIENTOS DEL USUARIO ##########
           (N'Limpieza Dental Profesional (Profilaxis)'
           ,N'Limpieza estándar para remover placa y sarro.'
           ,850.00
           ,1
           ,0), --

           (N'Aplicación de Flúor'
           ,N'Fortalece el esmalte dental para prevenir caries.'
           ,500.00
           ,1
           ,1), --

           (N'Puentes Dentales'
           ,N'Reemplazan uno o más dientes ausentes usando los dientes adyacentes como soporte.'
           ,13500.00
           ,15
           ,1), --

-- ########## DIAGNÓSTICO Y PREVENCIÓN ##########
           (N'Consulta de Diagnóstico'
           ,N'Evaluación inicial completa, plan de tratamiento y presupuesto.'
           ,450.00
           ,1
           ,1),
           
           (N'Radiografía Periapical'
           ,N'Toma de radiografía digital de un diente específico.'
           ,150.00
           ,1
           ,1),
           
           (N'Selladores de Fosas y Fisuras (por diente)'
           ,N'Capa protectora de resina aplicada en molares para prevenir caries.'
           ,350.00
           ,1
           ,1),

-- ########## ODONTOLOGÍA RESTAURADORA ##########
           (N'Resina (Empaste) 1 Superficie'
           ,N'Restauración de caries simple (una superficie) con resina del color del diente.'
           ,900.00
           ,1
           ,1),
           
           (N'Resina (Empaste) 2 Superficies'
           ,N'Restauración de caries compuesta (dos superficies) con resina.'
           ,1200.00
           ,1
           ,1),
           
           (N'Corona de Porcelana (Libre de Metal)'
           ,N'Funda completa de porcelana (Zirconia o E-Max) para un diente dañado.'
           ,7500.00
           ,14
           ,1),
           
           (N'Incrustación (Inlay/Onlay) de Porcelana'
           ,N'Restauración indirecta fabricada en laboratorio para caries extensas.'
           ,5500.00
           ,10
           ,1),

-- ########## ENDODONCIA ##########
           (N'Endodoncia (Tratamiento de Conducto) - Molar'
           ,N'Remoción del nervio infectado en un diente molar (multirradicular).'
           ,4500.00
           ,7
           ,1),
           
           (N'Endodoncia (Tratamiento de Conducto) - Premolar'
           ,N'Remoción del nervio infectado en un diente premolar (unirradicular).'
           ,3500.00
           ,7
           ,1),

-- ########## CIRUGÍA ORAL ##########
           (N'Extracción Simple'
           ,N'Extracción de un diente visible que no requiere cirugía.'
           ,800.00
           ,1
           ,1),
           
           (N'Extracción Tercer Molar (Muela del Juicio)'
           ,N'Extracción quirúrgica de muela del juicio (puede estar impactada).'
           ,3000.00
           ,3
           ,1),

-- ########## IMPLANTOLOGÍA ##########
           (N'Implante Dental (Fase Quirúrgica)'
           ,N'Colocación quirúrgica del tornillo de titanio en el hueso maxilar.'
           ,12000.00
           ,120 -- Incluye tiempo de osteointegración
           ,1),
           
           (N'Corona sobre Implante (Fase Protésica)'
           ,N'Colocación de la corona de porcelana sobre el implante ya integrado.'
           ,8000.00
           ,14
           ,1),

-- ########## PERIODONCIA ##########
           (N'Raspado y Alisado Radicular (Curetaje)'
           ,N'Limpieza profunda por cuadrante para tratar la enfermedad periodontal (gingivitis/periodontitis).'
           ,1500.00
           ,3
           ,1),

-- ########## ESTÉTICA DENTAL ##########
           (N'Blanqueamiento Dental (Consultorio)'
           ,N'Sesión de blanqueamiento profesional con luz LED.'
           ,4000.00
           ,1
           ,1),
           
           (N'Carillas de Porcelana (por diente)'
           ,N'Lámina fina de porcelana cementada en la cara frontal del diente para mejorar estética.'
           ,8500.00
           ,14
           ,1),

-- ########## ORTODONCIA Y ORTOPEDIA ##########
           (N'Tratamiento de Ortodoncia (Brackets Metálicos)'
           ,N'Tratamiento completo de alineación dental con brackets metálicos (precio total aprox).'
           ,25000.00
           ,730 -- 2 años
           ,1),
           
           (N'Guarda Oclusal (Bruxismo)'
           ,N'Férula de descarga personalizada para pacientes que rechinan los dientes.'
           ,3500.00
           ,7
           ,1),
           
           (N'Prótesis Dental Removible (Parcial)'
           ,N'Aparato removible que sustituye varios dientes ausentes.'
           ,6000.00
           ,21
           ,1)
GO