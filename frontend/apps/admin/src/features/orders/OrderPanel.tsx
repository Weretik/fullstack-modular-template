import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Drawer from '@mui/material/Drawer';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { nextStatuses } from './order-status';
import { useChangeOrderStatus, useOrder } from './orders-api';

/** One order in a side panel, with the status moves allowed from where it is now. */
export function OrderPanel({
  orderId,
  onClose,
}: {
  orderId: string;
  onClose: () => void;
}) {
  const order = useOrder(orderId);
  const change = useChangeOrderStatus();

  return (
    <Drawer anchor="right" open onClose={onClose}>
      <Stack spacing={2} sx={{ width: { xs: '100vw', sm: 440 }, p: 3 }}>
        {order.data && (
          <>
            <Typography variant="h6">{order.data.customerName}</Typography>
            <Typography color="text.secondary">
              {order.data.phone} · {order.data.status}
            </Typography>
            <Divider />
            {order.data.lines.map((line) => (
              <Stack
                key={line.productId}
                direction="row"
                sx={{ justifyContent: 'space-between' }}
              >
                <span>
                  {line.productName} × {line.quantity}
                </span>
                <span>{line.lineTotal.toFixed(2)}</span>
              </Stack>
            ))}
            <Divider />
            <Typography sx={{ fontWeight: 600 }}>
              Total {order.data.total.toFixed(2)}
            </Typography>
            {change.error && (
              <Alert severity="error">{change.error.message}</Alert>
            )}
            <Stack direction="row" spacing={1}>
              {nextStatuses[order.data.status].map((move) => (
                <Button
                  key={move.status}
                  variant={
                    move.status === 'Cancelled' ? 'outlined' : 'contained'
                  }
                  color={move.status === 'Cancelled' ? 'error' : 'primary'}
                  disabled={change.isPending}
                  onClick={() =>
                    change.mutate({ id: orderId, status: move.status })
                  }
                >
                  {move.label}
                </Button>
              ))}
            </Stack>
          </>
        )}
        {order.isError && <Alert severity="error">{order.error.message}</Alert>}
      </Stack>
    </Drawer>
  );
}
