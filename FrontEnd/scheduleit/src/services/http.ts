export async function fetchJson<T>(input: RequestInfo | URL, init?: RequestInit): Promise<T> {
  const res = await fetch(input, init);
  const data = await res.json().catch(() => undefined);
  if (!res.ok) {
    const message = (data && (data.error || data.title || data.message)) || `${res.status} ${res.statusText}`;
    throw new Error(message);
  }
  return data as T;
}


