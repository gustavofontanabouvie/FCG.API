using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;

namespace Fcg.Data.Context;

public static class DatabaseMigrationsExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<FcgDbContext>>();
            var context = services.GetRequiredService<FcgDbContext>(); // <-- Sua variável 'context'

            try
            {
                logger.LogInformation("Iniciando 'ApplyMigrations'. Tentando conectar ao banco de dados...");

                var retries = 10;
                while (retries > 0)
                {
                    try
                    {

                        context.Database.Migrate();

                        logger.LogInformation("Migrações aplicadas com sucesso. Banco de dados está pronto.");
                        break;
                    }
                    catch (NpgsqlException ex)
                    {
                        logger.LogWarning(ex, "Falha ao conectar ao banco (PostgreSQL). Tentando novamente em 5 segundos...");
                        retries--;
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Um erro inesperado ocorreu durante o ApplyMigrations.");
            }
        }
    }
}

