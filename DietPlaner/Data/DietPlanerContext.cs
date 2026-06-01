using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Models;

namespace DietPlaner.Data
{
    public class DietPlanerContext : DbContext
    {
        public DietPlanerContext (DbContextOptions<DietPlanerContext> options)
            : base(options)
        {
        }

        public DbSet<DietPlaner.Models.Diet> Diet { get; set; } = default!;
        public DbSet<DietPlaner.Models.Patient> Patient { get; set; } = default!;
        public DbSet<DietPlaner.Models.User> User { get; set; } = default!;
        public DbSet<DietPlaner.Models.Ward> Ward { get; set; } = default!;
    }
}
