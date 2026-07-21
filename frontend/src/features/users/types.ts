import type { UserFormValues } from "@/features/users/schemas/user.schema";

export interface SystemUser {
  id: string;
  fullName: string;
  email: string;
  role: UserFormValues["role"];
  isActive: boolean;
  lastLoginAt: string | null;
}
