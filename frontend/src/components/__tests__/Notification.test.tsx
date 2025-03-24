import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';
import userEvent from '@testing-library/user-event';

import Notification from '../notification-component';
import uiReducer, { closeNotification } from '@/store/slices/ui-slice';

vi.mock('@/store/slices/ui-slice', async () => {
  const actual = await vi.importActual('@/store/slices/ui-slice');
  return {
    ...actual,
    closeNotification: vi.fn(() => ({ type: 'ui/closeNotification' }))
  };
});

describe('Notification_Test', () => {
  const createTestStore = (initialState: { 
    open?: boolean; 
    message?: string; 
    type?: 'success' | 'error' | 'info' | 'warning' 
  } = {}) => {
    return configureStore({
      reducer: {
        ui: uiReducer
      },
      preloadedState: {
        ui: {
          notifications: {
            open: false,
            message: '',
            type: 'info' as const,
            ...initialState
          },
          confirmDialog: {
            open: false,
            title: '',
            message: '',
            dialogType: null,
            entityId: null,
            entityName: null
          },
          isFormDirty: false
        }
      }
    });
  };

  it('renders correctly when notification is open', () => {
    const store = createTestStore({
      open: true,
      message: 'Test notification',
      type: 'success'
    });

    render(
      <Provider store={store}>
        <Notification />
      </Provider>
    );

    expect(screen.getByText('Test notification')).toBeInTheDocument();
  });

  it('does not render notification when closed', () => {
    const store = createTestStore({
      open: false,
      message: 'Test notification',
      type: 'success'
    });

    render(
      <Provider store={store}>
        <Notification />
      </Provider>
    );

    expect(screen.queryByText('Test notification')).not.toBeInTheDocument();
  });

  it('dispatches closeNotification when closed', async () => {
    const store = createTestStore({
      open: true,
      message: 'Test notification',
      type: 'success'
    });

    const dispatchSpy = vi.spyOn(store, 'dispatch');

    render(
      <Provider store={store}>
        <Notification />
      </Provider>
    );

    const closeButton = screen.getByRole('button', { name: /close/i });
    await userEvent.click(closeButton);

    expect(closeNotification).toHaveBeenCalled();
    expect(dispatchSpy).toHaveBeenCalledWith({ type: 'ui/closeNotification' });
  });
}); 