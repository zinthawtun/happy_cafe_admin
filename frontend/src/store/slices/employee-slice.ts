import { createSlice, createAsyncThunk, PayloadAction } from "@reduxjs/toolkit";

import { Employee, EmployeeFormData, EmployeeState } from "@types";

import * as api from "@services/api-service";

const initialState: EmployeeState = {
  list: [],
  selectedEmployee: null,
  loading: false,
  error: null,
  pagination: {
    page: 0,
    limit: 10,
    total: 0,
  },
};

export const fetchEmployees = createAsyncThunk(
  "employees/fetchEmployees",
  async (
    params: { cafe?: string; page?: number; limit?: number } | undefined,
    { rejectWithValue }
  ) => {
    try {
      return await api.getEmployees(params);
    } catch {
      return rejectWithValue("Failed to fetch employees");
    }
  }
);

export const fetchEmployeeById = createAsyncThunk(
  "employees/fetchEmployeeById",
  async (id: string, { rejectWithValue }) => {
    try {
      return await api.getEmployeeById(id);
    } catch {
      return rejectWithValue("Failed to fetch employee details");
    }
  }
);

export const createEmployee = createAsyncThunk(
  "employees/createEmployee",
  async (employeeData: EmployeeFormData, { rejectWithValue }) => {
    try {
      return await api.createEmployee(employeeData);
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to create employee";
      return rejectWithValue(errorMessage);
    }
  }
);

export const updateEmployee = createAsyncThunk(
  "employees/updateEmployee",
  async (
    { id, data }: { id: string; data: EmployeeFormData },
    { rejectWithValue }
  ) => {
    try {
      return await api.updateEmployee(id, data);
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to update employee";
      return rejectWithValue(errorMessage);
    }
  }
);

export const deleteEmployee = createAsyncThunk(
  "employees/deleteEmployee",
  async (id: string, { rejectWithValue }) => {
    try {
      await api.deleteEmployee(id);
      return id;
    } catch {
      return rejectWithValue("Failed to delete employee");
    }
  }
);

const employeeSlice = createSlice({
  name: "employees",
  initialState,
  reducers: {
    setEmployees: (state, action: PayloadAction<Employee[]>) => {
      state.list = action.payload;
    },
    clearSelectedEmployee: (state) => {
      state.selectedEmployee = null;
    },
    setPage: (state, action: PayloadAction<number>) => {
      state.pagination.page = action.payload;
    },
    setLimit: (state, action: PayloadAction<number>) => {
      state.pagination.limit = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchEmployees.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchEmployees.fulfilled, (state, action) => {
        state.loading = false;
        state.list = action.payload.data;
        state.pagination.total = action.payload.total;
      })
      .addCase(fetchEmployees.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchEmployeeById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchEmployeeById.fulfilled, (state, action) => {
        state.loading = false;
        state.selectedEmployee = action.payload;
      })
      .addCase(fetchEmployeeById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(createEmployee.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createEmployee.fulfilled, (state, action) => {
        state.loading = false;
        state.list = [...state.list, action.payload];
      })
      .addCase(createEmployee.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(updateEmployee.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateEmployee.fulfilled, (state, action) => {
        state.loading = false;
        state.list = state.list.map((employee) =>
          employee.id === action.payload.id ? action.payload : employee
        );
        state.selectedEmployee = action.payload;
      })
      .addCase(updateEmployee.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(deleteEmployee.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteEmployee.fulfilled, (state, action) => {
        state.loading = false;
        state.list = state.list.filter(
          (employee) => employee.id !== action.payload
        );
        if (
          state.selectedEmployee &&
          state.selectedEmployee.id === action.payload
        ) {
          state.selectedEmployee = null;
        }
      })
      .addCase(deleteEmployee.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { setEmployees, clearSelectedEmployee, setPage, setLimit } =
  employeeSlice.actions;
export default employeeSlice.reducer;
