"use client";

import { ChevronRight, Home } from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useLocale } from "@/hooks/useLocale";
import { navGroups, type NavGroup, type NavItem } from "@/shared/nav-config";

export function Breadcrumbs() {
  const pathname = usePathname();
  const { t } = useLocale();

  let match: { groupKey: NavGroup["key"]; item: NavItem } | undefined;
  for (const group of navGroups) {
    const item = group.items.find(
      (i) => i.href.split("?")[0] === pathname || pathname.startsWith(`${i.href.split("?")[0]}/`),
    );
    if (item) {
      match = { groupKey: group.key, item };
      break;
    }
  }

  return (
    <nav className="flex min-w-0 items-center gap-1.5 text-sm" aria-label="Breadcrumb">
      <Link
        href="/dashboard"
        className="flex shrink-0 items-center gap-1 text-muted transition-colors hover:text-foreground"
      >
        <Home className="h-3.5 w-3.5" />
        <span className="hidden sm:inline">{t.header.home}</span>
      </Link>
      {match && match.groupKey !== "group_general" && (
        <>
          <ChevronRight className="h-3.5 w-3.5 shrink-0 text-surface-300" />
          <span className="hidden shrink-0 text-muted md:inline">{t.nav[match.groupKey]}</span>
        </>
      )}
      {match && (
        <>
          <ChevronRight className="h-3.5 w-3.5 shrink-0 text-surface-300" />
          <span className="truncate font-medium text-foreground">{t.nav[match.item.key]}</span>
        </>
      )}
    </nav>
  );
}
