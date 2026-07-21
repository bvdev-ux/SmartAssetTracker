using AssetTracking.Domain.Enums;

namespace AssetTracking.Application.DTOs.People;

public record PersonDto(
    Guid Id,
    string DocumentNumber,
    string? UniversityCode,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Career,
    string? Faculty,
    string? CardQrCode,
    PersonType PersonType,
    bool IsActive);

public record CreatePersonRequest(
    string DocumentNumber,
    string? UniversityCode,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Career,
    string? Faculty,
    string? CardQrCode,
    PersonType PersonType);

public record UpdatePersonRequest(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Career,
    string? Faculty,
    string? CardQrCode,
    PersonType PersonType,
    bool IsActive);
