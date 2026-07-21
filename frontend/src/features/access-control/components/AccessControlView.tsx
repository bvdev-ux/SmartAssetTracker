"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { CheckCircle2, XCircle } from "lucide-react";
import { Badge, Button, Card, Input, Select } from "@/components/ui";
import { getLocations } from "@/services/locations.service";
import { validateAccess } from "@/services/access-control.service";
import {
  accessControlSchema,
  type AccessControlFormValues,
} from "@/features/access-control/schemas/access-control.schema";
import { MOVEMENT_TYPE_LABELS } from "@/features/movements/schemas/movement.schema";
import type { AccessValidationResult, Location } from "@/types";

export function AccessControlView() {
  const [locations, setLocations] = useState<Location[]>([]);
  const [result, setResult] = useState<AccessValidationResult | null>(null);
  const [denialMessage, setDenialMessage] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<AccessControlFormValues>({
    resolver: zodResolver(accessControlSchema),
    defaultValues: { movementType: "Entry", validationMethod: "QR" },
  });

  useEffect(() => {
    getLocations().then((response) => {
      if (response.data) setLocations(response.data);
    });
  }, []);

  const submitHandler = handleSubmit(async (values) => {
    setResult(null);
    setDenialMessage(null);
    try {
      const response = await validateAccess(values);
      setResult(response.data ?? null);
    } catch (err: unknown) {
      const apiError = err as { response?: { data?: { message?: string; data?: AccessValidationResult } } };
      setResult(apiError.response?.data?.data ?? null);
      setDenialMessage(apiError.response?.data?.message ?? "Acceso denegado.");
    }
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Control de acceso</h1>
        <p className="text-muted">Valide el ingreso o salida de un activo mediante QR o carnet</p>
      </div>

      <Card title="Validar acceso">
        <form onSubmit={submitHandler} className="space-y-4" noValidate>
          <div className="grid gap-4 sm:grid-cols-2">
            <Input
              label="Persona (documento o carnet)"
              error={errors.personIdentifier?.message}
              {...register("personIdentifier")}
            />
            <Input
              label="Activo (código, serie o QR)"
              error={errors.assetIdentifier?.message}
              {...register("assetIdentifier")}
            />
          </div>
          <div className="grid gap-4 sm:grid-cols-3">
            <Select label="Ubicación" error={errors.locationId?.message} {...register("locationId")}>
              <option value="">Seleccione</option>
              {locations.map((l) => (
                <option key={l.id} value={l.id}>
                  {l.name}
                </option>
              ))}
            </Select>
            <Select label="Movimiento" error={errors.movementType?.message} {...register("movementType")}>
              {Object.entries(MOVEMENT_TYPE_LABELS).map(([value, label]) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </Select>
            <Select label="Método de validación" {...register("validationMethod")}>
              <option value="QR">Código QR</option>
              <option value="Carnet">Carnet universitario</option>
              <option value="Manual">Manual</option>
            </Select>
          </div>
          <Button type="submit" isLoading={isSubmitting}>
            Validar acceso
          </Button>
        </form>
      </Card>

      {result && (
        <Card>
          <div className="flex items-start gap-3">
            {result.authorized ? (
              <CheckCircle2 className="mt-0.5 h-6 w-6 text-green-600" aria-hidden />
            ) : (
              <XCircle className="mt-0.5 h-6 w-6 text-danger-600" aria-hidden />
            )}
            <div>
              <p className="font-semibold text-foreground">
                {result.authorized ? "Acceso autorizado" : "Acceso denegado"}
              </p>
              {!result.authorized && (
                <p className="mt-1 text-sm text-danger-700">
                  {denialMessage ?? result.denialReason}
                  {result.denialCode && <Badge tone="danger">{result.denialCode}</Badge>}
                </p>
              )}
              {result.movement && (
                <dl className="mt-3 grid grid-cols-2 gap-x-6 gap-y-1 text-sm">
                  <dt className="text-muted">Activo</dt>
                  <dd>{result.movement.assetTag}</dd>
                  <dt className="text-muted">Persona</dt>
                  <dd>{result.movement.personFullName}</dd>
                  <dt className="text-muted">Ubicación</dt>
                  <dd>{result.movement.locationName}</dd>
                  <dt className="text-muted">Hora</dt>
                  <dd>{new Date(result.movement.occurredAt).toLocaleString()}</dd>
                </dl>
              )}
            </div>
          </div>
        </Card>
      )}
    </div>
  );
}
