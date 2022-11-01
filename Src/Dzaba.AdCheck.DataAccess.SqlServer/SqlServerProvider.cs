using Microsoft.EntityFrameworkCore;

namespace Dzaba.AdCheck.DataAccess.SqlServer
{
    internal sealed class SqlServerProvider : ISqlServerProvider
    {
        public void Configure(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }
    }
}
