import { apiCall, type Schema } from '@template/api-client';
import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from '@tanstack/react-query';
import { api } from '../../platform/api';

export type ProductForm = Schema<'UpdateProductRequest'>;

const productsKey = ['products'] as const;

export function useCategories() {
  return useQuery({
    queryKey: ['categories'],
    queryFn: ({ signal }) =>
      apiCall(api.GET('/api/catalog/categories', { signal })),
    staleTime: 5 * 60_000,
  });
}

export function useProductSearch(search: string, page: number) {
  return useQuery({
    queryKey: [...productsKey, 'search', search, page],
    queryFn: ({ signal }) =>
      apiCall(
        api.GET('/api/admin/catalog/products', {
          params: { query: { search, page, pageSize: 20 } },
          signal,
        }),
      ),
    placeholderData: keepPreviousData,
  });
}

export function useProduct(id: string | undefined) {
  return useQuery({
    queryKey: [...productsKey, id],
    queryFn: ({ signal }) =>
      apiCall(
        api.GET('/api/admin/catalog/products/{id}', {
          params: { path: { id: id ?? '' } },
          signal,
        }),
      ),
    enabled: Boolean(id),
  });
}

/**
 * Creates a product when there is no id, otherwise replaces it. A repeated create answers 409 (the slug is
 * taken) and a repeated update gives the same product, so a double submit is harmless.
 */
export function useSaveProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, form }: { id?: string; form: ProductForm }) =>
      id
        ? apiCall(
            api.PUT('/api/admin/catalog/products/{id}', {
              params: { path: { id } },
              body: form,
            }),
          )
        : apiCall(api.POST('/api/admin/catalog/products', { body: form })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: productsKey }),
  });
}
