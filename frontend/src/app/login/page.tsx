import { LoginContainer } from "@/features/auth/containers/LoginContainer";
import { LoginCarousel } from "@/features/auth/components/LoginCarousel";
import { BRAND_LOGO_FULL } from "@/shared/constants";

export default function LoginPage() {
  return (
    <div className="flex min-h-screen bg-surface-100">
      <LoginCarousel />

      <div className="flex w-full flex-1 items-center justify-center px-4 py-12 lg:w-[440px] lg:flex-none">
        <div className="w-full max-w-sm rounded-2xl border border-surface-200 bg-card p-8 shadow-xl shadow-black/5">
          <div className="mb-8 flex flex-col items-center text-center">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={BRAND_LOGO_FULL} alt="Universidad Peruana Los Andes" className="h-28 w-auto" />
            <p className="mt-3 text-sm text-muted">Acceso seguro institucional</p>
          </div>

          <LoginContainer />
        </div>
      </div>
    </div>
  );
}
