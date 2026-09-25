using Microsoft.EntityFrameworkCore;
using VERITAS.Data;
using VERITAS.Models;
using VERITAS.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// RAZOR PAGES
// ============================================================

builder.Services.AddRazorPages();


// ============================================================
// DATABASE
// ============================================================

builder.Services.AddDbContext<VeritasDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "VeritasConnection")));


// ============================================================
// CLAIM DETECTION
// ============================================================

builder.Services.AddScoped<
    IDetectionService,
    ClaimAnalysisService>();


// ============================================================
// MACHINE LEARNING SERVICE
// ============================================================

builder.Services.AddHttpClient<
    IMLClassificationService,
    MLClassificationService>(
    client =>
    {
        client.BaseAddress =
            new Uri("http://127.0.0.1:8001/");
    });


// ============================================================
// EVIDENCE SERVICE
// ============================================================

builder.Services.AddScoped<
    IEvidenceService,
    EvidenceService>();

builder.Services.AddScoped<
    IEvidenceProvider,
    DatabaseEvidenceProvider>();


// ============================================================
// VERIFICATION SERVICE
// ============================================================

builder.Services.AddScoped<
    IVerificationService,
    VerificationService>();


// ============================================================
// EVIDENCE ANALYSIS
// ============================================================

builder.Services.AddScoped<
    IEvidenceAnalysisService,
    EvidenceAnalysisService>();


// ============================================================
// NEWS QUERY
// ============================================================

builder.Services.AddScoped<
    INewsQueryService,
    NewsQueryService>();


// ============================================================
// CLAIM COMPARISON
// ============================================================

builder.Services.AddScoped<
    IClaimComparisonService,
    ClaimComparisonService>();


// ============================================================
// SOURCE ANALYSIS
// ============================================================

builder.Services.AddScoped<
    ISourceAnalysisService,
    SourceAnalysisService>();


// ============================================================
// RESULT SCORING
// IMPORTANT: REQUIRED BY AnalyzeModel
// ============================================================

builder.Services.AddScoped<
    IResultScoringService,
    ResultScoringService>();


// ============================================================
// GNEWS
// ============================================================

builder.Services.AddHttpClient<
    INewsService,
    GNewsEvidenceProvider>(
    client =>
    {
        client.BaseAddress =
            new Uri("https://gnews.io/api/v4/");
    });


// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();


// ============================================================
// DATABASE INITIALIZATION
// ============================================================

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<VeritasDbContext>();

    await dbContext.Database.EnsureCreatedAsync();


    // --------------------------------------------------------
    // WATER EVIDENCE
    // --------------------------------------------------------

    bool waterEvidenceExists =
        await dbContext.EvidenceRecords
            .AnyAsync(e =>
                e.Claim.ToLower() ==
                "water freezes at 0 degrees celsius");

    if (!waterEvidenceExists)
    {
        dbContext.EvidenceRecords.Add(
            new EvidenceRecord
            {
                Claim =
                    "Water freezes at 0 degrees Celsius",

                Evidence =
                    "Pure water freezes at approximately 0°C " +
                    "under standard atmospheric pressure.",

                Source =
                    "Established scientific knowledge",

                EvidenceType =
                    "Supporting",

                RetrievedAt =
                    DateTime.UtcNow
            });
    }


    // --------------------------------------------------------
    // EARTH EVIDENCE
    // --------------------------------------------------------

    bool earthEvidenceExists =
        await dbContext.EvidenceRecords
            .AnyAsync(e =>
                e.Claim.ToLower() ==
                "earth is flat");

    if (!earthEvidenceExists)
    {
        dbContext.EvidenceRecords.Add(
            new EvidenceRecord
            {
                Claim =
                    "Earth is flat",

                Evidence =
                    "Scientific observations, satellite " +
                    "measurements, and astronomical evidence " +
                    "contradict the claim that Earth is flat.",

                Source =
                    "Established scientific evidence",

                EvidenceType =
                    "Contradicting",

                RetrievedAt =
                    DateTime.UtcNow
            });
    }

    await dbContext.SaveChangesAsync();
}


// ============================================================
// ERROR HANDLING
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


// ============================================================
// HTTP PIPELINE
// ============================================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();