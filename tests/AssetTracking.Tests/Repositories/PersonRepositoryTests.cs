using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Repositories;
using AssetTracking.Tests.Common;
using Xunit;

namespace AssetTracking.Tests.Repositories;

public class PersonRepositoryTests
{
    [Fact]
    public async Task ExistsByDocumentNumberAsync_WhenDocumentExists_ReturnsTrue()
    {
        var context = TestDbContextFactory.Create();
        var repository = new PersonRepository(context);

        context.People.Add(new Person
        {
            DocumentNumber = "70001122",
            FirstName = "Luis",
            LastName = "Ramos",
            PersonType = PersonType.Teacher
        });
        await context.SaveChangesAsync();

        var exists = await repository.ExistsByDocumentNumberAsync("70001122");

        Assert.True(exists);
    }

    [Fact]
    public async Task SearchAsync_FiltersByPersonTypeAndSearchTerm()
    {
        var context = TestDbContextFactory.Create();
        var repository = new PersonRepository(context);

        context.People.AddRange(
            new Person { DocumentNumber = "1", FirstName = "Ana", LastName = "Perez", PersonType = PersonType.Student },
            new Person { DocumentNumber = "2", FirstName = "Luis", LastName = "Gomez", PersonType = PersonType.Teacher });
        await context.SaveChangesAsync();

        var (items, total) = await repository.SearchAsync("ana", null, null, 1, 10);

        Assert.Equal(1, total);
        Assert.Equal("Ana", items.Single().FirstName);
    }

    [Fact]
    public async Task GetByIdentifierAsync_MatchesByCardQrCode()
    {
        var context = TestDbContextFactory.Create();
        var repository = new PersonRepository(context);

        context.People.Add(new Person
        {
            DocumentNumber = "3",
            FirstName = "Marco",
            LastName = "Diaz",
            PersonType = PersonType.Administrative,
            CardQrCode = "QR-777"
        });
        await context.SaveChangesAsync();

        var person = await repository.GetByIdentifierAsync("QR-777");

        Assert.NotNull(person);
        Assert.Equal("Marco", person!.FirstName);
    }
}
