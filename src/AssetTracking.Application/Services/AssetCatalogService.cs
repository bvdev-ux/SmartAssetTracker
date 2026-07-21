using AssetTracking.Application.Common;
using AssetTracking.Application.DTOs.Assets;
using AssetTracking.Domain.Entities;
using AssetTracking.Domain.Interfaces;

namespace AssetTracking.Application.Services;

public interface IAssetCategoryService
{
    Task<IReadOnlyList<AssetCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AssetCategoryDto>> CreateAsync(CreateAssetCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<AssetCategoryDto>> UpdateAsync(Guid id, UpdateAssetCategoryRequest request, CancellationToken cancellationToken = default);
}

public class AssetCategoryService(IRepository<AssetCategory> repository, IUnitOfWork unitOfWork) : IAssetCategoryService
{
    public async Task<IReadOnlyList<AssetCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        (await repository.GetAllAsync(cancellationToken))
            .OrderBy(c => c.Name)
            .Select(ToDto)
            .ToList();

    public async Task<Result<AssetCategoryDto>> CreateAsync(
        CreateAssetCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var exists = (await repository.FindAsync(c => c.Name == request.Name, cancellationToken)).Any();
        if (exists) return Result<AssetCategoryDto>.Failure("Ya existe una categoría con ese nombre.", "CATEGORY_DUPLICATE");

        var category = new AssetCategory { Name = request.Name, Description = request.Description };
        await repository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetCategoryDto>.Success(ToDto(category));
    }

    public async Task<Result<AssetCategoryDto>> UpdateAsync(
        Guid id, UpdateAssetCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetByIdAsync(id, cancellationToken);
        if (category is null) return Result<AssetCategoryDto>.Failure("Categoría no encontrada.", "CATEGORY_NOT_FOUND");

        category.Name = request.Name;
        category.Description = request.Description;
        category.IsActive = request.IsActive;
        repository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetCategoryDto>.Success(ToDto(category));
    }

    private static AssetCategoryDto ToDto(AssetCategory c) => new(c.Id, c.Name, c.Description, c.IsActive);
}

public interface IAssetBrandService
{
    Task<IReadOnlyList<AssetBrandDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AssetBrandDto>> CreateAsync(CreateAssetBrandRequest request, CancellationToken cancellationToken = default);
    Task<Result<AssetBrandDto>> UpdateAsync(Guid id, UpdateAssetBrandRequest request, CancellationToken cancellationToken = default);
}

public class AssetBrandService(IRepository<AssetBrand> repository, IUnitOfWork unitOfWork) : IAssetBrandService
{
    public async Task<IReadOnlyList<AssetBrandDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        (await repository.GetAllAsync(cancellationToken))
            .OrderBy(b => b.Name)
            .Select(ToDto)
            .ToList();

    public async Task<Result<AssetBrandDto>> CreateAsync(
        CreateAssetBrandRequest request, CancellationToken cancellationToken = default)
    {
        var exists = (await repository.FindAsync(b => b.Name == request.Name, cancellationToken)).Any();
        if (exists) return Result<AssetBrandDto>.Failure("Ya existe una marca con ese nombre.", "BRAND_DUPLICATE");

        var brand = new AssetBrand { Name = request.Name };
        await repository.AddAsync(brand, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetBrandDto>.Success(ToDto(brand));
    }

    public async Task<Result<AssetBrandDto>> UpdateAsync(
        Guid id, UpdateAssetBrandRequest request, CancellationToken cancellationToken = default)
    {
        var brand = await repository.GetByIdAsync(id, cancellationToken);
        if (brand is null) return Result<AssetBrandDto>.Failure("Marca no encontrada.", "BRAND_NOT_FOUND");

        brand.Name = request.Name;
        brand.IsActive = request.IsActive;
        repository.Update(brand);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetBrandDto>.Success(ToDto(brand));
    }

    private static AssetBrandDto ToDto(AssetBrand b) => new(b.Id, b.Name, b.IsActive);
}

public interface IAssetModelService
{
    Task<IReadOnlyList<AssetModelDto>> GetAllAsync(Guid? brandId, CancellationToken cancellationToken = default);
    Task<Result<AssetModelDto>> CreateAsync(CreateAssetModelRequest request, CancellationToken cancellationToken = default);
    Task<Result<AssetModelDto>> UpdateAsync(Guid id, UpdateAssetModelRequest request, CancellationToken cancellationToken = default);
}

public class AssetModelService(
    IRepository<AssetModel> repository,
    IRepository<AssetBrand> brandRepository,
    IUnitOfWork unitOfWork) : IAssetModelService
{
    public async Task<IReadOnlyList<AssetModelDto>> GetAllAsync(Guid? brandId, CancellationToken cancellationToken = default)
    {
        var models = brandId.HasValue
            ? await repository.FindAsync(m => m.BrandId == brandId.Value, cancellationToken)
            : await repository.GetAllAsync(cancellationToken);

        var brands = (await brandRepository.GetAllAsync(cancellationToken)).ToDictionary(b => b.Id, b => b.Name);

        return models
            .OrderBy(m => m.Name)
            .Select(m => new AssetModelDto(m.Id, m.Name, m.BrandId, brands.GetValueOrDefault(m.BrandId, string.Empty), m.IsActive))
            .ToList();
    }

    public async Task<Result<AssetModelDto>> CreateAsync(
        CreateAssetModelRequest request, CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
        if (brand is null) return Result<AssetModelDto>.Failure("Marca no encontrada.", "BRAND_NOT_FOUND");

        var exists = (await repository.FindAsync(
            m => m.BrandId == request.BrandId && m.Name == request.Name, cancellationToken)).Any();
        if (exists) return Result<AssetModelDto>.Failure("Ya existe ese modelo para la marca.", "MODEL_DUPLICATE");

        var model = new AssetModel { Name = request.Name, BrandId = request.BrandId };
        await repository.AddAsync(model, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetModelDto>.Success(new AssetModelDto(model.Id, model.Name, model.BrandId, brand.Name, model.IsActive));
    }

    public async Task<Result<AssetModelDto>> UpdateAsync(
        Guid id, UpdateAssetModelRequest request, CancellationToken cancellationToken = default)
    {
        var model = await repository.GetByIdAsync(id, cancellationToken);
        if (model is null) return Result<AssetModelDto>.Failure("Modelo no encontrado.", "MODEL_NOT_FOUND");

        var brand = await brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
        if (brand is null) return Result<AssetModelDto>.Failure("Marca no encontrada.", "BRAND_NOT_FOUND");

        model.Name = request.Name;
        model.BrandId = request.BrandId;
        model.IsActive = request.IsActive;
        repository.Update(model);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetModelDto>.Success(new AssetModelDto(model.Id, model.Name, model.BrandId, brand.Name, model.IsActive));
    }
}
