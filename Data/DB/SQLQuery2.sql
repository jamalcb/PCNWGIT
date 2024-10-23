
create FUNCTION [dbo].[udf_GetRangeFilter]
(
	-- Add the parameters for the function here
	 @RangeFrom numeric(18,2) = null  -- Poject EstCost From 100
	,@RangeTo numeric(18,2) = null   -- Poject EstCost To	 200
	,@RangeFromF numeric(18,2) = null -- Poject Filter From	 300	
	,@RangeToF numeric(18,2) = null   -- Poject Filter To	 500
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar int

	IF(ISNULL(@RangeTo,0) = 0)
		SET @RangeTo = @RangeFrom
	-- Add the T-SQL statements to compute the return value here
	IF(@RangeTo >= @RangeFromF)
	BEGIN
		WHILE(@RangeFrom <= @RangeTo)
		BEGIN
			IF(@RangeFrom BETWEEN @RangeFromF AND @RangeToF)
				SET @ResultVar = 1
				IF(@ResultVar = 1)
					SET @RangeFrom = @RangeTo
			SET @RangeFrom += 1
		END
	END
	ELSE
	BEGIN
		SET @ResultVar = 0
	END
	-- Return the result of the function
	RETURN @ResultVar

END
GO

