"use client";

import { ArrowLeftRight, ChevronDown, FileBarChart, Laptop, Plus, QrCode } from "lucide-react";
import { useRouter } from "next/navigation";
import { useLocale } from "@/hooks/useLocale";
import {
  Button,
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui";

export function QuickActionsMenu() {
  const router = useRouter();
  const { t } = useLocale();

  const actions = [
    { label: t.header.newAsset, icon: Laptop, href: "/assets" },
    { label: t.header.newMovement, icon: ArrowLeftRight, href: "/movements" },
    { label: t.header.scanQr, icon: QrCode, href: "/access-control" },
    { label: t.header.generateReport, icon: FileBarChart, href: "/reports" },
  ];

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button size="sm" className="hidden sm:inline-flex">
          <Plus className="h-4 w-4" />
          {t.header.quickActions}
          <ChevronDown className="h-3.5 w-3.5 opacity-70" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-64">
        <DropdownMenuLabel>{t.header.quickActions}</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {actions.map((action) => (
          <DropdownMenuItem key={action.label} onSelect={() => router.push(action.href)}>
            <action.icon />
            {action.label}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
