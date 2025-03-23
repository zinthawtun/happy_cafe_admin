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
  IconButton,
  Grid,
  Avatar,
  CircularProgress,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

import { RootState, useAppDispatch, useAppSelector } from '@store/index';
import { fetchCafes } from '@/store/slices/cafe-slice';
import { showConfirmDialog} from '@/store/slices/ui-slice';
import { getLogoUrl as getApiLogoUrl } from '@services/api-service';

import { DialogType, LogoCache } from '@/types';

const CafeOverViewPage = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { list: cafes, loading } = useAppSelector((state: RootState) => state.cafes);
  const [locationFilter, setLocationFilter] = useState('');
  const [logoCache, setLogoCache] = useState<LogoCache>({});
  const [loadingLogos, setLoadingLogos] = useState(false);

  const preloadLogos = useCallback(async () => {
    if (!cafes || cafes.length === 0) return;

    setLoadingLogos(true);
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
    } finally {
      setLoadingLogos(false);
    }
  }, [cafes]);

  const getLogoUrl = (logo: string | undefined) => {
    if (!logo) return undefined;
    return logoCache[logo] || undefined;
  };

  useEffect(() => {
    dispatch(fetchCafes(undefined));
  }, [dispatch]);

  useEffect(() => {
    if (cafes && cafes.length > 0) {
      preloadLogos();
    }
  }, [cafes, preloadLogos]);

  const handleLocationFilterChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setLocationFilter(event.target.value);
  };

  const handleFilterSubmit = () => {
    dispatch(fetchCafes(locationFilter.trim() || undefined)).then(() => {
        
      setLogoCache({});
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
        <Typography>Loading cafes...</Typography>
      ) : (
        <TableContainer component={Paper}>
          {loadingLogos && (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
              <CircularProgress size={24} sx={{ mr: 1 }} />
              <Typography>Loading logos...</Typography>
            </Box>
          )}
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Logo</TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Description</TableCell>
                <TableCell align="center">Employees</TableCell>
                <TableCell>Location</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {cafes.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                    <Typography variant="subtitle1">No cafes found.</Typography>
                  </TableCell>
                </TableRow>
              ) : (
                cafes.map((cafe) => (
                  <TableRow key={cafe.id} hover>
                    <TableCell>
                      {cafe.logo ? (
                        <Avatar
                          src={getLogoUrl(cafe.logo)}
                          alt={cafe.name}
                          sx={{ width: 40, height: 40 }}
                        />
                      ) : (
                        <Avatar sx={{ width: 40, height: 40 }}>{cafe.name[0]}</Avatar>
                      )}
                    </TableCell>
                    <TableCell>{cafe.name}</TableCell>
                    <TableCell>{cafe.description}</TableCell>
                    <TableCell align="center">
                      <Button onClick={() => handleViewEmployees(cafe.id)}>{cafe.employees}</Button>
                    </TableCell>
                    <TableCell>{cafe.location}</TableCell>
                    <TableCell align="center">
                      <IconButton
                        color="primary"
                        onClick={() => handleEditCafe(cafe.id)}
                        aria-label="edit"
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        color="error"
                        onClick={() => handleDeleteCafe(cafe.id, cafe.name)}
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

export default CafeOverViewPage; 