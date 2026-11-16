import type { OrderStatus } from './orders-api';

/** The moves staff can make from each status; the backend enforces the same rules. */
export const nextStatuses: Record<
  OrderStatus,
  { status: OrderStatus; label: string }[]
> = {
  New: [
    { status: 'Confirmed', label: 'Confirm' },
    { status: 'Cancelled', label: 'Cancel' },
  ],
  Confirmed: [
    { status: 'Completed', label: 'Complete' },
    { status: 'Cancelled', label: 'Cancel' },
  ],
  Completed: [],
  Cancelled: [],
};

export const allStatuses: OrderStatus[] = [
  'New',
  'Confirmed',
  'Completed',
  'Cancelled',
];
