"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input, Select } from "@/components/ui";
import { ASSET_STATUS_LABELS, assetSchema, type AssetFormValues } from "@/features/assets/schemas/asset.schema";
import type { AssetBrand, AssetCategory, AssetModel, Person } from "@/types";

interface AssetFormProps {
  categories: AssetCategory[];
  brands: AssetBrand[];
  models: AssetModel[];
  people: Person[];
  onSubmit: (values: AssetFormValues) => Promise<void>;
  onCancel: () => void;
  defaultValues?: Partial<AssetFormValues>;
  submitLabel?: string;
  showStatus?: boolean;
}

export function AssetForm({
  categories,
  brands,
  models,
  people,
  onSubmit,
  onCancel,
  defaultValues,
  submitLabel = "Guardar",
  showStatus = false,
}: AssetFormProps) {
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<AssetFormValues>({ resolver: zodResolver(assetSchema), defaultValues });

  const selectedBrandId = watch("brandId");
  const availableModels = models.filter((m) => m.brandId === selectedBrandId);

  const submitHandler = handleSubmit(async (values) => {
    await onSubmit(values);
  });

  return (
    <form onSubmit={submitHandler} className="space-y-4" noValidate>
      <div className="grid grid-cols-2 gap-4">
        <Input label="Código del activo" error={errors.assetTag?.message} {...register("assetTag")} />
        <Input label="Número de serie" error={errors.serialNumber?.message} {...register("serialNumber")} />
      </div>

      <Select label="Categoría" error={errors.categoryId?.message} {...register("categoryId")}>
        <option value="">Seleccione</option>
        {categories.map((c) => (
          <option key={c.id} value={c.id}>
            {c.name}
          </option>
        ))}
      </Select>

      <div className="grid grid-cols-2 gap-4">
        <Select label="Marca" error={errors.brandId?.message} {...register("brandId")}>
          <option value="">Seleccione</option>
          {brands.map((b) => (
            <option key={b.id} value={b.id}>
              {b.name}
            </option>
          ))}
        </Select>
        <Select label="Modelo" error={errors.modelId?.message} disabled={!selectedBrandId} {...register("modelId")}>
          <option value="">{selectedBrandId ? "Seleccione" : "Elija una marca primero"}</option>
          {availableModels.map((m) => (
            <option key={m.id} value={m.id}>
              {m.name}
            </option>
          ))}
        </Select>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <Input label="Color" error={errors.color?.message} {...register("color")} />
        <Select label="Propietario (opcional)" {...register("ownerId")}>
          <option value="">Sin asignar</option>
          {people.map((p) => (
            <option key={p.id} value={p.id}>
              {p.firstName} {p.lastName}
            </option>
          ))}
        </Select>
      </div>

      {showStatus && (
        <Select label="Estado" error={errors.status?.message} {...register("status")}>
          {Object.entries(ASSET_STATUS_LABELS).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </Select>
      )}

      <Input label="Características" error={errors.features?.message} {...register("features")} />
      <Input label="Notas" error={errors.notes?.message} {...register("notes")} />

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          {submitLabel}
        </Button>
      </div>
    </form>
  );
}
