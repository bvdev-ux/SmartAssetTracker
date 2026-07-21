using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Enums;
using AssetTracking.Infrastructure.Repositories;
using AssetTracking.Tests.Common;
using Xunit;

namespace AssetTracking.Tests.Repositories;

public class AssetRepositoryTests
{
    private static (AssetCategory Category, AssetBrand Brand, AssetModel Model) SeedCatalog(
        AssetTracking.Infrastructure.Persistence.ApplicationDbContext context)
    {
        var category = new AssetCategory { Name = "Laptops" };
        var brand = new AssetBrand { Name = "HP" };
        var model = new AssetModel { Name = "ProBook 450", Brand = brand, BrandId = brand.Id };

        context.AssetCategories.Add(category);
        context.AssetBrands.Add(brand);
        context.AssetModels.Add(model);

        return (category, brand, model);
    }

    [Fact]
    public async Task ExistsByAssetTagAsync_WhenTagExists_ReturnsTrue()
    {
        var context = TestDbContextFactory.Create();
        var (category, _, model) = SeedCatalog(context);
        var repository = new AssetRepository(context);

        context.Assets.Add(new Asset
        {
            AssetTag = "AT-100",
            CategoryId = category.Id,
            Category = category,
            ModelId = model.Id,
            Model = model
        });
        await context.SaveChangesAsync();

        var exists = await repository.ExistsByAssetTagAsync("AT-100");

        Assert.True(exists);
    }

    [Fact]
    public async Task SearchAsync_FiltersByStatusAndCategory()
    {
        var context = TestDbContextFactory.Create();
        var (category, _, model) = SeedCatalog(context);
        var repository = new AssetRepository(context);

        context.Assets.AddRange(
            new Asset { AssetTag = "AT-1", CategoryId = category.Id, Category = category, ModelId = model.Id, Model = model, Status = AssetStatusType.Available },
            new Asset { AssetTag = "AT-2", CategoryId = category.Id, Category = category, ModelId = model.Id, Model = model, Status = AssetStatusType.Retired });
        await context.SaveChangesAsync();

        var (items, total) = await repository.SearchAsync(null, category.Id, AssetStatusType.Available, null, 1, 10);

        Assert.Equal(1, total);
        Assert.Equal("AT-1", items.Single().AssetTag);
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_IncludesCategoryAndModel()
    {
        var context = TestDbContextFactory.Create();
        var (category, brand, model) = SeedCatalog(context);
        var repository = new AssetRepository(context);

        var asset = new Asset { AssetTag = "AT-500", CategoryId = category.Id, Category = category, ModelId = model.Id, Model = model };
        context.Assets.Add(asset);
        await context.SaveChangesAsync();

        var found = await repository.GetByIdWithDetailsAsync(asset.Id);

        Assert.NotNull(found);
        Assert.Equal("Laptops", found!.Category.Name);
        Assert.Equal(brand.Name, found.Model.Brand.Name);
    }
}
