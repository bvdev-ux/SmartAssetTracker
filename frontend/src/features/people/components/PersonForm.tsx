"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Input, Select } from "@/components/ui";
import { personSchema, type PersonFormValues } from "@/features/people/schemas/person.schema";

const PERSON_TYPE_LABELS: Record<PersonFormValues["personType"], string> = {
  Student: "Estudiante",
  Teacher: "Docente",
  Administrative: "Administrativo",
  Visitor: "Visitante",
};

interface PersonFormProps {
  onSubmit: (values: PersonFormValues) => Promise<void>;
  onCancel: () => void;
}

export function PersonForm({ onSubmit, onCancel }: PersonFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<PersonFormValues>({
    resolver: zodResolver(personSchema),
    defaultValues: { personType: "Student" },
  });

  const submitHandler = handleSubmit(async (values) => {
    await onSubmit(values);
  });

  return (
    <form onSubmit={submitHandler} className="space-y-4" noValidate>
      <div className="grid grid-cols-2 gap-4">
        <Input label="Documento" error={errors.documentNumber?.message} {...register("documentNumber")} />
        <Input label="Código universitario" error={errors.universityCode?.message} {...register("universityCode")} />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <Input label="Nombres" error={errors.firstName?.message} {...register("firstName")} />
        <Input label="Apellidos" error={errors.lastName?.message} {...register("lastName")} />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <Input label="Correo" type="email" error={errors.email?.message} {...register("email")} />
        <Input label="Teléfono" error={errors.phone?.message} {...register("phone")} />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <Input label="Carrera" error={errors.career?.message} {...register("career")} />
        <Input label="Facultad" error={errors.faculty?.message} {...register("faculty")} />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <Input label="Código de carnet / QR" error={errors.cardQrCode?.message} {...register("cardQrCode")} />
        <Select label="Tipo de persona" error={errors.personType?.message} {...register("personType")}>
          {Object.entries(PERSON_TYPE_LABELS).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </Select>
      </div>

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

export { PERSON_TYPE_LABELS };
