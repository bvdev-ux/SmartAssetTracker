import { z } from "zod";
import type { AssetStatusType } from "@/types";

export const assetSchema = z.object({
  assetTag: z.string().min(1, "El código es obligatorio"),
  serialNumber: z.string().optional(),
  rfidTag: z.string().optional(),
  categoryId: z.string().min(1, "Seleccione una categoría"),
  brandId: z.string().min(1, "Seleccione una marca"),
  modelId: z.string().min(1, "Seleccione un modelo"),
  color: z.string().optional(),
  features: z.string().optional(),
  ownerId: z.string().optional(),
  notes: z.string().optional(),
  status: z.enum(["Available", "InUse", "Maintenance", "Reported", "Retired"]).optional(),
});

export type AssetFormValues = z.infer<typeof assetSchema>;

export const ASSET_STATUS_LABELS: Record<AssetStatusType, string> = {
  Available: "Disponible",
  InUse: "En uso",
  Maintenance: "Mantenimiento",
  Reported: "Reportado",
  Retired: "Dado de baja",
};

export const categorySchema = z.object({
  name: z.string().min(1, "El nombre es obligatorio"),
  description: z.string().optional(),
});
export type CategoryFormValues = z.infer<typeof categorySchema>;

export const brandSchema = z.object({
  name: z.string().min(1, "El nombre es obligatorio"),
});
export type BrandFormValues = z.infer<typeof brandSchema>;

export const modelSchema = z.object({
  name: z.string().min(1, "El nombre es obligatorio"),
  brandId: z.string().min(1, "Seleccione una marca"),
});
export type ModelFormValues = z.infer<typeof modelSchema>;
