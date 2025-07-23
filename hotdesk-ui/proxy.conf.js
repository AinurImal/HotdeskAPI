const PROXY_CONFIG = [
  {
    context: ['/api/**'],
    target: 'http://localhost:5251',
    secure: false,
    changeOrigin: true,
    logLevel: 'debug',
    onProxyReq: function(proxyReq, req, res) {
      console.log('Proxy Request:', req.method, req.url);
    },
    onProxyRes: function(proxyRes, req, res) {
      console.log('Proxy Response:', proxyRes.statusCode, req.url);
      // Add CORS headers if needed
      res.setHeader('Access-Control-Allow-Origin', '*');
      res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
      res.setHeader('Access-Control-Allow-Headers', 'Content-Type, Authorization');
    },
    onError: function(err, req, res) {
      console.log('Proxy Error:', err.message);
    }
  }
];

module.exports = PROXY_CONFIG;
