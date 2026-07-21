"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useTheme } from "next-themes";
import { Check, LogOut, Monitor, Moon, Sun } from "lucide-react";
import { useAuth } from "@/hooks/useAuth";
import { useLocale } from "@/hooks/useLocale";
import { LOCALE_LABELS, type Locale } from "@/shared/i18n";
import { ADMIN_AVATAR_URL, API_BASE_URL, APP_NAME, isAdminRole } from "@/shared/constants";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
  Badge,
  Button,
  Card,
  PasswordInput,
  Switch,
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@/components/ui";
import { cn } from "@/utils/cn";

function initials(name: string) {
  return name
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase())
    .join("");
}

export function SettingsView() {
  const router = useRouter();
  const { user, logout } = useAuth();
  const { theme, setTheme } = useTheme();
  const { locale, setLocale, t } = useLocale();

  const [notifyEmail, setNotifyEmail] = useState(true);
  const [notifyAlerts, setNotifyAlerts] = useState(true);
  const [notifyDigest, setNotifyDigest] = useState(false);

  const handleLogout = () => {
    logout();
    router.push("/login");
  };

  const themeOptions = [
    { value: "light", label: t.header.themeLight, icon: Sun },
    { value: "dark", label: t.header.themeDark, icon: Moon },
    { value: "system", label: t.header.themeSystem, icon: Monitor },
  ] as const;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Configuración del sistema</h1>
        <p className="text-muted">Preferencias de cuenta, apariencia, notificaciones y seguridad</p>
      </div>

      <Tabs defaultValue="profile">
        <TabsList>
          <TabsTrigger value="profile">Perfil</TabsTrigger>
          <TabsTrigger value="appearance">Apariencia</TabsTrigger>
          <TabsTrigger value="notifications">Notificaciones</TabsTrigger>
          <TabsTrigger value="security">Seguridad</TabsTrigger>
          <TabsTrigger value="system">Sistema</TabsTrigger>
        </TabsList>

        <TabsContent value="profile">
          <Card>
            <div className="flex items-center gap-4">
              <Avatar className="h-16 w-16">
                {isAdminRole(user?.role) && <AvatarImage src={ADMIN_AVATAR_URL} alt={user?.fullName} />}
                <AvatarFallback className="text-lg">{user ? initials(user.fullName) : "?"}</AvatarFallback>
              </Avatar>
              <div>
                <p className="text-lg font-semibold text-foreground">{user?.fullName}</p>
                <p className="text-sm text-muted">{user?.email}</p>
                <Badge tone="brand" className="mt-1.5">
                  {user?.role}
                </Badge>
              </div>
            </div>
            {user?.permissions && user.permissions.length > 0 && (
              <div className="mt-6">
                <p className="mb-2 text-sm font-medium text-foreground">Permisos asignados</p>
                <div className="flex flex-wrap gap-1.5">
                  {user.permissions.map((permission) => (
                    <Badge key={permission} tone="neutral">
                      {permission}
                    </Badge>
                  ))}
                </div>
              </div>
            )}
          </Card>
        </TabsContent>

        <TabsContent value="appearance">
          <div className="space-y-6">
            <Card title={t.header.theme} description="Elige cómo se ve la plataforma en este dispositivo">
              <div className="grid gap-3 sm:grid-cols-3">
                {themeOptions.map((option) => (
                  <button
                    key={option.value}
                    type="button"
                    onClick={() => setTheme(option.value)}
                    className={cn(
                      "flex flex-col items-center gap-2 rounded-xl border p-5 text-sm font-medium transition-colors",
                      theme === option.value
                        ? "border-brand-500 bg-brand-50 text-brand-700"
                        : "border-surface-200 text-muted hover:bg-surface-50",
                    )}
                  >
                    <option.icon className="h-5 w-5" />
                    {option.label}
                    {theme === option.value && <Check className="h-3.5 w-3.5" />}
                  </button>
                ))}
              </div>
            </Card>

            <Card title={t.header.language} description="Idioma de la interfaz (los módulos operativos continúan en español)">
              <div className="grid gap-3 sm:grid-cols-2">
                {(Object.keys(LOCALE_LABELS) as Locale[]).map((code) => (
                  <button
                    key={code}
                    type="button"
                    onClick={() => setLocale(code)}
                    className={cn(
                      "flex items-center justify-between rounded-xl border p-4 text-sm font-medium transition-colors",
                      locale === code
                        ? "border-brand-500 bg-brand-50 text-brand-700"
                        : "border-surface-200 text-muted hover:bg-surface-50",
                    )}
                  >
                    {LOCALE_LABELS[code]}
                    {locale === code && <Check className="h-4 w-4" />}
                  </button>
                ))}
              </div>
            </Card>
          </div>
        </TabsContent>

        <TabsContent value="notifications">
          <Card title="Preferencias de notificación" description="Elige qué avisos quieres recibir">
            <div className="divide-y divide-surface-200">
              {[
                {
                  label: "Correo por alertas críticas",
                  description: "Recibe un correo cuando se genere una alerta de severidad alta o crítica",
                  checked: notifyEmail,
                  onChange: setNotifyEmail,
                },
                {
                  label: "Notificaciones en la plataforma",
                  description: "Muestra el indicador de notificaciones nuevas en el encabezado",
                  checked: notifyAlerts,
                  onChange: setNotifyAlerts,
                },
                {
                  label: "Resumen semanal",
                  description: "Recibe un resumen de movimientos y alertas cada lunes",
                  checked: notifyDigest,
                  onChange: setNotifyDigest,
                },
              ].map((item) => (
                <div key={item.label} className="flex items-center justify-between gap-4 py-4 first:pt-0 last:pb-0">
                  <div>
                    <p className="text-sm font-medium text-foreground">{item.label}</p>
                    <p className="text-sm text-muted">{item.description}</p>
                  </div>
                  <Switch checked={item.checked} onCheckedChange={item.onChange} />
                </div>
              ))}
            </div>
          </Card>
        </TabsContent>

        <TabsContent value="security">
          <div className="space-y-6">
            <Card title="Cambiar contraseña" description="Usa una contraseña segura que no compartas con nadie más">
              <form className="space-y-4" onSubmit={(e) => e.preventDefault()}>
                <PasswordInput label="Contraseña actual" name="currentPassword" />
                <div className="grid gap-4 sm:grid-cols-2">
                  <PasswordInput label="Nueva contraseña" name="newPassword" />
                  <PasswordInput label="Confirmar contraseña" name="confirmPassword" />
                </div>
                <div className="flex items-center justify-between pt-2">
                  <p className="text-xs text-muted">Disponible próximamente</p>
                  <Button type="submit" disabled>
                    Guardar cambios
                  </Button>
                </div>
              </form>
            </Card>

            <Card title="Sesión" description="Cierra tu sesión en este dispositivo">
              <Button variant="secondary" onClick={handleLogout}>
                <LogOut className="h-4 w-4" />
                {t.header.logout}
              </Button>
            </Card>
          </div>
        </TabsContent>

        <TabsContent value="system">
          <Card title="Información del sistema">
            <dl className="grid gap-4 sm:grid-cols-2">
              <div>
                <dt className="text-xs font-medium uppercase tracking-wide text-muted">Plataforma</dt>
                <dd className="text-sm text-foreground">{APP_NAME}</dd>
              </div>
              <div>
                <dt className="text-xs font-medium uppercase tracking-wide text-muted">Versión</dt>
                <dd className="text-sm text-foreground">0.1.0</dd>
              </div>
              <div>
                <dt className="text-xs font-medium uppercase tracking-wide text-muted">API</dt>
                <dd className="text-sm text-foreground">{API_BASE_URL}</dd>
              </div>
              <div>
                <dt className="text-xs font-medium uppercase tracking-wide text-muted">Entorno</dt>
                <dd className="text-sm text-foreground">
                  {process.env.NODE_ENV === "production" ? "Producción" : "Desarrollo"}
                </dd>
              </div>
            </dl>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
