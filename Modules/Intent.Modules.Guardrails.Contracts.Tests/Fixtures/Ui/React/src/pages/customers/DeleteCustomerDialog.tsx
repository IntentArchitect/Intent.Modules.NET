import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";

export function DeleteCustomerDialog({ open, name, onConfirm, onClose }: { open: boolean; name?: string; onConfirm: () => void; onClose: () => void }) {
  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>Delete customer</DialogTitle>
      <DialogContent>Delete {name}?</DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button color="error" onClick={onConfirm}>Delete</Button>
      </DialogActions>
    </Dialog>
  );
}
