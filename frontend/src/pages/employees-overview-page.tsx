import { useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Grid,
  Chip,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

import { RootState, useAppDispatch, useAppSelector } from '@store/index';
import { fetchEmployees } from '@/store/slices/employee-slice';
import { showConfirmDialog } from '@/store/slices/ui-slice';

import { DialogType } from '@/types';

const Employees = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [searchParams] = useSearchParams();
  const cafeId = searchParams.get('cafe');
  
  const { list: employees, loading } = useAppSelector((state: RootState) => state.employees);

  useEffect(() => {
    dispatch(fetchEmployees(cafeId || undefined));
  }, [dispatch, cafeId]);

  const handleAddEmployee = () => {
    navigate('/employees/new');
  };

  const handleEditEmployee = (id: string) => {
    navigate(`/employees/edit/${id}`);
  };

  const handleDeleteEmployee = (id: string, name: string) => {
    dispatch(
      showConfirmDialog({
        title: 'Delete Employee',
        message: `Are you sure you want to delete "${name}"? This action cannot be undone.`,
        dialogType: DialogType.DELETE_EMPLOYEE,
        entityId: id,
        entityName: name,
      })
    );
  };

  return (
    <Box>
      <Grid container justifyContent="space-between" alignItems="center" spacing={2} sx={{ mb: 3 }}>
        <Grid item>
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            {cafeId && (
              <IconButton
                color="primary"
                onClick={() => navigate('/cafes')}
                sx={{ mr: 1 }}
                aria-label="back to cafes"
              >
                <ArrowBackIcon />
              </IconButton>
            )}
            <Typography variant="h4" component="h1" gutterBottom>
              {cafeId ? 'Cafe Employees' : 'All Employees'}
            </Typography>
          </Box>
        </Grid>
        <Grid item>
          <Button variant="contained" color="primary" onClick={handleAddEmployee}>
            Add New Employee
          </Button>
        </Grid>
      </Grid>

      {loading ? (
        <Typography>Loading employees...</Typography>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>ID</TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Phone</TableCell>
                <TableCell align="center">Days Worked</TableCell>
                <TableCell>Café</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {employees.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 4 }}>
                    <Typography variant="subtitle1">No employees found.</Typography>
                  </TableCell>
                </TableRow>
              ) : (
                employees.map((employee) => (
                  <TableRow key={employee.id} hover>
                    <TableCell>{employee.id}</TableCell>
                    <TableCell>{employee.name}</TableCell>
                    <TableCell>{employee.email}</TableCell>
                    <TableCell>{employee.phoneNumber}</TableCell>
                    <TableCell align="center">{employee.days_worked}</TableCell>
                    <TableCell>
                      {employee.cafe ? (
                        employee.cafe
                      ) : (
                        <Chip label="Not Assigned" size="small" color="warning" />
                      )}
                    </TableCell>
                    <TableCell align="center">
                      <IconButton
                        color="primary"
                        onClick={() => handleEditEmployee(employee.id)}
                        aria-label="edit"
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        color="error"
                        onClick={() => handleDeleteEmployee(employee.id, employee.name)}
                        aria-label="delete"
                      >
                        <DeleteIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
};

export default Employees; 