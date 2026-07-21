import { apiClient } from "@/services/api-client";
import type { ApiResponse, PagedResult, Person, PersonType } from "@/types";

export interface PeopleQuery {
  search?: string;
  personType?: PersonType;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface CreatePersonPayload {
  documentNumber: string;
  universityCode?: string | null;
  firstName: string;
  lastName: string;
  email?: string | null;
  phone?: string | null;
  career?: string | null;
  faculty?: string | null;
  cardQrCode?: string | null;
  personType: PersonType;
}

export type UpdatePersonPayload = Omit<CreatePersonPayload, "documentNumber" | "universityCode"> & {
  isActive: boolean;
};

export async function getPeople(query: PeopleQuery) {
  const { data } = await apiClient.get<ApiResponse<PagedResult<Person>>>("/people", { params: query });
  return data;
}

export async function createPerson(payload: CreatePersonPayload) {
  const { data } = await apiClient.post<ApiResponse<Person>>("/people", payload);
  return data;
}

export async function updatePerson(id: string, payload: UpdatePersonPayload) {
  const { data } = await apiClient.put<ApiResponse<Person>>(`/people/${id}`, payload);
  return data;
}

export async function deactivatePerson(id: string) {
  const { data } = await apiClient.delete<ApiResponse<object>>(`/people/${id}`);
  return data;
}
