using Microsoft.IdentityModel.Tokens;

namespace TrashTracker.Web.Utils
{

    /// <summary>
    /// A builder for hierarchy between roles with policies
    /// </summary>
    public static class PolicyBuilder
    {
        /// <summary>
        /// Add policies which are built from <typeparamref name="TRoles"/>
        /// containing the roles in ascending order by desired hierarchy
        /// </summary>
        /// <typeparam name="TRoles">The enum's type that will be used to build the policies</typeparam>
        /// <param name="builder">The web application's builder to build the policies into</param>
        public static void BuildPolicies<TRoles>(WebApplicationBuilder builder)
        {
            foreach (var role in Enum.GetValues(typeof(TRoles)))
            {
                List<string> superiorRoles = [];

                foreach (var otherRole in Enum.GetValues(typeof(TRoles)))
                    if ((Int32)role <= (Int32)otherRole)
                        superiorRoles.Add(otherRole.ToString()!);

                if (!superiorRoles.IsNullOrEmpty())
                    builder.Services.AddAuthorization(options =>
                    {
                        options.AddPolicy(role.ToString()!,
                            policy => policy.RequireRole(superiorRoles));
                    });

            }
        }
    }
}
