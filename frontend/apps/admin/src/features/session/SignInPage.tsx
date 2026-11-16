import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { ApiError } from '@template/api-client';
import type { FormEvent } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useSignIn } from './session-api';

export function SignInPage() {
  const signIn = useSignIn();
  const navigate = useNavigate();
  const from = (useLocation().state as { from?: string } | null)?.from ?? '/';
  const error = signIn.error instanceof ApiError ? signIn.error : undefined;

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (signIn.isPending) return;
    const form = new FormData(event.currentTarget);
    signIn.mutate(
      {
        email: String(form.get('email')),
        password: String(form.get('password')),
      },
      { onSuccess: () => navigate(from, { replace: true }) },
    );
  }

  return (
    <Stack
      sx={{
        minHeight: '100vh',
        alignItems: 'center',
        justifyContent: 'center',
        p: 2,
      }}
    >
      <Paper
        component="form"
        onSubmit={submit}
        noValidate
        variant="outlined"
        sx={{ p: 4, width: '100%', maxWidth: 380 }}
      >
        <Stack spacing={2}>
          <Typography variant="h5" component="h1">
            Sign in
          </Typography>
          {signIn.error && (
            <Alert severity="error">
              {error?.message ?? 'Could not sign in. Try again.'}
            </Alert>
          )}
          <TextField
            name="email"
            label="E-mail"
            type="email"
            autoComplete="username"
            error={Boolean(error?.fieldErrors['email'])}
            helperText={error?.fieldErrors['email']?.[0]}
          />
          <TextField
            name="password"
            label="Password"
            type="password"
            autoComplete="current-password"
            error={Boolean(error?.fieldErrors['password'])}
            helperText={error?.fieldErrors['password']?.[0]}
          />
          <Button type="submit" variant="contained" disabled={signIn.isPending}>
            {signIn.isPending ? 'Signing in…' : 'Sign in'}
          </Button>
        </Stack>
      </Paper>
    </Stack>
  );
}
