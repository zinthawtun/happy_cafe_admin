import { useDispatch, useSelector } from "react-redux";
import { Snackbar, Alert } from "@mui/material";

import { RootState } from "@store/index";
import { closeNotification } from "@/store/slices/ui-slice";

const Notification = () => {
  const dispatch = useDispatch();
  const { open, message, type } = useSelector(
    (state: RootState) => state.ui.notifications
  );

  const handleClose = () => {
    dispatch(closeNotification());
  };

  return (
    <Snackbar
      open={open}
      autoHideDuration={6000}
      onClose={handleClose}
      anchorOrigin={{ vertical: "top", horizontal: "right" }}
    >
      <Alert onClose={handleClose} severity={type} sx={{ width: "100%" }}>
        {message}
      </Alert>
    </Snackbar>
  );
};

export default Notification;
