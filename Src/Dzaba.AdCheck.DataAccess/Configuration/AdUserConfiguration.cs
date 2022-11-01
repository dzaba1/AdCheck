using Dzaba.AdCheck.DataAccess.Contracts.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dzaba.AdCheck.DataAccess.Configuration
{
    internal sealed class AdUserConfiguration : EntityConfiguration<AdUser>
    {
        protected override void Configure(EntityTypeBuilder<AdUser> builder)
        {
            builder.HasKey(p => new {p.PollingId, p.Sid});

            builder.HasOne(p => p.Polling)
                .WithMany(p => p.Users)
                .HasForeignKey(p => p.PollingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
