import { ChangeEvent, useState } from 'react';

import { Button, Typography, Box, FormHelperText } from '@mui/material';
import UploadFileIcon from '@mui/icons-material/UploadFile';

import { FormFileUploadProps } from '@/types';

const FormFileUpload = ({
  name,
  label,
  accept,
  maxSize = 2 * 1024 * 1024, 
  onChange,
  error = false,
  helperText = '',
  previewUrl,
}: FormFileUploadProps) => {
  const [preview, setPreview] = useState<string | undefined>(previewUrl);

  const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] || null;
    
    if (file) {

      if (file.size > maxSize) {
        onChange(null);
        return;
      }

      const fileReader = new FileReader();
      fileReader.onload = () => {
        setPreview(fileReader.result as string);
      };
      fileReader.readAsDataURL(file);
    } else {
      setPreview(undefined);
    }

    onChange(file);
  };

  return (
    <Box sx={{ mt: 2, mb: 2 }}>
      <Typography variant="subtitle1" gutterBottom>
        {label}
      </Typography>
      <input
        accept={accept}
        style={{ display: 'none' }}
        id={name}
        type="file"
        onChange={handleFileChange}
      />
      <label htmlFor={name}>
        <Button
          variant="contained"
          component="span"
          startIcon={<UploadFileIcon />}
          color={error ? 'error' : 'primary'}
        >
          Upload
        </Button>
      </label>
      {error && <FormHelperText error>{helperText}</FormHelperText>}
      
      {preview && (
        <Box sx={{ mt: 2, maxWidth: '200px' }}>
          <img src={preview} alt="Preview" style={{ maxWidth: '100%' }} />
        </Box>
      )}
    </Box>
  );
};

export default FormFileUpload; 