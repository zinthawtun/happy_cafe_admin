import React, { useState } from 'react';
import axios from 'axios';
import { Button, Card, Container, Typography, Box, Alert, TextField, CircularProgress, Snackbar } from '@mui/material';

const API_BASE_URL = import.meta.env.VITE_BACKEND_API_URL || "http://localhost:5222";

const DevSettings: React.FC = () => {
  const [apiKey, setApiKey] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [currentKey, setCurrentKey] = useState(import.meta.env.VITE_API_KEY || 'No API key found');

  const generateApiKey = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      const response = await axios.get(`${API_BASE_URL}/api/dev/key/generate`);
      setApiKey(response.data.apiKey);
      setSuccess(true);
      
      setCurrentKey(response.data.apiKey);
      
      setTimeout(() => {
        setSuccess(false);
      }, 5000);
    } catch (err) {
      if (axios.isAxiosError(err) && err.response) {
        setError(`Error: ${err.response.data.message || err.message}`);
      } else {
        setError('Failed to generate API key. This feature only works in development environment.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        Development Settings
      </Typography>
      
      <Card sx={{ p: 3, mb: 4 }}>
        <Typography variant="h6" gutterBottom>
          API Key Management
        </Typography>
        
        <Box sx={{ mb: 2 }}>
          <Typography variant="body1" gutterBottom>
            Current API Key:
          </Typography>
          <TextField 
            fullWidth
            variant="outlined"
            value={currentKey}
            InputProps={{
              readOnly: true,
            }}
          />
        </Box>
        
        <Typography variant="body2" color="text.secondary" paragraph>
          Generate a new API key for development. This will update both the frontend and backend environment files.
        </Typography>
        
        <Button 
          variant="contained" 
          color="primary" 
          onClick={generateApiKey}
          disabled={isLoading}
          startIcon={isLoading ? <CircularProgress size={20} color="inherit" /> : null}
        >
          Generate New API Key
        </Button>
        
        {apiKey && (
          <Box sx={{ mt: 2 }}>
            <Alert severity="success">
              <Typography variant="body1">
                API Key generated successfully:
              </Typography>
              <Typography variant="body2" component="pre" sx={{ mt: 1, p: 1, bgcolor: 'background.paper', borderRadius: 1 }}>
                {apiKey}
              </Typography>
              <Typography variant="body2" sx={{ mt: 1 }}>
                The key has been saved to the environment files. Restart your application to apply the changes.
              </Typography>
            </Alert>
          </Box>
        )}
        
        {error && (
          <Box sx={{ mt: 2 }}>
            <Alert severity="error">{error}</Alert>
          </Box>
        )}
      </Card>
      
      <Snackbar
        open={success}
        autoHideDuration={5000}
        onClose={() => setSuccess(false)}
        message="API key generated and saved successfully"
      />
    </Container>
  );
};

export default DevSettings; 