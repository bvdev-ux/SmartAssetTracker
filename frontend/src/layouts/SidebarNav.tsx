"use client";

import { motion } from "framer-motion";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useLocale } from "@/hooks/useLocale";
import { navGroups } from "@/shared/nav-config";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui";
import { cn } from "@/utils/cn";

interface SidebarNavProps {
  collapsed?: boolean;
  onNavigate?: () => void;
}

export function SidebarNav({ collapsed = false, onNavigate }: SidebarNavProps) {
  const pathname = usePathname();
  const { t } = useLocale();

  return (
    <nav className="min-h-0 flex-1 space-y-5 overflow-y-auto px-3 py-4" aria-label="Navegación principal">
      {navGroups.map((group) => (
        <div key={group.key}>
          {!collapsed && (
            <p className="mb-1.5 px-2.5 text-[11px] font-semibold uppercase tracking-wider text-sidebar-muted">
              {t.nav[group.key]}
            </p>
          )}
          <div className="space-y-0.5">
            {group.items.map((item) => {
              const Icon = item.icon;
              const basePath = item.href.split("?")[0];
              const isActive = pathname === basePath || pathname.startsWith(`${basePath}/`);

              const link = (
                <Link
                  key={item.key}
                  href={item.href}
                  onClick={onNavigate}
                  aria-current={isActive ? "page" : undefined}
                  className={cn(
                    "group relative flex items-center gap-3 rounded-lg px-2.5 py-2 text-sm font-medium transition-colors",
                    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-sidebar-ring",
                    collapsed && "justify-center px-0 py-2.5",
                    isActive
                      ? "text-sidebar-accent-foreground"
                      : "text-sidebar-foreground/75 hover:bg-surface-200/40 hover:text-sidebar-foreground",
                  )}
                >
                  {isActive && (
                    <motion.span
                      layoutId="sidebar-active-pill"
                      className="absolute inset-0 rounded-lg bg-sidebar-accent"
                      transition={{ type: "spring", stiffness: 500, damping: 40 }}
                    />
                  )}
                  <Icon className="relative z-10 h-[18px] w-[18px] shrink-0" aria-hidden />
                  {!collapsed && <span className="relative z-10 truncate">{t.nav[item.key]}</span>}
                </Link>
              );

              if (!collapsed) return link;

              return (
                <Tooltip key={item.key} delayDuration={200}>
                  <TooltipTrigger asChild>{link}</TooltipTrigger>
                  <TooltipContent side="right">{t.nav[item.key]}</TooltipContent>
                </Tooltip>
              );
            })}
          </div>
        </div>
      ))}
    </nav>
  );
}
