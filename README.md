# Godot OIDC Authentication with Descope

This project demonstrates how to integrate user authentication and multi-factor authentication (MFA) into a [Godot Engine](https://godotengine.org/) game using [Descope](https://www.descope.com/) as an OIDC (OpenID Connect) provider.

It features:

- Passwordless login with [enhanced links](https://docs.descope.com/auth-methods/enchanted-link/with-sdks/backend)
- [Social authentication](https://www.descope.com/learn/post/social-login) with Discord
- [SMS-based MFA](https://www.descope.com/learn/post/sms-authentication)
- Secure token handling and session management
- A simple login/logout UI in Godot using C#

## ✨ Features

- **OIDC Authorization Code Flow with PKCE** - Securely authenticate users without needing to handle or store passwords.
- **Multi-Factor Authentication (MFA)** - Add an extra layer of security with SMS-based OTP verification.
- **Social Login (Discord)** - Allow users to sign in with their Discord account.
- **Godot C# Integration** - All authentication logic is handled within Godot using C# and standard HTTP libraries.

## 📋 Prerequisites

- [Godot Engine (C# version)](https://godotengine.org/) (latest stable release)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download) installed
- A [Descope account](https://www.descope.com/sign-up)
- Basic familiarity with C# and Godot

## ⚙️ Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/stamd/godot-descope-oidc-auth-demo.git
   cd godot-descope-oidc-auth-demo
   ```

2. **Configure Environment Variables**

   Create a `.env` file (or otherwise securely store) the following values:

   - `CLIENT_ID` (Descope Project ID)
   - `REDIRECT_URI`
   - `AUTHORIZATION_URL`
   - `TOKEN_URL`
   - `LOGOUT_URL`

   *(Alternatively, you can hardcode these values for testing purposes, but environment variables are recommended for security.)*

3. **Install Dependencies**

   Make sure your Godot project is properly set up to use C# scripting with the .NET SDK.

4. **Run the Project**

   Open the project in Godot, then press **Play** to launch the game.
    Use the "Log In" button to authenticate through Descope and view user details in-game.

## 🗂️ Project Structure

```plaintext
GodotDescopeAuth/
├── LoginManager.cs     # Handles login, logout, and token exchanges
├── MainScene.tscn      # Basic UI with login/logout and user info
├── .env                # Environment variables (not committed)
├── README.md
└── ...
```