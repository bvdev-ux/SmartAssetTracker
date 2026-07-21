"use client";

import { useEffect, useState } from "react";
import { Plus } from "lucide-react";
import { Badge, Button, Card, DataTable, Modal } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import { createLocation, deactivateLocation, getLocations } from "@/services/locations.service";
import { LocationForm } from "@/features/locations/components/LocationForm";
import { LOCATION_TYPE_LABELS, type LocationFormValues } from "@/features/locations/schemas/location.schema";
import type { Location } from "@/types";

export function LocationsView() {
  const [locations, setLocations] = useState<Location[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadLocations = async () => {
    setIsLoading(true);
    try {
      const response = await getLocations();
      if (response.data) setLocations(response.data);
    } catch {
      setError("No se pudo cargar el listado de ubicaciones.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadLocations();
  }, []);

  const handleCreate = async (values: LocationFormValues) => {
    setError(null);
    const response = await createLocation({
      ...values,
      parentLocationId: values.parentLocationId || null,
    });

    if (!response.success) {
      setError(response.message ?? "No se pudo registrar la ubicación.");
      return;
    }

    setIsModalOpen(false);
    await loadLocations();
  };

  const handleDeactivate = async (id: string) => {
    if (!confirm("¿Desactivar esta ubicación?")) return;
    await deactivateLocation(id);
    await loadLocations();
  };

  const columns: DataTableColumn<Location>[] = [
    { header: "Nombre", render: (l) => l.name },
    { header: "Código", render: (l) => l.code },
    { header: "Tipo", render: (l) => LOCATION_TYPE_LABELS[l.locationType] },
    { header: "Ubicación padre", render: (l) => l.parentLocationName ?? "—" },
    {
      header: "Estado",
      render: (l) => <Badge tone={l.isActive ? "success" : "neutral"}>{l.isActive ? "Activa" : "Inactiva"}</Badge>,
    },
    {
      header: "Acciones",
      render: (l) =>
        l.isActive ? (
          <button
            type="button"
            onClick={() => handleDeactivate(l.id)}
            className="text-sm font-medium text-danger-600 hover:underline"
          >
            Desactivar
          </button>
        ) : (
          <span className="text-sm text-muted">—</span>
        ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Ubicaciones</h1>
          <p className="text-muted">Campus, edificios, puertas, laboratorios y más</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)}>
          <Plus className="h-4 w-4" /> Nueva ubicación
        </Button>
      </div>

      {error && (
        <p className="rounded-lg bg-danger-50 px-3 py-2 text-sm text-danger-700" role="alert">
          {error}
        </p>
      )}

      <Card>
        <DataTable columns={columns} rows={locations} keyExtractor={(l) => l.id} isLoading={isLoading} />
      </Card>

      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Registrar ubicación">
        <LocationForm locations={locations} onSubmit={handleCreate} onCancel={() => setIsModalOpen(false)} />
      </Modal>
    </div>
  );
}
