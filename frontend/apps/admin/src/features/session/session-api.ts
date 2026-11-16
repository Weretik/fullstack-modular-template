import { apiCall, ApiError, type Schema } from '@template/api-client';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '../../platform/api';

export const sessionKey = ['session'] as const;

/** The signed-in user; null when nobody is signed in. */
export function useCurrentUser() {
  return useQuery({
    queryKey: sessionKey,
    queryFn: async ({ signal }) => {
      try {
        return await apiCall(api.GET('/api/session', { signal }));
      } catch (error) {
        if (error instanceof ApiError && error.status === 401) return null;
        throw error;
      }
    },
  });
}

export function useSignIn() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: Schema<'SignInRequest'>) =>
      apiCall(api.POST('/api/session/sign-in', { body })),
    onSuccess: (user) => queryClient.setQueryData(sessionKey, user),
  });
}

export function useSignOut() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => apiCall(api.POST('/api/session/sign-out')),
    onSuccess: () => {
      queryClient.clear();
      queryClient.setQueryData(sessionKey, null);
    },
  });
}
