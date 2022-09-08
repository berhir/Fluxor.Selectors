namespace Fluxor.Selectors.MauiDemo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<MainPage>();

        builder.Services.AddFluxor(o =>
		{
			o.ScanAssemblies(typeof(Demo.DemoRoot).Assembly);
			o.WithLifetime(StoreLifetime.Singleton);
		});

		return builder.Build();
	}
}
