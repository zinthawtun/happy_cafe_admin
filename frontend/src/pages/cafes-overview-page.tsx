import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  TextField,
  InputAdornment,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  IconButton,
  Grid,
  Avatar,
  CircularProgress,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import PeopleIcon from '@mui/icons-material/People';

import { RootState, useAppDispatch, useAppSelector } from '@store/index';
import { fetchCafes, setPage, setLimit } from '@/store/slices/cafe-slice';
import { showConfirmDialog} from '@/store/slices/ui-slice';
import { getLogoUrl as getApiLogoUrl } from '@services/api-service';

import { DialogType, LogoCache } from '@/types';

const CafeOverViewPage = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { list: cafes, loading, pagination } = useAppSelector((state: RootState) => state.cafes);
  const [locationFilter, setLocationFilter] = useState('');
  const [logoCache, setLogoCache] = useState<LogoCache>({});

  const preloadLogos = useCallback(async () => {
    if (!cafes || cafes.length === 0) return;

    const newCache: LogoCache = {};
    
    const logoFilenames = cafes
      .filter(cafe => cafe.logo)
      .map(cafe => cafe.logo as string);
    
    try {
      await Promise.all(
        logoFilenames.map(async (filename) => {
          try {
            const url = getApiLogoUrl(filename);
            
            const response = await fetch(url);
            if (response.ok) {
              const blob = await response.blob();
              const dataUrl = URL.createObjectURL(blob);
              newCache[filename] = dataUrl;
            } else {
              console.error('Failed to load logo:', filename, 'Status:', response.status);
            }
          } catch (error) {
            console.error(`Error loading logo ${filename}:`, error);
          }
        })
      );
      
      setLogoCache(newCache);
    } catch (error) {
      console.error('Error preloading logos:', error);
    }
  }, [cafes]);

  const getLogoUrl = (logo: string | undefined) => {
    if (!logo) return undefined;
    return logoCache[logo] || undefined;
  };

  const fetchCafesData = useCallback(() => {
    dispatch(fetchCafes({
      location: locationFilter.trim() || undefined, 
      page: pagination.page,
      limit: pagination.limit
    }));
  }, [dispatch, locationFilter, pagination.page, pagination.limit]);

  useEffect(() => {
    fetchCafesData();
  }, [fetchCafesData]);

  useEffect(() => {
    if (cafes && cafes.length > 0) {
      preloadLogos();
    }
  }, [cafes, preloadLogos]);

  const handleLocationFilterChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setLocationFilter(event.target.value);
  };

  const handleFilterSubmit = () => {
    dispatch(fetchCafes({
      location: locationFilter.trim() || undefined,
      page: 0,
      limit: pagination.limit
    })).then(() => {
      setLogoCache({});
      dispatch(setPage(0));
    });
  };

  const handleAddCafe = () => {
    navigate('/cafes/new');
  };

  const handleEditCafe = (id: string) => {
    navigate(`/cafes/edit/${id}`);
  };

  const handleViewEmployees = (cafeId: string) => {
    navigate(`/employees?cafe=${cafeId}`);
  };

  const handleDeleteCafe = (id: string, name: string) => {
    dispatch(
      showConfirmDialog({
        title: 'Delete Cafe',
        message: `Are you sure you want to delete "${name}"? This action cannot be undone.`,
        dialogType: DialogType.DELETE_CAFE,
        entityId: id,
        entityName: name,
      })
    );
  };

  const handleChangePage = (_event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    dispatch(setPage(newPage));
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    dispatch(setLimit(parseInt(event.target.value, 10)));
    dispatch(setPage(0));
  };

  return (
    <Box>
      <Grid container justifyContent="space-between" alignItems="center" spacing={2} sx={{ mb: 3 }}>
        <Grid item>
          <Typography variant="h4" component="h1" gutterBottom>
            Cafes
          </Typography>
        </Grid>
        <Grid item>
          <Button variant="contained" color="primary" onClick={handleAddCafe}>
            Add New Café
          </Button>
        </Grid>
      </Grid>

      <Grid container spacing={2} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={4}>
          <TextField
            fullWidth
            label="Filter by Location"
            variant="outlined"
            value={locationFilter}
            onChange={handleLocationFilterChange}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton onClick={handleFilterSubmit} edge="end">
                    <SearchIcon />
                  </IconButton>
                </InputAdornment>
              ),
            }}
            onKeyPress={(e) => {
              if (e.key === 'Enter') {
                handleFilterSubmit();
              }
            }}
          />
        </Grid>
      </Grid>

      {loading ? (
        <Box display="flex" justifyContent="center" my={4}>
          <CircularProgress />
        </Box>
      ) : cafes && cafes.length > 0 ? (
        <Paper 
          elevation={0} 
          sx={{ 
            width: '100%', 
            borderRadius: 2,
            overflow: 'hidden',
            boxShadow: '0 2px 10px rgba(0,0,0,0.08)',
          }}
        >
          <TableContainer sx={{ maxHeight: 'calc(100vh - 300px)' }}>
            <Table stickyHeader sx={{ minWidth: '100%' }}>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 'bold', bgcolor: 'background.paper' }}>Logo</TableCell>
                  <TableCell sx={{ fontWeight: 'bold', bgcolor: 'background.paper' }}>Name</TableCell>
                  <TableCell sx={{ fontWeight: 'bold', bgcolor: 'background.paper' }}>Description</TableCell>
                  <TableCell sx={{ fontWeight: 'bold', bgcolor: 'background.paper' }}>Location</TableCell>
                  <TableCell sx={{ fontWeight: 'bold', bgcolor: 'background.paper' }}>Employees</TableCell>
                  <TableCell sx={{ fontWeight: 'bold', bgcolor: 'background.paper' }} align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {cafes.map((cafe) => (
                  <TableRow key={cafe.id} hover>
                    <TableCell>
                    {cafe.logo ? (
                      <Avatar
                        src={getLogoUrl(cafe.logo)}
                        alt={cafe.name}
                        variant="rounded"
                        sx={{ width: 40, height: 40 }}
                      />
                    ) : (
                      <Avatar sx={{ width: 40, height: 40 }} variant="rounded">{cafe.name[0]}</Avatar>
                    )}
                    </TableCell>
                    <TableCell>{cafe.name}</TableCell>
                    <TableCell>{cafe.description}</TableCell>
                    <TableCell>{cafe.location}</TableCell>
                    <TableCell>{cafe.employees || 0}</TableCell>
                    <TableCell align="right">
                      <IconButton
                        color="primary"
                        onClick={() => handleViewEmployees(cafe.id)}
                        title="View Employees"
                      >
                        <PeopleIcon />
                      </IconButton>
                      <IconButton
                        color="primary"
                        onClick={() => handleEditCafe(cafe.id)}
                        title="Edit Cafe"
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        color="error"
                        onClick={() => handleDeleteCafe(cafe.id, cafe.name)}
                        title="Delete Cafe"
                      >
                        <DeleteIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          <TablePagination
            rowsPerPageOptions={[5, 10, 25]}
            component="div"
            count={pagination.total}
            rowsPerPage={pagination.limit}
            page={pagination.page}
            onPageChange={handleChangePage}
            onRowsPerPageChange={handleChangeRowsPerPage}
            labelDisplayedRows={({ from, to, count }) => (
              <Typography variant="body2" component="span">
                {from}-{to} of {count} (Page {pagination.page + 1})
              </Typography>
            )}
          />
        </Paper>
      ) : (
        <Paper
          sx={{
            p: 4,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            textAlign: 'center',
            borderRadius: 2,
          }}
        >
          <Typography variant="h6" gutterBottom>
            No cafes found
          </Typography>
          <Typography variant="body1" color="textSecondary" paragraph>
            {locationFilter
              ? 'No cafes found with the specified location filter.'
              : 'There are no cafes in the system yet. Add your first cafe!'}
          </Typography>
          <Button
            variant="contained"
            color="primary"
            onClick={handleAddCafe}
            sx={{ mt: 2 }}
          >
            Add New Café
          </Button>
        </Paper>
      )}
    </Box>
  );
};

export default CafeOverViewPage; 