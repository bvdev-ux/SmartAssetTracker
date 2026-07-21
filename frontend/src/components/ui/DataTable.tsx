export interface DataTableColumn<T> {
  header: string;
  render: (row: T) => React.ReactNode;
  className?: string;
}

interface DataTableProps<T> {
  columns: DataTableColumn<T>[];
  rows: T[];
  keyExtractor: (row: T) => string;
  isLoading?: boolean;
  emptyMessage?: string;
}

export function DataTable<T>({
  columns,
  rows,
  keyExtractor,
  isLoading,
  emptyMessage = "No hay registros para mostrar.",
}: DataTableProps<T>) {
  return (
    <div className="overflow-x-auto rounded-xl border border-surface-200">
      <table className="w-full min-w-[640px] text-left text-sm">
        <thead className="bg-surface-50 text-xs font-semibold uppercase text-muted">
          <tr>
            {columns.map((col) => (
              <th key={col.header} className="px-4 py-3">
                {col.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-surface-200 bg-card">
          {isLoading ? (
            <tr>
              <td colSpan={columns.length} className="px-4 py-6 text-center text-muted">
                Cargando...
              </td>
            </tr>
          ) : rows.length === 0 ? (
            <tr>
              <td colSpan={columns.length} className="px-4 py-6 text-center text-muted">
                {emptyMessage}
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <tr key={keyExtractor(row)} className="hover:bg-surface-50">
                {columns.map((col) => (
                  <td key={col.header} className={col.className ?? "px-4 py-3"}>
                    {col.render(row)}
                  </td>
                ))}
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
