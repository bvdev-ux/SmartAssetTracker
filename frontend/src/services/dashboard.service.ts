import { apiClient } from "@/services/api-client";
import type { ApiResponse, DashboardStats } from "@/types";

export async function getDashboardSummary() {
  const { data } = await apiClient.get<ApiResponse<DashboardStats>>("/dashboard/resumen");
  return data;
}
