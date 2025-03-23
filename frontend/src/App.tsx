import { lazy, Suspense } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, CssBaseline } from '@mui/material';
import { Provider } from 'react-redux';
import { store } from '@store/index';
import theme from '@styles/theme';
import './App.css';

const CafesPage = lazy(() => import('@pages/cafes-overview-page'));
const EmployeesPage = lazy(() => import('@pages/employees-overview-page'));
const AddEditCafePage = lazy(() => import('@/pages/add-edit-cafe-page'));
const AddEditEmployeePage = lazy(() => import('@/pages/add-edit-employee-page'));
const NotFoundPage = lazy(() => import('@pages/not-found-page'));

const Notification = lazy(() => import('@/components/notification-component'));
const ConfirmDialog = lazy(() => import('./components/confirm-dialog-component'));
const Layout = lazy(() => import('@components/layout-component'));

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Suspense fallback={<div>Loading...</div>}>
          <Router>
            <Notification />
            <ConfirmDialog />
            <Routes>
              <Route path="/" element={<Layout />}>
                <Route index element={<Navigate to="/cafes" replace />} />
                <Route path="cafes" element={<CafesPage />} />
                <Route path="cafes/new" element={<AddEditCafePage />} />
                <Route path="cafes/edit/:id" element={<AddEditCafePage />} />
                <Route path="employees" element={<EmployeesPage />} />
                <Route path="employees/new" element={<AddEditEmployeePage />} />
                <Route path="employees/edit/:id" element={<AddEditEmployeePage />} />
                <Route path="*" element={<NotFoundPage />} />
              </Route>
            </Routes>
          </Router>
        </Suspense>
      </ThemeProvider>
    </Provider>
  );
}

export default App;
