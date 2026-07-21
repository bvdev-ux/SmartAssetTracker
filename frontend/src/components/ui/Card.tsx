import { cn } from "@/utils/cn";
import { HTMLAttributes } from "react";

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  title?: string;
  description?: string;
}

export function Card({ className, title, description, children, ...props }: CardProps) {
  return (
    <div
      className={cn(
        "rounded-xl border border-surface-200 bg-card p-6 shadow-sm",
        className,
      )}
      {...props}
    >
      {(title || description) && (
        <div className="mb-4">
          {title && <h3 className="text-lg font-semibold text-foreground">{title}</h3>}
          {description && <p className="mt-1 text-sm text-muted">{description}</p>}
        </div>
      )}
      {children}
    </div>
  );
}
