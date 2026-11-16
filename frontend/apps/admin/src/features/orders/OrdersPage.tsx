import Chip from '@mui/material/Chip';
import MenuItem from '@mui/material/MenuItem';
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
import { OrderPanel } from './OrderPanel';
import { allStatuses } from './order-status';
import { useOrders, type OrderStatus } from './orders-api';

export function OrdersPage() {
  const [status, setStatus] = useState<OrderStatus | ''>('');
  const [page, setPage] = useState(1);
  const [openId, setOpenId] = useState<string | null>(null);
  const orders = useOrders(status || undefined, page);
  const pages = orders.data
    ? Math.max(1, Math.ceil(orders.data.totalCount / orders.data.pageSize))
    : 1;

  return (
    <Stack spacing={2}>
      <Typography variant="h5" component="h1">
        Orders
      </Typography>
      <TextField
        select
        label="Status"
        value={status}
        onChange={(event) => {
          setStatus(event.target.value as OrderStatus | '');
          setPage(1);
        }}
        sx={{ maxWidth: 220 }}
      >
        <MenuItem value="">All</MenuItem>
        {allStatuses.map((value) => (
          <MenuItem key={value} value={value}>
            {value}
          </MenuItem>
        ))}
      </TextField>
      {orders.isError && (
        <Typography color="error">{orders.error.message}</Typography>
      )}
      <Paper variant="outlined">
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Placed</TableCell>
              <TableCell>Customer</TableCell>
              <TableCell>Phone</TableCell>
              <TableCell align="right">Total</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {orders.data?.items.map((order) => (
              <TableRow
                key={order.id}
                hover
                onClick={() => setOpenId(order.id)}
                sx={{ cursor: 'pointer' }}
              >
                <TableCell>
                  {new Date(order.placedAtUtc).toLocaleString()}
                </TableCell>
                <TableCell>
                  {order.customerName}{' '}
                  {/* Only orders that wait for staff are marked. */}
                  {order.status === 'New' && (
                    <Chip
                      label="New"
                      size="small"
                      color="info"
                      variant="outlined"
                    />
                  )}
                </TableCell>
                <TableCell>{order.phone}</TableCell>
                <TableCell align="right">{order.total.toFixed(2)}</TableCell>
              </TableRow>
            ))}
            {orders.data?.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={4}>No orders yet.</TableCell>
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
      {openId && (
        <OrderPanel orderId={openId} onClose={() => setOpenId(null)} />
      )}
    </Stack>
  );
}
