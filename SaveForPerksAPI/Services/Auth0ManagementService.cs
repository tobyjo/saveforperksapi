using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using System.Text.Json.Serialization;

namespace SaveForPerksAPI.Services;

public class Auth0ManagementService : IAuth0ManagementService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Auth0ManagementService> _logger;
    private readonly string _domain;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public Auth0ManagementService(
        IConfiguration configuration,
        ILogger<Auth0ManagementService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _domain = _configuration["Auth0:Domain"] 
            ?? throw new InvalidOperationException("Auth0:Domain not configured");
        _clientId = _configuration["Auth0:ManagementApi:ClientId"] 
            ?? throw new InvalidOperationException("Auth0:ManagementApi:ClientId not configured");
        _clientSecret = _configuration["Auth0:ManagementApi:ClientSecret"] 
            ?? throw new InvalidOperationException("Auth0:ManagementApi:ClientSecret not configured");
    }

    public async Task<bool> DeleteUserAsync(string authProviderId)
    {
        if (string.IsNullOrWhiteSpace(authProviderId))
        {
            _logger.LogWarning("DeleteUserAsync called with empty authProviderId");
            return false;
        }

        try
        {
            _logger.LogInformation(
                "Attempting to delete user from Auth0. AuthProviderId: {AuthProviderId}",
                authProviderId);

            // Get Management API access token
            var token = await GetManagementApiTokenAsync();

            // Create Management API client
            var client = new ManagementApiClient(token, _domain);

            // Delete the user from Auth0
            // Auth0 user IDs are in format: auth0|123abc or google-oauth2|456def
            await client.Users.DeleteAsync(authProviderId);

            _logger.LogInformation(
                "Successfully deleted user from Auth0. AuthProviderId: {AuthProviderId}",
                authProviderId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete user from Auth0. AuthProviderId: {AuthProviderId}, Error: {Error}",
                authProviderId, ex.Message);
            return false;
        }
    }

    private async Task<string> GetManagementApiTokenAsync()
    {
        try
        {
            using var httpClient = new HttpClient();

            var tokenRequest = new
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                audience = $"https://{_domain}/api/v2/",
                grant_type = "client_credentials"
            };

            var tokenUrl = $"https://{_domain}/oauth/token";

            _logger.LogDebug(
                "Requesting Auth0 Management API token. URL: {Url}, ClientId: {ClientId}, Audience: {Audience}",
                tokenUrl, _clientId, $"https://{_domain}/api/v2/");

            var response = await httpClient.PostAsJsonAsync(tokenUrl, tokenRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Auth0 token request failed. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, errorContent);
                throw new InvalidOperationException(
                    $"Auth0 token request failed with status {response.StatusCode}: {errorContent}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

            if (tokenResponse?.AccessToken == null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Auth0 token response missing access_token. Response: {Response}",
                    responseContent);
                throw new InvalidOperationException(
                    $"Auth0 token response missing access_token. Response: {responseContent}");
            }

            _logger.LogDebug("Successfully obtained Auth0 Management API token");

            return tokenResponse.AccessToken;
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx,
                "HTTP request to Auth0 token endpoint failed. Domain: {Domain}, Error: {Error}",
                _domain, httpEx.Message);
            throw new InvalidOperationException(
                $"Failed to connect to Auth0 at {_domain}. Check Domain configuration.", httpEx);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex,
                "Unexpected error obtaining Auth0 Management API token. Error: {Error}",
                ex.Message);
            throw new InvalidOperationException(
                "Failed to obtain Auth0 Management API token", ex);
        }
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }
}
