import {
  ArrowLeftRight,
  BellRing,
  History,
  LayoutDashboard,
  type LucideIcon,
  MapPin,
  ScanLine,
  Settings,
  ShieldCheck,
  Tags,
  Laptop,
  UserCog,
  Users,
  FileBarChart,
} from "lucide-react";
import type { Dictionary } from "@/shared/i18n";

export type NavKey = keyof Dictionary["nav"];

export interface NavItem {
  key: Exclude<NavKey, `group_${string}`>;
  href: string;
  icon: LucideIcon;
}

export interface NavGroup {
  key: Extract<NavKey, `group_${string}`>;
  items: NavItem[];
}

export const navGroups: NavGroup[] = [
  {
    key: "group_general",
    items: [{ key: "dashboard", href: "/dashboard", icon: LayoutDashboard }],
  },
  {
    key: "group_operations",
    items: [
      { key: "assets", href: "/assets", icon: Laptop },
      { key: "movements", href: "/movements", icon: ArrowLeftRight },
      { key: "traceability", href: "/traceability", icon: History },
      { key: "accessControl", href: "/access-control", icon: ScanLine },
    ],
  },
  {
    key: "group_org",
    items: [
      { key: "people", href: "/people", icon: Users },
      { key: "users", href: "/users", icon: UserCog },
      { key: "locations", href: "/locations", icon: MapPin },
      { key: "catalog", href: "/assets?tab=catalog", icon: Tags },
    ],
  },
  {
    key: "group_insights",
    items: [
      { key: "reports", href: "/reports", icon: FileBarChart },
      { key: "alerts", href: "/alerts", icon: BellRing },
      { key: "audit", href: "/audit", icon: ShieldCheck },
    ],
  },
  {
    key: "group_system",
    items: [{ key: "settings", href: "/settings", icon: Settings }],
  },
];

export const flatNavItems: NavItem[] = navGroups.flatMap((group) => group.items);

export function findNavItemByPath(pathname: string): NavItem | undefined {
  return (
    flatNavItems.find((item) => item.href.split("?")[0] === pathname) ??
    flatNavItems.find((item) => pathname.startsWith(`${item.href.split("?")[0]}/`))
  );
}
