import {
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Button,
} from '@mui/material';
import { useSelector } from 'react-redux';
import { RootState, useAppDispatch } from '@store/index';

import { closeConfirmDialog, showNotification } from '@/store/slices/ui-slice';
import { deleteCafe } from '@/store/slices/cafe-slice';
import { deleteEmployee } from '@/store/slices/employee-slice';

import { DialogType } from '@/types';

const ConfirmDialog = () => {
  const dispatch = useAppDispatch();
  const { open, title, message, dialogType, entityId, entityName } = useSelector(
    (state: RootState) => state.ui.confirmDialog
  );

  const handleClose = () => {
    dispatch(closeConfirmDialog());
  };

  const handleConfirm = async () => {
    if (!dialogType || !entityId) {
      handleClose();
      return;
    }

    try {
      switch (dialogType) {
        case DialogType.DELETE_CAFE:
          await dispatch(deleteCafe(entityId)).unwrap();
          dispatch(
            showNotification({
              message: `Cafe "${entityName}" was successfully deleted.`,
              type: 'success',
            })
          );
          break;

        case DialogType.DELETE_EMPLOYEE:
          await dispatch(deleteEmployee(entityId)).unwrap();
          dispatch(
            showNotification({
              message: `Employee "${entityName}" was successfully deleted.`,
              type: 'success',
            })
          );
          break;

        default:
          console.warn('Unknown dialog type:', dialogType);
      }
    } catch {
      dispatch(
        showNotification({
          message: `Failed to delete ${dialogType === DialogType.DELETE_CAFE ? 'cafe' : 'employee'}. Please try again.`,
          type: 'error',
        })
      );
    } finally {
      handleClose();
    }
  };

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">{title}</DialogTitle>
      <DialogContent>
        <DialogContentText id="alert-dialog-description">{message}</DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} color="primary">
          Cancel
        </Button>
        <Button onClick={handleConfirm} color="primary" variant="contained" autoFocus>
          Confirm
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default ConfirmDialog; 