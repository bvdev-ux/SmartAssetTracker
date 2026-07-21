"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input, Select } from "@/components/ui";
import {
  MOVEMENT_TYPE_LABELS,
  movementSchema,
  type MovementFormValues,
} from "@/features/movements/schemas/movement.schema";
import type { Location } from "@/types";

interface MovementFormProps {
  locations: Location[];
  onSubmit: (values: MovementFormValues) => Promise<void>;
}

export function MovementForm({ locations, onSubmit }: MovementFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<MovementFormValues>({
    resolver: zodResolver(movementSchema),
    defaultValues: { movementType: "Entry" },
  });

  const submitHandler = handleSubmit(async (values) => {
    await onSubmit(values);
    reset({ ...values, assetIdentifier: "", personIdentifier: "", notes: "" });
  });

  return (
    <form onSubmit={submitHandler} className="space-y-4" noValidate>
      <div className="grid gap-4 sm:grid-cols-2">
        <Input
          label="Activo (código, serie o QR)"
          error={errors.assetIdentifier?.message}
          {...register("assetIdentifier")}
        />
        <Input
          label="Persona (documento o código)"
          error={errors.personIdentifier?.message}
          {...register("personIdentifier")}
        />
      </div>
      <div className="grid gap-4 sm:grid-cols-2">
        <Select label="Ubicación" error={errors.locationId?.message} {...register("locationId")}>
          <option value="">Seleccione</option>
          {locations.map((l) => (
            <option key={l.id} value={l.id}>
              {l.name}
            </option>
          ))}
        </Select>
        <Select label="Tipo de movimiento" error={errors.movementType?.message} {...register("movementType")}>
          {Object.entries(MOVEMENT_TYPE_LABELS).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </Select>
      </div>
      <Input label="Notas (opcional)" {...register("notes")} />

      <Button type="submit" isLoading={isSubmitting}>
        Registrar movimiento
      </Button>
    </form>
  );
}
