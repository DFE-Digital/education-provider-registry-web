using DfE.EducationProviderRegistry.Web.Mvc.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions;

public sealed class DatabaseExtensionsTests
{
    [Fact]
    public void CheckConnectionStringValue_ThrowsArgumentNullException_WhenConnectionStringIsNull()
    {
        // arrange
        string? connectionString = null;

        // act/assert
        Assert.Throws<ArgumentNullException>(
            () => connectionString!.CheckConnectionStringValue());
    }

    [Fact]
    public void CheckConnectionStringValue_DoesNotThrow_WhenConnectionStringIsValid()
    {
        // arrange
        const string connectionString =
            "Host=localhost;Database=test;Username=user;Password=password";

        // act
        Exception? exception = Record.Exception(
            () => connectionString.CheckConnectionStringValue());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void CheckConnectionStringValue_ThrowsInvalidOperationException_WhenConnectionStringIsInvalid()
    {
        // arrange
        const string connectionString =
            "Host=localhost;Port=invalid";

        // act
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => connectionString.CheckConnectionStringValue());

        // assert
        Assert.Equal(
            "The PostgreSQL connection string is invalid.",
            exception.Message);

        Assert.IsType<ArgumentException>(
            exception.InnerException);
    }
}