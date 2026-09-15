# Railway Booking System

Online railway ticket booking platform built with ASP.NET Core and Angular.

## Features

- User authentication & authorization
- Train trip management
- Seat inventory management
- Real-time seat reservation
- Prevent seat overselling
- Online booking & payment
- Ticket management
- Admin dashboard
- High concurrency handling
- RESTful API architecture

---

# Tech Stack

## Backend

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Redis
- JWT Authentication
- Docker

## Frontend

- Angular
- TailwindCSS

## Infrastructure

- Docker Compose
- Nginx
- GitHub Actions

---

# Project Structure

```bash
src/
 ├── RailwayBooking.API
 ├── RailwayBooking.Application
 ├── RailwayBooking.Domain
 ├── RailwayBooking.Infrastructure

tests/
docs/
docker/
scripts/
```

---

# payOS setup

VNPay integration has been replaced by payOS. The backend now creates payOS checkout links and confirms paid booking orders from payOS redirects/webhooks.

## API endpoints

- Create payOS payment link: `POST /api/payments/payos/orders/{orderId}`
  - Requires JWT authentication.
  - Returns `paymentUrl`; redirect the customer/browser to this URL.
- payOS success return URL: `GET /api/payments/payos/return`
  - Must be public so payOS can redirect the customer back to the API.
- payOS cancel URL: `GET /api/payments/payos/cancel`
  - Must be public so payOS can redirect cancelled payments back to the API.
- payOS webhook URL: `POST /api/payments/payos/webhook`
  - Configure this URL in the payOS payment channel so the backend receives bank transfer notifications and verifies the HMAC SHA-256 signature.

## Configuration

Copy `RailwayBooking.Api/appsettings.example.json` to your local `RailwayBooking.Api/appsettings.json`, then update the public API host and payOS credentials in `PaymentProviders:PayOS`.

For local testing, expose the API through a public HTTPS tunnel such as ngrok/cloudflared and set:

```json
"PaymentProviders": {
  "PayOS": {
    "BaseUrl": "https://api-merchant.payos.vn",
    "ReturnUrl": "https://<your-public-host>/api/payments/payos/return",
    "CancelUrl": "https://<your-public-host>/api/payments/payos/cancel",
    "ClientId": "<your-payos-client-id>",
    "ApiKey": "<your-payos-api-key>",
    "ChecksumKey": "<your-payos-checksum-key>",
    "ExpireMinutes": 15
  }
}
```

Use the Client ID, API Key, and Checksum Key from your payOS payment channel. Do not commit production credentials.

## Manual payOS flow

1. Create a seat hold and booking order as usual.
2. Call `POST /api/payments/payos/orders/{orderId}` with the user's bearer token.
3. Open the returned `paymentUrl` in a browser.
4. Complete payment in payOS.
5. payOS redirects to `/api/payments/payos/return` and sends a webhook to `/api/payments/payos/webhook`; the backend verifies the webhook signature, updates the payment transaction, and confirms the order when payOS reports success.
