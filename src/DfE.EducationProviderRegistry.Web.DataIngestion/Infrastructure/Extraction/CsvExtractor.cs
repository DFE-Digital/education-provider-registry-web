using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Models;
using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Extraction;

/// <summary>
/// Extracts establishment data from the GIAS daily extract CSV file and maps it
/// into canonical <see cref="EstablishmentReadModel"/> instances.
/// </summary>
/// <remarks>
/// This extractor uses CsvHelper with a strongly typed <see cref="GiasEstablishmentMap"/>
/// to safely parse the GIAS CSV, applies validation and normalisation rules,
/// and generates deterministic GUIDs based on URN values.
/// </remarks>
public sealed class CsvExtractor : IEstablishmentDataExtractor
{
    private readonly string _csvFilePath;
    private readonly CsvConfiguration _csvConfig;
    private readonly ILogger<CsvExtractor> _logger;

    /// <summary>
    /// Creates a new instance of the <see cref="CsvExtractor"/> class.
    /// </summary>
    /// <param name="csvFilePath">The absolute path to the GIAS daily extract CSV file.</param>
    /// <param name="logger">The logger used to record extraction diagnostics.</param>
    public CsvExtractor(
        string csvFilePath,
        ILogger<CsvExtractor> logger)
    {
        _csvFilePath = csvFilePath;
        _logger = logger;

        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            BadDataFound = null,
            HeaderValidated = null,
            DetectDelimiter = true
        };
    }

    /// <summary>
    /// Extracts establishment data from the GIAS CSV file and returns canonical
    /// <see cref="EstablishmentReadModel"/> instances.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the extraction operation.</param>
    /// <returns>A collection of canonical establishment read models.</returns>
    public async Task<IEnumerable<EstablishmentReadModel>> ExtractAsync(
        CancellationToken cancellationToken)
    {
        List<EstablishmentReadModel> results = [];

        using (FileStream stream = new(
            _csvFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.Asynchronous))
        {
            using StreamReader reader = new(stream, Encoding.UTF8);
            using CsvReader csv = new(reader, _csvConfig);
            csv.Context.RegisterClassMap<GiasEstablishmentMap>();

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                GiasEstablishmentRow row;

                try
                {
                    row = csv.GetRecord<GiasEstablishmentRow>();
                }
                catch (TypeConverterException ex)
                {
                    _logger.LogWarning("Skipping malformed row: {Message}", ex.Message);
                    continue;
                }

                if (!ValidateRow(row))
                {
                    _logger.LogWarning("Skipping invalid row with URN {Urn}", row.Urn);
                    continue;
                }

                EstablishmentReadModel model = new EstablishmentReadModel
                {
                    Id = CreateDeterministicGuid(row.Urn),
                    Urn = row.Urn,
                    Name = row.Name
                };

                results.Add(model);
            }
        }

        return results;
    }

    /// <summary>
    /// Validates a parsed GIAS CSV row.
    /// </summary>
    private static bool ValidateRow(GiasEstablishmentRow row)
    {
        if (row.Urn <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(row.Name))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a deterministic GUID based on the URN value.
    /// Ensures stable IDs across ETL runs.
    /// </summary>
    private static Guid CreateDeterministicGuid(int urn)
    {
        byte[] bytes = Encoding.UTF8.GetBytes("GIAS-URN-" + urn.ToString(CultureInfo.InvariantCulture));
        byte[] hash = SHA256.HashData(bytes);
        byte[] guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, 16);

        return new Guid(guidBytes);
    }
}
