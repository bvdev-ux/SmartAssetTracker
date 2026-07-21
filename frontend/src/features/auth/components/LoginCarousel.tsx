"use client";

import { AnimatePresence, motion } from "framer-motion";
import { useEffect, useState } from "react";
import { APP_NAME, APP_DESCRIPTION } from "@/shared/constants";
import { LOGIN_CAROUSEL_SLIDES, LOGIN_VIDEO_SRC } from "@/shared/login-carousel";
import { cn } from "@/utils/cn";

const INTERVAL_MS = 6000;

function ImageCarouselBackground() {
  const [index, setIndex] = useState(0);
  const slides = LOGIN_CAROUSEL_SLIDES;

  useEffect(() => {
    if (slides.length <= 1) return;
    const id = setInterval(() => {
      setIndex((prev) => (prev + 1) % slides.length);
    }, INTERVAL_MS);
    return () => clearInterval(id);
  }, [slides.length]);

  return (
    <>
      <AnimatePresence initial={false}>
        <motion.div
          key={index}
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 1.1, ease: "easeInOut" }}
          className="absolute inset-0 bg-cover bg-center"
          style={{ backgroundImage: `url(${slides[index]?.src})` }}
        />
      </AnimatePresence>

      {slides.length > 1 && (
        <div className="absolute bottom-10 left-1/2 z-10 flex -translate-x-1/2 gap-2">
          {slides.map((slide, i) => (
            <button
              key={slide.src}
              type="button"
              onClick={() => setIndex(i)}
              aria-label={`Ir a la diapositiva ${i + 1}`}
              className={cn(
                "h-1.5 rounded-full transition-all",
                i === index ? "w-8 bg-white" : "w-1.5 bg-white/40 hover:bg-white/60",
              )}
            />
          ))}
        </div>
      )}
    </>
  );
}

export function LoginCarousel() {
  return (
    <div className="relative hidden overflow-hidden bg-slate-900 lg:block lg:flex-1">
      {LOGIN_VIDEO_SRC ? (
        <video
          className="absolute inset-0 h-full w-full object-cover"
          src={LOGIN_VIDEO_SRC}
          autoPlay
          loop
          muted
          playsInline
        />
      ) : (
        <ImageCarouselBackground />
      )}

      <div className="absolute inset-0 bg-black/50" />
      <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-transparent to-black/40" />

      <div className="relative z-10 flex h-full flex-col items-center justify-center px-10 text-center">
        <p className="text-sm font-semibold uppercase tracking-[0.3em] text-white">Bienvenido a</p>
        <h1 className="mt-3 text-5xl font-extrabold text-white drop-shadow-[0_2px_12px_rgba(0,0,0,0.45)] md:text-6xl">
          {APP_NAME}
        </h1>
        <p className="mt-5 max-w-md text-lg text-white drop-shadow-sm">{APP_DESCRIPTION}</p>
      </div>
    </div>
  );
}
