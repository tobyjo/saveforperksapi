namespace SaveForPerksAPI.Services;

public interface IAuth0ManagementService
{
    /// <summary>
    /// Deletes a user from Auth0 by their Auth Provider ID (sub claim)
    /// </summary>
    /// <param name="authProviderId">The Auth0 user ID (e.g., auth0|123abc or google-oauth2|456def)</param>
    /// <returns>True if deletion succeeded, false otherwise</returns>
    Task<bool> DeleteUserAsync(string authProviderId);
}
