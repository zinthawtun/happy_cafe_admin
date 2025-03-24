import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';
import userEvent from '@testing-library/user-event';

import ConfirmDialog from '../confirm-dialog-component';
import { DialogType } from '@/types';
import uiReducer, { closeConfirmDialog, showNotification } from '@/store/slices/ui-slice';
import { deleteCafe } from '@/store/slices/cafe-slice';
import { deleteEmployee } from '@/store/slices/employee-slice';

vi.mock('@/store/slices/ui-slice', async () => {
  const actual = await vi.importActual('@/store/slices/ui-slice');
  return {
    ...actual,
    closeConfirmDialog: vi.fn(() => ({ type: 'ui/closeConfirmDialog' })),
    showNotification: vi.fn((payload) => ({ 
      type: 'ui/showNotification', 
      payload 
    }))
  };
});

vi.mock('@/store/slices/cafe-slice', async () => {
  const actual = await vi.importActual('@/store/slices/cafe-slice');
  return {
    ...actual,
    deleteCafe: vi.fn((id) => ({
      type: 'cafe/deleteCafe',
      payload: id,
      unwrap: vi.fn().mockResolvedValue({ id })
    }))
  };
});

vi.mock('@/store/slices/employee-slice', async () => {
  const actual = await vi.importActual('@/store/slices/employee-slice');
  return {
    ...actual,
    deleteEmployee: vi.fn((id) => ({
      type: 'employee/deleteEmployee',
      payload: id,
      unwrap: vi.fn().mockResolvedValue({ id })
    }))
  };
});

vi.mock('../../redux/slices/uiSlice', () => ({
  setConfirmDialog: (state: { open: boolean; type?: string; entityId?: string }) => ({ 
    type: 'ui/setConfirmDialog', 
    payload: state 
  }),
  hideNotification: () => ({ type: 'ui/hideNotification' }),
  showNotification: (notification: { message: string; type: string }) => ({ 
    type: 'ui/showNotification', 
    payload: notification 
  }),
}));

vi.mock('../../redux/slices/cafeSlice', () => ({
  deleteCafe: (id: string) => ({ type: 'cafe/deleteCafe', payload: id }),
}));

vi.mock('../../redux/slices/employeeSlice', () => ({
  deleteEmployee: (id: string) => ({ type: 'employee/deleteEmployee', payload: id }),
}));

describe('ConfirmDialog', () => {
  const createTestStore = (initialState = {}) => {
    return configureStore({
      reducer: {
        ui: uiReducer
      },
      middleware: (getDefaultMiddleware) => 
        getDefaultMiddleware({
          serializableCheck: false
        }),
      preloadedState: {
        ui: {
          confirmDialog: {
            open: false,
            title: '',
            message: '',
            dialogType: null,
            entityId: null,
            entityName: null,
            ...initialState
          },
          notifications: {
            open: false,
            message: '',
            type: 'info' as const,
          },
          isFormDirty: false
        }
      }
    });
  };

  it('renders correctly when dialog is open', () => {
    const store = createTestStore({
      open: true,
      title: 'Confirm Delete',
      message: 'Are you sure you want to delete this cafe?'
    });

    render(
      <Provider store={store}>
        <ConfirmDialog />
      </Provider>
    );

    expect(screen.getByText('Confirm Delete')).toBeInTheDocument();
    expect(screen.getByText('Are you sure you want to delete this cafe?')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /cancel/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /confirm/i })).toBeInTheDocument();
  });

  it('dispatches closeConfirmDialog when Cancel button is clicked', async () => {
    const store = createTestStore({
      open: true,
      title: 'Confirm Delete'
    });

    const dispatchSpy = vi.spyOn(store, 'dispatch');

    render(
      <Provider store={store}>
        <ConfirmDialog />
      </Provider>
    );

    await userEvent.click(screen.getByRole('button', { name: /cancel/i }));

    expect(closeConfirmDialog).toHaveBeenCalled();
    expect(dispatchSpy).toHaveBeenCalled();
  });

  it('deletes a cafe when confirmed', async () => {
    const store = createTestStore({
      open: true,
      title: 'Delete Cafe',
      message: 'Are you sure you want to delete "Test Cafe"?',
      dialogType: DialogType.DELETE_CAFE,
      entityId: '123',
      entityName: 'Test Cafe'
    });

    render(
      <Provider store={store}>
        <ConfirmDialog />
      </Provider>
    );

    await userEvent.click(screen.getByRole('button', { name: /confirm/i }));

    expect(deleteCafe).toHaveBeenCalledWith('123');
    expect(showNotification).toHaveBeenCalledWith({
      message: 'Cafe "Test Cafe" was successfully deleted.',
      type: 'success'
    });
    expect(closeConfirmDialog).toHaveBeenCalled();
  });

  it('deletes an employee when confirmed', async () => {
    const store = createTestStore({
      open: true,
      title: 'Delete Employee',
      message: 'Are you sure you want to delete "John Doe"?',
      dialogType: DialogType.DELETE_EMPLOYEE,
      entityId: '456',
      entityName: 'John Doe'
    });

    render(
      <Provider store={store}>
        <ConfirmDialog />
      </Provider>
    );

    await userEvent.click(screen.getByRole('button', { name: /confirm/i }));

    expect(deleteEmployee).toHaveBeenCalledWith('456');
    expect(showNotification).toHaveBeenCalledWith({
      message: 'Employee "John Doe" was successfully deleted.',
      type: 'success'
    });
    expect(closeConfirmDialog).toHaveBeenCalled();
  });

  it('shows error notification when delete fails', async () => {
    // @ts-expect-error - Mock implementation doesn't match exact thunk action type
    vi.mocked(deleteCafe).mockImplementationOnce((id) => ({
      type: 'cafe/deleteCafe',
      payload: id,
      unwrap: vi.fn().mockRejectedValue(new Error('Failed to delete'))
    }));

    const store = createTestStore({
      open: true,
      title: 'Delete Cafe',
      message: 'Are you sure you want to delete "Test Cafe"?',
      dialogType: DialogType.DELETE_CAFE,
      entityId: '123',
      entityName: 'Test Cafe'
    });

    render(
      <Provider store={store}>
        <ConfirmDialog />
      </Provider>
    );

    await userEvent.click(screen.getByRole('button', { name: /confirm/i }));

    expect(showNotification).toHaveBeenCalledWith({
      message: 'Failed to delete cafe. Please try again.',
      type: 'error'
    });
    expect(closeConfirmDialog).toHaveBeenCalled();
  });
}); 