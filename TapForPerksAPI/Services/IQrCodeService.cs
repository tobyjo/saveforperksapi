namespace TapForPerksAPI.Services;

public interface IQrCodeService
{
    string GenerateQrCodeValue();
    Task<bool> IsQrCodeUniqueAsync(string qrCodeValue);
}