import { API_BASE_URL } from '../config/api';
import { fetchJson } from './http';
import type { Customer } from '../types';

class CustomerService {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  /**
   * Search customers by name or email
   */
  async searchCustomers(query: string): Promise<Customer[]> {
    const url = new URL(`${this.baseUrl}/api/customers`);
    if (query.trim()) {
      url.searchParams.set('search', query.trim());
    }
    return fetchJson<Customer[]>(url.toString(), { method: 'GET', headers: { 'Content-Type': 'application/json' } });
  }

  /**
   * Get all customers (with optional limit)
   */
  async getAllCustomers(limit: number = 20): Promise<Customer[]> {
    return this.searchCustomers(''); // Empty search returns all
  }

  /**
   * Get a specific customer by ID
   */
  async getCustomer(id: string): Promise<Customer> {
    return fetchJson<Customer>(`${this.baseUrl}/api/customers/${id}`, { method: 'GET', headers: { 'Content-Type': 'application/json' } });
  }

  /**
   * Create a new customer
   */
  async createCustomer(name: string, email: string): Promise<Customer> {
    const result = await fetchJson<{ id: string }>(`${this.baseUrl}/api/customers`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name, email })
    });
    return { id: result.id, name, email };
  }
}

// Create a singleton instance
const customerService = new CustomerService();

export default customerService;
