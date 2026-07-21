"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input, Select } from "@/components/ui";
import {
  LOCATION_TYPE_LABELS,
  locationSchema,
  type LocationFormValues,
} from "@/features/locations/schemas/location.schema";
import type { Location } from "@/types";

interface LocationFormProps {
  locations: Location[];
  onSubmit: (values: LocationFormValues) => Promise<void>;
  onCancel: () => void;
}

export function LocationForm({ locations, onSubmit, onCancel }: LocationFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LocationFormValues>({
    resolver: zodResolver(locationSchema),
    defaultValues: { locationType: "Campus" },
  });

  const submitHandler = handleSubmit(async (values) => {
    await onSubmit(values);
  });

  return (
    <form onSubmit={submitHandler} className="space-y-4" noValidate>
      <Input label="Nombre" error={errors.name?.message} {...register("name")} />
      <Input label="Código" error={errors.code?.message} {...register("code")} />
      <Select label="Tipo" error={errors.locationType?.message} {...register("locationType")}>
        {Object.entries(LOCATION_TYPE_LABELS).map(([value, label]) => (
          <option key={value} value={value}>
            {label}
          </option>
        ))}
      </Select>
      <Select label="Ubicación padre (opcional)" {...register("parentLocationId")}>
        <option value="">Ninguna</option>
        {locations.map((l) => (
          <option key={l.id} value={l.id}>
            {l.name}
          </option>
        ))}
      </Select>

      <div className="flex justify-end gap-2 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          Guardar
        </Button>
      </div>
    </form>
  );
}
