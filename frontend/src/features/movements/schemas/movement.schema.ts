import { z } from "zod";

export const movementSchema = z.object({
  assetIdentifier: z.string().min(1, "Ingrese el código, serie o QR del activo"),
  personIdentifier: z.string().min(1, "Ingrese el documento o código de la persona"),
  locationId: z.string().min(1, "Seleccione una ubicación"),
  movementType: z.enum(["Entry", "Exit", "ReEntry", "Loan", "Return"]),
  notes: z.string().optional(),
});

export type MovementFormValues = z.infer<typeof movementSchema>;

export const MOVEMENT_TYPE_LABELS: Record<MovementFormValues["movementType"], string> = {
  Entry: "Ingreso",
  Exit: "Salida",
  ReEntry: "Reingreso",
  Loan: "Préstamo interno",
  Return: "Devolución",
};
