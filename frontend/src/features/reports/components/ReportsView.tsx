"use client";

import { useState } from "react";
import { FileDown } from "lucide-react";
import { Badge, Button, Card, DataTable, Select } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import {
  downloadReport,
  getAlertsReport,
  getEntriesOfDay,
  getExitsOfDay,
  getMostUsedAssets,
  getRetainedAssets,
  type ReportKey,
} from "@/services/reports.service";
import type { Alert, Asset, AssetUsageCount, Movement } from "@/types";

type ReportRow = Movement | AssetUsageCount | Asset | Alert;

const REPORT_OPTIONS: { key: ReportKey; label: string }[] = [
  { key: "entradas-del-dia", label: "Entradas del día" },
  { key: "salidas-del-dia", label: "Salidas del día" },
  { key: "equipos-mas-usados", label: "Equipos más utilizados" },
  { key: "equipos-retenidos", label: "Equipos retenidos" },
  { key: "alertas", label: "Alertas" },
];

function isMovement(row: ReportRow): row is Movement {
  return (row as Movement).assetTag !== undefined && (row as Movement).personFullName !== undefined;
}
function isUsage(row: ReportRow): row is AssetUsageCount {
  return (row as AssetUsageCount).movementCount !== undefined;
}
function isAsset(row: ReportRow): row is Asset {
  return (row as Asset).isInsideCampus !== undefined;
}

const movementColumns: DataTableColumn<Movement>[] = [
  { header: "Fecha", render: (m) => new Date(m.occurredAt).toLocaleString() },
  { header: "Activo", render: (m) => m.assetTag },
  { header: "Persona", render: (m) => m.personFullName },
  { header: "Ubicación", render: (m) => m.locationName },
];

const usageColumns: DataTableColumn<AssetUsageCount>[] = [
  { header: "Activo", render: (u) => u.assetTag },
  { header: "Categoría", render: (u) => u.categoryName },
  { header: "Movimientos", render: (u) => u.movementCount },
];

const assetColumns: DataTableColumn<Asset>[] = [
  { header: "Código", render: (a) => a.assetTag },
  { header: "Categoría", render: (a) => a.categoryName },
  { header: "Ubicación", render: (a) => a.currentLocationName ?? "—" },
];

const alertColumns: DataTableColumn<Alert>[] = [
  { header: "Fecha", render: (a) => new Date(a.createdAt).toLocaleString() },
  { header: "Tipo", render: (a) => a.alertType },
  { header: "Severidad", render: (a) => <Badge tone="warning">{a.severity}</Badge> },
  { header: "Mensaje", render: (a) => a.message },
];

export function ReportsView() {
  const [reportKey, setReportKey] = useState<ReportKey>("entradas-del-dia");
  const [rows, setRows] = useState<ReportRow[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadReport = async () => {
    setIsLoading(true);
    setError(null);
    try {
      let data: ReportRow[] = [];
      switch (reportKey) {
        case "entradas-del-dia":
          data = (await getEntriesOfDay()).data ?? [];
          break;
        case "salidas-del-dia":
          data = (await getExitsOfDay()).data ?? [];
          break;
        case "equipos-mas-usados":
          data = (await getMostUsedAssets()).data ?? [];
          break;
        case "equipos-retenidos":
          data = (await getRetainedAssets()).data ?? [];
          break;
        case "alertas":
          data = (await getAlertsReport()).data ?? [];
          break;
      }
      setRows(data);
    } catch {
      setError("No se pudo generar el reporte.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleExport = async (format: "Excel" | "Pdf") => {
    try {
      await downloadReport(reportKey, format);
    } catch {
      setError("No se pudo exportar el reporte.");
    }
  };

  let columns: DataTableColumn<ReportRow>[] = movementColumns as DataTableColumn<ReportRow>[];
  if (rows.length > 0) {
    if (isUsage(rows[0])) columns = usageColumns as DataTableColumn<ReportRow>[];
    else if (reportKey === "alertas") columns = alertColumns as DataTableColumn<ReportRow>[];
    else if (isAsset(rows[0])) columns = assetColumns as DataTableColumn<ReportRow>[];
    else if (isMovement(rows[0])) columns = movementColumns as DataTableColumn<ReportRow>[];
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Reportes</h1>
        <p className="text-muted">Genera y exporta reportes de auditoría del sistema</p>
      </div>

      <Card>
        <div className="mb-4 flex flex-wrap items-end gap-3">
          <Select
            label="Reporte"
            value={reportKey}
            onChange={(e) => {
              setReportKey(e.target.value as ReportKey);
              setRows([]);
            }}
            className="w-64"
          >
            {REPORT_OPTIONS.map((opt) => (
              <option key={opt.key} value={opt.key}>
                {opt.label}
              </option>
            ))}
          </Select>
          <Button variant="secondary" onClick={loadReport} isLoading={isLoading}>
            Generar
          </Button>
          <Button variant="secondary" onClick={() => handleExport("Excel")}>
            <FileDown className="h-4 w-4" /> Excel
          </Button>
          <Button variant="secondary" onClick={() => handleExport("Pdf")}>
            <FileDown className="h-4 w-4" /> PDF
          </Button>
        </div>

        {error && <p className="mb-4 text-sm text-danger-600">{error}</p>}

        <DataTable
          columns={columns}
          rows={rows}
          keyExtractor={(row) => (row as { id?: string; assetId?: string }).id ?? (row as AssetUsageCount).assetId}
          isLoading={isLoading}
          emptyMessage="Genere el reporte para ver los resultados."
        />
      </Card>
    </div>
  );
}
