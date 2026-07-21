import { cn } from "@/utils/cn";
import { LucideIcon } from "lucide-react";

interface StatCardProps {
  title: string;
  value: string | number;
  icon: LucideIcon;
  trend?: string;
  className?: string;
}

export function StatCard({ title, value, icon: Icon, trend, className }: StatCardProps) {
  return (
    <div
      className={cn(
        "rounded-xl border border-surface-200 bg-card p-5 shadow-sm",
        className,
      )}
    >
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-muted">{title}</p>
          <p className="mt-2 text-3xl font-bold text-foreground">{value}</p>
          {trend && <p className="mt-1 text-xs text-brand-600">{trend}</p>}
        </div>
        <div className="rounded-lg bg-brand-50 p-2 text-brand-600">
          <Icon className="h-5 w-5" aria-hidden />
        </div>
      </div>
    </div>
  );
}
