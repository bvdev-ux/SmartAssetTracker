"use client";

import { useRouter } from "next/navigation";
import { LoginForm } from "@/features/auth/components/LoginForm";
import { loginRequest } from "@/services/auth.service";
import type { LoginFormValues } from "@/features/auth/schemas/login.schema";

export function LoginContainer() {
  const router = useRouter();

  const handleLogin = async (values: LoginFormValues) => {
    const response = await loginRequest(values);

    if (!response.success || !response.data) {
      throw new Error(response.message ?? "Error de autenticación");
    }

    localStorage.setItem("accessToken", response.data.accessToken);
    localStorage.setItem("user", JSON.stringify(response.data.user));
    router.push("/dashboard");
  };

  return <LoginForm onSubmit={handleLogin} />;
}
