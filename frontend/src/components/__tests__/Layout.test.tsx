import { vi, describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { Provider } from 'react-redux';
import Layout from '../layout-component';
import * as reactRouterDom from 'react-router-dom';
import { store } from '../../store';

vi.mock('../../assets/smile-face.svg', () => 'mocked-smile-face-path');

describe('Layout_Test', () => {
  it.skip('renders the header and sidebar correctly', () => {
    render(
      <BrowserRouter>
        <Provider store={store}>
          <Layout />
        </Provider>
      </BrowserRouter>
    );

    expect(screen.getByText('Happy Cafe Admin')).toBeInTheDocument();
    
    expect(screen.getByLabelText('open drawer')).toBeInTheDocument();
    
    expect(screen.getAllByText('Dashboard')[0]).toBeInTheDocument();
    expect(screen.getAllByText('Cafes')[0]).toBeInTheDocument();
    expect(screen.getAllByText('Employees')[0]).toBeInTheDocument();
  });

  it.skip('toggles mobile drawer when menu button is clicked', () => {
    render(
      <BrowserRouter>
        <Provider store={store}>
          <Layout />
        </Provider>
      </BrowserRouter>
    );
    
    const menuButton = screen.getByLabelText('open drawer');
    
    expect(screen.queryByRole('presentation')).not.toBeInTheDocument();
    
    menuButton.click();
    
    expect(screen.getByRole('presentation')).toBeInTheDocument();
    
    menuButton.click();
    
    expect(screen.queryByRole('presentation')).not.toBeInTheDocument();
  });

  it.skip('highlights the active route in the navigation', () => {
    vi.spyOn(reactRouterDom, 'useLocation').mockReturnValue({
      pathname: '/cafes',
      search: '',
      hash: '',
      state: null,
      key: 'default',
    });

    render(
      <BrowserRouter>
        <Provider store={store}>
          <Layout />
        </Provider>
      </BrowserRouter>
    );

    const cafeNavItem = screen.getAllByText('Cafes')[0].closest('li')?.querySelector('.Mui-selected');
    expect(cafeNavItem).not.toBeNull();
  });

  it.skip('renders all navigation items with correct links', () => {
    render(
      <BrowserRouter>
        <Provider store={store}>
          <Layout />
        </Provider>
      </BrowserRouter>
    );

    const dashboardLink = screen.getAllByText('Dashboard')[0].closest('a');
    expect(dashboardLink).toHaveAttribute('href', '/');

    const cafesLink = screen.getAllByText('Cafes')[0].closest('a');
    expect(cafesLink).toHaveAttribute('href', '/cafes');

    const employeesLink = screen.getAllByText('Employees')[0].closest('a');
    expect(employeesLink).toHaveAttribute('href', '/employees');
  });
}); 