export interface LoginCarouselSlide {
  src: string;
  alt: string;
}

// Video de fondo del login (frontend/public/login/campus-login-background.mp4).
// Si se define, tiene prioridad sobre el carrusel de imágenes de abajo.
export const LOGIN_VIDEO_SRC = "/login/campus-login-background.mp4";

// Coloca tus imágenes en frontend/public/login/ con estos mismos nombres
// (o cambia las rutas de abajo si prefieres otros nombres de archivo).
// Solo se usan como respaldo si LOGIN_VIDEO_SRC está vacío.
export const LOGIN_CAROUSEL_SLIDES: LoginCarouselSlide[] = [
  { src: "/login/slide-1.jpg", alt: "Campus e infraestructura tecnológica" },
  { src: "/login/slide-2.jpg", alt: "Personal registrando activos" },
  { src: "/login/slide-3.jpg", alt: "Laboratorio de cómputo" },
  { src: "/login/slide-4.jpg", alt: "Control de acceso institucional" },
];
