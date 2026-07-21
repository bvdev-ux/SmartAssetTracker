"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input, PasswordInput } from "@/components/ui";
import { loginSchema, type LoginFormValues } from "@/features/auth/schemas/login.schema";

interface LoginFormProps {
  onSubmit: (values: LoginFormValues) => Promise<void>;
}

export function LoginForm({ onSubmit }: LoginFormProps) {
  const [serverError, setServerError] = useState<string | null>(null);
  const [showForgotHint, setShowForgotHint] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  const submitHandler = handleSubmit(async (values) => {
    setServerError(null);
    try {
      await onSubmit(values);
    } catch {
      setServerError("Credenciales inválidas. Verifique su correo y contraseña.");
    }
  });

  return (
    <form onSubmit={submitHandler} className="space-y-5" noValidate>
      <Input
        label="Correo electrónico"
        type="email"
        autoComplete="email"
        placeholder="Digite su correo institucional"
        error={errors.email?.message}
        {...register("email")}
      />

      <PasswordInput
        label="Contraseña"
        autoComplete="current-password"
        placeholder="Digite su contraseña"
        error={errors.password?.message}
        {...register("password")}
      />

      <div className="flex items-center justify-between text-sm">
        <label className="flex items-center gap-2 text-foreground">
          <input
            type="checkbox"
            className="h-4 w-4 rounded border-surface-300 text-brand-600 focus-visible:ring-brand-500"
          />
          Recuérdame
        </label>
        <button
          type="button"
          onClick={() => setShowForgotHint((v) => !v)}
          className="font-medium text-brand-600 hover:underline"
        >
          ¿Contraseña olvidada?
        </button>
      </div>

      {showForgotHint && (
        <p className="rounded-lg bg-surface-100 px-3 py-2 text-sm text-muted">
          Contacte al administrador del sistema para restablecer su contraseña.
        </p>
      )}

      {serverError && (
        <p className="rounded-lg bg-danger-50 px-3 py-2 text-sm text-danger-700" role="alert">
          {serverError}
        </p>
      )}

      <Button type="submit" className="w-full" size="lg" isLoading={isSubmitting}>
        Ingresar
      </Button>
    </form>
  );
}
