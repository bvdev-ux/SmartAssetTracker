import { z } from "zod";

export const accessControlSchema = z.object({
  personIdentifier: z.string().min(1, "Ingrese el documento o carnet de la persona"),
  assetIdentifier: z.string().min(1, "Ingrese el código, serie o QR del activo"),
  locationId: z.string().min(1, "Seleccione una ubicación"),
  movementType: z.enum(["Entry", "Exit", "ReEntry", "Loan", "Return"]),
  validationMethod: z.enum(["QR", "Carnet", "Manual"]),
});

export type AccessControlFormValues = z.infer<typeof accessControlSchema>;
