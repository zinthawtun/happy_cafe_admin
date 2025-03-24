import { describe, it, expect, vi, beforeAll } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import FormFileUpload from '../form-file-upload-component';

class MockFileReader {
  onload: ((this: FileReader, ev: ProgressEvent<FileReader>) => void) | null = null;
  readAsDataURL = vi.fn(function(this: MockFileReader) {
    setTimeout(() => {
      if (this.onload) {
        this.onload.call(this as unknown as FileReader, { 
          target: { result: 'data:image/jpeg;base64,mockbase64data' } 
        } as unknown as ProgressEvent<FileReader>);
      }
    }, 0);
  });
}

beforeAll(() => {
  // @ts-expect-error - we're intentionally mocking this
  window.FileReader = MockFileReader;
});

describe('FormFileUpload_Test', () => {
  it('renders with label correctly', () => {
    render(
      <FormFileUpload 
        name="testFile" 
        label="Test File Upload" 
        accept="image/*" 
        onChange={() => {}} 
      />
    );
    
    expect(screen.getByText('Test File Upload')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /upload/i })).toBeInTheDocument();
  });

  it('calls onChange with file when file is selected', () => {
    const handleChange = vi.fn();
    
    render(
      <FormFileUpload 
        name="testFile" 
        label="Test File Upload" 
        accept="image/*" 
        onChange={handleChange} 
      />
    );
    
    const file = new File(['file contents'], 'test.png', { type: 'image/png' });
    const input = document.getElementById('testFile') as HTMLInputElement;
    
    fireEvent.change(input, { target: { files: [file] } });
    
    expect(handleChange).toHaveBeenCalledWith(file);
  });

  it('calls onChange with null when file exceeds maxSize', () => {
    const handleChange = vi.fn();
    const maxSize = 1000; 
    
    render(
      <FormFileUpload 
        name="testFile" 
        label="Test File Upload" 
        accept="image/*" 
        maxSize={maxSize}
        onChange={handleChange} 
      />
    );
    
    const largeMockFile = new File(['a'.repeat(maxSize + 100)], 'large.png', { type: 'image/png' });
    Object.defineProperty(largeMockFile, 'size', { value: maxSize + 1 });
    
    const input = document.getElementById('testFile') as HTMLInputElement;
    fireEvent.change(input, { target: { files: [largeMockFile] } });
    
    expect(handleChange).toHaveBeenCalledWith(null);
  });

  it('displays error message when error is true', () => {
    render(
      <FormFileUpload 
        name="testFile" 
        label="Test File Upload" 
        accept="image/*" 
        onChange={() => {}} 
        error={true}
        helperText="This field is required"
      />
    );
    
    expect(screen.getByText('This field is required')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /upload/i })).toHaveClass('MuiButton-colorError');
  });

  it('displays preview image when provided', () => {
    render(
      <FormFileUpload 
        name="testFile" 
        label="Test File Upload" 
        accept="image/*" 
        onChange={() => {}} 
        previewUrl="data:image/jpeg;base64,mockpreviewdata"
      />
    );
    
    const previewImg = screen.getByAltText('Preview');
    expect(previewImg).toBeInTheDocument();
    expect(previewImg.getAttribute('src')).toBe('data:image/jpeg;base64,mockpreviewdata');
  });

  it.skip('displays preview after file selection', async () => {
    render(
      <FormFileUpload 
        name="testFile" 
        label="Test File Upload" 
        accept="image/*" 
        onChange={() => {}} 
      />
    );
    
    expect(screen.queryByAltText('Preview')).not.toBeInTheDocument();
    
    const file = new File(['file contents'], 'test.png', { type: 'image/png' });
    const input = document.getElementById('testFile') as HTMLInputElement;
    fireEvent.change(input, { target: { files: [file] } });
    
    await waitFor(() => {
      expect(screen.getByAltText('Preview')).toBeInTheDocument();
    });
    
    const previewImg = screen.getByAltText('Preview');
    expect(previewImg.getAttribute('src')).toBe('data:image/jpeg;base64,mockbase64data');
  });
}); 