"use client";

import { LogOut, PanelLeftClose, PanelLeftOpen } from "lucide-react";
import { motion } from "framer-motion";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import { ADMIN_AVATAR_URL, APP_NAME, BRAND_LOGO_MARK, isAdminRole } from "@/shared/constants";
import { SidebarNav } from "./SidebarNav";
import { Avatar, AvatarFallback, AvatarImage, Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui";
import { cn } from "@/utils/cn";

function initials(name: string) {
  return name
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase())
    .join("");
}

export function Sidebar() {
  const router = useRouter();
  const { user, logout } = useAuth();
  const [collapsed, setCollapsed] = useState(false);

  const toggleCollapsed = () => setCollapsed((prev) => !prev);

  const handleLogout = () => {
    logout();
    router.push("/login");
  };

  return (
    <motion.aside
      initial={false}
      animate={{ width: collapsed ? 76 : 264 }}
      transition={{ type: "spring", stiffness: 400, damping: 40 }}
      className="sticky top-0 hidden h-screen flex-col border-r border-sidebar-border bg-sidebar text-sidebar-foreground md:flex"
    >
      <div
        className={cn(
          "flex h-16 shrink-0 items-center border-b border-sidebar-border px-4",
          collapsed && "justify-center px-0",
        )}
      >
        <Link href="/dashboard" className="flex min-w-0 items-center gap-2.5">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={BRAND_LOGO_MARK} alt={APP_NAME} className="h-9 w-9 shrink-0" />
          {!collapsed && (
            <span className="min-w-0">
              <span className="block truncate text-sm font-semibold text-sidebar-foreground">{APP_NAME}</span>
              <span className="block truncate text-[11px] text-sidebar-muted">Gestión institucional</span>
            </span>
          )}
        </Link>
      </div>

      <SidebarNav collapsed={collapsed} />

      <div className="shrink-0 border-t border-sidebar-border p-3">
        <div className={cn("flex items-center gap-2.5 rounded-lg px-1.5 py-1.5", collapsed && "justify-center")}>
          <Avatar className="h-8 w-8 shrink-0">
            {isAdminRole(user?.role) && <AvatarImage src={ADMIN_AVATAR_URL} alt={user?.fullName} />}
            <AvatarFallback>{user ? initials(user.fullName) : "?"}</AvatarFallback>
          </Avatar>
          {!collapsed && (
            <div className="min-w-0 flex-1">
              <p className="truncate text-sm font-medium text-sidebar-foreground">{user?.fullName ?? "—"}</p>
              <p className="truncate text-xs text-sidebar-muted">{user?.role ?? ""}</p>
            </div>
          )}
          <Tooltip delayDuration={200}>
            <TooltipTrigger asChild>
              <button
                type="button"
                onClick={handleLogout}
                aria-label="Cerrar sesión"
                className="shrink-0 rounded-lg p-1.5 text-sidebar-muted hover:bg-surface-200/40 hover:text-sidebar-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-sidebar-ring"
              >
                <LogOut className="h-4 w-4" />
              </button>
            </TooltipTrigger>
            <TooltipContent side="right">Cerrar sesión</TooltipContent>
          </Tooltip>
        </div>

        <button
          type="button"
          onClick={toggleCollapsed}
          className={cn(
            "mt-2 flex w-full items-center gap-2 rounded-lg px-2.5 py-2 text-xs font-medium text-sidebar-muted hover:bg-surface-200/40 hover:text-sidebar-foreground",
            collapsed && "justify-center",
          )}
        >
          {collapsed ? <PanelLeftOpen className="h-4 w-4" /> : <PanelLeftClose className="h-4 w-4" />}
          {!collapsed && "Contraer menú"}
        </button>
      </div>
    </motion.aside>
  );
}
