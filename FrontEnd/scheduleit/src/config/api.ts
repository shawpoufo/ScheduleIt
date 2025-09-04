// Central API configuration
const envBaseUrl = import.meta.env?.VITE_API_BASE_URL;
const isDevelopment = import.meta.env?.DEV;
const isProduction = import.meta.env?.PROD;

const resolvedBaseUrl = envBaseUrl
  ? envBaseUrl
  : isDevelopment
    ? 'http://localhost:5522'
    : isProduction
      ? 'https://api.scheduleit.com'
      : 'http://localhost:5522';

export const API_BASE_URL = resolvedBaseUrl;
export const API_TIMEOUT = 10000;
