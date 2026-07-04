using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.LogicalOperators;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class LogicalOperatorFactoryTestDouble
{
    public static Mock<ILogicalOperatorFactory> Mock() =>
        new(MockBehavior.Strict);

    public static (Mock<ILogicalOperatorFactory> factory, Mock<ILogicalOperator> op)
    MockFor(string opKey, string opExpression)
    {
        var opMock = new Mock<ILogicalOperator>(MockBehavior.Strict);
        opMock.Setup(o => o.GetOperatorExpression()).Returns(opExpression);

        var factoryMock = new Mock<ILogicalOperatorFactory>(MockBehavior.Strict);
        factoryMock.Setup(f => f.CreateLogicalOperator(opKey))
                   .Returns(opMock.Object);

        return (factoryMock, opMock);
    }

}
