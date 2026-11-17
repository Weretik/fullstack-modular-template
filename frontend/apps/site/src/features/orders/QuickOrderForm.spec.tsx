import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ApiError } from '@template/api-client';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { placeOrder } from './place-order';
import { QuickOrderForm } from './QuickOrderForm';

vi.mock('./place-order', () => ({ placeOrder: vi.fn() }));

const sentKeys = () => vi.mocked(placeOrder).mock.calls.map((call) => call[1]);

async function fillAndSubmit() {
  await userEvent.type(screen.getByLabelText('Name'), 'Olena');
  await userEvent.type(screen.getByLabelText('Phone'), '+380501112233');
  await userEvent.click(screen.getByRole('button', { name: 'Place order' }));
}

describe('QuickOrderForm', () => {
  beforeEach(() => {
    vi.mocked(placeOrder).mockReset();
  });

  it('sends the order with the product, the form data and an idempotency key', async () => {
    vi.mocked(placeOrder).mockResolvedValue({ orderId: 'o-1' } as never);
    render(<QuickOrderForm productId="p-1" />);

    await fillAndSubmit();

    expect(await screen.findByRole('status')).toBeTruthy();
    expect(placeOrder).toHaveBeenCalledWith(
      {
        productId: 'p-1',
        customerName: 'Olena',
        phone: '+380501112233',
        quantity: 1,
      },
      expect.stringMatching(/^[0-9a-f-]{36}$/),
    );
  });

  it('repeats a failed attempt with the same idempotency key, so the retry cannot place a second order', async () => {
    vi.mocked(placeOrder)
      .mockRejectedValueOnce(new TypeError('Failed to fetch'))
      .mockResolvedValueOnce({ orderId: 'o-1' } as never);
    render(<QuickOrderForm productId="p-1" />);

    await fillAndSubmit();
    expect(await screen.findByRole('alert')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Place order' }));

    expect(await screen.findByRole('status')).toBeTruthy();
    const [first, retry] = sentKeys();
    expect(retry).toBe(first);
  });

  it('takes a new idempotency key when the data changes after a failure', async () => {
    vi.mocked(placeOrder).mockRejectedValue(new TypeError('Failed to fetch'));
    render(<QuickOrderForm productId="p-1" />);

    await fillAndSubmit();
    await screen.findByRole('alert');
    await userEvent.clear(screen.getByLabelText('Quantity'));
    await userEvent.type(screen.getByLabelText('Quantity'), '3');
    await userEvent.click(screen.getByRole('button', { name: 'Place order' }));

    const [first, second] = sentKeys();
    expect(second).not.toBe(first);
    expect(vi.mocked(placeOrder).mock.calls[1][0].quantity).toBe(3);
  });

  it('sends one order on a double click', async () => {
    vi.mocked(placeOrder).mockReturnValue(new Promise(() => undefined));
    render(<QuickOrderForm productId="p-1" />);

    await userEvent.type(screen.getByLabelText('Name'), 'Olena');
    await userEvent.type(screen.getByLabelText('Phone'), '+380501112233');
    await userEvent.dblClick(
      screen.getByRole('button', { name: 'Place order' }),
    );

    expect(placeOrder).toHaveBeenCalledTimes(1);
    expect(screen.getByRole('button', { name: 'Sending…' })).toBeTruthy();
  });

  it('shows the server validation message under its field', async () => {
    vi.mocked(placeOrder).mockRejectedValue(
      new ApiError(400, {
        detail: 'Check the form.',
        errors: { Phone: ['Enter a phone number.'] },
      }),
    );
    render(<QuickOrderForm productId="p-1" />);

    await fillAndSubmit();

    expect(await screen.findByText('Enter a phone number.')).toBeTruthy();
    expect(screen.getByRole('alert').textContent).toBe('Check the form.');
  });
});
