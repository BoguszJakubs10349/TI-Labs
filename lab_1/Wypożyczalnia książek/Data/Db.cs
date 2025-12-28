using Microsoft.Data.SqlClient;

namespace TI_Lab01_Library.Data;

public sealed class Db
{
    private readonly string _cs;
    public Db(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;

    public SqlConnection CreateConnection() => new SqlConnection(_cs);
}
