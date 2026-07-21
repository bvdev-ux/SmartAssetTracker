"use client";

import { useMemo, useState } from "react";
import { Pencil, Plus, ShieldOff, ShieldCheck as ShieldCheckIcon } from "lucide-react";
import { Avatar, AvatarFallback, AvatarImage, Badge, Button, Card, DataTable, Input, Modal, Select } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import { UserForm } from "./UserForm";
import { USER_ROLE_LABELS, type UserFormValues } from "@/features/users/schemas/user.schema";
import type { SystemUser } from "@/features/users/types";
import { ADMIN_AVATAR_URL, isAdminRole } from "@/shared/constants";

const ROLE_TONE: Record<UserFormValues["role"], "brand" | "success" | "warning" | "neutral"> = {
  Administrator: "brand",
  Operator: "success",
  Auditor: "warning",
  Viewer: "neutral",
};

const INITIAL_USERS: SystemUser[] = [
  {
    id: "u-1",
    fullName: "Brayan Vargas Sedano",
    email: "brayan.vargas@institucion.edu",
    role: "Administrator",
    isActive: true,
    lastLoginAt: "2026-07-16T14:32:00Z",
  },
  {
    id: "u-2",
    fullName: "María Fernanda Ruiz",
    email: "maria.ruiz@institucion.edu",
    role: "Operator",
    isActive: true,
    lastLoginAt: "2026-07-16T09:10:00Z",
  },
  {
    id: "u-3",
    fullName: "Carlos Andrés Peña",
    email: "carlos.pena@institucion.edu",
    role: "Auditor",
    isActive: true,
    lastLoginAt: "2026-07-14T18:45:00Z",
  },
  {
    id: "u-4",
    fullName: "Lucía Gómez Torres",
    email: "lucia.gomez@institucion.edu",
    role: "Viewer",
    isActive: false,
    lastLoginAt: "2026-06-30T11:00:00Z",
  },
];

const USERS_STORAGE_KEY = "asset-tracking:mock-users";

function loadInitialUsers(): SystemUser[] {
  if (typeof window === "undefined") return INITIAL_USERS;
  try {
    const stored = localStorage.getItem(USERS_STORAGE_KEY);
    return stored ? (JSON.parse(stored) as SystemUser[]) : INITIAL_USERS;
  } catch {
    return INITIAL_USERS;
  }
}

function initials(name: string) {
  return name
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase())
    .join("");
}

export function UsersView() {
  const [users, setUsersState] = useState<SystemUser[]>(loadInitialUsers);

  const setUsers = (updater: SystemUser[] | ((prev: SystemUser[]) => SystemUser[])) => {
    setUsersState((prev) => {
      const next = typeof updater === "function" ? (updater as (p: SystemUser[]) => SystemUser[])(prev) : updater;
      try {
        localStorage.setItem(USERS_STORAGE_KEY, JSON.stringify(next));
      } catch {
        // localStorage no disponible (modo privado, cuota excedida, etc.) — el cambio queda solo en memoria.
      }
      return next;
    });
  };
  const [search, setSearch] = useState("");
  const [roleFilter, setRoleFilter] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<SystemUser | null>(null);

  const filtered = useMemo(() => {
    return users.filter((u) => {
      const matchesSearch =
        !search ||
        u.fullName.toLowerCase().includes(search.toLowerCase()) ||
        u.email.toLowerCase().includes(search.toLowerCase());
      const matchesRole = !roleFilter || u.role === roleFilter;
      return matchesSearch && matchesRole;
    });
  }, [users, search, roleFilter]);

  const handleCreate = async (values: UserFormValues) => {
    setUsers((prev) => [
      { id: `u-${Date.now()}`, ...values, isActive: true, lastLoginAt: null },
      ...prev,
    ]);
    setIsModalOpen(false);
  };

  const handleUpdate = async (values: UserFormValues) => {
    if (!editingUser) return;
    setUsers((prev) => prev.map((u) => (u.id === editingUser.id ? { ...u, ...values } : u)));
    setEditingUser(null);
  };

  const toggleActive = (id: string) => {
    setUsers((prev) => prev.map((u) => (u.id === id ? { ...u, isActive: !u.isActive } : u)));
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingUser(null);
  };

  const columns: DataTableColumn<SystemUser>[] = [
    {
      header: "Usuario",
      render: (u) => (
        <div className="flex items-center gap-3">
          <Avatar className="h-8 w-8">
            {isAdminRole(u.role) && <AvatarImage src={ADMIN_AVATAR_URL} alt={u.fullName} />}
            <AvatarFallback>{initials(u.fullName)}</AvatarFallback>
          </Avatar>
          <div className="min-w-0">
            <p className="truncate font-medium text-foreground">{u.fullName}</p>
            <p className="truncate text-xs text-muted">{u.email}</p>
          </div>
        </div>
      ),
    },
    { header: "Rol", render: (u) => <Badge tone={ROLE_TONE[u.role]}>{USER_ROLE_LABELS[u.role]}</Badge> },
    {
      header: "Estado",
      render: (u) => <Badge tone={u.isActive ? "success" : "neutral"}>{u.isActive ? "Activo" : "Inactivo"}</Badge>,
    },
    {
      header: "Último acceso",
      render: (u) => (u.lastLoginAt ? new Date(u.lastLoginAt).toLocaleString() : "Nunca"),
    },
    {
      header: "Acciones",
      render: (u) => (
        <div className="flex items-center gap-4">
          <button
            type="button"
            onClick={() => setEditingUser(u)}
            className="inline-flex items-center gap-1.5 text-sm font-medium text-muted hover:text-foreground"
          >
            <Pencil className="h-4 w-4" /> Editar
          </button>
          <button
            type="button"
            onClick={() => toggleActive(u.id)}
            className="inline-flex items-center gap-1.5 text-sm font-medium text-muted hover:text-foreground"
          >
            {u.isActive ? (
              <>
                <ShieldOff className="h-4 w-4" /> Desactivar
              </>
            ) : (
              <>
                <ShieldCheckIcon className="h-4 w-4" /> Activar
              </>
            )}
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Gestión de usuarios</h1>
          <p className="text-muted">Cuentas del sistema, roles y permisos de acceso a la plataforma</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)}>
          <Plus className="h-4 w-4" /> Nuevo usuario
        </Button>
      </div>

      <Card>
        <div className="mb-4 flex flex-wrap gap-3">
          <Input
            placeholder="Buscar por nombre o correo"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-72"
          />
          <Select value={roleFilter} onChange={(e) => setRoleFilter(e.target.value)} className="w-56">
            <option value="">Todos los roles</option>
            {Object.entries(USER_ROLE_LABELS).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </Select>
        </div>

        <DataTable columns={columns} rows={filtered} keyExtractor={(u) => u.id} emptyMessage="No se encontraron usuarios." />
      </Card>

      <Modal
        isOpen={isModalOpen || editingUser !== null}
        onClose={closeModal}
        title={editingUser ? "Editar usuario" : "Registrar usuario"}
      >
        <UserForm
          key={editingUser?.id ?? "new"}
          onSubmit={editingUser ? handleUpdate : handleCreate}
          onCancel={closeModal}
          defaultValues={editingUser ?? undefined}
          submitLabel={editingUser ? "Guardar cambios" : "Guardar"}
        />
      </Modal>
    </div>
  );
}
