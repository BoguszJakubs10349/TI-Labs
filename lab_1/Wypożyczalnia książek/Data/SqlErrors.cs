using Microsoft.Data.SqlClient;

namespace TI_Lab01_Library.Data;

public static class SqlErrors
{
    // 2627: Violation of PRIMARY KEY/UNIQUE constraint
    // 2601: Cannot insert duplicate key row in object with unique index
    public static bool IsUniqueViolation(SqlException ex) => ex.Number is 2627 or 2601;
}
