"use client";

import { useEffect, useState } from "react";
import { Plus } from "lucide-react";
import { Badge, Button, Card, DataTable, Input, Pagination, Modal, Select } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import { createPerson, deactivatePerson, getPeople } from "@/services/people.service";
import type { PersonFormValues } from "@/features/people/schemas/person.schema";
import { PersonForm, PERSON_TYPE_LABELS } from "@/features/people/components/PersonForm";
import type { Person, PersonType } from "@/types";

const PAGE_SIZE = 10;

export function PeopleView() {
  const [people, setPeople] = useState<Person[]>([]);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [personType, setPersonType] = useState<PersonType | "">("");
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadPeople = async () => {
    setIsLoading(true);
    try {
      const response = await getPeople({
        search: search || undefined,
        personType: personType || undefined,
        page,
        pageSize: PAGE_SIZE,
      });
      if (response.data) {
        setPeople(response.data.items);
        setTotalPages(response.data.totalPages || 1);
      }
    } catch {
      setError("No se pudo cargar el listado de personas.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadPeople();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, personType]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    loadPeople();
  };

  const handleCreate = async (values: PersonFormValues) => {
    setError(null);
    const response = await createPerson({
      ...values,
      universityCode: values.universityCode || null,
      email: values.email || null,
      phone: values.phone || null,
      career: values.career || null,
      faculty: values.faculty || null,
      cardQrCode: values.cardQrCode || null,
    });

    if (!response.success) {
      setError(response.message ?? "No se pudo registrar la persona.");
      return;
    }

    setIsModalOpen(false);
    setPage(1);
    await loadPeople();
  };

  const handleDeactivate = async (id: string) => {
    if (!confirm("¿Desactivar esta persona?")) return;
    await deactivatePerson(id);
    await loadPeople();
  };

  const columns: DataTableColumn<Person>[] = [
    { header: "Documento", render: (p) => p.documentNumber },
    { header: "Nombre", render: (p) => `${p.firstName} ${p.lastName}` },
    { header: "Tipo", render: (p) => PERSON_TYPE_LABELS[p.personType] },
    { header: "Correo", render: (p) => p.email ?? "—" },
    {
      header: "Estado",
      render: (p) => <Badge tone={p.isActive ? "success" : "neutral"}>{p.isActive ? "Activo" : "Inactivo"}</Badge>,
    },
    {
      header: "Acciones",
      render: (p) =>
        p.isActive ? (
          <button
            type="button"
            onClick={() => handleDeactivate(p.id)}
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
          <h1 className="text-2xl font-bold text-foreground">Personas</h1>
          <p className="text-muted">Estudiantes, docentes, administrativos y visitantes</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)}>
          <Plus className="h-4 w-4" /> Nueva persona
        </Button>
      </div>

      {error && (
        <p className="rounded-lg bg-danger-50 px-3 py-2 text-sm text-danger-700" role="alert">
          {error}
        </p>
      )}

      <Card>
        <form onSubmit={handleSearchSubmit} className="mb-4 flex flex-wrap gap-3">
          <Input
            placeholder="Buscar por nombre o documento"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-64"
          />
          <Select
            value={personType}
            onChange={(e) => {
              setPersonType(e.target.value as PersonType | "");
              setPage(1);
            }}
            className="w-48"
          >
            <option value="">Todos los tipos</option>
            {Object.entries(PERSON_TYPE_LABELS).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </Select>
          <Button type="submit" variant="secondary">
            Buscar
          </Button>
        </form>

        <DataTable
          columns={columns}
          rows={people}
          keyExtractor={(p) => p.id}
          isLoading={isLoading}
          emptyMessage="No se encontraron personas."
        />
        <div className="pt-4">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </Card>

      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Registrar persona">
        <PersonForm onSubmit={handleCreate} onCancel={() => setIsModalOpen(false)} />
      </Modal>
    </div>
  );
}
