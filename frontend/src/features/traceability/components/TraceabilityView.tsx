"use client";

import { useEffect, useRef, useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  History,
  HandHelping,
  LogIn,
  LogOut,
  MapPin,
  RefreshCcw,
  Search,
  Undo2,
  type LucideIcon,
} from "lucide-react";
import { Badge, Card, Input, Pagination } from "@/components/ui";
import { getAssets } from "@/services/assets.service";
import { getMovementsByAsset } from "@/services/movements.service";
import { MOVEMENT_TYPE_LABELS } from "@/features/movements/schemas/movement.schema";
import type { Asset, Movement, MovementType } from "@/types";

const PAGE_SIZE = 8;

const MOVEMENT_ICON: Record<MovementType, LucideIcon> = {
  Entry: LogIn,
  Exit: LogOut,
  ReEntry: RefreshCcw,
  Loan: HandHelping,
  Return: Undo2,
};

const MOVEMENT_TONE: Record<MovementType, "success" | "danger" | "brand" | "warning" | "neutral"> = {
  Entry: "success",
  Exit: "danger",
  ReEntry: "brand",
  Loan: "warning",
  Return: "neutral",
};

export function TraceabilityView() {
  const [search, setSearch] = useState("");
  const [results, setResults] = useState<Asset[]>([]);
  const [showResults, setShowResults] = useState(false);
  const [selectedAsset, setSelectedAsset] = useState<Asset | null>(null);
  const [movements, setMovements] = useState<Movement[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [searchedTerm, setSearchedTerm] = useState<string | null>(null);
  const [loadedHistoryFor, setLoadedHistoryFor] = useState<string | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const isSearching = search.trim() !== "" && searchedTerm !== search.trim();

  useEffect(() => {
    const term = search.trim();
    if (!term) return;
    const handle = setTimeout(() => {
      getAssets({ search: term, page: 1, pageSize: 6 })
        .then((res) => setResults(res.data?.items ?? []))
        .catch(() => setResults([]))
        .finally(() => setSearchedTerm(term));
    }, 300);
    return () => clearTimeout(handle);
  }, [search]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setShowResults(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const historyKey = selectedAsset ? `${selectedAsset.id}:${page}` : null;
  const isLoadingHistory = historyKey !== null && loadedHistoryFor !== historyKey;

  useEffect(() => {
    if (!selectedAsset || !historyKey) return;
    getMovementsByAsset(selectedAsset.id, page, PAGE_SIZE)
      .then((res) => {
        setMovements(res.data?.items ?? []);
        setTotalPages(res.data?.totalPages || 1);
      })
      .catch(() => setMovements([]))
      .finally(() => setLoadedHistoryFor(historyKey));
  }, [selectedAsset, page, historyKey]);

  const selectAsset = (asset: Asset) => {
    setSelectedAsset(asset);
    setPage(1);
    setSearch("");
    setResults([]);
    setShowResults(false);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Historial y trazabilidad</h1>
        <p className="text-muted">Línea de tiempo completa de ingresos, salidas y préstamos de un activo</p>
      </div>

      <Card>
        <div ref={containerRef} className="relative">
          <Input
            placeholder="Buscar por código, número de serie o QR…"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setShowResults(true);
            }}
            onFocus={() => setShowResults(true)}
            className="w-full pl-10 sm:w-96"
          />
          <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted" />

          <AnimatePresence>
            {showResults && search.trim() && (
              <motion.div
                initial={{ opacity: 0, y: -4 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -4 }}
                transition={{ duration: 0.12 }}
                className="absolute z-20 mt-1.5 w-full overflow-hidden rounded-xl border border-border bg-popover shadow-lg sm:w-96"
              >
                {isSearching ? (
                  <p className="px-4 py-4 text-sm text-muted">Buscando…</p>
                ) : results.length === 0 ? (
                  <p className="px-4 py-4 text-sm text-muted">Sin resultados.</p>
                ) : (
                  results.map((asset) => (
                    <button
                      key={asset.id}
                      type="button"
                      onClick={() => selectAsset(asset)}
                      className="flex w-full flex-col items-start gap-0.5 px-4 py-2.5 text-left hover:bg-surface-100"
                    >
                      <span className="text-sm font-medium text-foreground">{asset.assetTag}</span>
                      <span className="text-xs text-muted">
                        {asset.brandName} {asset.modelName} · {asset.categoryName}
                      </span>
                    </button>
                  ))
                )}
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </Card>

      {!selectedAsset ? (
        <Card className="flex flex-col items-center justify-center gap-3 py-16 text-center">
          <span className="flex h-12 w-12 items-center justify-center rounded-full bg-brand-50 text-brand-600">
            <History className="h-6 w-6" />
          </span>
          <p className="font-medium text-foreground">Busca un activo para ver su trazabilidad</p>
          <p className="max-w-sm text-sm text-muted">
            Encuentra el historial completo de movimientos: ingresos, salidas, préstamos y devoluciones.
          </p>
        </Card>
      ) : (
        <>
          <Card className="flex flex-wrap items-center justify-between gap-4">
            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-muted">Activo seleccionado</p>
              <p className="text-lg font-semibold text-foreground">{selectedAsset.assetTag}</p>
              <p className="text-sm text-muted">
                {selectedAsset.brandName} {selectedAsset.modelName} · {selectedAsset.categoryName}
              </p>
            </div>
            <div className="flex items-center gap-2 text-sm text-muted">
              <MapPin className="h-4 w-4" />
              {selectedAsset.currentLocationName ?? "Sin ubicación"}
            </div>
            <Badge tone={selectedAsset.status === "Available" ? "success" : "brand"}>{selectedAsset.status}</Badge>
          </Card>

          <Card>
            {isLoadingHistory ? (
              <p className="py-8 text-center text-sm text-muted">Cargando historial…</p>
            ) : movements.length === 0 ? (
              <p className="py-8 text-center text-sm text-muted">Este activo aún no tiene movimientos registrados.</p>
            ) : (
              <ol className="relative space-y-6 before:absolute before:left-[15px] before:top-2 before:h-[calc(100%-1rem)] before:w-px before:bg-surface-200">
                {movements.map((movement) => {
                  const Icon = MOVEMENT_ICON[movement.movementType];
                  return (
                    <li key={movement.id} className="relative flex gap-4 pl-0.5">
                      <span className="relative z-10 flex h-8 w-8 shrink-0 items-center justify-center rounded-full border border-surface-200 bg-card text-muted">
                        <Icon className="h-4 w-4" />
                      </span>
                      <div className="min-w-0 flex-1 pb-1">
                        <div className="flex flex-wrap items-center gap-2">
                          <Badge tone={MOVEMENT_TONE[movement.movementType]}>
                            {MOVEMENT_TYPE_LABELS[movement.movementType]}
                          </Badge>
                          <span className="text-xs text-muted">{new Date(movement.occurredAt).toLocaleString()}</span>
                        </div>
                        <p className="mt-1 text-sm text-foreground">
                          {movement.personFullName} · {movement.locationName}
                        </p>
                        {movement.notes && <p className="mt-0.5 text-sm text-muted">{movement.notes}</p>}
                      </div>
                    </li>
                  );
                })}
              </ol>
            )}
            <div className="pt-4">
              <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
            </div>
          </Card>
        </>
      )}
    </div>
  );
}
