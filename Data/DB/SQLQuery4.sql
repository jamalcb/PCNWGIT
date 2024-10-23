USE [OCPCProjectDB]
GO

/****** Object:  View [dbo].[VwPojectRange]    Script Date: 03-01-2023 00:38:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VwPojectRange] AS
			SELECT Idx
			,EstCost
			,dbo.fn_removecharactersfromstring(Case when idx > 0 then substring(Num1, 0, Idx) else Num1 end) AS Num1
			,dbo.fn_removecharactersfromstring(Case when idx > 0 then substring(Num1, Idx+1, len(Num1)) else null end) AS Num2
			,Num1 Num
			  FROM (
				SELECT 
					--dbo.fn_removecharactersfromstring(Num1) as NNum,
					 CHARINDEX('-',Num1) Idx,
					Num1,EstCost 
					FROM(
						SELECT Num, CASE WHEN Idx > 0 THEN SUBSTRING(Num,0,Idx) else Num END Num1,CHARINDEX(Num,' ') Idx1,EstCost FROM(
						SELECT REPLACE(REPLACE(REPLACE(EstCost,'$',''),'<','0-'),'&','') Num,CHARINDEX('(',REPLACE(REPLACE(REPLACE(EstCost,'$',''),'<','0-'),'&','')) Idx 
						,EstCost
						FROM  tblProject where EstCost is not null and EstCost <> '-' and EstCost <> 'N/A'
					)T 
				)T1
			)T2
GO


