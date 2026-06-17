namespace ApplicationCore.Authorization;

public enum AppPolicies
{
    AdminOnly,
    ActiveUser
}

public static class AppPoliciesExtensions
{
    public static string Name(this AppPolicies policy) => policy.ToString();
}