import { apiClient } from "@/services/api-client";
import type { ApiResponse, Location, LocationType } from "@/types";

export interface CreateLocationPayload {
  name: string;
  code: string;
  locationType: LocationType;
  parentLocationId?: string | null;
}

export type UpdateLocationPayload = CreateLocationPayload & { isActive: boolean };

export async function getLocations() {
  const { data } = await apiClient.get<ApiResponse<Location[]>>("/locations");
  return data;
}

export async function createLocation(payload: CreateLocationPayload) {
  const { data } = await apiClient.post<ApiResponse<Location>>("/locations", payload);
  return data;
}

export async function updateLocation(id: string, payload: UpdateLocationPayload) {
  const { data } = await apiClient.put<ApiResponse<Location>>(`/locations/${id}`, payload);
  return data;
}

export async function deactivateLocation(id: string) {
  const { data } = await apiClient.delete<ApiResponse<object>>(`/locations/${id}`);
  return data;
}
