import { apiCall, type Schema } from '@template/api-client';
import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from '@tanstack/react-query';
import { api } from '../../platform/api';

export type OrderStatus = Schema<'OrderStatus'>;

const ordersKey = ['orders'] as const;

export function useOrders(status: OrderStatus | undefined, page: number) {
  return useQuery({
    queryKey: [...ordersKey, 'list', status, page],
    queryFn: ({ signal }) =>
      apiCall(
        api.GET('/api/admin/orders', {
          params: { query: { status, page, pageSize: 20 } },
          signal,
        }),
      ),
    placeholderData: keepPreviousData,
  });
}

export function useOrder(id: string) {
  return useQuery({
    queryKey: [...ordersKey, id],
    queryFn: ({ signal }) =>
      apiCall(
        api.GET('/api/admin/orders/{id}', { params: { path: { id } }, signal }),
      ),
  });
}

/** Setting the status an order already has succeeds, so repeating the click is safe. */
export function useChangeOrderStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: OrderStatus }) =>
      apiCall(
        api.PUT('/api/admin/orders/{id}/status', {
          params: { path: { id } },
          body: { status },
        }),
      ),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ordersKey }),
  });
}
