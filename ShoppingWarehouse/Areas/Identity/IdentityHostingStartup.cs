using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ShoppingWarehouse.Areas.Identity.IdentityHostingStartup))]
namespace ShoppingWarehouse.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}