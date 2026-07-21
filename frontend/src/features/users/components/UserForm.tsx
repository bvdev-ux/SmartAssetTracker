"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input, Select } from "@/components/ui";
import { USER_ROLE_LABELS, userSchema, type UserFormValues } from "@/features/users/schemas/user.schema";

interface UserFormProps {
  onSubmit: (values: UserFormValues) => Promise<void>;
  onCancel: () => void;
  defaultValues?: Partial<UserFormValues>;
  submitLabel?: string;
}

export function UserForm({ onSubmit, onCancel, defaultValues, submitLabel = "Guardar" }: UserFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<UserFormValues>({
    resolver: zodResolver(userSchema),
    defaultValues: { role: "Operator", ...defaultValues },
  });

  const submitHandler = handleSubmit(async (values) => {
    await onSubmit(values);
  });

  return (
    <form onSubmit={submitHandler} className="space-y-4" noValidate>
      <Input label="Nombre completo" error={errors.fullName?.message} {...register("fullName")} />
      <Input label="Correo institucional" type="email" error={errors.email?.message} {...register("email")} />
      <Select label="Rol" error={errors.role?.message} {...register("role")}>
        {Object.entries(USER_ROLE_LABELS).map(([value, label]) => (
          <option key={value} value={value}>
            {label}
          </option>
        ))}
      </Select>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          {submitLabel}
        </Button>
      </div>
    </form>
  );
}
