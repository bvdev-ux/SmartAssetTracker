import { apiClient } from "@/services/api-client";
import type { AccessValidationResult, ApiResponse, MovementType } from "@/types";

export interface ValidateAccessPayload {
  personIdentifier: string;
  assetIdentifier: string;
  locationId: string;
  movementType: MovementType;
  validationMethod: string;
}

export async function validateAccess(payload: ValidateAccessPayload) {
  const { data } = await apiClient.post<ApiResponse<AccessValidationResult>>(
    "/accesscontrol/validar",
    payload,
  );
  return data;
}
