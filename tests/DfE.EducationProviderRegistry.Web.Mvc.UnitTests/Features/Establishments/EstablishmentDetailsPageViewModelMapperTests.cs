using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments;

public sealed class EstablishmentDetailsPageViewModelMapperTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenBasicMapperIsNull()
    {
        // Arrange
        TimeProvider timeProvider = TimeProvider.System;

        // Act
        Func<EstablishmentDetailsPageViewModelMapper> construct = () =>
            new EstablishmentDetailsPageViewModelMapper(
                basicMapper: null!,
                timeProvider);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenTimeProviderIsNull()
    {
        // Arrange
        Mock<IMapper<EstablishmentDetailsModel, GovUkTable>> basicMapper = new();

        // Act
        Func<EstablishmentDetailsPageViewModelMapper> construct = () =>
            new EstablishmentDetailsPageViewModelMapper(
                basicMapper.Object,
                null!);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenModelIsNull()
    {
        // Arrange
        EstablishmentDetailsPageViewModelMapper mapper =
            CreateMapper(new DateOnly(2026, 11, 27));

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
        EstablishmentDetailsModel model = new()
        {
            Identifiers = new EstablishmentIdentifiersModel(
                EstablishmentUrnModel.Create("123456"),
                new Ukprn("12345678"),
                new DfeNumber("123/4567")),
            Name = new EstablishmentNameModel("testEstablishmentName"),
            Governors = []
        };

        GovUkTable basicDetailsTable = CreateTable();

        Mock<IMapper<EstablishmentDetailsModel, GovUkTable>> basicMapper = new();

        basicMapper
            .Setup(m => m.Map(model))
            .Returns(basicDetailsTable);

        EstablishmentDetailsPageViewModelMapper mapper = new(
            basicMapper.Object,
            CreateTimeProvider(new DateTimeOffset(2026, 11, 27, 0, 0, 0, TimeSpan.Zero)));

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.Equal("testEstablishmentName", result.Heading);
        Assert.Same(basicDetailsTable, result.BasicDetailsTable);

        Assert.NotNull(result.GovernorsTable);
        Assert.NotNull(result.HistoricGovernorsTable);

        Assert.Equal("Board of governors", result.GovernorsTable.Caption);
        Assert.Equal(
            "Governors who have left within the last 12 months",
            result.HistoricGovernorsTable.Caption);

        basicMapper.Verify(
            candidate => candidate.Map(model),
            Times.Once);
    }

    [Fact]
    public void Map_PopulatesActiveGovernorsTable()
    {
        // Arrange
        DateOnly today = new(2026, 11, 27);

        GovernorModel governor = CreateGovernor(
            "Title",
            "John",
            "Smith",
            startDate: today.AddYears(-2),
            endDate: null,
            roleOrder: 1);

        EstablishmentDetailsModel model = CreateModel(governor);

        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper(today);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.NotNull(result.GovernorsTable);
        Assert.Single(result.GovernorsTable.Rows);

        Assert.Equal(
            "Title John Smith (Governor)",
            result.GovernorsTable.Rows[0].Cells[0].Text);

        Assert.Equal(
            $"From {today.AddYears(-2):d MMMM yyyy}",
            result.GovernorsTable.Rows[0].Cells[3].Text);
    }

    [Fact]
    public void Map_PopulatesHistoricGovernorsTable_ForGovernorsWhoLeftWithinLastTwelveMonths()
    {
        // Arrange
        DateOnly today = new(2026, 11, 27);

        GovernorModel governor = CreateGovernor(
            "Title",
            "Jane",
            "Jones",
            startDate: today.AddYears(-3),
            endDate: today.AddMonths(-3),
            roleOrder: 1);

        EstablishmentDetailsModel model = CreateModel(governor);

        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper(today);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.NotNull(result.HistoricGovernorsTable);
        Assert.Single(result.HistoricGovernorsTable.Rows);

        Assert.Equal(
            "Title Jane Jones (Governor)",
            result.HistoricGovernorsTable.Rows[0].Cells[0].Text);

        Assert.Equal(
            $"{today.AddYears(-3):d MMMM yyyy} to {today.AddMonths(-3):d MMMM yyyy}",
            result.HistoricGovernorsTable.Rows[0].Cells[3].Text);
    }

    [Fact]
    public void Map_ExcludesGovernorsWhoLeftMoreThanTwelveMonthsAgo()
    {
        // Arrange
        DateOnly today = new(2026, 11, 27);

        GovernorModel governor = CreateGovernor(
            "Title",
            "Old",
            "Governor",
            startDate: today.AddYears(-5),
            endDate: today.AddYears(-2),
            roleOrder: 1);

        EstablishmentDetailsModel model = CreateModel(governor);

        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper(today);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.NotNull(result.GovernorsTable);
        Assert.Empty(result.GovernorsTable.Rows);
        Assert.NotNull(result.HistoricGovernorsTable);
        Assert.Empty(result.HistoricGovernorsTable.Rows);
    }

    [Fact]
    public void Map_IncludesGovernorWhoLeftExactlyTwelveMonthsAgo()
    {
        // Arrange
        DateOnly today = new(2026, 11, 27);

        GovernorModel governor = CreateGovernor(
            "Title",
            "Boundary",
            "Governor",
            today.AddYears(-5),
            today.AddYears(-1),
            1);

        EstablishmentDetailsModel model = CreateModel(governor);

        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper(today);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.NotNull(result.HistoricGovernorsTable);
        Assert.Single(result.HistoricGovernorsTable.Rows);
    }

    [Fact]
    public void Map_DoesNotIncludeGovernorWhoLeavesTodayInHistoricTable()
    {
        // Arrange
        DateOnly today = new(2026, 11, 27);

        GovernorModel governor = CreateGovernor(
            "Title",
            "Today",
            "Governor",
            today.AddYears(-2),
            today,
            1);

        EstablishmentDetailsModel model = CreateModel(governor);

        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper(today);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.NotNull(result.HistoricGovernorsTable);
        Assert.Empty(result.HistoricGovernorsTable.Rows);
    }

    [Fact]
    public void Map_OrdersGovernorsByRoleOrder()
    {
        // Arrange
        DateOnly today = new(2026, 11, 27);

        GovernorModel governor2 = CreateGovernor(
            "Title",
            "Second",
            "Governor",
            today.AddYears(-1),
            null,
            roleOrder: 2);

        GovernorModel governor1 = CreateGovernor(
            "Title",
            "First",
            "Governor",
            today.AddYears(-1),
            null,
            roleOrder: 1);

        EstablishmentDetailsModel model = CreateModel(governor2, governor1);

        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper(today);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.Equal(
            "Title First Governor (Governor)",
            result.GovernorsTable?.Rows[0].Cells[0].Text);

        Assert.Equal(
            "Title Second Governor (Governor)",
            result.GovernorsTable?.Rows[1].Cells[0].Text);
    }

    [Fact]
    public void Map_IncludesGovernorWhoseTermEndsTodayInActiveGovernors()
    {
        // Arrange
        DateOnly today = new(2026, 11, 27);

        GovernorModel governor = CreateGovernor(
            "Title",
            "John",
            "Smith",
            today.AddYears(-1),
            today,
            1);

        EstablishmentDetailsModel model = CreateModel(governor);

        EstablishmentDetailsPageViewModelMapper mapper = CreateMapper(today);

        // Act
        EstablishmentDetailsPageViewModel result = mapper.Map(model);

        // Assert
        Assert.NotNull(result.GovernorsTable);
        Assert.Single(result.GovernorsTable.Rows);
        Assert.NotNull(result.HistoricGovernorsTable);
        Assert.Empty(result.HistoricGovernorsTable.Rows);
    }

    private static EstablishmentDetailsPageViewModelMapper CreateMapper(DateOnly today)
    {
        Mock<IMapper<EstablishmentDetailsModel, GovUkTable>> basicMapper = new();

        basicMapper
            .Setup(x => x.Map(It.IsAny<EstablishmentDetailsModel>()))
            .Returns(CreateTable());

        return new EstablishmentDetailsPageViewModelMapper(
            basicMapper.Object,
            CreateTimeProvider(today.ToDateTime(TimeOnly.MinValue)));
    }

    private static TimeProvider CreateTimeProvider(DateTime date)
    {
        Mock<TimeProvider> timeProvider = new();

        timeProvider
            .Setup(x => x.GetUtcNow())
            .Returns(new DateTimeOffset(date, TimeSpan.Zero));

        return timeProvider.Object;
    }

    private static TimeProvider CreateTimeProvider(DateTimeOffset date)
    {
        Mock<TimeProvider> timeProvider = new();

        timeProvider
            .Setup(x => x.GetUtcNow())
            .Returns(date);

        return timeProvider.Object;
    }

    private static EstablishmentDetailsModel CreateModel(
        params GovernorModel[] governors)
    {
        return new EstablishmentDetailsModel
        {
            Identifiers = new EstablishmentIdentifiersModel(
                EstablishmentUrnModel.Create("123456"),
                new Ukprn("12345678"),
                new DfeNumber("123/4567")),
            Name = new EstablishmentNameModel("Test Establishment"),
            Governors = governors
        };
    }

    private static GovernorModel CreateGovernor(
        string title,
        string firstName,
        string lastName,
        DateOnly startDate,
        DateOnly? endDate,
        int roleOrder)
    {
        return new GovernorModel(
            Identifier: new GovernanceIdentifier("12345"),
            Title: title,
            Firstname: firstName,
            Lastname: lastName,
            Role: "Governor",
            RoleOrder: roleOrder,
            AppointedBy: "Trust",
            StartDate: startDate,
            EndDate: endDate);
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
