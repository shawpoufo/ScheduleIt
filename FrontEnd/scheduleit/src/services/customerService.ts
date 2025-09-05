import { API_BASE_URL } from '../config/api';

export interface Customer {
  id: string;
  name: string;
  email: string;
}

class CustomerService {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  /**
   * Search customers by name or email
   */
  async searchCustomers(query: string): Promise<Customer[]> {
    try {
      const url = new URL(`${this.baseUrl}/api/customers`);
      if (query.trim()) {
        url.searchParams.set('search', query.trim());
      }

      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to search customers: ${response.status} ${response.statusText}`);
      }

      const customers = await response.json();
      return customers;
    } catch (error) {
      console.error('Error searching customers:', error);
      throw error;
    }
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
    try {
      const response = await fetch(`${this.baseUrl}/api/customers/${id}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        if (response.status === 404) {
          throw new Error('Customer not found');
        }
        throw new Error(`Failed to get customer: ${response.status} ${response.statusText}`);
      }

      const customer = await response.json();
      return customer;
    } catch (error) {
      console.error('Error getting customer:', error);
      throw error;
    }
  }

  /**
   * Create a new customer
   */
  async createCustomer(name: string, email: string): Promise<Customer> {
    try {
      const response = await fetch(`${this.baseUrl}/api/customers`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ name, email }),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.error || `Failed to create customer: ${response.status}`);
      }

      const result = await response.json();
      
      // Return the customer with the created ID
      return {
        id: result.id,
        name,
        email,
      };
    } catch (error) {
      console.error('Error creating customer:', error);
      throw error;
    }
  }
}

// Create a singleton instance
const customerService = new CustomerService();

export default customerService;
