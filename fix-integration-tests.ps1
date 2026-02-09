# PowerShell script to fix remaining integration tests
# This adds businessUser to all ProcessScanAndRewardsAsync calls

$filePath = "SaveForPerksAPI.Tests\Integration\Services\RewardServiceIntegrationTests.cs"
$content = Get-Content $filePath -Raw

# Pattern 1: Tests that create user and reward but not businessUser
# Find: var user = await _testData.CreateUser(...);
#       var reward = await _testData.CreateReward(...);
# Replace: Add businessUser creation and pass to CreateReward

# Pattern 2: ProcessScanAndRewardsAsync(request) → ProcessScanAndRewardsAsync(businessUser.BusinessId, businessUser.Id, request)
$content = $content -replace 'await _fixture\.Service\.ProcessScanAndRewardsAsync\(request\)', 'await _fixture.Service.ProcessScanAndRewardsAsync(businessUser.BusinessId, businessUser.Id, request)'
$content = $content -replace 'await _fixture\.Service\.ProcessScanAndRewardsAsync\(new ScanEventForCreationDto', 'await _fixture.Service.ProcessScanAndRewardsAsync(businessUser.BusinessId, businessUser.Id, new ScanEventForCreationDto'

# Pattern 3: Add businessUser = await _testData.CreateRewardOwner(); before reward creation
# This is complex, so we'll note it for manual fix

Write-Host "Replaced ProcessScanAndRewardsAsync calls"
Write-Host "Note: You still need to manually add:"
Write-Host "  1. var businessUser = await _testData.CreateRewardOwner();"
Write-Host "  2. Pass businessUser to CreateReward() calls"
Write-Host ""
Write-Host "Review the file before saving!"

# Save to a temp file for review
$content | Set-Content "$filePath.fixed"
Write-Host "Output saved to: $filePath.fixed"
Write-Host "Review and then rename to replace original"
