using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments.Mappers;

public sealed class EstablishmentDetailsPageViewModelMapperTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenBasicMapperIsNull()
    {
        // Arrange
        Mock<IMapper<IEnumerable<GovernorModel>, GovUkTable>> governorMapper = new();

        // Act
        Func<EstablishmentDetailsPageViewModelMapper> construct = () =>
            new EstablishmentDetailsPageViewModelMapper(
                basicMapper: null!,
                governorMapper: governorMapper.Object);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenGovernorMapperIsNull()
    {
        // Arrange
        Mock<IMapper<EstablishmentDetailsModel, GovUkTable>> basicMapper = new();

        // Act
        Func<EstablishmentDetailsPageViewModelMapper> construct = () =>
            new EstablishmentDetailsPageViewModelMapper(
                basicMapper: basicMapper.Object,
                governorMapper: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenModelIsNull()
    {
        // Arrange
        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper();

        // Act
        Func<EstablishmentDetailsPageViewModel> map = () =>
            mapper.Map(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_ReturnsViewModelWithHeadingAndMappedTables()
    {
        // Arrange
        GovernorModel[] governors = [];

        EstablishmentDetailsModel model = new()
        {
            Urn = EstablishmentUrnModel.Create("123456"),
            Name = new EstablishmentNameModel("testEstablishmentName"),
            Governors = governors
        };

        GovUkTable basicDetailsTable = CreateTable();
        GovUkTable governorsTable = CreateTable();

        Mock<IMapper<EstablishmentDetailsModel, GovUkTable>> basicMapper = new();

        basicMapper
            .Setup(mapper => mapper.Map(model))
            .Returns(basicDetailsTable);

        Mock<IMapper<IEnumerable<GovernorModel>, GovUkTable>> governorMapper = new();

        governorMapper
            .Setup(mapper => mapper.Map(governors))
            .Returns(governorsTable);

        EstablishmentDetailsPageViewModelMapper mapper = new(
            basicMapper.Object,
            governorMapper.Object);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.Equal("testEstablishmentName", result.Heading);
        Assert.Same(basicDetailsTable, result.BasicDetails);
        Assert.Same(governorsTable, result.Governors);

        basicMapper.Verify(
            candidate => candidate.Map(model),
            Times.Once);

        governorMapper.Verify(
            candidate => candidate.Map(governors),
            Times.Once);
    }

    private static EstablishmentDetailsPageViewModelMapper CreateMapper()
    {
        Mock<IMapper<EstablishmentDetailsModel, GovUkTable>> basicMapper = new();
        Mock<IMapper<IEnumerable<GovernorModel>, GovUkTable>> governorMapper = new();

        return new EstablishmentDetailsPageViewModelMapper(
            basicMapper.Object,
            governorMapper.Object);
    }

    private static GovUkTable CreateTable()
    {
        return new GovUkTable(
            columns:
            [
                new TableColumn()
            ],
            rows: []);
    }
}