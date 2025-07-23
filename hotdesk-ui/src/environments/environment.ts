// Environment configuration for development mode
export const environment = {
  production: false,    // Flag indicating this is development environment
  apiUrl: '/api'        // Base API URL - uses proxy to avoid CORS issues
                        // Proxy forwards /api/* requests to http://localhost:5251/api
};
