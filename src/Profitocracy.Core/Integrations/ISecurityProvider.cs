namespace Profitocracy.Core.Integrations;

public interface ISecurityProvider
{
    Task<bool> ValidatePassword(string password);
}
