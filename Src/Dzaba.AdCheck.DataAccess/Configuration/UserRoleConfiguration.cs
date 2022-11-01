using Dzaba.AdCheck.DataAccess.Contracts.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dzaba.AdCheck.DataAccess.Configuration
{
    internal sealed class UserRoleConfiguration : EntityConfiguration<UserRole>
    {
        protected override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasOne(p => p.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
