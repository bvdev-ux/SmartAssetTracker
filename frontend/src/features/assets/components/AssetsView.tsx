"use client";

import { useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";
import { Pencil, Plus } from "lucide-react";
import { Badge, Button, Card, DataTable, Input, Modal, Pagination, Select } from "@/components/ui";
import type { DataTableColumn } from "@/components/ui";
import {
  createAsset,
  createAssetBrand,
  createAssetCategory,
  createAssetModel,
  getAssetBrands,
  getAssetCategories,
  getAssetModels,
  getAssets,
  retireAsset,
  updateAsset,
} from "@/services/assets.service";
import { getPeople } from "@/services/people.service";
import { AssetForm } from "@/features/assets/components/AssetForm";
import { CatalogManager } from "@/features/assets/components/CatalogManager";
import { ASSET_STATUS_LABELS, type AssetFormValues } from "@/features/assets/schemas/asset.schema";
import type { Asset, AssetBrand, AssetCategory, AssetModel, AssetStatusType, Person } from "@/types";

const PAGE_SIZE = 10;

const STATUS_LABELS = ASSET_STATUS_LABELS;

const STATUS_TONE: Record<AssetStatusType, "success" | "warning" | "danger" | "neutral" | "brand"> = {
  Available: "success",
  InUse: "brand",
  Maintenance: "warning",
  Reported: "danger",
  Retired: "neutral",
};

export function AssetsView() {
  const searchParams = useSearchParams();
  const [tab, setTab] = useState<"assets" | "catalog">(
    searchParams.get("tab") === "catalog" ? "catalog" : "assets",
  );
  const [assets, setAssets] = useState<Asset[]>([]);
  const [categories, setCategories] = useState<AssetCategory[]>([]);
  const [brands, setBrands] = useState<AssetBrand[]>([]);
  const [models, setModels] = useState<AssetModel[]>([]);
  const [people, setPeople] = useState<Person[]>([]);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingAsset, setEditingAsset] = useState<Asset | null>(null);
  const [error, setError] = useState<string | null>(null);

  const loadCatalog = async () => {
    const [categoriesRes, brandsRes, modelsRes, peopleRes] = await Promise.all([
      getAssetCategories(),
      getAssetBrands(),
      getAssetModels(),
      getPeople({ page: 1, pageSize: 200, isActive: true }),
    ]);
    if (categoriesRes.data) setCategories(categoriesRes.data);
    if (brandsRes.data) setBrands(brandsRes.data);
    if (modelsRes.data) setModels(modelsRes.data);
    if (peopleRes.data) setPeople(peopleRes.data.items);
  };

  const loadAssets = async () => {
    setIsLoading(true);
    try {
      const response = await getAssets({
        search: search || undefined,
        categoryId: categoryId || undefined,
        page,
        pageSize: PAGE_SIZE,
      });
      if (response.data) {
        setAssets(response.data.items);
        setTotalPages(response.data.totalPages || 1);
      }
    } catch {
      setError("No se pudo cargar el listado de activos.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadCatalog();
  }, []);

  useEffect(() => {
    loadAssets();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, categoryId]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    loadAssets();
  };

  const handleCreateAsset = async (values: AssetFormValues) => {
    setError(null);
    const response = await createAsset({
      assetTag: values.assetTag,
      serialNumber: values.serialNumber || null,
      rfidTag: values.rfidTag || null,
      categoryId: values.categoryId,
      modelId: values.modelId,
      color: values.color || null,
      features: values.features || null,
      photoUrl: null,
      ownerId: values.ownerId || null,
      currentLocationId: null,
      notes: values.notes || null,
    });

    if (!response.success) {
      setError(response.message ?? "No se pudo registrar el activo.");
      return;
    }

    setIsModalOpen(false);
    setPage(1);
    await loadAssets();
  };

  const handleUpdateAsset = async (values: AssetFormValues) => {
    if (!editingAsset) return;
    setError(null);
    const response = await updateAsset(editingAsset.id, {
      assetTag: values.assetTag,
      serialNumber: values.serialNumber || null,
      rfidTag: values.rfidTag || null,
      categoryId: values.categoryId,
      modelId: values.modelId,
      color: values.color || null,
      features: values.features || null,
      photoUrl: editingAsset.photoUrl ?? null,
      ownerId: values.ownerId || null,
      notes: values.notes || null,
      status: values.status ?? editingAsset.status,
    });

    if (!response.success) {
      setError(response.message ?? "No se pudo actualizar el activo.");
      return;
    }

    setEditingAsset(null);
    await loadAssets();
  };

  const handleRetire = async (id: string) => {
    if (!confirm("¿Dar de baja este activo?")) return;
    await retireAsset(id);
    await loadAssets();
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingAsset(null);
  };

  const editingAssetDefaults: Partial<AssetFormValues> | undefined = editingAsset
    ? {
        assetTag: editingAsset.assetTag,
        serialNumber: editingAsset.serialNumber ?? "",
        rfidTag: editingAsset.rfidTag ?? "",
        categoryId: editingAsset.categoryId,
        brandId: models.find((m) => m.id === editingAsset.modelId)?.brandId ?? "",
        modelId: editingAsset.modelId,
        color: editingAsset.color ?? "",
        features: editingAsset.features ?? "",
        ownerId: editingAsset.ownerId ?? "",
        notes: editingAsset.notes ?? "",
        status: editingAsset.status,
      }
    : undefined;

  const columns: DataTableColumn<Asset>[] = [
    { header: "Código", render: (a) => a.assetTag },
    { header: "Categoría", render: (a) => a.categoryName },
    { header: "Marca / Modelo", render: (a) => `${a.brandName} ${a.modelName}` },
    { header: "Propietario", render: (a) => a.ownerFullName ?? "Sin asignar" },
    { header: "Estado", render: (a) => <Badge tone={STATUS_TONE[a.status]}>{STATUS_LABELS[a.status]}</Badge> },
    {
      header: "Ubicación",
      render: (a) => (
        <span>
          {a.currentLocationName ?? "—"}{" "}
          <span className="text-xs text-muted">({a.isInsideCampus ? "dentro" : "fuera"})</span>
        </span>
      ),
    },
    {
      header: "Acciones",
      render: (a) => (
        <div className="flex items-center gap-4">
          <button
            type="button"
            onClick={() => setEditingAsset(a)}
            className="inline-flex items-center gap-1.5 text-sm font-medium text-muted hover:text-foreground"
          >
            <Pencil className="h-4 w-4" /> Editar
          </button>
          {a.status !== "Retired" ? (
            <button
              type="button"
              onClick={() => handleRetire(a.id)}
              className="text-sm font-medium text-danger-600 hover:underline"
            >
              Dar de baja
            </button>
          ) : (
            <span className="text-sm text-muted">—</span>
          )}
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Activos tecnológicos</h1>
          <p className="text-muted">Laptops, tablets, cámaras, proyectores y más</p>
        </div>
        {tab === "assets" && (
          <Button onClick={() => setIsModalOpen(true)}>
            <Plus className="h-4 w-4" /> Nuevo activo
          </Button>
        )}
      </div>

      <div className="flex gap-2 border-b border-surface-200">
        {[
          { key: "assets" as const, label: "Activos" },
          { key: "catalog" as const, label: "Catálogo" },
        ].map((t) => (
          <button
            key={t.key}
            onClick={() => setTab(t.key)}
            className={`border-b-2 px-4 py-2 text-sm font-medium ${
              tab === t.key ? "border-brand-600 text-brand-700" : "border-transparent text-muted"
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {error && (
        <p className="rounded-lg bg-danger-50 px-3 py-2 text-sm text-danger-700" role="alert">
          {error}
        </p>
      )}

      {tab === "assets" ? (
        <Card>
          <form onSubmit={handleSearchSubmit} className="mb-4 flex flex-wrap gap-3">
            <Input
              placeholder="Buscar por código, serie, modelo o marca"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-72"
            />
            <Select
              value={categoryId}
              onChange={(e) => {
                setCategoryId(e.target.value);
                setPage(1);
              }}
              className="w-56"
            >
              <option value="">Todas las categorías</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </Select>
            <Button type="submit" variant="secondary">
              Buscar
            </Button>
          </form>

          <DataTable
            columns={columns}
            rows={assets}
            keyExtractor={(a) => a.id}
            isLoading={isLoading}
            emptyMessage="No se encontraron activos."
          />
          <div className="pt-4">
            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
          </div>
        </Card>
      ) : (
        <CatalogManager
          categories={categories}
          brands={brands}
          models={models}
          onCreateCategory={async (values) => {
            await createAssetCategory({ name: values.name, description: values.description || null });
            await loadCatalog();
          }}
          onCreateBrand={async (values) => {
            await createAssetBrand(values);
            await loadCatalog();
          }}
          onCreateModel={async (values) => {
            await createAssetModel(values);
            await loadCatalog();
          }}
        />
      )}

      <Modal
        isOpen={isModalOpen || editingAsset !== null}
        onClose={closeModal}
        title={editingAsset ? "Editar activo" : "Registrar activo"}
      >
        <AssetForm
          key={editingAsset?.id ?? "new"}
          categories={categories}
          brands={brands}
          models={models}
          people={people}
          onSubmit={editingAsset ? handleUpdateAsset : handleCreateAsset}
          onCancel={closeModal}
          defaultValues={editingAssetDefaults}
          submitLabel={editingAsset ? "Guardar cambios" : "Guardar"}
          showStatus={editingAsset !== null}
        />
      </Modal>
    </div>
  );
}
