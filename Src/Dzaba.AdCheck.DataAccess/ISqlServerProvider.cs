using Microsoft.EntityFrameworkCore;

namespace Dzaba.AdCheck.DataAccess
{
    public interface ISqlServerProvider
    {
        void Configure(DbContextOptionsBuilder builder, string connectionString);
    }
}
