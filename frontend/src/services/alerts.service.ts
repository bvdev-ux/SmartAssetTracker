import { apiClient } from "@/services/api-client";
import type { Alert, AlertSeverity, AlertStatus, AlertType, ApiResponse, PagedResult } from "@/types";

export interface AlertsQuery {
  status?: AlertStatus;
  severity?: AlertSeverity;
  alertType?: AlertType;
  page?: number;
  pageSize?: number;
}

export async function getAlerts(query: AlertsQuery) {
  const { data } = await apiClient.get<ApiResponse<PagedResult<Alert>>>("/alerts", { params: query });
  return data;
}

export async function resolveAlert(id: string) {
  const { data } = await apiClient.post<ApiResponse<Alert>>(`/alerts/${id}/resolver`);
  return data;
}

export async function acknowledgeAlert(id: string) {
  const { data } = await apiClient.post<ApiResponse<Alert>>(`/alerts/${id}/reconocer`);
  return data;
}
