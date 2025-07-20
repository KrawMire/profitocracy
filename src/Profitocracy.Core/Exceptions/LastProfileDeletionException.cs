namespace Profitocracy.Core.Exceptions;

public class LastProfileDeletionException : Exception
{
    public string ProfileName { get; }

    public LastProfileDeletionException(string profileName)
    {
        ProfileName = profileName;
    }
}