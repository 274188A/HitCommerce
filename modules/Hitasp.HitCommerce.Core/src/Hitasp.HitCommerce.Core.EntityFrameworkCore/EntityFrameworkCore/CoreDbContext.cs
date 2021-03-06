using Hitasp.HitCommerce.Core.Countries;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Hitasp.HitCommerce.Core.EntityFrameworkCore
{
    [ConnectionStringName(CoreDbProperties.ConnectionStringName)]
    public class CoreDbContext : AbpDbContext<CoreDbContext>, ICoreDbContext
    {
        public DbSet<Country> Countries { get; set; }
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        public CoreDbContext(DbContextOptions<CoreDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureCore();
        }
    }
}