"use client";

import { useEffect, useState } from "react";
import { Card, DataTable, Input, Button } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import { getLocations } from "@/services/locations.service";
import { getMovementsByAsset, registerMovement } from "@/services/movements.service";
import { getAssets } from "@/services/assets.service";
import { MovementForm } from "@/features/movements/components/MovementForm";
import { MOVEMENT_TYPE_LABELS, type MovementFormValues } from "@/features/movements/schemas/movement.schema";
import type { Location, Movement } from "@/types";

export function MovementsView() {
  const [locations, setLocations] = useState<Location[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const [historySearch, setHistorySearch] = useState("");
  const [historyResults, setHistoryResults] = useState<Movement[]>([]);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [historyError, setHistoryError] = useState<string | null>(null);

  useEffect(() => {
    getLocations().then((response) => {
      if (response.data) setLocations(response.data);
    });
  }, []);

  const handleRegister = async (values: MovementFormValues) => {
    setError(null);
    setSuccess(null);
    try {
      const response = await registerMovement(values);
      if (!response.success) {
        setError(response.message ?? "No se pudo registrar el movimiento.");
        return;
      }
      setSuccess(`Movimiento registrado: ${MOVEMENT_TYPE_LABELS[values.movementType]} de ${values.assetIdentifier}.`);
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        "No se pudo registrar el movimiento.";
      setError(message);
    }
  };

  const handleHistorySearch = async (e: React.FormEvent) => {
    e.preventDefault();
    setHistoryError(null);
    setHistoryLoading(true);
    try {
      const assetsResponse = await getAssets({ search: historySearch, page: 1, pageSize: 1 });
      const asset = assetsResponse.data?.items[0];
      if (!asset) {
        setHistoryError("No se encontró un activo con ese código.");
        setHistoryResults([]);
        return;
      }
      const historyResponse = await getMovementsByAsset(asset.id, 1, 20);
      setHistoryResults(historyResponse.data?.items ?? []);
    } catch {
      setHistoryError("No se pudo cargar el historial.");
    } finally {
      setHistoryLoading(false);
    }
  };

  const columns: DataTableColumn<Movement>[] = [
    { header: "Fecha", render: (m) => new Date(m.occurredAt).toLocaleString() },
    { header: "Tipo", render: (m) => MOVEMENT_TYPE_LABELS[m.movementType] },
    { header: "Persona", render: (m) => m.personFullName },
    { header: "Ubicación", render: (m) => m.locationName },
    { header: "Notas", render: (m) => m.notes ?? "—" },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Registro de movimientos</h1>
        <p className="text-muted">Ingreso, salida, reingreso, préstamo y devolución de activos</p>
      </div>

      <Card title="Registrar movimiento">
        {error && (
          <p className="mb-4 rounded-lg bg-danger-50 px-3 py-2 text-sm text-danger-700" role="alert">
            {error}
          </p>
        )}
        {success && (
          <p className="mb-4 rounded-lg bg-green-50 px-3 py-2 text-sm text-green-700" role="status">
            {success}
          </p>
        )}
        <MovementForm locations={locations} onSubmit={handleRegister} />
      </Card>

      <Card title="Historial de un activo" description="Busque por código, número de serie o QR">
        <form onSubmit={handleHistorySearch} className="mb-4 flex gap-3">
          <Input
            placeholder="Código del activo"
            value={historySearch}
            onChange={(e) => setHistorySearch(e.target.value)}
            className="w-64"
          />
          <Button type="submit" variant="secondary">
            Buscar
          </Button>
        </form>
        {historyError && <p className="mb-4 text-sm text-danger-600">{historyError}</p>}
        <DataTable
          columns={columns}
          rows={historyResults}
          keyExtractor={(m) => m.id}
          isLoading={historyLoading}
          emptyMessage="Busque un activo para ver su historial."
        />
      </Card>
    </div>
  );
}
