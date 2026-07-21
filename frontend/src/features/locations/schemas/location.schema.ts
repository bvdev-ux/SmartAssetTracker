import { z } from "zod";

export const locationSchema = z.object({
  name: z.string().min(1, "El nombre es obligatorio"),
  code: z.string().min(1, "El código es obligatorio"),
  locationType: z.enum(["Campus", "Gate", "Laboratory", "Library", "Office"]),
  parentLocationId: z.string().optional(),
});

export type LocationFormValues = z.infer<typeof locationSchema>;

export const LOCATION_TYPE_LABELS: Record<LocationFormValues["locationType"], string> = {
  Campus: "Campus",
  Gate: "Puerta",
  Laboratory: "Laboratorio",
  Library: "Biblioteca",
  Office: "Oficina",
};
