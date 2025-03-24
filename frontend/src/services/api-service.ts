import axios from "axios";

import {
  Cafe,
  Employee,
  EmployeeFormData,
  BackendEmployeeResponse,
  BackendEmployeeDetailResponse,
} from "@/types";

const API_BASE_URL = import.meta.env.BACKEND_API_URL || "http://localhost:5222";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

export const getApiUrl = (path: string): string => {
  try {
    return `${API_BASE_URL}${path.startsWith("/") ? path : `/${path}`}`;
  } catch (error) {
    console.error("Error creating API URL:", error);

    return `${API_BASE_URL}/${path}`;
  }
};

export const getLogoUrl = (logoFileName: string): string => {
  return getApiUrl(`api/file/logos/${logoFileName}`);
};

export const getFileUploadUrl = (): string => {
  return getApiUrl("api/file/upload-logo");
};

export const getCafes = async (params?: {
  location?: string;
  page?: number;
  limit?: number;
}): Promise<{ data: Cafe[]; total: number }> => {
  const queryParams = new URLSearchParams();

  if (params?.location) {
    queryParams.append("location", params.location);
  }

  if (params?.page !== undefined) {
    queryParams.append("page", params.page.toString());
  }

  if (params?.limit !== undefined) {
    queryParams.append("limit", params.limit.toString());
  }

  const url = `/cafes${
    queryParams.toString() ? `?${queryParams.toString()}` : ""
  }`;

  const response = await api.get(url);

  const allData = response.data.data || response.data;
  const total = response.data.total || allData.length;

  let paginatedData = allData;

  if (Array.isArray(allData) && params?.page !== undefined && params?.limit) {
    const startIndex = params.page * params.limit;
    const endIndex = startIndex + params.limit;
    paginatedData = allData.slice(startIndex, endIndex);
  }

  return {
    data: paginatedData,
    total: total,
  };
};

export const getCafeById = async (id: string): Promise<Cafe> => {
  const response = await api.get(`/cafes/cafe`, {
    params: { id },
  });

  return response.data;
};

export const createCafe = async (cafeData: FormData): Promise<Cafe> => {
  const response = await api.post("/cafe", cafeData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return response.data;
};

export const updateCafe = async (cafeData: FormData): Promise<Cafe> => {
  const response = await api.put(`/cafe`, cafeData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return response.data;
};

export const deleteCafe = async (id: string): Promise<void> => {
  await api.delete("/cafe", { data: { id } });
};

export const getEmployees = async (params?: {
  cafe?: string;
  page?: number;
  limit?: number;
}): Promise<{ data: Employee[]; total: number }> => {
  const queryParams = new URLSearchParams();

  if (params?.cafe) {
    queryParams.append("cafe", params.cafe);
  }

  if (params?.page !== undefined) {
    queryParams.append("page", params.page.toString());
  }

  if (params?.limit !== undefined) {
    queryParams.append("limit", params.limit.toString());
  }

  const url = `/employees${
    queryParams.toString() ? `?${queryParams.toString()}` : ""
  }`;

  const response = await api.get(url);

  const allData = response.data.data || response.data;
  const total = response.data.total || allData.length;

  let paginatedData = allData;

  if (Array.isArray(allData) && params?.page !== undefined && params?.limit) {
    const startIndex = params.page * params.limit;
    const endIndex = startIndex + params.limit;
    paginatedData = allData
      .map((employee: BackendEmployeeResponse) => ({
        id: employee.id,
        name: employee.name,
        email: employee.emailAddress,
        phoneNumber: employee.phone,
        days_worked: employee.daysWorked,
        cafe: employee.cafe,
        gender: (employee.gender?.toLowerCase() || "male") as "male" | "female",
      }))
      .slice(startIndex, endIndex);
  } else {
    paginatedData = paginatedData.map((employee: BackendEmployeeResponse) => ({
      id: employee.id,
      name: employee.name,
      email: employee.emailAddress,
      phoneNumber: employee.phone,
      days_worked: employee.daysWorked,
      cafe: employee.cafe,
      gender: (employee.gender?.toLowerCase() || "male") as "male" | "female",
    }));
  }

  return {
    data: paginatedData,
    total: total,
  };
};

export const getEmployeeById = async (id: string): Promise<Employee> => {
  const response = await api.get(`/employees/employee`, {
    params: { id },
  });

  const employeeData: BackendEmployeeDetailResponse = response.data;

  return {
    id: employeeData.id,
    name: employeeData.name,
    email: employeeData.emailAddress,
    phoneNumber: employeeData.phone,
    gender: employeeData.gender.toLowerCase() as "male" | "female",
    days_worked: employeeData.daysWorked || 0,
    cafe: employeeData.cafeName || "",
  };
};

export const createEmployee = async (
  employeeData: EmployeeFormData
): Promise<Employee> => {
  if (!employeeData.email?.trim()) {
    throw new Error("Email address is required");
  }

  if (!employeeData.phoneNumber?.trim()) {
    throw new Error("Phone number is required");
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(employeeData.email)) {
    throw new Error("Please enter a valid email address");
  }

  const backendData: {
    name: string;
    emailAddress: string;
    phone: string;
    gender: string;
    cafeId?: string;
  } = {
    name: employeeData.name,
    emailAddress: employeeData.email,
    phone: employeeData.phoneNumber,
    gender: employeeData.gender,
  };

  if (employeeData.cafeId) {
    backendData.cafeId = employeeData.cafeId;
  }

  try {
    const response = await api.post("/employee", backendData);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data || "Failed to create employee");
    }
    throw error;
  }
};

export const updateEmployee = async (
  id: string,
  employeeData: EmployeeFormData
): Promise<Employee> => {
  if (!employeeData.email?.trim()) {
    throw new Error("Email address is required");
  }

  if (!employeeData.phoneNumber?.trim()) {
    throw new Error("Phone number is required");
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(employeeData.email)) {
    throw new Error("Please enter a valid email address");
  }

  const backendData: {
    id: string;
    name: string;
    emailAddress: string;
    phone: string;
    gender: string;
    cafeId?: string;
  } = {
    id,
    name: employeeData.name,
    emailAddress: employeeData.email,
    phone: employeeData.phoneNumber,
    gender: employeeData.gender,
  };

  if (employeeData.cafeId) {
    backendData.cafeId = employeeData.cafeId;
  }

  try {
    const response = await api.put(`/employee`, backendData);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data || "Failed to update employee");
    }
    throw error;
  }
};

export const deleteEmployee = async (id: string): Promise<void> => {
  await api.delete(`/employee`, { data: { id } });
};

export const deleteLogoFile = async (fileName: string): Promise<void> => {
  await api.delete(`/api/file/logos/${fileName}`);
};

export const APIService = api;
