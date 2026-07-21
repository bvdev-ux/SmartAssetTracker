"use client";

import { useEffect, useState } from "react";
import { Badge, Card, DataTable, Pagination, Select } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import { acknowledgeAlert, getAlerts, resolveAlert } from "@/services/alerts.service";
import type { Alert, AlertSeverity, AlertStatus } from "@/types";

const PAGE_SIZE = 10;

const STATUS_LABELS: Record<AlertStatus, string> = {
  Active: "Activa",
  Acknowledged: "Reconocida",
  Resolved: "Resuelta",
};

const SEVERITY_LABELS: Record<AlertSeverity, string> = {
  Low: "Baja",
  Medium: "Media",
  High: "Alta",
  Critical: "Crítica",
};

const SEVERITY_TONE: Record<AlertSeverity, "neutral" | "warning" | "danger"> = {
  Low: "neutral",
  Medium: "warning",
  High: "danger",
  Critical: "danger",
};

const ALERT_TYPE_LABELS: Record<Alert["alertType"], string> = {
  UnregisteredAsset: "Activo no registrado",
  UnauthorizedExit: "Salida no autorizada",
  ReportedAsset: "Activo reportado",
  ExceededPermanence: "Permanencia excesiva",
  InvalidQrCode: "Código QR inválido",
  DuplicateAsset: "Activo duplicado",
  AssetWithoutOwner: "Sin propietario",
  StolenAsset: "Activo robado",
};

export function AlertsView() {
  const [alerts, setAlerts] = useState<Alert[]>([]);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState<AlertStatus | "">("Active");
  const [isLoading, setIsLoading] = useState(true);

  const loadAlerts = async () => {
    setIsLoading(true);
    const response = await getAlerts({ status: status || undefined, page, pageSize: PAGE_SIZE });
    if (response.data) {
      setAlerts(response.data.items);
      setTotalPages(response.data.totalPages || 1);
    }
    setIsLoading(false);
  };

  useEffect(() => {
    loadAlerts();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, status]);

  const handleResolve = async (id: string) => {
    await resolveAlert(id);
    await loadAlerts();
  };

  const handleAcknowledge = async (id: string) => {
    await acknowledgeAlert(id);
    await loadAlerts();
  };

  const columns: DataTableColumn<Alert>[] = [
    { header: "Fecha", render: (a) => new Date(a.createdAt).toLocaleString() },
    { header: "Tipo", render: (a) => ALERT_TYPE_LABELS[a.alertType] },
    { header: "Severidad", render: (a) => <Badge tone={SEVERITY_TONE[a.severity]}>{SEVERITY_LABELS[a.severity]}</Badge> },
    { header: "Activo", render: (a) => a.assetTag ?? "—" },
    { header: "Mensaje", render: (a) => a.message },
    {
      header: "Estado",
      render: (a) => (
        <Badge tone={a.status === "Resolved" ? "success" : a.status === "Acknowledged" ? "brand" : "warning"}>
          {STATUS_LABELS[a.status]}
        </Badge>
      ),
    },
    {
      header: "Acciones",
      render: (a) =>
        a.status === "Active" ? (
          <div className="flex gap-3">
            <button onClick={() => handleAcknowledge(a.id)} className="text-sm font-medium text-brand-600 hover:underline">
              Reconocer
            </button>
            <button onClick={() => handleResolve(a.id)} className="text-sm font-medium text-green-700 hover:underline">
              Resolver
            </button>
          </div>
        ) : a.status === "Acknowledged" ? (
          <button onClick={() => handleResolve(a.id)} className="text-sm font-medium text-green-700 hover:underline">
            Resolver
          </button>
        ) : (
          <span className="text-sm text-muted">—</span>
        ),
    },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Alertas</h1>
        <p className="text-muted">Situaciones anormales detectadas automáticamente</p>
      </div>

      <Card>
        <div className="mb-4 flex justify-end">
          <Select
            value={status}
            onChange={(e) => {
              setStatus(e.target.value as AlertStatus | "");
              setPage(1);
            }}
            className="w-56"
          >
            <option value="">Todos los estados</option>
            {Object.entries(STATUS_LABELS).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </Select>
        </div>

        <DataTable columns={columns} rows={alerts} keyExtractor={(a) => a.id} isLoading={isLoading} />
        <div className="pt-4">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </Card>
    </div>
  );
}
