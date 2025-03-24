import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import FormTextField from "../form-text-field-component";

describe("FormTextField_Test", () => {
  it("renders with label and value correctly", () => {
    render(
      <FormTextField
        name="testField"
        label="Test Label"
        value="Test Value"
        onChange={() => {}}
      />
    );

    expect(screen.getByLabelText(/Test Label/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Test Label/i)).toHaveValue("Test Value");
  });

  it("calls onChange when input changes", () => {
    const handleChange = vi.fn();

    render(
      <FormTextField
        name="testField"
        label="Test Label"
        value="Test Value"
        onChange={handleChange}
      />
    );

    const input = screen.getByLabelText(/Test Label/i);
    fireEvent.change(input, { target: { value: "New Value" } });

    expect(handleChange).toHaveBeenCalledTimes(1);
  });

  it("displays error and helper text when provided", () => {
    render(
      <FormTextField
        name="testField"
        label="Test Label"
        value="Test Value"
        onChange={() => {}}
        error={true}
        helperText="This field has an error"
      />
    );

    expect(screen.getByText("This field has an error")).toBeInTheDocument();
    const input = screen.getByLabelText(/Test Label/i);
    expect(input).toHaveAttribute("aria-invalid", "true");
  });

  it("applies maxLength attribute correctly", () => {
    render(
      <FormTextField
        name="testField"
        label="Test Label"
        value="Test Value"
        onChange={() => {}}
        maxLength={10}
      />
    );

    const input = screen.getByLabelText(/Test Label/i);
    expect(input).toHaveAttribute("maxLength", "10");
  });
});
