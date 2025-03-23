import { configureStore } from '@reduxjs/toolkit';
import { useDispatch, useSelector, TypedUseSelectorHook } from 'react-redux';

import cafeReducer from '@store/slices/cafe-slice';
import employeeReducer from '@store/slices/employee-slice';
import uiReducer from '@store/slices/ui-slice';

export const store = configureStore({
  reducer: {
    cafes: cafeReducer,
    employees: employeeReducer,
    ui: uiReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector; 