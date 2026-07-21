"use client";

import Link from "next/link";
import { Sheet, SheetContent } from "@/components/ui";
import { APP_NAME, BRAND_LOGO_MARK } from "@/shared/constants";
import { SidebarNav } from "./SidebarNav";

interface MobileSidebarProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function MobileSidebar({ open, onOpenChange }: MobileSidebarProps) {
  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent side="left" className="flex w-72 flex-col p-0">
        <div className="flex h-16 shrink-0 items-center border-b border-sidebar-border px-4">
          <Link href="/dashboard" className="flex items-center gap-2.5" onClick={() => onOpenChange(false)}>
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={BRAND_LOGO_MARK} alt={APP_NAME} className="h-9 w-9 shrink-0" />
            <span>
              <span className="block text-sm font-semibold text-sidebar-foreground">{APP_NAME}</span>
              <span className="block text-[11px] text-sidebar-muted">Gestión institucional</span>
            </span>
          </Link>
        </div>
        <SidebarNav onNavigate={() => onOpenChange(false)} />
      </SheetContent>
    </Sheet>
  );
}
