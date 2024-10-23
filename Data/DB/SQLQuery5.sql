Create proc [dbo].[SP_GetFilterProject]
(
	 @RangeFromF numeric(18,2) = 10000000 
	,@RangeToF numeric(18,2) = 20000000
)
AS
BEGIN
	select *
	,[dbo].[udf_GetRangeFilter](ISNULL(Case when Num1 = '' THEN '0' ELSE Num1 END,0),ISNULL(Case when Num2 = '' THEN '0' ELSE Num2 END,0),@RangeFromF,@RangeToF) AS Flag
	from [VwPojectRange]
END
