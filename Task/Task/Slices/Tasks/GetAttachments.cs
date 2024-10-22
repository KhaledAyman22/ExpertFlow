using System.IO.Compression;
using System.Security.Claims;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities;

namespace TaskManager.Slices.Tasks;

public static class GetAttachments
{
    
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("tasks/{id}/attachments", Handler).WithTags("Comments")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handler(Guid id, AppDbContext context)
    {
        var attachments = await context.Attachments.Where(a => a.TaskId == id)
            .Select(a => a.File)
            .ToListAsync();

        var archiveStream = await PackageFiles(attachments);

        return Results.File(archiveStream, "application/zip", $"attachments.zip");
    }

    private static async Task<Stream> PackageFiles(List<byte[]> files)
    {
        var memoryStream = new MemoryStream();

        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
        
        var i = 0;
        foreach (var file in files)
        {
            var entry = archive.CreateEntry(Path.GetFileName($"{i++}.txt"));

            await using var entryStream = entry.Open();
            await entryStream.WriteAsync(file);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;

    }
}