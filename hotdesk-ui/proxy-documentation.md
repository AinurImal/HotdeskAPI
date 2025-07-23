/* 
PROXY CONFIGURATION DOCUMENTATION
File: proxy.conf.json

PURPOSE:
This proxy configuration solves CORS (Cross-Origin Resource Sharing) issues
when developing Angular applications that need to call APIs on different ports.

PROBLEM SOLVED:
1. Angular dev server runs on http://localhost:4200
2. HotdeskAPI runs on http://localhost:5251  
3. Browser blocks direct calls from 4200 to 5251 due to CORS policy
4. Results in "Error Code 0" or CORS-related failures

SOLUTION:
The proxy intercepts API calls and forwards them, making them appear same-origin.

HOW IT WORKS:
1. Angular makes request to: http://localhost:4200/api/Users
2. Proxy catches /api/* pattern and forwards to: http://localhost:5251/api/Users
3. API response comes back through proxy to Angular
4. Browser sees everything as same-origin (no CORS issues)

CONFIGURATION PROPERTIES:
- "/api/*": Pattern to match (all requests starting with /api/)
- "target": Where to forward the requests (HotdeskAPI server)
- "secure": false = Allow HTTP connections (not HTTPS)
- "changeOrigin": true = Change origin header to match target
- "logLevel": "debug" = Enable detailed logging for troubleshooting

USAGE:
- Used automatically when running: npm start
- Configured in package.json: "ng serve --proxy-config proxy.conf.json"
- Only needed for development (production uses different setup)

BENEFITS:
✅ Eliminates CORS errors during development
✅ No server-side CORS configuration needed
✅ Seamless API integration
✅ All CRUD operations work (GET, POST, PUT, DELETE)
*/
