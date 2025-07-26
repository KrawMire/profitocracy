using Profitocracy.Core.Integrations;

namespace Profitocracy.Infrastructure.Integrations;

public class SecurityProvider : ISecurityProvider
{
    public Task<bool> ValidatePassword(string password)
    {
        if (password == "1234")
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
