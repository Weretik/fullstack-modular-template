'use client';

import { useRef, useState, type FormEvent } from 'react';
import { ApiError, newIdempotencyKey } from '@template/api-client';
import { placeOrder } from './place-order';

type State =
  | { kind: 'editing'; error?: string; fieldErrors?: Record<string, string[]> }
  | { kind: 'sending' }
  | { kind: 'placed'; orderId: string };

/**
 * Order one product without an account. The button is locked while the request runs, and every attempt keeps
 * one idempotency key until it succeeds or the data changes, so a double click or a retry places one order.
 */
export function QuickOrderForm({ productId }: { productId: string }) {
  const [state, setState] = useState<State>({ kind: 'editing' });
  const idempotencyKey = useRef(newIdempotencyKey());

  const renewKey = () => {
    idempotencyKey.current = newIdempotencyKey();
  };

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (state.kind === 'sending') return;

    const form = new FormData(event.currentTarget);
    setState({ kind: 'sending' });
    try {
      const placed = await placeOrder(
        {
          productId,
          customerName: String(form.get('customerName') ?? ''),
          phone: String(form.get('phone') ?? ''),
          quantity: Number(form.get('quantity') ?? 1),
        },
        idempotencyKey.current,
      );
      renewKey();
      setState({ kind: 'placed', orderId: placed.orderId });
    } catch (error) {
      const failure = error instanceof ApiError ? error : undefined;
      setState({
        kind: 'editing',
        error: failure?.message ?? 'Could not send the order. Try again.',
        fieldErrors: failure?.fieldErrors,
      });
    }
  }

  if (state.kind === 'placed') {
    return (
      <p className="notice" role="status">
        Thank you! Your order is placed. We will call you to confirm it.
      </p>
    );
  }

  const fieldError = (name: string) =>
    state.kind === 'editing' ? state.fieldErrors?.[name]?.[0] : undefined;

  return (
    <form
      className="order-form"
      onSubmit={submit}
      onChange={renewKey}
      noValidate
    >
      <h2>Quick order</h2>
      <label>
        Name
        <input
          name="customerName"
          autoComplete="name"
          required
          maxLength={100}
        />
        {fieldError('customerName') && (
          <span className="field-error">{fieldError('customerName')}</span>
        )}
      </label>
      <label>
        Phone
        <input name="phone" type="tel" autoComplete="tel" required />
        {fieldError('phone') && (
          <span className="field-error">{fieldError('phone')}</span>
        )}
      </label>
      <label>
        Quantity
        <input
          name="quantity"
          type="number"
          min={1}
          max={1000}
          defaultValue={1}
          required
        />
        {fieldError('lines[0].quantity') && (
          <span className="field-error">{fieldError('lines[0].quantity')}</span>
        )}
      </label>
      {state.kind === 'editing' && state.error && (
        <p className="form-error" role="alert">
          {state.error}
        </p>
      )}
      <button type="submit" disabled={state.kind === 'sending'}>
        {state.kind === 'sending' ? 'Sending…' : 'Place order'}
      </button>
    </form>
  );
}
