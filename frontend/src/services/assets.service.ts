import { apiClient } from "@/services/api-client";
import type {
  ApiResponse,
  Asset,
  AssetBrand,
  AssetCategory,
  AssetModel,
  AssetStatusType,
  PagedResult,
} from "@/types";

export interface AssetsQuery {
  search?: string;
  categoryId?: string;
  status?: AssetStatusType;
  insideCampus?: boolean;
  page?: number;
  pageSize?: number;
}

export interface CreateAssetPayload {
  assetTag: string;
  serialNumber?: string | null;
  rfidTag?: string | null;
  categoryId: string;
  modelId: string;
  color?: string | null;
  features?: string | null;
  photoUrl?: string | null;
  ownerId?: string | null;
  currentLocationId?: string | null;
  notes?: string | null;
}

export interface UpdateAssetPayload extends Omit<CreateAssetPayload, "currentLocationId"> {
  status: AssetStatusType;
}

export async function getAssets(query: AssetsQuery) {
  const { data } = await apiClient.get<ApiResponse<PagedResult<Asset>>>("/assets", { params: query });
  return data;
}

export async function createAsset(payload: CreateAssetPayload) {
  const { data } = await apiClient.post<ApiResponse<Asset>>("/assets", payload);
  return data;
}

export async function updateAsset(id: string, payload: UpdateAssetPayload) {
  const { data } = await apiClient.put<ApiResponse<Asset>>(`/assets/${id}`, payload);
  return data;
}

export async function retireAsset(id: string) {
  const { data } = await apiClient.delete<ApiResponse<object>>(`/assets/${id}`);
  return data;
}

export async function getAssetCategories() {
  const { data } = await apiClient.get<ApiResponse<AssetCategory[]>>("/assetcategories");
  return data;
}

export async function createAssetCategory(payload: { name: string; description?: string | null }) {
  const { data } = await apiClient.post<ApiResponse<AssetCategory>>("/assetcategories", payload);
  return data;
}

export async function getAssetBrands() {
  const { data } = await apiClient.get<ApiResponse<AssetBrand[]>>("/assetbrands");
  return data;
}

export async function createAssetBrand(payload: { name: string }) {
  const { data } = await apiClient.post<ApiResponse<AssetBrand>>("/assetbrands", payload);
  return data;
}

export async function getAssetModels(brandId?: string) {
  const { data } = await apiClient.get<ApiResponse<AssetModel[]>>("/assetmodels", {
    params: brandId ? { brandId } : undefined,
  });
  return data;
}

export async function createAssetModel(payload: { name: string; brandId: string }) {
  const { data } = await apiClient.post<ApiResponse<AssetModel>>("/assetmodels", payload);
  return data;
}
