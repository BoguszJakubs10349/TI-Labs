using Microsoft.Data.SqlClient;

namespace Lab02_Shop.Data;

public class Db
{
    private readonly string _cs;
    public Db(IConfiguration cfg)
        => _cs = cfg.GetConnectionString("Default")!;

    public SqlConnection CreateConnection()
        => new SqlConnection(_cs);
}
