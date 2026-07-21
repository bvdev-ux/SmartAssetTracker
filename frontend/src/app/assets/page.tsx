import { Suspense } from "react";
import { AssetsView } from "@/features/assets/components/AssetsView";
import { AppLayout } from "@/layouts/AppLayout";

export default function AssetsPage() {
  return (
    <AppLayout>
      <Suspense fallback={null}>
        <AssetsView />
      </Suspense>
    </AppLayout>
  );
}
