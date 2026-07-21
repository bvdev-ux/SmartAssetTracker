"use client";

import { useEffect, useState } from "react";
import { AlertTriangle, ArrowDownLeft, ArrowUpRight, Laptop, Users } from "lucide-react";
import { StatCard } from "@/components/ui/StatCard";
import { Card } from "@/components/ui/Card";
import { getDashboardSummary } from "@/services/dashboard.service";
import type { DashboardStats } from "@/types";

const emptyStats: DashboardStats = {
  assetsInside: 0,
  assetsOutside: 0,
  entriesToday: 0,
  exitsToday: 0,
  activeAlerts: 0,
  visitorsToday: 0,
};

export function DashboardView() {
  const [stats, setStats] = useState<DashboardStats>(emptyStats);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    getDashboardSummary()
      .then((response) => {
        if (isMounted && response.data) setStats(response.data);
      })
      .catch(() => {
        if (isMounted) setError("No se pudieron cargar los indicadores del dashboard.");
      })
      .finally(() => {
        if (isMounted) setIsLoading(false);
      });

    return () => {
      isMounted = false;
    };
  }, []);

  const cards = [
    { title: "Equipos en campus", value: stats.assetsInside, icon: Laptop },
    { title: "Equipos fuera", value: stats.assetsOutside, icon: ArrowUpRight },
    { title: "Entradas hoy", value: stats.entriesToday, icon: ArrowDownLeft },
    { title: "Salidas hoy", value: stats.exitsToday, icon: ArrowUpRight },
    { title: "Alertas activas", value: stats.activeAlerts, icon: AlertTriangle },
    { title: "Visitantes hoy", value: stats.visitorsToday, icon: Users },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Dashboard</h1>
        <p className="text-muted">Indicadores en tiempo real del control de activos tecnológicos</p>
      </div>

      {error && (
        <p className="rounded-lg bg-danger-50 px-3 py-2 text-sm text-danger-700" role="alert">
          {error}
        </p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
        {cards.map((stat) => (
          <StatCard key={stat.title} {...stat} value={isLoading ? "…" : stat.value} />
        ))}
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <Card title="Movimientos" description="Revisa el historial completo en el módulo de Movimientos">
          <p className="text-sm text-muted">
            Registra ingresos, salidas, préstamos y devoluciones desde el módulo de Movimientos o Control
            de acceso.
          </p>
        </Card>
        <Card title="Alertas" description="Eventos que requieren atención">
          <p className="text-sm text-muted">
            {stats.activeAlerts > 0
              ? `Hay ${stats.activeAlerts} alerta(s) activa(s). Revísalas en el módulo de Alertas.`
              : "No hay alertas activas en este momento."}
          </p>
        </Card>
      </div>
    </div>
  );
}
