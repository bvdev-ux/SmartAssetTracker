import { z } from "zod";

export const userSchema = z.object({
  fullName: z.string().min(1, "El nombre es obligatorio"),
  email: z.string().email("Correo inválido"),
  role: z.enum(["Administrator", "Operator", "Auditor", "Viewer"]),
});

export type UserFormValues = z.infer<typeof userSchema>;

export const USER_ROLE_LABELS: Record<UserFormValues["role"], string> = {
  Administrator: "Administrador",
  Operator: "Operador",
  Auditor: "Auditor",
  Viewer: "Consulta",
};
