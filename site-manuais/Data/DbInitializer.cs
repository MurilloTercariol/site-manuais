using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace site_manuais.Data
{
    /// <summary>
    /// Responsável por inicializar o banco de dados.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Método que cria usuário Admin inicial se não existir
        /// </summary>
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            // Obtém os serviços necessários
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Define os dados do usuário de Admin
            string adminUsername = "tz";
            string adminPassword = "Tz156450";

            // verificar se o papel Admin existe
            //se não existir, criar

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                Console.WriteLine("Criando papel de Admin...");
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                Console.WriteLine("Papel Admin criado com sucesso.");

                // verificar se já existe um usuário com esse username

                var existingUser = await userManager.FindByNameAsync(adminUsername);

                if (existingUser == null)
                {
                    Console.WriteLine("Criando usuário de Admin...");

                    // Criar usuário
                    var adminUser = new IdentityUser()
                    {
                        UserName = adminUsername,
                        Email = "admin@localhost.com",   // Email opcional (pode ser fake)
                        EmailConfirmed = true
                    };

                    // Tenta criar usuário com a senha
                    var result = await userManager.CreateAsync(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        Console.WriteLine("Usuário admin criado com sucesso");

                        // atribuir papel de admin ao usuário admin
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        Console.WriteLine("Papel admin atribuído ao usuário.");
                    }
                    else
                    {
                        // Se falhar
                        Console.WriteLine("Erro ao criar usuário admin: ");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($" - {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Usuário Admin já existe.");
                }
            }
        }
    }
}
