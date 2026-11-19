using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HjemIKroppenBooking.Api.OpenApi;

internal sealed class DocumentInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Hjem i Kroppen Booking API",
            Version = "v1",
            Description = "Booking of dance, conversation and group sessions. All times are UTC (ISO 8601). Errors are problem details with a stable `code` and a Danish `detail`."
        };
        return Task.CompletedTask;
    }
}
