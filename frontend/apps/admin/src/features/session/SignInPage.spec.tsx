import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { api } from '../../platform/api';
import { SignInPage } from './SignInPage';

vi.mock('../../platform/api', () => ({ api: { POST: vi.fn() } }));

function renderPage() {
  render(
    <QueryClientProvider client={new QueryClient()}>
      <MemoryRouter>
        <SignInPage />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('SignInPage', () => {
  beforeEach(() => {
    vi.mocked(api.POST).mockReset();
  });

  it('shows the message the API returns for wrong credentials', async () => {
    vi.mocked(api.POST).mockResolvedValue({
      error: {
        detail: 'Wrong e-mail or password.',
        code: 'identity.invalid_credentials',
      },
      response: new Response(null, { status: 401 }),
    } as never);
    renderPage();

    await userEvent.type(screen.getByLabelText('E-mail'), 'admin@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'wrong');
    await userEvent.click(screen.getByRole('button', { name: 'Sign in' }));

    expect(await screen.findByText('Wrong e-mail or password.')).toBeTruthy();
  });

  it('sends the e-mail and password once, even after a double click', async () => {
    vi.mocked(api.POST).mockReturnValue(new Promise(() => undefined) as never);
    renderPage();

    await userEvent.type(screen.getByLabelText('E-mail'), 'admin@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'secret');
    await userEvent.dblClick(screen.getByRole('button', { name: 'Sign in' }));

    expect(api.POST).toHaveBeenCalledTimes(1);
    expect(api.POST).toHaveBeenCalledWith('/api/session/sign-in', {
      body: { email: 'admin@example.com', password: 'secret' },
    });
  });
});
