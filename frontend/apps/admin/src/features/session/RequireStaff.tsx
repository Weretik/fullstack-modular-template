import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useCurrentUser } from './session-api';

/** Shows its children to signed-in staff and sends everyone else to the sign-in page. */
export function RequireStaff({ children }: { children: ReactNode }) {
  const { data: user, isPending } = useCurrentUser();
  const location = useLocation();

  if (isPending) {
    return (
      <Box sx={{ display: 'grid', placeItems: 'center', minHeight: '50vh' }}>
        <CircularProgress aria-label="Loading" />
      </Box>
    );
  }

  if (!user)
    return (
      <Navigate to="/sign-in" replace state={{ from: location.pathname }} />
    );
  return children;
}
