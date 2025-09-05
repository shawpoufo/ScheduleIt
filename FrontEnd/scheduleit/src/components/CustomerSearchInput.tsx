import React, { useState, useCallback } from 'react';
import { useCombobox } from 'downshift';
import type { Customer } from '../types';

interface CustomerSearchInputProps {
  value: Customer | null;
  onChange: (customer: Customer | null) => void;
  onSearch: (query: string) => Promise<Customer[]>;
  onQueryChange?: (query: string) => void;
  onCreateRequested?: (query: string) => void;
  placeholder?: string;
  className?: string;
  required?: boolean;
  disabled?: boolean;
}

const CustomerSearchInput: React.FC<CustomerSearchInputProps> = ({
  value,
  onChange,
  onSearch,
  onQueryChange,
  onCreateRequested,
  placeholder = "Search customers...",
  className = "",
  required = false,
  disabled = false
}) => {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const handleInputValueChange = useCallback(async (inputValue: string) => {
    if (inputValue.length < 2) {
      setCustomers([]);
      return;
    }

    setIsLoading(true);
    try {
      const results = await onSearch(inputValue);
      setCustomers(results);
    } catch (error) {
      console.error('Error searching customers:', error);
      setCustomers([]);
    } finally {
      setIsLoading(false);
    }
  }, [onSearch]);

  const {
    isOpen,
    getToggleButtonProps,
    getLabelProps,
    getMenuProps,
    getInputProps,
    highlightedIndex,
    getItemProps,
    selectedItem,
    selectItem,
    inputValue,
    setInputValue,
    closeMenu,
  } = useCombobox({
    items: customers,
    selectedItem: value,
    onSelectedItemChange: ({ selectedItem }) => {
      onChange(selectedItem || null);
    },
    onInputValueChange: ({ inputValue = "" }) => {
      handleInputValueChange(inputValue);
      onQueryChange?.(inputValue);
    },
    itemToString: (item) => item?.name || '',
    getA11yStatusMessage: ({ isOpen }) => {
      if (!isOpen) return '';
      if (customers.length === 0) return 'No customers found';
      return `${customers.length} customer${customers.length === 1 ? '' : 's'} available`;
    },
  });

  return (
    <div className={`relative ${className}`}>
      <div className="relative">
        <input
          {...getInputProps({
            placeholder,
            required,
            disabled,
            className: "mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 pr-10 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)] bg-white"
          })}
        />
        
        {/* Search/Toggle Button */}
        <button
          {...getToggleButtonProps({
            disabled,
            type: "button",
            className: "absolute inset-y-0 right-0 flex items-center pr-3 focus:outline-none"
          })}
        >
          {isLoading ? (
            <svg className="animate-spin h-4 w-4 text-gray-400" fill="none" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
          ) : (
            <svg className="h-4 w-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          )}
        </button>

        {/* Selected Customer Display */}
        {selectedItem && (
          <div className="absolute inset-y-0 right-8 flex items-center pr-2">
            <button
              type="button"
              onClick={() => {
                selectItem(null);
                setInputValue('');
              }}
              className="h-4 w-4 text-gray-400 hover:text-gray-600 focus:outline-none"
              disabled={disabled}
            >
              <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        )}
      </div>

      {/* Dropdown Menu */}
      <ul
        {...getMenuProps({
          className: `absolute z-50 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white py-1 text-base shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none ${
            isOpen && (customers.length > 0 || inputValue.length >= 2) ? 'block' : 'hidden'
          }`
        })}
      >
        {customers.map((customer, index) => (
          <li
            key={customer.id}
            {...getItemProps({
              item: customer,
              index,
              className: `relative cursor-pointer select-none py-2 pl-3 pr-9 hover:bg-gray-50 ${
                highlightedIndex === index ? 'bg-blue-50 text-blue-900' : 'text-gray-900'
              }`
            })}
          >
            <div className="flex items-center">
              <div className="flex-1 min-w-0">
                <div className="font-medium truncate">{customer.name}</div>
                <div className="text-sm text-gray-500 truncate">{customer.email}</div>
              </div>
              {selectedItem?.id === customer.id && (
                <div className="absolute inset-y-0 right-0 flex items-center pr-4 text-blue-600">
                  <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                  </svg>
                </div>
              )}
            </div>
          </li>
        ))}
        
        {isOpen && customers.length === 0 && inputValue.length >= 2 && !isLoading && (
          <>
            <li className="relative cursor-default select-none py-2 pl-3 pr-9 text-gray-500">
              No customers found for "{inputValue}"
            </li>
            {onCreateRequested && (
              <li className="relative select-none py-2 pl-3 pr-9">
                <button
                  type="button"
                  className="w-full text-left text-sm text-[var(--primary-color)] hover:bg-[var(--accent-color)] border border-[var(--primary-color)] rounded-md px-2 py-1"
                  onClick={() => {
                    onCreateRequested(inputValue);
                    selectItem(null);
                    setInputValue('');
                    setCustomers([]);
                    closeMenu();
                  }}
                  disabled={disabled}
                >
                  + Create new customer: "{inputValue}"
                </button>
              </li>
            )}
          </>
        )}
      </ul>
    </div>
  );
};

export default CustomerSearchInput;
