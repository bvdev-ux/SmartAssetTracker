"use client";

import { BookOpen, HelpCircle, Keyboard, LifeBuoy } from "lucide-react";
import { useLocale } from "@/hooks/useLocale";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui";

export function HelpMenu() {
  const { t } = useLocale();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button
          type="button"
          aria-label={t.header.help}
          className="flex h-9 w-9 items-center justify-center rounded-lg text-muted transition-colors hover:bg-surface-100 hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        >
          <HelpCircle className="h-[18px] w-[18px]" />
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-56">
        <DropdownMenuLabel>{t.header.help}</DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem>
          <BookOpen />
          {t.header.helpDocs}
        </DropdownMenuItem>
        <DropdownMenuItem>
          <Keyboard />
          {t.header.helpShortcuts}
          <span className="ml-auto text-xs text-muted">Ctrl K</span>
        </DropdownMenuItem>
        <DropdownMenuItem>
          <LifeBuoy />
          {t.header.helpSupport}
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
