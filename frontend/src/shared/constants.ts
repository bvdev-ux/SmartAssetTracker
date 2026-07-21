export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000/api/v1";

export const APP_NAME = "Asset Tracking";
export const APP_DESCRIPTION =
  "Plataforma para gestión y trazabilidad de activos tecnológicos";

// Logo institucional completo (UPLA) — usado en contextos claros como la tarjeta de login.
export const BRAND_LOGO_FULL = "/brand/upla-logo-full.png";
// Isotipo institucional (solo el ícono hexagonal) — usado en espacios reducidos:
// sidebar, encabezado, favicon.
export const BRAND_LOGO_MARK = "/brand/upla-logo-mark.png";

// Foto de perfil del usuario administrador (mientras el backend no exponga avatarUrl).
export const ADMIN_AVATAR_URL = "/avatars/admin.jpg";

export function isAdminRole(role?: string | null) {
  return !!role && role.toLowerCase().includes("admin");
}
