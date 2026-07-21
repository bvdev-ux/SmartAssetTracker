export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
  permissions: string[];
}

export interface LoginResponse {
  accessToken: string;
  expiresAt: string;
  user: User;
}

export interface DashboardStats {
  assetsInside: number;
  assetsOutside: number;
  entriesToday: number;
  exitsToday: number;
  activeAlerts: number;
  visitorsToday: number;
}

export type PersonType = "Student" | "Teacher" | "Administrative" | "Visitor";

export interface Person {
  id: string;
  documentNumber: string;
  universityCode?: string | null;
  firstName: string;
  lastName: string;
  email?: string | null;
  phone?: string | null;
  career?: string | null;
  faculty?: string | null;
  cardQrCode?: string | null;
  personType: PersonType;
  isActive: boolean;
}

export type LocationType = "Campus" | "Gate" | "Laboratory" | "Library" | "Office";

export interface Location {
  id: string;
  name: string;
  code: string;
  locationType: LocationType;
  parentLocationId?: string | null;
  parentLocationName?: string | null;
  isActive: boolean;
}

export interface AssetCategory {
  id: string;
  name: string;
  description?: string | null;
  isActive: boolean;
}

export interface AssetBrand {
  id: string;
  name: string;
  isActive: boolean;
}

export interface AssetModel {
  id: string;
  name: string;
  brandId: string;
  brandName: string;
  isActive: boolean;
}

export type AssetStatusType = "Available" | "InUse" | "Maintenance" | "Reported" | "Retired";

export interface Asset {
  id: string;
  assetTag: string;
  serialNumber?: string | null;
  qrCode?: string | null;
  rfidTag?: string | null;
  categoryId: string;
  categoryName: string;
  modelId: string;
  modelName: string;
  brandName: string;
  color?: string | null;
  features?: string | null;
  photoUrl?: string | null;
  ownerId?: string | null;
  ownerFullName?: string | null;
  status: AssetStatusType;
  currentLocationId?: string | null;
  currentLocationName?: string | null;
  isInsideCampus: boolean;
  notes?: string | null;
}

export type MovementType = "Entry" | "Exit" | "ReEntry" | "Loan" | "Return";

export interface Movement {
  id: string;
  assetId: string;
  assetTag: string;
  categoryName: string;
  personId: string;
  personFullName: string;
  locationId: string;
  locationName: string;
  movementType: MovementType;
  occurredAt: string;
  notes?: string | null;
  validationMethod?: string | null;
}

export interface AccessValidationResult {
  authorized: boolean;
  denialReason?: string | null;
  denialCode?: string | null;
  movement?: Movement | null;
}

export type AlertType =
  | "UnregisteredAsset"
  | "UnauthorizedExit"
  | "ReportedAsset"
  | "ExceededPermanence"
  | "InvalidQrCode"
  | "DuplicateAsset"
  | "AssetWithoutOwner"
  | "StolenAsset";

export type AlertSeverity = "Low" | "Medium" | "High" | "Critical";
export type AlertStatus = "Active" | "Acknowledged" | "Resolved";

export interface Alert {
  id: string;
  assetId?: string | null;
  assetTag?: string | null;
  alertType: AlertType;
  severity: AlertSeverity;
  status: AlertStatus;
  message: string;
  resolvedAt?: string | null;
  resolvedBy?: string | null;
  createdAt: string;
}

export interface AssetUsageCount {
  assetId: string;
  assetTag: string;
  categoryName: string;
  movementCount: number;
}

export type AuditAction =
  | "Create"
  | "Update"
  | "Delete"
  | "Login"
  | "Logout"
  | "Export"
  | "AccessDenied";

export interface AuditLog {
  id: string;
  userId?: string | null;
  userEmail?: string | null;
  action: AuditAction;
  entityName: string;
  entityId?: string | null;
  details?: string | null;
  ipAddress?: string | null;
  userAgent?: string | null;
  result: boolean;
  occurredAt: string;
}
