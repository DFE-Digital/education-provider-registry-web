using Npgsql;

namespace DfE.EducationProviderRegistry.Web.Mvc.Extensions
{
    public static class DatabaseExtensions
    {
        public static void CheckConnectionStringValue(this string connectionString)
        {
            try
            {
                _ = new NpgsqlConnectionStringBuilder(connectionString);
            }
            catch (ArgumentException exception)
            {
                throw new InvalidOperationException(
                    "The PostgreSQL connection string is invalid.",
                    exception);
            }
        }
    }
}
