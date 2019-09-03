using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace onlineShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseStartup<Startup>();
    }
}
