"use client";

import { Bell } from "lucide-react";
import Link from "next/link";
import { useEffect, useState } from "react";
import { useLocale } from "@/hooks/useLocale";
import { getAlerts } from "@/services/alerts.service";
import type { Alert, AlertSeverity } from "@/types";
import { Badge, Popover, PopoverContent, PopoverTrigger } from "@/components/ui";
import { cn } from "@/utils/cn";

const SEVERITY_TONE: Record<AlertSeverity, "danger" | "warning" | "brand" | "neutral"> = {
  Critical: "danger",
  High: "danger",
  Medium: "warning",
  Low: "neutral",
};

function relativeTime(iso: string) {
  const diffMs = Date.now() - new Date(iso).getTime();
  const minutes = Math.round(diffMs / 60000);
  if (minutes < 1) return "ahora";
  if (minutes < 60) return `hace ${minutes} min`;
  const hours = Math.round(minutes / 60);
  if (hours < 24) return `hace ${hours} h`;
  return `hace ${Math.round(hours / 24)} d`;
}

export function NotificationsMenu() {
  const { t } = useLocale();
  const [open, setOpen] = useState(false);
  const [alerts, setAlerts] = useState<Alert[]>([]);
  const [loaded, setLoaded] = useState(false);
  const isLoading = open && !loaded;

  useEffect(() => {
    if (!open || loaded) return;
    getAlerts({ status: "Active", page: 1, pageSize: 6 })
      .then((res) => {
        if (res.data) setAlerts(res.data.items);
      })
      .catch(() => setAlerts([]))
      .finally(() => {
        setLoaded(true);
      });
  }, [open, loaded]);

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <button
          type="button"
          aria-label={t.header.notifications}
          className="relative flex h-9 w-9 items-center justify-center rounded-lg text-muted transition-colors hover:bg-surface-100 hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        >
          <Bell className="h-[18px] w-[18px]" />
          {alerts.length > 0 && (
            <span className="absolute right-1.5 top-1.5 h-2 w-2 rounded-full bg-danger-500 ring-2 ring-background" />
          )}
        </button>
      </PopoverTrigger>
      <PopoverContent className="w-96 p-0">
        <div className="flex items-center justify-between border-b border-border px-4 py-3">
          <p className="text-sm font-semibold text-foreground">{t.header.notifications}</p>
          {alerts.length > 0 && <Badge tone="danger">{alerts.length}</Badge>}
        </div>
        <div className="max-h-80 overflow-y-auto">
          {isLoading ? (
            <p className="px-4 py-8 text-center text-sm text-muted">Cargando…</p>
          ) : alerts.length === 0 ? (
            <p className="px-4 py-8 text-center text-sm text-muted">{t.header.notificationsEmpty}</p>
          ) : (
            alerts.map((alert) => (
              <div
                key={alert.id}
                className={cn(
                  "flex gap-3 border-l-2 px-4 py-3 hover:bg-surface-50",
                  alert.severity === "Critical" || alert.severity === "High"
                    ? "border-danger-500"
                    : alert.severity === "Medium"
                      ? "border-warning-500"
                      : "border-surface-300",
                )}
              >
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm font-medium text-foreground">{alert.assetTag ?? alert.alertType}</p>
                  <p className="line-clamp-2 text-xs text-muted">{alert.message}</p>
                  <div className="mt-1 flex items-center gap-2">
                    <Badge tone={SEVERITY_TONE[alert.severity]}>{alert.severity}</Badge>
                    <span className="text-[11px] text-muted">{relativeTime(alert.createdAt)}</span>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
        <Link
          href="/alerts"
          onClick={() => setOpen(false)}
          className="block border-t border-border px-4 py-2.5 text-center text-sm font-medium text-brand-600 hover:bg-surface-50"
        >
          {t.header.viewAllAlerts}
        </Link>
      </PopoverContent>
    </Popover>
  );
}
