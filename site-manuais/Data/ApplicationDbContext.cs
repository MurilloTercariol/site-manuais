using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using site_manuais.Models;

namespace site_manuais.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Construtor que recebe as opções de configurações do banco
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet representa uma tabela no banco de dados
        // Cada DbSet permite fazer CRUD na tabela
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Documento> Documentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da relação categoria -> Modulos
            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.Modulos)                //Uma categoria tem muitos módulos
                .WithOne(m => m.Categoria)              //Cada módulo tem uma categoria
                .HasForeignKey(m => m.CategoriaId)      //A chave estrangeira é CategoriaId
                .OnDelete(DeleteBehavior.Cascade);      //Se deletar Categoria, deleta módulos

            // Configuração da categoria Modulos -> Documentos
            modelBuilder.Entity<Modulo>()
                .HasMany(m => m.Documentos)
                .WithOne(d => d.Modulo)
                .HasForeignKey(d => d.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);

            // configuração de índicies para melhorar performance de buscas
            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nome);             // ìndicie na coluna Nome

            modelBuilder.Entity<Modulo>()
                .HasIndex(m => m.Nome);             // Índice na coluna Nome

            modelBuilder.Entity<Documento>()
                .HasIndex(d => d.Titulo);           // ìndice na coluna Titulo

        }
    }
}



