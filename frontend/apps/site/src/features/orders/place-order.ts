import { apiCall, type Schema } from '@template/api-client';
import { browserApi } from '@/platform/browser-api';

export interface QuickOrder {
  customerName: string;
  phone: string;
  productId: string;
  quantity: number;
}

/** Sends the order with the attempt's idempotency key: a retry of the same attempt cannot create a second order. */
export function placeOrder(
  order: QuickOrder,
  idempotencyKey: string,
): Promise<Schema<'PlacedOrderResponse'>> {
  return apiCall(
    browserApi.POST('/api/orders', {
      params: { header: { 'Idempotency-Key': idempotencyKey } },
      body: {
        customerName: order.customerName,
        phone: order.phone,
        lines: [{ productId: order.productId, quantity: order.quantity }],
      },
    }),
  );
}
