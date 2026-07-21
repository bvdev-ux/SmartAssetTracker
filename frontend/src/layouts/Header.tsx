"use client";

import { Menu, Search } from "lucide-react";
import { useEffect, useState } from "react";
import { useLocale } from "@/hooks/useLocale";
import { Breadcrumbs } from "./header/Breadcrumbs";
import { QuickActionsMenu } from "./header/QuickActionsMenu";
import { NotificationsMenu } from "./header/NotificationsMenu";
import { HelpMenu } from "./header/HelpMenu";
import { LanguageMenu } from "./header/LanguageMenu";
import { ThemeMenu } from "./header/ThemeMenu";
import { UserMenu } from "./header/UserMenu";
import { CommandPalette } from "./CommandPalette";
import { Separator } from "@/components/ui";

interface HeaderProps {
  onMenuClick: () => void;
}

export function Header({ onMenuClick }: HeaderProps) {
  const { t } = useLocale();
  const [commandOpen, setCommandOpen] = useState(false);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === "k") {
        event.preventDefault();
        setCommandOpen((prev) => !prev);
      }
    };
    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, []);

  return (
    <>
      <header className="sticky top-0 z-30 flex h-16 shrink-0 items-center gap-3 border-b border-border bg-background/80 px-4 backdrop-blur-md md:px-6">
        <button
          type="button"
          onClick={onMenuClick}
          aria-label="Abrir menú"
          className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg text-muted hover:bg-surface-100 hover:text-foreground md:hidden"
        >
          <Menu className="h-[18px] w-[18px]" />
        </button>

        <div className="min-w-0 flex-1">
          <Breadcrumbs />
        </div>

        <button
          type="button"
          onClick={() => setCommandOpen(true)}
          className="hidden items-center gap-2.5 rounded-lg border border-surface-300 bg-card px-3 py-1.5 text-sm text-muted shadow-sm transition-colors hover:border-surface-300 hover:bg-surface-50 hover:text-foreground sm:flex md:w-72"
        >
          <Search className="h-4 w-4 shrink-0" />
          <span className="hidden truncate md:inline">{t.header.searchPlaceholder}</span>
          <span className="ml-auto hidden shrink-0 items-center gap-0.5 rounded border border-surface-300 bg-surface-100 px-1.5 py-0.5 text-[11px] font-medium text-muted md:flex">
            Ctrl K
          </span>
        </button>

        <button
          type="button"
          onClick={() => setCommandOpen(true)}
          aria-label={t.header.searchShortcutHint}
          className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg text-muted hover:bg-surface-100 hover:text-foreground sm:hidden"
        >
          <Search className="h-[18px] w-[18px]" />
        </button>

        <QuickActionsMenu />

        <Separator orientation="vertical" className="hidden h-6 sm:block" />

        <div className="flex shrink-0 items-center gap-0.5">
          <NotificationsMenu />
          <HelpMenu />
          <LanguageMenu />
          <ThemeMenu />
        </div>

        <Separator orientation="vertical" className="h-6" />

        <UserMenu />
      </header>

      <CommandPalette open={commandOpen} onOpenChange={setCommandOpen} />
    </>
  );
}
