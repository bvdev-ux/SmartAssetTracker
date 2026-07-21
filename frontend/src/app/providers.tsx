"use client";

import { ThemeProvider } from "next-themes";
import { useMemo, useState } from "react";
import { LocaleContext } from "@/hooks/useLocale";
import { LOCALE_STORAGE_KEY, translations, type Locale } from "@/shared/i18n";

function LocaleProvider({ children }: { children: React.ReactNode }) {
  const [locale, setLocaleState] = useState<Locale>("es");

  const setLocale = (next: Locale) => {
    setLocaleState(next);
    localStorage.setItem(LOCALE_STORAGE_KEY, next);
  };

  const value = useMemo(
    () => ({ locale, setLocale, t: translations[locale] }),
    [locale],
  );

  return <LocaleContext.Provider value={value}>{children}</LocaleContext.Provider>;
}

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <ThemeProvider attribute="class" defaultTheme="system" enableSystem disableTransitionOnChange>
      <LocaleProvider>{children}</LocaleProvider>
    </ThemeProvider>
  );
}
