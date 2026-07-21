import { apiClient } from "@/services/api-client";
import type { ApiResponse, LoginResponse } from "@/types";

export interface LoginPayload {
  email: string;
  password: string;
}

export async function loginRequest(payload: LoginPayload) {
  const { data } = await apiClient.post<ApiResponse<LoginResponse>>(
    "/auth/login",
    payload,
  );
  return data;
}
