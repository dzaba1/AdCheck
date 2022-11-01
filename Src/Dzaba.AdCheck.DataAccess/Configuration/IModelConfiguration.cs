using Microsoft.EntityFrameworkCore;

namespace Dzaba.AdCheck.DataAccess.Configuration
{
    public interface IModelConfiguration
    {
        void Configure(ModelBuilder builder);
    }
}
