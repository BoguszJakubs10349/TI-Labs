using Microsoft.Data.SqlClient;

namespace Lab05_Kanban.Data;

public class Db
{
    private readonly string _cs;
    public Db(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;
    public SqlConnection CreateConnection() => new SqlConnection(_cs);
}
