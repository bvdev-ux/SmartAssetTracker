import { apiClient } from "@/services/api-client";
import type { ApiResponse, Movement, MovementType, PagedResult } from "@/types";

export interface RegisterMovementPayload {
  assetIdentifier: string;
  personIdentifier: string;
  locationId: string;
  movementType: MovementType;
  notes?: string | null;
  validationMethod?: string | null;
}

export async function registerMovement(payload: RegisterMovementPayload) {
  const { data } = await apiClient.post<ApiResponse<Movement>>("/movements", payload);
  return data;
}

export async function getMovementsByAsset(assetId: string, page: number, pageSize: number) {
  const { data } = await apiClient.get<ApiResponse<PagedResult<Movement>>>(`/movements/activo/${assetId}`, {
    params: { page, pageSize },
  });
  return data;
}
