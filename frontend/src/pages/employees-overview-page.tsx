import { useEffect, useCallback } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
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
  TablePagination,
  IconButton,
  Grid,
  Chip,
  CircularProgress,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";

import { RootState, useAppDispatch, useAppSelector } from "@store/index";
import {
  fetchEmployees,
  setPage,
  setLimit,
} from "@/store/slices/employee-slice";
import { showConfirmDialog } from "@/store/slices/ui-slice";

import { DialogType } from "@/types";

const Employees = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [searchParams] = useSearchParams();
  const cafeId = searchParams.get("cafe");

  const {
    list: employees,
    loading,
    pagination,
  } = useAppSelector((state: RootState) => state.employees);

  const fetchEmployeesData = useCallback(() => {
    dispatch(
      fetchEmployees({
        cafe: cafeId || undefined,
        page: pagination.page,
        limit: pagination.limit,
      })
    );
  }, [dispatch, cafeId, pagination.page, pagination.limit]);

  useEffect(() => {
    fetchEmployeesData();
  }, [fetchEmployeesData]);

  const handleAddEmployee = () => {
    navigate("/employees/new");
  };

  const handleEditEmployee = (id: string) => {
    navigate(`/employees/edit/${id}`);
  };

  const handleDeleteEmployee = (id: string, name: string) => {
    dispatch(
      showConfirmDialog({
        title: "Delete Employee",
        message: `Are you sure you want to delete "${name}"? This action cannot be undone.`,
        dialogType: DialogType.DELETE_EMPLOYEE,
        entityId: id,
        entityName: name,
      })
    );
  };

  const handleChangePage = (
    _event: React.MouseEvent<HTMLButtonElement> | null,
    newPage: number
  ) => {
    dispatch(setPage(newPage));
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    dispatch(setLimit(parseInt(event.target.value, 10)));
    dispatch(setPage(0));
  };

  return (
    <Box>
      <Grid
        container
        justifyContent="space-between"
        alignItems="center"
        spacing={2}
        sx={{ mb: 3 }}
      >
        <Grid item>
          <Box sx={{ display: "flex", alignItems: "center" }}>
            {cafeId && (
              <IconButton
                color="primary"
                onClick={() => navigate("/cafes")}
                sx={{ mr: 1 }}
                aria-label="back to cafes"
              >
                <ArrowBackIcon />
              </IconButton>
            )}
            <Typography variant="h4" component="h1" gutterBottom>
              {cafeId ? "Cafe Employees" : "All Employees"}
            </Typography>
          </Box>
        </Grid>
        <Grid item>
          <Button
            variant="contained"
            color="primary"
            onClick={handleAddEmployee}
          >
            Add New Employee
          </Button>
        </Grid>
      </Grid>

      {loading ? (
        <Box display="flex" justifyContent="center" my={4}>
          <CircularProgress />
        </Box>
      ) : employees && employees.length > 0 ? (
        <Paper
          elevation={0}
          sx={{
            width: "100%",
            borderRadius: 2,
            overflow: "hidden",
            boxShadow: "0 2px 10px rgba(0,0,0,0.08)",
          }}
        >
          <TableContainer sx={{ maxHeight: "calc(100vh - 300px)" }}>
            <Table stickyHeader sx={{ minWidth: "100%" }}>
              <TableHead>
                <TableRow>
                  <TableCell
                    sx={{ fontWeight: "bold", bgcolor: "background.paper" }}
                  >
                    ID
                  </TableCell>
                  <TableCell
                    sx={{ fontWeight: "bold", bgcolor: "background.paper" }}
                  >
                    Name
                  </TableCell>
                  <TableCell
                    sx={{ fontWeight: "bold", bgcolor: "background.paper" }}
                  >
                    Email
                  </TableCell>
                  <TableCell
                    sx={{ fontWeight: "bold", bgcolor: "background.paper" }}
                  >
                    Phone
                  </TableCell>
                  <TableCell
                    sx={{ fontWeight: "bold", bgcolor: "background.paper" }}
                  >
                    Days Worked
                  </TableCell>
                  <TableCell
                    sx={{ fontWeight: "bold", bgcolor: "background.paper" }}
                  >
                    Café
                  </TableCell>
                  <TableCell
                    sx={{ fontWeight: "bold", bgcolor: "background.paper" }}
                    align="right"
                  >
                    Actions
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {employees.map((employee) => (
                  <TableRow key={employee.id} hover>
                    <TableCell>{employee.id}</TableCell>
                    <TableCell>{employee.name}</TableCell>
                    <TableCell>{employee.email}</TableCell>
                    <TableCell>{employee.phoneNumber}</TableCell>
                    <TableCell>{employee.days_worked}</TableCell>
                    <TableCell>
                      {employee.cafe ? (
                        employee.cafe
                      ) : (
                        <Chip
                          label="Not Assigned"
                          size="small"
                          color="warning"
                        />
                      )}
                    </TableCell>
                    <TableCell align="right">
                      <IconButton
                        color="primary"
                        onClick={() => handleEditEmployee(employee.id)}
                        title="Edit Employee"
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        color="error"
                        onClick={() =>
                          handleDeleteEmployee(employee.id, employee.name)
                        }
                        title="Delete Employee"
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
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            textAlign: "center",
            borderRadius: 2,
          }}
        >
          <Typography variant="h6" gutterBottom>
            No employees found
          </Typography>
          <Typography variant="body1" color="textSecondary" paragraph>
            {cafeId
              ? "No employees assigned to this cafe yet."
              : "There are no employees in the system yet. Add your first employee!"}
          </Typography>
          <Button
            variant="contained"
            color="primary"
            onClick={handleAddEmployee}
            sx={{ mt: 2 }}
          >
            Add New Employee
          </Button>
        </Paper>
      )}
    </Box>
  );
};

export default Employees;
