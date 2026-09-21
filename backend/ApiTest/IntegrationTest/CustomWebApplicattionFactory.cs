//using Infraestrutura.BancoContexto;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.VisualStudio.TestPlatform.TestHost;

//namespace ApiTest.IntegrationTestes;

//public class CustomWebApplicationFactory : WebApplicationFactory<Program>
//{
//    protected override void ConfigureWebHost(IWebHostBuilder builder)
//    {
//        builder.ConfigureServices(services =>
//        {
//            services.RemoveAll<DbContextOptions<ApiContext>>();

//            services.RemoveAll<ApiContext>();


//            var options = new DbContextOptionsBuilder<ApiContext>();

//            options.UseInMemoryDatabase(Guid.NewGuid().ToString());

//            var serviceProvider =
//                services.BuildServiceProvider();

//            using var scope =
//                serviceProvider.CreateScope();

//            var context = scope.ServiceProvider
//                .GetRequiredService<ApiContext>();

//            context.Database.EnsureCreated();
//        });
//    }
//}