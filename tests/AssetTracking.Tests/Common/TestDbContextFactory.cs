using AssetTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AssetTracking.Tests.Common;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public static IConfiguration CreateJwtConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "unit-test-signing-key-with-enough-length-1234567890",
            ["Jwt:Issuer"] = "AssetTracking.Tests",
            ["Jwt:Audience"] = "AssetTracking.Tests"
        };

        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }
}
