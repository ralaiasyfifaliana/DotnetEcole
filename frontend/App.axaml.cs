namespace GestionHospitaliere.Frontend;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using GestionHospitaliere.Backend;
using GestionHospitaliere.Frontend.ViewModels;
using GestionHospitaliere.Frontend.Views;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Services;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();

            // 1. Enregistrement des services
            services.AddBackendServices();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<IMedecinServices, MedecinServices>();
            services.AddTransient<MedecinsViewModel>();
            services.AddTransient<IChambreServices, ChambreServices>();
            services.AddTransient<ChambresViewModel>();
            services.AddTransient<IPatientServices, PatientServices>();
            services.AddTransient<PatientsViewModel>();
            services.AddTransient<IConsultationServices, ConsultationServices>();
            services.AddTransient<ConsultationsViewModel>();
            services.AddTransient<MedecinSessionViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            // 2. Récupération de MainWindowViewModel
            var mainVM = serviceProvider.GetRequiredService<MainWindowViewModel>();

            // 3. Récupération de LoginViewModel
            var loginVM = serviceProvider.GetRequiredService<LoginViewModel>();

            // 4. On affecte le premier écran
            mainVM.CurrentViewModel = loginVM;

            // 5. Affichage de la fenêtre
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainVM
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
