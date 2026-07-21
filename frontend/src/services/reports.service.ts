import { apiClient } from "@/services/api-client";
import type { Alert, ApiResponse, Asset, AssetUsageCount, Movement } from "@/types";

export type ReportKey =
  | "entradas-del-dia"
  | "salidas-del-dia"
  | "equipos-mas-usados"
  | "equipos-retenidos"
  | "alertas";

export async function getEntriesOfDay(date?: string) {
  const { data } = await apiClient.get<ApiResponse<Movement[]>>("/reports/entradas-del-dia", {
    params: date ? { date } : undefined,
  });
  return data;
}

export async function getExitsOfDay(date?: string) {
  const { data } = await apiClient.get<ApiResponse<Movement[]>>("/reports/salidas-del-dia", {
    params: date ? { date } : undefined,
  });
  return data;
}

export async function getMostUsedAssets(top = 10) {
  const { data } = await apiClient.get<ApiResponse<AssetUsageCount[]>>("/reports/equipos-mas-usados", {
    params: { top },
  });
  return data;
}

export async function getRetainedAssets(hoursThreshold = 24) {
  const { data } = await apiClient.get<ApiResponse<Asset[]>>("/reports/equipos-retenidos", {
    params: { hoursThreshold },
  });
  return data;
}

export async function getAlertsReport() {
  const { data } = await apiClient.get<ApiResponse<Alert[]>>("/reports/alertas");
  return data;
}

export async function downloadReport(reportKey: ReportKey, format: "Excel" | "Pdf") {
  const response = await apiClient.get(`/reports/${reportKey}`, {
    params: { format },
    responseType: "blob",
  });

  const extension = format === "Excel" ? "xlsx" : "pdf";
  const url = window.URL.createObjectURL(new Blob([response.data]));
  const link = document.createElement("a");
  link.href = url;
  link.download = `${reportKey}.${extension}`;
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
}
