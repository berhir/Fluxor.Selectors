using Fluxor;
using Fluxor.Selectors.BlazorDemo;
using Fluxor.Selectors.Demo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddFluxor(o =>
{
    o.ScanAssemblies(typeof(DemoRoot).Assembly);
#if DEBUG
    o.UseReduxDevTools();
#endif
});

await builder.Build().RunAsync();
