import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Drawer from '@mui/material/Drawer';
import FormControlLabel from '@mui/material/FormControlLabel';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { ApiError } from '@template/api-client';
import { useEffect, useState } from 'react';
import { slugFromName } from './slug-from-name';
import {
  useCategories,
  useProduct,
  useSaveProduct,
  type ProductForm,
} from './products-api';

const emptyForm: ProductForm = {
  name: '',
  slug: '',
  description: '',
  price: 0,
  categoryId: '',
  isPublished: false,
};

/** Creates or edits a product in a side panel. Server validation messages appear under their fields. */
export function ProductEditor({
  productId,
  onClose,
}: {
  productId?: string;
  onClose: () => void;
}) {
  const product = useProduct(productId);
  const categories = useCategories();
  const save = useSaveProduct();
  const [form, setForm] = useState<ProductForm>(emptyForm);
  const [slugEdited, setSlugEdited] = useState(Boolean(productId));
  const error = save.error instanceof ApiError ? save.error : undefined;

  useEffect(() => {
    if (product.data)
      setForm({ ...product.data, description: product.data.description ?? '' });
  }, [product.data]);

  const set = <K extends keyof ProductForm>(key: K, value: ProductForm[K]) =>
    setForm((current) => ({ ...current, [key]: value }));
  const fieldError = (field: keyof ProductForm) =>
    error?.fieldErrors[field]?.[0];

  function submit() {
    if (save.isPending) return;
    save.mutate(
      {
        id: productId,
        form: { ...form, description: form.description || null },
      },
      { onSuccess: onClose },
    );
  }

  return (
    <Drawer anchor="right" open onClose={onClose}>
      <Stack spacing={2} sx={{ width: { xs: '100vw', sm: 440 }, p: 3 }}>
        <Typography variant="h6">
          {productId ? 'Edit product' : 'New product'}
        </Typography>
        {error && !Object.keys(error.fieldErrors).length && (
          <Alert severity="error">{error.message}</Alert>
        )}
        <TextField
          label="Name"
          value={form.name}
          onChange={(event) => {
            set('name', event.target.value);
            if (!slugEdited) set('slug', slugFromName(event.target.value));
          }}
          error={Boolean(fieldError('name'))}
          helperText={fieldError('name')}
        />
        <TextField
          label="Slug"
          value={form.slug}
          onChange={(event) => {
            setSlugEdited(true);
            set('slug', event.target.value);
          }}
          error={Boolean(fieldError('slug'))}
          helperText={fieldError('slug') ?? 'The address of the product page.'}
        />
        <TextField
          select
          label="Category"
          value={categories.data ? form.categoryId : ''}
          onChange={(event) => set('categoryId', event.target.value)}
          error={Boolean(fieldError('categoryId'))}
          helperText={fieldError('categoryId')}
        >
          {categories.data?.map((category) => (
            <MenuItem key={category.id} value={category.id}>
              {category.name}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          label="Price"
          type="number"
          value={form.price || ''}
          onChange={(event) => set('price', Number(event.target.value))}
          slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
          error={Boolean(fieldError('price'))}
          helperText={fieldError('price')}
        />
        <TextField
          label="Description"
          multiline
          minRows={4}
          value={form.description ?? ''}
          onChange={(event) => set('description', event.target.value)}
          error={Boolean(fieldError('description'))}
          helperText={fieldError('description')}
        />
        {productId && (
          <FormControlLabel
            control={
              <Switch
                checked={form.isPublished}
                onChange={(event) => set('isPublished', event.target.checked)}
              />
            }
            label="Show on the site"
          />
        )}
        <Stack direction="row" spacing={1} sx={{ justifyContent: 'flex-end' }}>
          <Button onClick={onClose}>Cancel</Button>
          <Button
            variant="contained"
            onClick={submit}
            disabled={
              save.isPending || (product.isPending && Boolean(productId))
            }
          >
            {save.isPending ? 'Saving…' : 'Save'}
          </Button>
        </Stack>
      </Stack>
    </Drawer>
  );
}
