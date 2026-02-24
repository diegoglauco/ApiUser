using Microsoft.EntityFrameworkCore;
using UsuariosApi.Models;

namespace UsuariosApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios"); // nome da tabela no banco

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("usu_codigo");
                entity.Property(e => e.Login).HasColumnName("usu_login");
                entity.Property(e => e.Senha).HasColumnName("usu_senha");

                // Mapeando TINYINT para bool
                entity.Property(e => e.Admin) 
                    .HasColumnName("usu_admin")
                    .HasConversion(
                    v => v ? (byte)1 : (byte)0,   // bool → tinyint (byte)
                    v => v == (byte)1             // tinyint (byte) → bool
                );



                entity.Property(e => e.DataCriacao).HasColumnName("usu_datacriacao");
            });
        }
    }
}
