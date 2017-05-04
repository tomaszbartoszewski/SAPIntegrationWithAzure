using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CoreApplication.Models
{
    public class CoreApplicationContext : DbContext
    {
        public CoreApplicationContext (DbContextOptions<CoreApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<CoreApplication.Models.Movie> Movie { get; set; }
    }
}
