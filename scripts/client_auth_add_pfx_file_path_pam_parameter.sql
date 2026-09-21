-- This script has been tested against Keyfactor Command 25.4. Script is subject to change depending on Keyfactor Command version.
-- Add PfxFilePath parameter to PAM configuration
DECLARE @ProviderTypeName NVARCHAR(255) = 'CyberArk-ClientAuth-CentralCredentialProvider';
DECLARE @ParameterName NVARCHAR(255) = 'PfxFilePath';
DECLARE @ParameterDisplayName NVARCHAR(255) = 'PFX File Path';
DECLARE @ParameterDataType INT = 1; -- 1 = String, 2 = Secret
DECLARE @ParameterInstanceLevel INT = 0; -- 0 = Initialization Info, 1 = Instance Info

-- Calculated value
DECLARE @ProviderTypeId UNIQUEIDENTIFIER;

SELECT @ProviderTypeId = Id FROM [pam].[ProviderTypes] WHERE Name = @ProviderTypeName;
IF @ProviderTypeId IS NULL
BEGIN
    RAISERROR('Provider type ''%s'' not found.', 16, 1, @ProviderTypeName);
    RETURN;
END

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM [pam].[ProviderTypeParams] WHERE ProviderTypeId = @ProviderTypeId AND Name = @ParameterName)
BEGIN
    INSERT INTO [pam].[ProviderTypeParams] (ProviderTypeId, Name, DisplayName, DataType, InstanceLevel)
    VALUES (@ProviderTypeId, @ParameterName, @ParameterDisplayName, @ParameterDataType, @ParameterInstanceLevel);
    PRINT 'Parameter ''' + @ParameterName + ''' added to provider type ''' + @ProviderTypeName + '''.';
    COMMIT TRANSACTION;
END
ELSE
BEGIN
    PRINT 'Parameter ''' + @ParameterName + ''' already exists for provider type ''' + @ProviderTypeName + '''.';
    ROLLBACK TRANSACTION;
END