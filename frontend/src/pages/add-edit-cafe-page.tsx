import { useState, useEffect, FormEvent, ChangeEvent } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  Paper,
  Grid,
  Card,
  CardContent,
  FormHelperText,
  Avatar,
  CircularProgress,
} from "@mui/material";

import { RootState, useAppDispatch, useAppSelector } from "@store/index";
import {
  fetchCafeById,
  createCafe,
  updateCafe,
} from "@/store/slices/cafe-slice";
import { showNotification, setFormDirty } from "@/store/slices/ui-slice";
import {
  getLogoUrl,
  getFileUploadUrl,
  deleteLogoFile,
} from "@services/api-service";

import FormTextField from "@components/form-text-field-component";

import { CafeFormData, CafeFormErrors } from "@types";

const AddEditCafePage = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { id } = useParams<{ id: string }>();
  const isEditing = Boolean(id);

  const { selectedCafe, loading } = useAppSelector(
    (state: RootState) => state.cafes
  );
  const isDirty = useAppSelector((state: RootState) => state.ui.isFormDirty);

  const [formData, setFormData] = useState<CafeFormData>({
    name: "",
    description: "",
    location: "",
    logo: null,
  });
  const [errors, setErrors] = useState<CafeFormErrors>({});
  const [logoPreview, setLogoPreview] = useState<string | null>(null);
  const [isUploading, setIsUploading] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (isEditing && id) {
      dispatch(fetchCafeById(id));
    }
  }, [dispatch, isEditing, id]);

  useEffect(() => {
    if (isEditing && selectedCafe) {
      setFormData({
        name: selectedCafe.name,
        description: selectedCafe.description || "",
        location: selectedCafe.location,
        logoFileName: selectedCafe.logo,
      });

      if (selectedCafe.logo) {
        try {
          setLogoPreview(getLogoUrl(selectedCafe.logo));
        } catch (error) {
          console.error("Error creating logo URL:", error);
          setLogoPreview(null);
        }
      }
    }
  }, [isEditing, selectedCafe]);

  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (isDirty) {
        e.preventDefault();
        (e as unknown as { returnValue: string }).returnValue = "";
        return "";
      }
    };

    window.addEventListener("beforeunload", handleBeforeUnload);
    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
      dispatch(setFormDirty(false));
    };
  }, [isDirty, dispatch]);

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });

    if (errors[name as keyof CafeFormErrors]) {
      setErrors({ ...errors, [name]: undefined });
    }

    dispatch(setFormDirty(true));
  };

  const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files || e.target.files.length === 0) {
      return;
    }

    const file = e.target.files[0];

    if (!file.type.match("image.*")) {
      setErrors({ ...errors, logo: "Please upload an image file" });
      return;
    }

    if (file.size > 2 * 1024 * 1024) {
      setErrors({ ...errors, logo: "File size should not exceed 2MB" });
      return;
    }

    const fileExtension = file.name.split(".").pop();
    const temporaryFileName = `temp_${Date.now()}_${Math.random()
      .toString(36)
      .substring(2, 9)}.${fileExtension}`;

    const localPreview = URL.createObjectURL(file);
    setLogoPreview(localPreview);

    setFormData({
      ...formData,
      logo: file,
      logoFileName: temporaryFileName,
    });

    if (errors.logo) {
      setErrors({ ...errors, logo: undefined });
    }

    dispatch(setFormDirty(true));
  };

  const validateForm = (): boolean => {
    const newErrors: CafeFormErrors = {};

    if (!formData.name.trim()) {
      newErrors.name = "Name is required";
    } else if (formData.name.length < 6) {
      newErrors.name = "Name must be at least 6 characters";
    } else if (formData.name.length > 10) {
      newErrors.name = "Name must be at most 10 characters";
    }

    if (!formData.description.trim()) {
      newErrors.description = "Description is required";
    } else if (formData.description.length > 256) {
      newErrors.description = "Description must be at most 256 characters";
    }

    if (!formData.location.trim()) {
      newErrors.location = "Location is required";
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
      setSubmitting(true);

      const cafeFormData = new FormData();
      cafeFormData.append("name", formData.name);
      cafeFormData.append("description", formData.description);
      cafeFormData.append("location", formData.location);

      if (isEditing && id) {
        cafeFormData.append("id", id);
      }

      if (formData.logoFileName) {
        cafeFormData.append("logo", formData.logoFileName);
      }

      let savedCafeData;

      if (isEditing && id) {
        const oldLogoFileName = selectedCafe?.logo;
        const isLogoChanged =
          formData.logo && oldLogoFileName && oldLogoFileName !== "";

        savedCafeData = await dispatch(
          updateCafe({ formData: cafeFormData })
        ).unwrap();

        if (savedCafeData && isLogoChanged && oldLogoFileName) {
          setTimeout(async () => {
            try {
              await deleteLogoFile(oldLogoFileName);
            } catch (error) {
              console.error("Warning: Failed to delete old logo file:", error);
            }
          }, 1000);
        }
      } else {
        savedCafeData = await dispatch(createCafe(cafeFormData)).unwrap();
      }

      if (formData.logo && formData.logoFileName && savedCafeData) {
        try {
          setIsUploading(true);

          const uploadFormData = new FormData();
          uploadFormData.append("file", formData.logo);

          const uploadUrl = getFileUploadUrl();

          const response = await fetch(uploadUrl, {
            method: "POST",
            body: uploadFormData,
          });

          if (!response.ok) {
            const errorText = await response.text();
            console.error("Upload failed:", response.status, errorText);
            throw new Error(
              `Failed to upload logo: ${response.status} ${errorText}`
            );
          }

          const data = await response.json();

          if (data.fileName !== formData.logoFileName) {
            const updateLogoFormData = new FormData();
            updateLogoFormData.append("id", savedCafeData.id);
            updateLogoFormData.append("name", savedCafeData.name);
            updateLogoFormData.append("description", savedCafeData.description);
            updateLogoFormData.append("location", savedCafeData.location);
            updateLogoFormData.append("logo", data.fileName);

            const oldLogoFileName = isEditing && selectedCafe?.logo;

            await dispatch(
              updateCafe({
                formData: updateLogoFormData,
              })
            ).unwrap();

            if (
              isEditing &&
              oldLogoFileName &&
              !oldLogoFileName.startsWith("temp_")
            ) {
              setTimeout(async () => {
                try {
                  await deleteLogoFile(oldLogoFileName);
                } catch (error) {
                  console.error(
                    "Warning: Failed to delete old logo file:",
                    error
                  );
                }
              }, 1000);
            }
          }
        } catch (error) {
          console.error(
            "Warning: Logo upload failed after cafe was saved:",
            error
          );
          dispatch(
            showNotification({
              message: "Cafe was saved but logo upload failed",
              type: "warning",
            })
          );
        }
      }

      dispatch(
        showNotification({
          message: `Cafe ${isEditing ? "updated" : "created"} successfully`,
          type: "success",
        })
      );

      dispatch(setFormDirty(false));
      navigate("/cafes");
    } catch (error) {
      console.error("Error submitting form:", error);
      dispatch(
        showNotification({
          message: "Failed to submit form. Please try again.",
          type: "error",
        })
      );
    } finally {
      setSubmitting(false);
      setIsUploading(false);
    }
  };

  const handleCancel = () => {
    if (isDirty) {
      if (
        window.confirm(
          "You have unsaved changes. Are you sure you want to leave?"
        )
      ) {
        dispatch(setFormDirty(false));
        navigate("/cafes");
      }
    } else {
      navigate("/cafes");
    }
  };

  if (loading && isEditing) {
    return <Typography>Loading data...</Typography>;
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        {isEditing ? "Edit Cafe" : "Add New Cafe"}
      </Typography>

      <Card component={Paper} elevation={2}>
        <CardContent>
          <Box component="form" onSubmit={handleSubmit} noValidate>
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}>
                <FormTextField
                  required
                  name="name"
                  label="Cafe Name"
                  value={formData.name}
                  onChange={handleInputChange}
                  error={!!errors.name}
                  helperText={errors.name || "Between 6-10 characters"}
                  minLength={6}
                  maxLength={10}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormTextField
                  required
                  name="location"
                  label="Location"
                  value={formData.location}
                  onChange={handleInputChange}
                  error={!!errors.location}
                  helperText={errors.location}
                />
              </Grid>

              <Grid item xs={12}>
                <FormTextField
                  required
                  name="description"
                  label="Description"
                  value={formData.description}
                  onChange={handleInputChange}
                  error={!!errors.description}
                  helperText={
                    errors.description ||
                    `${formData.description.length}/256 characters`
                  }
                  multiline
                  rows={4}
                  inputProps={{ maxLength: 256 }}
                />
              </Grid>

              <Grid item xs={12}>
                <input
                  accept="image/*"
                  style={{ display: "none" }}
                  id="logo-upload"
                  type="file"
                  onChange={handleFileChange}
                  disabled={isUploading}
                />
                <label htmlFor="logo-upload">
                  <Button
                    variant="outlined"
                    component="span"
                    disabled={isUploading}
                    startIcon={
                      isUploading ? <CircularProgress size={20} /> : undefined
                    }
                  >
                    {isUploading
                      ? "Uploading..."
                      : formData.logoFileName
                      ? "Change Logo"
                      : "Upload Logo"}
                  </Button>
                </label>

                {logoPreview && (
                  <Box mt={2} display="flex" justifyContent="center">
                    <img
                      src={logoPreview}
                      alt="Cafe logo preview"
                      style={{ maxWidth: "200px", maxHeight: "150px" }}
                    />
                  </Box>
                )}

                {formData.logoFileName && !logoPreview && (
                  <Box
                    mt={2}
                    display="flex"
                    justifyContent="center"
                    alignItems="center"
                  >
                    <Avatar sx={{ width: 80, height: 80, fontSize: 40 }}>
                      {formData.name ? formData.name[0] : "?"}
                    </Avatar>
                    <Typography ml={2} color="text.secondary">
                      Logo preview unavailable
                    </Typography>
                  </Box>
                )}

                {errors.logo && (
                  <FormHelperText error>{errors.logo}</FormHelperText>
                )}
              </Grid>

              <Grid item xs={12}>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "flex-end",
                    gap: 2,
                    mt: 2,
                  }}
                >
                  <Button variant="outlined" onClick={handleCancel}>
                    Cancel
                  </Button>
                  <Button
                    type="submit"
                    variant="contained"
                    color="primary"
                    disabled={submitting || isUploading}
                  >
                    {isEditing ? "Update Cafe" : "Create Cafe"}
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

export default AddEditCafePage;
