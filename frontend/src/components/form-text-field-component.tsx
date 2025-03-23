import { TextField } from '@mui/material';
import { FormTextFieldProps } from '@types';

const FormTextField = ({
  name,
  label,
  value,
  onChange,
  onBlur,
  error = false,
  helperText = '',
  maxLength,
  minLength,
  ...props
}: FormTextFieldProps) => {
  return (
    <TextField
      fullWidth
      margin="normal"
      name={name}
      label={label}
      value={value}
      onChange={onChange}
      onBlur={onBlur}
      error={error}
      helperText={helperText}
      inputProps={{
        maxLength,
        minLength,
      }}
      {...props}
    />
  );
};

export default FormTextField; 