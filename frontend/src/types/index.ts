import { TextFieldProps } from "@mui/material";

export interface Cafe {
  id: string;
  name: string;
  description: string;
  employees: number;
  logo?: string;
  location: string;
}

export interface CafeFormData {
  id?: string;
  name: string;
  description: string;
  logo?: File | null;
  location: string;
}

export interface Employee {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  gender: "male" | "female";
  days_worked: number;
  cafe: string;
  address?: string;
}

export interface EmployeeFormData {
  id?: string;
  name: string;
  email: string;
  phoneNumber: string;
  gender: "male" | "female";
  cafeId: string;
}

export interface EmployeeFormErrors {
  name?: string;
  email?: string;
  phoneNumber?: string;
  gender?: string;
  cafeId?: string;
}

export interface FormFileUploadProps {
  name: string;
  label: string;
  accept: string;
  maxSize?: number;
  onChange: (file: File | null) => void;
  error?: boolean;
  helperText?: string;
  previewUrl?: string;
}

export interface BackendEmployeeResponse {
  id: string;
  name: string;
  emailAddress: string;
  phone: string;
  daysWorked: number;
  cafe: string;
  gender?: string;
}

export interface BackendEmployeeDetailResponse {
  id: string;
  name: string;
  emailAddress: string;
  phone: string;
  gender: string;
  cafeId?: string;
  cafeName?: string;
  startDate?: string;
  daysWorked?: number;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

export interface FormTextFieldProps extends Omit<TextFieldProps, "onChange"> {
  name: string;
  label: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onBlur?: (e: React.FocusEvent<HTMLInputElement>) => void;
  error?: boolean;
  helperText?: string;
  maxLength?: number;
  minLength?: number;
}

export interface NavItem {
  text: string;
  path: string;
  icon: React.ReactNode;
}

export interface CafeFormData {
  name: string;
  description: string;
  location: string;
  logo?: File | null;
  logoFileName?: string;
  employees?: number;
}

export interface CafeFormErrors {
  name?: string;
  description?: string;
  location?: string;
  logo?: string;
}

export interface LogoCache {
  [key: string]: string;
}

export interface Pagination {
  page: number;
  limit: number;
  total: number;
}

export interface CafeState {
  list: Cafe[];
  selectedCafe: Cafe | null;
  loading: boolean;
  error: string | null;
  pagination: Pagination;
}

export interface EmployeeState {
  list: Employee[];
  selectedEmployee: Employee | null;
  loading: boolean;
  error: string | null;
  pagination: Pagination;
}

export enum DialogType {
  DELETE_CAFE = "DELETE_CAFE",
  DELETE_EMPLOYEE = "DELETE_EMPLOYEE",
}

export interface UIState {
  notifications: {
    open: boolean;
    message: string;
    type: "success" | "error" | "info" | "warning";
  };
  confirmDialog: {
    open: boolean;
    title: string;
    message: string;
    dialogType: DialogType | null;
    entityId: string | null;
    entityName: string | null;
  };
  isFormDirty: boolean;
}
