namespace GestionHospitaliere.Backend;

using Microsoft.Extensions.DependencyInjection;
using GestionHospitaliere.Backend.Data;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("--- Test de connexion au serveur PostgreSQL ---");

        // 1. Initialisation du conteneur de services
        var services = new ServiceCollection();

        // 2. Appel de notre méthode d'extension : charge appsettings.json, Database, Repositories et Services
        services.AddBackendServices();

        // 3. Construction du ServiceProvider
        var serviceProvider = services.BuildServiceProvider();

        // 4. Récupération de l'instance de Database
        var database = serviceProvider.GetRequiredService<Database>();

        // 5. Exécution du test de connexion
        bool estConnecte = await database.TesterConnexionAsync();

        // 6. Affichage du résultat avec 'if'
        if (estConnecte)
        {
            Console.WriteLine(" SUCCESS : Connexion à PostgreSQL établie avec succès !");
        }
        else
        {
            Console.WriteLine(" FAILED : Impossible de se connecter à la base de données.");
        }
    }
}