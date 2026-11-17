import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { api } from '../../platform/api';
import { ProductEditor } from './ProductEditor';

vi.mock('../../platform/api', () => ({
  api: { GET: vi.fn(), POST: vi.fn(), PUT: vi.fn() },
}));

const ok = (data: unknown) =>
  Promise.resolve({ data, response: new Response(null, { status: 200 }) });

const categories = [{ id: 'c-1', name: 'Pens', slug: 'pens' }];
const product = {
  id: 'p-1',
  name: 'Blue pen',
  slug: 'blue-pen',
  description: null,
  price: 12.5,
  categoryId: 'c-1',
  isPublished: false,
};

function renderEditor(productId?: string) {
  const onClose = vi.fn();
  const client = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  render(
    <QueryClientProvider client={client}>
      <ProductEditor productId={productId} onClose={onClose} />
    </QueryClientProvider>,
  );
  return { onClose };
}

describe('ProductEditor', () => {
  beforeEach(() => {
    vi.mocked(api.GET).mockReset();
    vi.mocked(api.POST).mockReset();
    vi.mocked(api.PUT).mockReset();
    vi.mocked(api.GET).mockImplementation(((path: string) =>
      ok(path === '/api/catalog/categories' ? categories : product)) as never);
  });

  it('suggests the slug from the name until the slug is edited by hand', async () => {
    renderEditor();
    const name = screen.getByLabelText('Name');
    const slug = screen.getByLabelText('Slug');

    await userEvent.type(name, 'Red Pen 2');
    expect(screen.getByDisplayValue('red-pen-2')).toBe(slug);

    await userEvent.clear(slug);
    await userEvent.type(slug, 'my-pen');
    await userEvent.type(name, 'x');
    expect(screen.getByDisplayValue('my-pen')).toBe(slug);
  });

  it('shows server validation messages under their fields and keeps the panel open', async () => {
    vi.mocked(api.POST).mockResolvedValue({
      error: {
        detail: 'Check the form.',
        errors: { Slug: ['This slug is already taken.'] },
      },
      response: new Response(null, { status: 400 }),
    } as never);
    const { onClose } = renderEditor();

    await userEvent.type(screen.getByLabelText('Name'), 'Blue pen');
    await userEvent.click(screen.getByRole('button', { name: 'Save' }));

    expect(await screen.findByText('This slug is already taken.')).toBeTruthy();
    expect(onClose).not.toHaveBeenCalled();
  });

  it('creates a product once, even after a double click', async () => {
    vi.mocked(api.POST).mockReturnValue(new Promise(() => undefined) as never);
    renderEditor();

    await userEvent.type(screen.getByLabelText('Name'), 'Blue pen');
    await userEvent.dblClick(screen.getByRole('button', { name: 'Save' }));

    expect(api.POST).toHaveBeenCalledTimes(1);
    expect(api.POST).toHaveBeenCalledWith('/api/admin/catalog/products', {
      body: expect.objectContaining({
        name: 'Blue pen',
        slug: 'blue-pen',
        description: null,
      }),
    });
  });

  it('loads an existing product and saves the changes with its id', async () => {
    vi.mocked(api.PUT).mockReturnValue(ok(product) as never);
    const { onClose } = renderEditor('p-1');

    const name = await screen.findByDisplayValue('Blue pen');
    await userEvent.clear(name);
    await userEvent.type(name, 'Black pen');
    await userEvent.click(screen.getByRole('button', { name: 'Save' }));

    await waitFor(() => expect(onClose).toHaveBeenCalled());
    expect(api.PUT).toHaveBeenCalledWith('/api/admin/catalog/products/{id}', {
      params: { path: { id: 'p-1' } },
      body: expect.objectContaining({ name: 'Black pen', slug: 'blue-pen' }),
    });
  });
});
