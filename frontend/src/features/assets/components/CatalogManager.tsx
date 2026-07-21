"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Badge, Button, Card, Input, Select } from "@/components/ui";
import {
  brandSchema,
  categorySchema,
  modelSchema,
  type BrandFormValues,
  type CategoryFormValues,
  type ModelFormValues,
} from "@/features/assets/schemas/asset.schema";
import type { AssetBrand, AssetCategory, AssetModel } from "@/types";

interface CatalogManagerProps {
  categories: AssetCategory[];
  brands: AssetBrand[];
  models: AssetModel[];
  onCreateCategory: (values: CategoryFormValues) => Promise<void>;
  onCreateBrand: (values: BrandFormValues) => Promise<void>;
  onCreateModel: (values: ModelFormValues) => Promise<void>;
}

export function CatalogManager({
  categories,
  brands,
  models,
  onCreateCategory,
  onCreateBrand,
  onCreateModel,
}: CatalogManagerProps) {
  const categoryForm = useForm<CategoryFormValues>({ resolver: zodResolver(categorySchema) });
  const brandForm = useForm<BrandFormValues>({ resolver: zodResolver(brandSchema) });
  const modelForm = useForm<ModelFormValues>({ resolver: zodResolver(modelSchema) });

  return (
    <div className="grid gap-4 lg:grid-cols-3">
      <Card title="Categorías">
        <form
          onSubmit={categoryForm.handleSubmit(async (values) => {
            await onCreateCategory(values);
            categoryForm.reset();
          })}
          className="mb-4 space-y-2"
        >
          <Input placeholder="Nombre" error={categoryForm.formState.errors.name?.message} {...categoryForm.register("name")} />
          <Input placeholder="Descripción (opcional)" {...categoryForm.register("description")} />
          <Button type="submit" size="sm" className="w-full" isLoading={categoryForm.formState.isSubmitting}>
            Agregar categoría
          </Button>
        </form>
        <div className="flex flex-wrap gap-2">
          {categories.map((c) => (
            <Badge key={c.id} tone="brand">
              {c.name}
            </Badge>
          ))}
        </div>
      </Card>

      <Card title="Marcas">
        <form
          onSubmit={brandForm.handleSubmit(async (values) => {
            await onCreateBrand(values);
            brandForm.reset();
          })}
          className="mb-4 space-y-2"
        >
          <Input placeholder="Nombre" error={brandForm.formState.errors.name?.message} {...brandForm.register("name")} />
          <Button type="submit" size="sm" className="w-full" isLoading={brandForm.formState.isSubmitting}>
            Agregar marca
          </Button>
        </form>
        <div className="flex flex-wrap gap-2">
          {brands.map((b) => (
            <Badge key={b.id} tone="brand">
              {b.name}
            </Badge>
          ))}
        </div>
      </Card>

      <Card title="Modelos">
        <form
          onSubmit={modelForm.handleSubmit(async (values) => {
            await onCreateModel(values);
            modelForm.reset();
          })}
          className="mb-4 space-y-2"
        >
          <Input placeholder="Nombre" error={modelForm.formState.errors.name?.message} {...modelForm.register("name")} />
          <Select error={modelForm.formState.errors.brandId?.message} {...modelForm.register("brandId")}>
            <option value="">Seleccione una marca</option>
            {brands.map((b) => (
              <option key={b.id} value={b.id}>
                {b.name}
              </option>
            ))}
          </Select>
          <Button type="submit" size="sm" className="w-full" isLoading={modelForm.formState.isSubmitting}>
            Agregar modelo
          </Button>
        </form>
        <div className="flex flex-wrap gap-2">
          {models.map((m) => (
            <Badge key={m.id} tone="brand">
              {m.brandName} · {m.name}
            </Badge>
          ))}
        </div>
      </Card>
    </div>
  );
}
