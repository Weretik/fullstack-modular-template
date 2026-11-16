import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import { NavLink, Outlet } from 'react-router-dom';
import { useCurrentUser, useSignOut } from '../features/session/session-api';

const sections = [
  { path: '/products', label: 'Products' },
  { path: '/orders', label: 'Orders' },
];

export function AdminLayout() {
  const { data: user } = useCurrentUser();
  const signOut = useSignOut();

  return (
    <>
      <AppBar
        position="static"
        color="inherit"
        elevation={0}
        sx={{ borderBottom: 1, borderColor: 'divider' }}
      >
        <Toolbar sx={{ gap: 1 }}>
          <Typography variant="h6" sx={{ mr: 3 }}>
            Admin
          </Typography>
          {sections.map((section) => (
            <Button
              key={section.path}
              component={NavLink}
              to={section.path}
              sx={{ '&.active': { bgcolor: 'action.selected' } }}
            >
              {section.label}
            </Button>
          ))}
          <Box sx={{ flexGrow: 1 }} />
          <Typography variant="body2" color="text.secondary">
            {user?.displayName}
          </Typography>
          <Button onClick={() => signOut.mutate()} disabled={signOut.isPending}>
            Sign out
          </Button>
        </Toolbar>
      </AppBar>
      <Container maxWidth="lg" sx={{ py: 3 }}>
        <Outlet />
      </Container>
    </>
  );
}
