import { useState, useEffect, FormEvent, ChangeEvent } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Paper,
  Grid,
  Card,
  CardContent,
  FormControl,
  FormLabel,
  RadioGroup,
  FormControlLabel,
  Radio,
  FormHelperText,
  MenuItem,
  CircularProgress,
} from '@mui/material';

import { RootState, useAppDispatch, useAppSelector } from '@store/index';
import { fetchEmployeeById, createEmployee, updateEmployee } from '@/store/slices/employee-slice';
import { fetchCafes } from '@/store/slices/cafe-slice';
import { showNotification, setFormDirty } from '@/store/slices/ui-slice';

import FormTextField from '@components/form-text-field-component';

import { EmployeeFormData, EmployeeFormErrors } from '@types';

const AddEditEmployeePage = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { id } = useParams<{ id: string }>();
  const isEditing = Boolean(id);
  
  const { selectedEmployee, loading: employeeLoading } = useAppSelector(
    (state: RootState) => state.employees
  );
  const { list: cafes, loading: cafesLoading } = useAppSelector((state: RootState) => state.cafes);
  const isDirty = useAppSelector((state: RootState) => state.ui.isFormDirty);

  const [formData, setFormData] = useState<EmployeeFormData>({
    name: '',
    email: '',
    phoneNumber: '',
    gender: 'male',
    cafeId: '',
  });
  const [errors, setErrors] = useState<EmployeeFormErrors>({});
  const [formReady, setFormReady] = useState<boolean>(!isEditing);

  useEffect(() => {
    dispatch(fetchCafes(undefined));
    
    if (isEditing && id) {
      dispatch(fetchEmployeeById(id));
    }
  }, [dispatch, isEditing, id]);

  useEffect(() => {
    if (isEditing && selectedEmployee) {
      setFormData({
        name: selectedEmployee.name || '',
        email: selectedEmployee.email || '',
        phoneNumber: selectedEmployee.phoneNumber || '',
        gender: selectedEmployee.gender || 'male',
        cafeId: '',
      });
      setFormReady(true);
    }
  }, [isEditing, selectedEmployee]);

  useEffect(() => {
    if (isEditing && selectedEmployee && cafes.length > 0) {
      const matchingCafe = cafes.find(cafe => cafe.name === selectedEmployee.cafe);
      
      if (matchingCafe) {
        setFormData(prevData => ({
          ...prevData,
          cafeId: matchingCafe.id
        }));
      }
    }
  }, [isEditing, selectedEmployee, cafes]);

  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (isDirty) {
        e.preventDefault();
        (e as unknown as { returnValue: string }).returnValue = '';
        return '';
      }
    };

    window.addEventListener('beforeunload', handleBeforeUnload);
    return () => {
      window.removeEventListener('beforeunload', handleBeforeUnload);
      dispatch(setFormDirty(false));
    };
  }, [isDirty, dispatch]);

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    
    if (errors[name as keyof EmployeeFormErrors]) {
      setErrors({ ...errors, [name]: undefined });
    }
    
    dispatch(setFormDirty(true));
  };

  const handleGenderChange = (e: ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, gender: e.target.value as 'male' | 'female' });
    dispatch(setFormDirty(true));
  };

  const validateForm = (): boolean => {
    const newErrors: EmployeeFormErrors = {};
    
    if (!formData.name.trim()) {
      newErrors.name = 'Name is required';
    } else if (formData.name.length < 6) {
      newErrors.name = 'Name must be at least 6 characters';
    } else if (formData.name.length > 10) {
      newErrors.name = 'Name must be at most 10 characters';
    }
    
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    }
    
    if (!formData.phoneNumber.trim()) {
      newErrors.phoneNumber = 'Phone number is required';
    } else if (!/^[89]\d{7}$/.test(formData.phoneNumber)) {
      newErrors.phoneNumber = 'Phone number must start with 8 or 9 and have 8 digits';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }
    
    try {
      if (isEditing && id) {
        await dispatch(updateEmployee({ id, data: formData })).unwrap();
        dispatch(showNotification({ message: 'Employee updated successfully', type: 'success' }));
      } else {
        await dispatch(createEmployee(formData)).unwrap();
        dispatch(showNotification({ message: 'Employee created successfully', type: 'success' }));
      }
      
      dispatch(setFormDirty(false));
      navigate('/employees');
    } catch {
      dispatch(
        showNotification({
          message: `Failed to ${isEditing ? 'update' : 'create'} employee. Please try again.`,
          type: 'error',
        })
      );
    }
  };

  const handleCancel = () => {
    if (isDirty) {
      if (window.confirm('You have unsaved changes. Are you sure you want to leave?')) {
        dispatch(setFormDirty(false));
        navigate('/employees');
      }
    } else {
      navigate('/employees');
    }
  };

  if ((employeeLoading && isEditing) || cafesLoading || !formReady) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        {isEditing ? 'Edit Employee' : 'Add New Employee'}
      </Typography>
      
      <Card component={Paper} elevation={2}>
        <CardContent>
          <Box component="form" onSubmit={handleSubmit} noValidate>
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}>
                <FormTextField
                  required
                  name="name"
                  label="Employee Name"
                  value={formData.name}
                  onChange={handleInputChange}
                  error={!!errors.name}
                  helperText={errors.name || 'Between 6-10 characters'}
                  minLength={6}
                  maxLength={10}
                />
              </Grid>
              
              <Grid item xs={12} md={6}>
                <FormTextField
                  required
                  name="phoneNumber"
                  label="Phone Number"
                  value={formData.phoneNumber}
                  onChange={handleInputChange}
                  error={!!errors.phoneNumber}
                  helperText={errors.phoneNumber || 'Must start with 8 or 9 and have 8 digits'}
                  inputProps={{ maxLength: 8 }}
                />
              </Grid>
              
              <Grid item xs={12}>
                <FormTextField
                  required
                  name="email"
                  label="Email Address"
                  type="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  error={!!errors.email}
                  helperText={errors.email}
                />
              </Grid>
              
              <Grid item xs={12}>
                <FormControl component="fieldset" margin="normal">
                  <FormLabel component="legend">Gender</FormLabel>
                  <RadioGroup
                    row
                    name="gender"
                    value={formData.gender}
                    onChange={handleGenderChange}
                  >
                    <FormControlLabel value="male" control={<Radio />} label="Male" />
                    <FormControlLabel value="female" control={<Radio />} label="Female" />
                  </RadioGroup>
                </FormControl>
              </Grid>
              
              <Grid item xs={12}>
                <FormControl fullWidth margin="normal" error={!!errors.cafeId}>
                  <FormTextField
                    select
                    name="cafeId"
                    label="Assigned Cafe"
                    value={formData.cafeId}
                    onChange={handleInputChange}
                    error={!!errors.cafeId}
                    helperText={errors.cafeId}
                  >
                    <MenuItem value="">
                      <em>Not Assigned</em>
                    </MenuItem>
                    {cafes.map((cafe) => (
                      <MenuItem key={cafe.id} value={cafe.id}>
                        {cafe.name}
                      </MenuItem>
                    ))}
                  </FormTextField>
                  {errors.cafeId && <FormHelperText>{errors.cafeId}</FormHelperText>}
                </FormControl>
              </Grid>
              
              <Grid item xs={12}>
                <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2, mt: 2 }}>
                  <Button variant="outlined" onClick={handleCancel}>
                    Cancel
                  </Button>
                  <Button type="submit" variant="contained" color="primary">
                    {isEditing ? 'Update Employee' : 'Create Employee'}
                  </Button>
                </Box>
              </Grid>
            </Grid>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
};

export default AddEditEmployeePage; 