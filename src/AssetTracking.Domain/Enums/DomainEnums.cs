namespace AssetTracking.Domain.Enums;

public enum PersonType
{
    Student = 1,
    Teacher = 2,
    Administrative = 3,
    Visitor = 4
}

public enum AssetStatusType
{
    Available = 1,
    InUse = 2,
    Maintenance = 3,
    Reported = 4,
    Retired = 5
}

public enum MovementType
{
    Entry = 1,
    Exit = 2,
    ReEntry = 3,
    Loan = 4,
    Return = 5
}

public enum LocationType
{
    Campus = 1,
    Gate = 2,
    Laboratory = 3,
    Library = 4,
    Office = 5
}

public enum AlertType
{
    UnregisteredAsset = 1,
    UnauthorizedExit = 2,
    ReportedAsset = 3,
    ExceededPermanence = 4,
    InvalidQrCode = 5,
    DuplicateAsset = 6,
    AssetWithoutOwner = 7,
    StolenAsset = 8
}

public enum AlertSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum AlertStatus
{
    Active = 1,
    Acknowledged = 2,
    Resolved = 3
}

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    Export = 6,
    AccessDenied = 7
}
