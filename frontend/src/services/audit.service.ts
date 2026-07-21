import { apiClient } from "@/services/api-client";
import type { ApiResponse, AuditLog, PagedResult } from "@/types";

export interface AuditQuery {
  page?: number;
  pageSize?: number;
}

export async function getAuditLogs(query: AuditQuery) {
  const { data } = await apiClient.get<ApiResponse<PagedResult<AuditLog>>>("/audit", { params: query });
  return data;
}
