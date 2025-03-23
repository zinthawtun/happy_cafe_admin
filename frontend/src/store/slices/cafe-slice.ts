import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';

import { Cafe, CafeState } from '@types';

import * as api from '@services/api-service';

const initialState: CafeState = {
  list: [],
  selectedCafe: null,
  loading: false,
  error: null,
  pagination: {
    page: 0,
    limit: 10,
    total: 0
  }
};


export const fetchCafes = createAsyncThunk(
  'cafes/fetchCafes',
  async (params: { location?: string, page?: number, limit?: number } | undefined, { rejectWithValue }) => {
    try {
      return await api.getCafes(params);
    } catch {
      return rejectWithValue('Failed to fetch cafes');
    }
  }
);

export const fetchCafeById = createAsyncThunk(
  'cafes/fetchCafeById',
  async (id: string, { rejectWithValue }) => {
    try {
      return await api.getCafeById(id);
    } catch {
      return rejectWithValue('Failed to fetch cafe details');
    }
  }
);

export const createCafe = createAsyncThunk(
  'cafes/createCafe',
  async (cafeData: FormData, { rejectWithValue }) => {
    try {
      return await api.createCafe(cafeData);
    } catch {
      return rejectWithValue('Failed to create cafe');
    }
  }
);

export const updateCafe = createAsyncThunk(
  'cafes/updateCafe',
  async ({ formData }: { formData: FormData }, { rejectWithValue }) => {
    try {
      return await api.updateCafe(formData);
    } catch {
      return rejectWithValue('Failed to update cafe');
    }
  }
);

export const deleteCafe = createAsyncThunk(
  'cafes/deleteCafe',
  async (id: string, { rejectWithValue }) => {
    try {
      await api.deleteCafe(id);
      return id;
    } catch {
      return rejectWithValue('Failed to delete cafe');
    }
  }
);

const cafeSlice = createSlice({
  name: 'cafes',
  initialState,
  reducers: {
    setCafes: (state, action: PayloadAction<Cafe[]>) => {
      state.list = action.payload;
    },
    clearSelectedCafe: (state) => {
      state.selectedCafe = null;
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
      .addCase(fetchCafes.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCafes.fulfilled, (state, action) => {
        state.loading = false;
        state.list = action.payload.data;
        state.pagination.total = action.payload.total;
      })
      .addCase(fetchCafes.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      .addCase(fetchCafeById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCafeById.fulfilled, (state, action) => {
        state.loading = false;
        state.selectedCafe = action.payload;
      })
      .addCase(fetchCafeById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      .addCase(createCafe.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createCafe.fulfilled, (state, action) => {
        state.loading = false;
        state.list = [...state.list, action.payload];
      })
      .addCase(createCafe.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      .addCase(updateCafe.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateCafe.fulfilled, (state, action) => {
        state.loading = false;
        state.list = state.list.map((cafe) =>
          cafe.id === action.payload.id ? action.payload : cafe
        );
        state.selectedCafe = action.payload;
      })
      .addCase(updateCafe.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      .addCase(deleteCafe.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteCafe.fulfilled, (state, action) => {
        state.loading = false;
        state.list = state.list.filter((cafe) => cafe.id !== action.payload);
        if (state.selectedCafe && state.selectedCafe.id === action.payload) {
          state.selectedCafe = null;
        }
      })
      .addCase(deleteCafe.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { setCafes, clearSelectedCafe, setPage, setLimit } = cafeSlice.actions;
export default cafeSlice.reducer; 