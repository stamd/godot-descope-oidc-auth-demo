using Godot;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public partial class LoginManager : Control
{
	[Export] private Button loginButton;
	[Export] private Button logoutButton;
	[Export] private Label userInfoLabel;
	
	private string _codeVerifier = "N7QLWqAp2aYxDGRtqbXjbfusYLE97XAui-nnW9hOofI";
	private string _codeChallenge = "KM6LN2hMVS5xeS3CHhoNuHtqtWD0-EIbkIq8u_KZl3U";
	private string _state = "KM6LN2fFDS32duHtqtWD0-EIbkIq8u_KfG3U";
	
	private string _authUrl = "https://api.descope.com/oauth2/v1/authorize";
	private string _tokenUrl = "https://api.descope.com/oauth2/v1/token";
	private string _logoutUrl = "https://api.descope.com/oauth2/v1/logout";
	
	private string _clientId = "your-client-id-from-descope";
	
	private string _redirectUri = "http://localhost:3000/callback";
	private string _redirectLogoutUri = "http://localhost:3000/logout-callback";
	
	private bool _isLoggedIn = false; 

	private string _id_token = "";

  public override void _Ready()
  {
  // Get references from the scene
	loginButton = GetNode<Button>("VBoxContainer/Login");
	logoutButton = GetNode<Button>("VBoxContainer/Logout");
	userInfoLabel = GetNode<Label>("VBoxContainer/UserInfo");
		
  // Connect button signals
	loginButton.Pressed += OnLoginPressed;
	logoutButton.Pressed += OnLogoutPressed;
	
// Choose the content thats visible based on wheather the user is logged in or not
	UpdateUIBasedOnAuth();
  }

private void UpdateUIBasedOnAuth()
{
	if (_isLoggedIn)
	{
		loginButton.Visible = false;
		logoutButton.Visible = true;

	}
	else
	{
		loginButton.Visible = true;
		logoutButton.Visible = false;
		userInfoLabel.SetText("User not logged in!");
	}
}


  private async void OnLoginPressed()
  {
	GD.Print("Log in Button pressed!");
	
	 string authUrl = $"{_authUrl}?" +
					$"client_id={_clientId}" +
					$"&response_type=code" +
					$"&scope=openid profile email" +
					$"&redirect_uri={HttpUtility.UrlEncode(_redirectUri)}" +
					$"&code_challenge={_codeChallenge}" +
					$"&code_challenge_method=S256" +
					$"&state={_state}";

	OS.ShellOpen(authUrl);
	
	string code = await ListenForAuthCode();
	GD.Print("Auth code: " + code);

	var tokens = await ExchangeCodeForTokens(code);
	GD.Print("Access Token: " + tokens.access_token);
	GD.Print("ID Token: " + tokens.id_token);
	_id_token = tokens.id_token;
	
	var userInfo = await FetchUserProfile(tokens.access_token);
	string userName = "";
	string userEmail = "";
	
	var userInfoJson = System.Text.Json.JsonDocument.Parse(userInfo).RootElement;
	userEmail = userInfoJson.GetProperty("email").GetString();
	userName = userInfoJson.GetProperty("name").GetString();
	
	userInfoLabel.SetText("User logged in! \n Username - " + userName + "\nEmail: " + userEmail);
	
	// User logged in - now changing the screen
	_isLoggedIn = true;
 	UpdateUIBasedOnAuth();
  }

private async Task<string> ListenForAuthCode()
{
	var listener = new HttpListener();
	listener.Prefixes.Add("http://localhost:3000/callback/");
	listener.Start();

	var context = await listener.GetContextAsync();
	string code = HttpUtility.ParseQueryString(context.Request.Url.Query).Get("code");

	byte[] responseBytes = Encoding.UTF8.GetBytes("Descope login successful. You may close this tab.");
	context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
	context.Response.Close();
	listener.Stop();

	return code;
}

private async Task<(string access_token, string id_token, string refresh_token)> ExchangeCodeForTokens(string code)
{
	var client = new System.Net.Http.HttpClient();

	// Define a dictionary to store token data into
	var tokenData = new Dictionary<string, string>
	{
		{ "grant_type", "authorization_code" },
		{ "code", code }, 
		{ "redirect_uri", _redirectUri },  
		{ "client_id", _clientId },
		{ "code_verifier", _codeVerifier }
	};

	// Get the response from the token endpoint
	var content = new FormUrlEncodedContent(tokenData);
	var response = await client.PostAsync(_tokenUrl, content);

	// Read the response as a string
	string json = await response.Content.ReadAsStringAsync();

	// Convert the response to JSON format
	var result = System.Text.Json.JsonDocument.Parse(json).RootElement;

	// The function returns a tuple containing three tokens (strings)
	return (
		result.GetProperty("access_token").GetString(),
		result.GetProperty("id_token").GetString(),
		result.TryGetProperty("refresh_token", out var refresh) ? refresh.GetString() : null
	);
}

private async Task<string> FetchUserProfile(string accessToken)
{
	var client = new System.Net.Http.HttpClient();
	client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

	var response = await client.GetAsync("https://api.descope.com/oauth2/v1/userinfo");
	return await response.Content.ReadAsStringAsync();
}

private void OnLogoutPressed()
  {
	GD.Print("Log out Button pressed!");
	
	 string authUrl = $"{_logoutUrl}?" +
					$"id_token_hint={_id_token}" +
					$"&post_logout_redirect_uri={HttpUtility.UrlEncode(_redirectLogoutUri)}";

	OS.ShellOpen(authUrl);
	
	ListenForLogout();
  }

private async void ListenForLogout()
{
	var listener = new HttpListener();
	listener.Prefixes.Add("http://localhost:3000/logout-callback/");
	listener.Start();

	var context = await listener.GetContextAsync();

	byte[] responseBytes = Encoding.UTF8.GetBytes("You have been logged out.");
	context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
	context.Response.Close();
	listener.Stop();

	// Clear local tokens or update UI
	_isLoggedIn = false;
	UpdateUIBasedOnAuth();
}

}
