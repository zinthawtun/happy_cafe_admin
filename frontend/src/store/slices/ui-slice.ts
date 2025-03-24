import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import { UIState, DialogType } from "@types";

const initialState: UIState = {
  notifications: {
    open: false,
    message: "",
    type: "info",
  },
  confirmDialog: {
    open: false,
    title: "",
    message: "",
    dialogType: null,
    entityId: null,
    entityName: null,
  },
  isFormDirty: false,
};

const uiSlice = createSlice({
  name: "ui",
  initialState,
  reducers: {
    showNotification: (
      state,
      action: PayloadAction<{
        message: string;
        type: "success" | "error" | "info" | "warning";
      }>
    ) => {
      state.notifications = {
        open: true,
        message: action.payload.message,
        type: action.payload.type,
      };
    },
    closeNotification: (state) => {
      state.notifications.open = false;
    },
    showConfirmDialog: (
      state,
      action: PayloadAction<{
        title: string;
        message: string;
        dialogType: DialogType;
        entityId: string;
        entityName: string;
      }>
    ) => {
      state.confirmDialog = {
        open: true,
        title: action.payload.title,
        message: action.payload.message,
        dialogType: action.payload.dialogType,
        entityId: action.payload.entityId,
        entityName: action.payload.entityName,
      };
    },
    closeConfirmDialog: (state) => {
      state.confirmDialog = {
        ...state.confirmDialog,
        open: false,
      };
    },
    setFormDirty: (state, action: PayloadAction<boolean>) => {
      state.isFormDirty = action.payload;
    },
  },
});

export const {
  showNotification,
  closeNotification,
  showConfirmDialog,
  closeConfirmDialog,
  setFormDirty,
} = uiSlice.actions;

export default uiSlice.reducer;
