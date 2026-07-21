"use client";

import { FileBarChart, Laptop, QrCode, ArrowLeftRight } from "lucide-react";
import { useRouter } from "next/navigation";
import { useLocale } from "@/hooks/useLocale";
import { flatNavItems } from "@/shared/nav-config";
import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "@/components/ui";

interface CommandPaletteProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CommandPalette({ open, onOpenChange }: CommandPaletteProps) {
  const router = useRouter();
  const { t } = useLocale();

  const go = (href: string) => {
    onOpenChange(false);
    router.push(href);
  };

  const quickActions = [
    { label: t.header.newAsset, icon: Laptop, href: "/assets" },
    { label: t.header.newMovement, icon: ArrowLeftRight, href: "/movements" },
    { label: t.header.scanQr, icon: QrCode, href: "/access-control" },
    { label: t.header.generateReport, icon: FileBarChart, href: "/reports" },
  ];

  return (
    <CommandDialog open={open} onOpenChange={onOpenChange}>
      <CommandInput placeholder={t.command.placeholder} />
      <CommandList>
        <CommandEmpty>{t.command.empty}</CommandEmpty>
        <CommandGroup heading={t.command.actions}>
          {quickActions.map((action) => (
            <CommandItem key={action.label} onSelect={() => go(action.href)}>
              <action.icon />
              {action.label}
            </CommandItem>
          ))}
        </CommandGroup>
        <CommandSeparator />
        <CommandGroup heading={t.command.navigation}>
          {flatNavItems.map((item) => (
            <CommandItem key={item.key} onSelect={() => go(item.href)}>
              <item.icon />
              {t.nav[item.key]}
            </CommandItem>
          ))}
        </CommandGroup>
      </CommandList>
    </CommandDialog>
  );
}
