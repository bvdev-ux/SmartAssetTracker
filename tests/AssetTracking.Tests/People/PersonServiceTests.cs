using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.People;
using AssetTracking.Application.Services;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Repositories;
using AssetTracking.Infrastructure.Services;
using AssetTracking.Tests.Common;
using Xunit;

namespace AssetTracking.Tests.People;

public class PersonServiceTests
{
    private static PersonService CreateService(out AssetTracking.Infrastructure.Persistence.ApplicationDbContext context)
    {
        context = TestDbContextFactory.Create();
        return new PersonService(
            new PersonRepository(context),
            new AssetMovementRepository(context),
            new UnitOfWork(context),
            new AuditService(context));
    }

    private static CreatePersonRequest ValidRequest(string documentNumber = "12345678") => new(
        DocumentNumber: documentNumber,
        UniversityCode: "U001",
        FirstName: "Ana",
        LastName: "Torres",
        Email: "ana.torres@upla.edu.pe",
        Phone: "999999999",
        Career: "Ingeniería de Sistemas",
        Faculty: "Ingeniería",
        CardQrCode: "QR-001",
        PersonType: PersonType.Student);

    [Fact]
    public async Task CreateAsync_WithNewDocumentNumber_Succeeds()
    {
        var service = CreateService(out _);

        var result = await service.CreateAsync(ValidRequest(), AuditContext.Empty);

        Assert.True(result.IsSuccess);
        Assert.Equal("Ana", result.Value!.FirstName);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateDocumentNumber_ReturnsFailure()
    {
        var service = CreateService(out _);
        await service.CreateAsync(ValidRequest(), AuditContext.Empty);

        var result = await service.CreateAsync(ValidRequest(), AuditContext.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal("PERSON_DUPLICATE", result.ErrorCode);
    }

    [Fact]
    public async Task DeactivateAsync_WithUnknownId_ReturnsFailure()
    {
        var service = CreateService(out _);

        var result = await service.DeactivateAsync(Guid.NewGuid(), AuditContext.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal("PERSON_NOT_FOUND", result.ErrorCode);
    }
}
