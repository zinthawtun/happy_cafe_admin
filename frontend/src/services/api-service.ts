import axios from 'axios';

import { 
  Cafe, 
  Employee, 
  EmployeeFormData, 
  BackendEmployeeResponse, 
  BackendEmployeeDetailResponse 
} from '@/types';

const API_BASE_URL = import.meta.env.BACKEND_API_URL || 'http://localhost:5222';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const getApiUrl = (path: string): string => {
  try {
    return `${API_BASE_URL}${path.startsWith('/') ? path : `/${path}`}`;
  } catch (error) {
    console.error('Error creating API URL:', error);

    return `${API_BASE_URL}/${path}`;
  }
};

export const getLogoUrl = (logoFileName: string): string => {
  return getApiUrl(`api/file/logos/${logoFileName}`);
};

export const getFileUploadUrl = (): string => {
  return getApiUrl('api/file/upload-logo');
};

export const getCafes = async (location?: string): Promise<Cafe[]> => {
  const url = location ? `/cafes?location=${location}` : '/cafes';
  const response = await api.get(url);

  return response.data;
};

export const getCafeById = async (id: string): Promise<Cafe> => {
  const response = await api.get(`/cafes/cafe`, { 
    params: { id }
  });

  return response.data;
};

export const createCafe = async (cafeData: FormData): Promise<Cafe> => {
  const response = await api.post('/cafe', cafeData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });

  return response.data;
};

export const updateCafe = async (cafeData: FormData): Promise<Cafe> => {
  const response = await api.put(`/cafe`, cafeData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });

  return response.data;
};

export const deleteCafe = async (id: string): Promise<void> => {
  await api.delete('/cafe', { data: { id } });
};

export const getEmployees = async (cafe?: string): Promise<Employee[]> => {
  const url = cafe ? `/employees?cafe=${cafe}` : '/employees';
  const response = await api.get(url);

  return response.data.map((employee: BackendEmployeeResponse) => ({
    id: employee.id,
    name: employee.name,
    email: employee.emailAddress,
    phoneNumber: employee.phone,
    days_worked: employee.daysWorked,
    cafe: employee.cafe,
    gender: (employee.gender?.toLowerCase() || 'male') as 'male' | 'female',
  }));
};

export const getEmployeeById = async (id: string): Promise<Employee> => {
  const response = await api.get(`/employees/employee`, { 
    params: { id }
  });

  const employeeData: BackendEmployeeDetailResponse = response.data;

  return {
    id: employeeData.id,
    name: employeeData.name,
    email: employeeData.emailAddress,
    phoneNumber: employeeData.phone,
    gender: employeeData.gender.toLowerCase() as 'male' | 'female',
    days_worked: employeeData.daysWorked || 0,
    cafe: employeeData.cafeName || '',
  };
};

export const createEmployee = async (employeeData: EmployeeFormData): Promise<Employee> => {
  if (!employeeData.email?.trim()) {
    throw new Error('Email address is required');
  }
  
  if (!employeeData.phoneNumber?.trim()) {
    throw new Error('Phone number is required');
  }
  
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(employeeData.email)) {
    throw new Error('Please enter a valid email address');
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
  
  const response = await api.post('/employee', backendData);
  return response.data;
};

export const updateEmployee = async (id: string, employeeData: EmployeeFormData): Promise<Employee> => {
  if (!employeeData.email?.trim()) {
    throw new Error('Email address is required');
  }
  
  if (!employeeData.phoneNumber?.trim()) {
    throw new Error('Phone number is required');
  }
  
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(employeeData.email)) {
    throw new Error('Please enter a valid email address');
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
  
  const response = await api.put(`/employee`, backendData);
  return response.data;
};

export const deleteEmployee = async (id: string): Promise<void> => {
  await api.delete(`/employee`, { data: { id } });
};

export const deleteLogoFile = async (fileName: string): Promise<void> => {
  await api.delete(`/api/file/logos/${fileName}`);
};

export const APIService = api; 