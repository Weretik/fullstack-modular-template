import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Pagination from '@mui/material/Pagination';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { useState } from 'react';
import { ProductEditor } from './ProductEditor';
import { useProductSearch } from './products-api';

/** The product list; a row opens the editor in a side panel, so the list stays in view. */
export function ProductsPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [editing, setEditing] = useState<{ id?: string } | null>(null);
  const products = useProductSearch(search, page);
  const pages = products.data
    ? Math.max(1, Math.ceil(products.data.totalCount / products.data.pageSize))
    : 1;

  return (
    <Stack spacing={2}>
      <Stack
        direction="row"
        sx={{ alignItems: 'center', justifyContent: 'space-between' }}
      >
        <Typography variant="h5" component="h1">
          Products
        </Typography>
        <Button variant="contained" onClick={() => setEditing({})}>
          New product
        </Button>
      </Stack>
      <TextField
        label="Search by name or slug"
        value={search}
        onChange={(event) => {
          setSearch(event.target.value);
          setPage(1);
        }}
        sx={{ maxWidth: 360 }}
      />
      {products.isError && (
        <Typography color="error">{products.error.message}</Typography>
      )}
      <Paper variant="outlined">
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Category</TableCell>
              <TableCell align="right">Price</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {products.data?.items.map((product) => (
              <TableRow
                key={product.id}
                hover
                onClick={() => setEditing({ id: product.id })}
                sx={{ cursor: 'pointer' }}
              >
                <TableCell>
                  {product.name}{' '}
                  {/* Only what needs attention is marked: a draft is not on the site yet. */}
                  {!product.isPublished && (
                    <Chip
                      label="Draft"
                      size="small"
                      color="warning"
                      variant="outlined"
                    />
                  )}
                </TableCell>
                <TableCell>{product.categoryName}</TableCell>
                <TableCell align="right">{product.price.toFixed(2)}</TableCell>
              </TableRow>
            ))}
            {products.data?.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={3}>No products found.</TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Paper>
      {pages > 1 && (
        <Pagination
          count={pages}
          page={page}
          onChange={(_, value) => setPage(value)}
        />
      )}
      {editing && (
        <ProductEditor
          productId={editing.id}
          onClose={() => setEditing(null)}
        />
      )}
    </Stack>
  );
}
