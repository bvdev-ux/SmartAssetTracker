"use client";

import { useEffect, useState } from "react";
import { Badge, Card, DataTable, Pagination } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import { getAuditLogs } from "@/services/audit.service";
import type { AuditLog } from "@/types";

const PAGE_SIZE = 20;

export function AuditView() {
  const [logs, setLogs] = useState<AuditLog[]>([]);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    setIsLoading(true);
    getAuditLogs({ page, pageSize: PAGE_SIZE }).then((response) => {
      if (response.data) {
        setLogs(response.data.items);
        setTotalPages(response.data.totalPages || 1);
      }
      setIsLoading(false);
    });
  }, [page]);

  const columns: DataTableColumn<AuditLog>[] = [
    { header: "Fecha", render: (l) => new Date(l.occurredAt).toLocaleString() },
    { header: "Usuario", render: (l) => l.userEmail ?? "—" },
    { header: "Acción", render: (l) => <Badge tone="brand">{l.action}</Badge> },
    { header: "Entidad", render: (l) => l.entityName },
    { header: "Detalle", render: (l) => l.details ?? "—" },
    { header: "IP", render: (l) => l.ipAddress ?? "—" },
    {
      header: "Resultado",
      render: (l) => <Badge tone={l.result ? "success" : "danger"}>{l.result ? "Éxito" : "Denegado"}</Badge>,
    },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Auditoría</h1>
        <p className="text-muted">Registro transversal de acciones del sistema</p>
      </div>

      <Card>
        <DataTable columns={columns} rows={logs} keyExtractor={(l) => l.id} isLoading={isLoading} />
        <div className="pt-4">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </Card>
    </div>
  );
}
