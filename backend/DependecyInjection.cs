namespace GestionHospitaliere.Backend;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GestionHospitaliere.Backend.Data;
using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Repositories;
using System.IO;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddBackendServices(this IServiceCollection services)
    {
        // 1. Charger appsettings.json
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsetting.json", optional: false, reloadOnChange: true)
            .Build();

        // 2. Enregistrer IConfiguration et Database
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<Database>();

        // 3. Enregistrer les Repositories
        services.AddScoped<IMedecinRepository, MedecinRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IConsultationRepository, ConsultationRepository>();
        services.AddScoped<IChambreRepository, ChambreRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        //4. Enregistrer les services
        services.AddScoped < IChambreServices,ChambreServices>();
        services.AddScoped<IMedecinServices,MedecinServices>();
        services.AddScoped<IUsersServices,UsersServices>();
        services.AddScoped<IPatientServices,PatientServices>();
        services.AddScoped<IConsultationServices,ConsultationServices>();                                                                                                                                                                                                            

        return services;
    }
}