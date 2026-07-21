using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Application.Services;
using AssetTracking.Domain.Entities;
using AssetTracking.Infrastructure.Persistence;
using AssetTracking.Infrastructure.Repositories;
using AssetTracking.Infrastructure.Services;
using AssetTracking.Tests.Common;
using Xunit;

namespace AssetTracking.Tests.Assets;

public class AssetServiceTests
{
    private static async Task<(AssetService Service, AssetCategory Category, AssetModel Model)> CreateServiceAsync()
    {
        var context = TestDbContextFactory.Create();

        var brand = new AssetBrand { Name = "Dell" };
        var category = new AssetCategory { Name = "Laptops" };
        var model = new AssetModel { Name = "Latitude 5420", Brand = brand, BrandId = brand.Id };

        context.AssetBrands.Add(brand);
        context.AssetCategories.Add(category);
        context.AssetModels.Add(model);
        await context.SaveChangesAsync();

        var service = new AssetService(
            new AssetRepository(context),
            new Repository<AssetCategory>(context),
            new Repository<AssetModel>(context),
            new UnitOfWork(context),
            new AuditService(context));

        return (service, category, model);
    }

    private static CreateAssetRequest ValidRequest(AssetCategory category, AssetModel model, string assetTag = "AT-0001") => new(
        AssetTag: assetTag,
        SerialNumber: "SN-0001",
        RfidTag: null,
        CategoryId: category.Id,
        ModelId: model.Id,
        Color: "Gris",
        Features: "16GB RAM",
        PhotoUrl: null,
        OwnerId: null,
        CurrentLocationId: null,
        Notes: null);

    [Fact]
    public async Task CreateAsync_WithValidData_Succeeds()
    {
        var (service, category, model) = await CreateServiceAsync();

        var result = await service.CreateAsync(ValidRequest(category, model), AuditContext.Empty);

        Assert.True(result.IsSuccess);
        Assert.Equal("AT-0001", result.Value!.AssetTag);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateAssetTag_ReturnsFailure()
    {
        var (service, category, model) = await CreateServiceAsync();
        await service.CreateAsync(ValidRequest(category, model), AuditContext.Empty);

        var result = await service.CreateAsync(ValidRequest(category, model), AuditContext.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal("ASSET_DUPLICATE_TAG", result.ErrorCode);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSerialNumber_ReturnsFailure()
    {
        var (service, category, model) = await CreateServiceAsync();
        await service.CreateAsync(ValidRequest(category, model, "AT-0001"), AuditContext.Empty);

        var result = await service.CreateAsync(ValidRequest(category, model, "AT-0002"), AuditContext.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal("ASSET_DUPLICATE_SERIAL", result.ErrorCode);
    }
}
