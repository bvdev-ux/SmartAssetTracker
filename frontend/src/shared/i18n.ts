export type Locale = "es" | "en";

export const LOCALE_STORAGE_KEY = "asset-tracking:locale";

export const LOCALE_LABELS: Record<Locale, string> = {
  es: "Español",
  en: "English",
};

export interface Dictionary {
  nav: Record<
    | "group_general"
    | "group_operations"
    | "group_org"
    | "group_insights"
    | "group_system"
    | "dashboard"
    | "assets"
    | "movements"
    | "traceability"
    | "accessControl"
    | "people"
    | "users"
    | "locations"
    | "catalog"
    | "reports"
    | "alerts"
    | "audit"
    | "settings",
    string
  >;
  header: Record<
    | "searchPlaceholder"
    | "searchShortcutHint"
    | "quickActions"
    | "newAsset"
    | "newMovement"
    | "scanQr"
    | "generateReport"
    | "notifications"
    | "notificationsEmpty"
    | "viewAllAlerts"
    | "help"
    | "helpDocs"
    | "helpShortcuts"
    | "helpSupport"
    | "language"
    | "theme"
    | "themeLight"
    | "themeDark"
    | "themeSystem"
    | "profile"
    | "accountSettings"
    | "logout"
    | "home",
    string
  >;
  command: Record<"placeholder" | "empty" | "navigation" | "actions", string>;
}

export const translations: Record<Locale, Dictionary> = {
  es: {
    nav: {
      group_general: "General",
      group_operations: "Gestión de activos",
      group_org: "Organización",
      group_insights: "Análisis y control",
      group_system: "Sistema",
      dashboard: "Dashboard",
      assets: "Gestión de Activos Tecnológicos",
      movements: "Registro de Ingresos y Salidas",
      traceability: "Historial y Trazabilidad",
      accessControl: "Control de Acceso (QR/RFID)",
      people: "Gestión de Personas",
      users: "Gestión de Usuarios",
      locations: "Gestión de Ubicaciones",
      catalog: "Categorías y Catálogos",
      reports: "Reportes",
      alerts: "Alertas y Notificaciones",
      audit: "Auditoría",
      settings: "Configuración del Sistema",
    },
    header: {
      searchPlaceholder: "Buscar activos, personas, ubicaciones…",
      searchShortcutHint: "Buscar",
      quickActions: "Registrar",
      newAsset: "Registrar Activo",
      newMovement: "Registrar Ingreso/Salida",
      scanQr: "Escanear QR",
      generateReport: "Generar Reporte",
      notifications: "Notificaciones",
      notificationsEmpty: "No tienes notificaciones nuevas",
      viewAllAlerts: "Ver todas las alertas",
      help: "Ayuda",
      helpDocs: "Documentación",
      helpShortcuts: "Atajos de teclado",
      helpSupport: "Soporte técnico",
      language: "Idioma",
      theme: "Tema",
      themeLight: "Claro",
      themeDark: "Oscuro",
      themeSystem: "Sistema",
      profile: "Perfil",
      accountSettings: "Configuración",
      logout: "Cerrar sesión",
      home: "Inicio",
    },
    command: {
      placeholder: "Escribe un comando o busca…",
      empty: "Sin resultados.",
      navigation: "Navegación",
      actions: "Acciones rápidas",
    },
  },
  en: {
    nav: {
      group_general: "General",
      group_operations: "Asset management",
      group_org: "Organization",
      group_insights: "Insights & control",
      group_system: "System",
      dashboard: "Dashboard",
      assets: "Technology Asset Management",
      movements: "Entry & Exit Logging",
      traceability: "History & Traceability",
      accessControl: "Access Control (QR/RFID)",
      people: "People Management",
      users: "User Management",
      locations: "Location Management",
      catalog: "Categories & Catalogs",
      reports: "Reports",
      alerts: "Alerts & Notifications",
      audit: "Audit Log",
      settings: "System Settings",
    },
    header: {
      searchPlaceholder: "Search assets, people, locations…",
      searchShortcutHint: "Search",
      quickActions: "Create",
      newAsset: "Register Asset",
      newMovement: "Log Entry/Exit",
      scanQr: "Scan QR",
      generateReport: "Generate Report",
      notifications: "Notifications",
      notificationsEmpty: "You have no new notifications",
      viewAllAlerts: "View all alerts",
      help: "Help",
      helpDocs: "Documentation",
      helpShortcuts: "Keyboard shortcuts",
      helpSupport: "Contact support",
      language: "Language",
      theme: "Theme",
      themeLight: "Light",
      themeDark: "Dark",
      themeSystem: "System",
      profile: "Profile",
      accountSettings: "Settings",
      logout: "Log out",
      home: "Home",
    },
    command: {
      placeholder: "Type a command or search…",
      empty: "No results found.",
      navigation: "Navigation",
      actions: "Quick actions",
    },
  },
};
