import { z } from "zod";

export const personSchema = z.object({
  documentNumber: z.string().min(1, "El documento es obligatorio"),
  universityCode: z.string().optional(),
  firstName: z.string().min(1, "El nombre es obligatorio"),
  lastName: z.string().min(1, "El apellido es obligatorio"),
  email: z.union([z.literal(""), z.string().email("Correo inválido")]).optional(),
  phone: z.string().optional(),
  career: z.string().optional(),
  faculty: z.string().optional(),
  cardQrCode: z.string().optional(),
  personType: z.enum(["Student", "Teacher", "Administrative", "Visitor"]),
});

export type PersonFormValues = z.infer<typeof personSchema>;
