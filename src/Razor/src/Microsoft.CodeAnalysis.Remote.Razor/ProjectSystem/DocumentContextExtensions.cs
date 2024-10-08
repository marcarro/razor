﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor;
using Microsoft.CodeAnalysis.Razor.ProjectSystem;
using Microsoft.CodeAnalysis.Razor.Workspaces;
using Microsoft.CodeAnalysis.Remote.Razor.ProjectSystem;

namespace Microsoft.CodeAnalysis.Remote.Razor;

internal static class DocumentContextExtensions
{
    public static async Task<Document> GetGeneratedDocumentAsync(
        this VersionedDocumentContext documentContext,
        IFilePathService filePathService,
        CancellationToken cancellationToken)
    {
        Debug.Assert(documentContext.Snapshot is RemoteDocumentSnapshot, "This method only works on document contexts created in the OOP process");

        var snapshot = (RemoteDocumentSnapshot)documentContext.Snapshot;

        return await snapshot.GetOrAddGeneratedDocumentAsync(
            (snapshot, documentContext, filePathService, cancellationToken),
            static async arg =>
            {
                var (snapshot, documentContext, filePathService, cancellationToken) = arg;

                var razorDocument = snapshot.TextDocument;
                var projectKey = snapshot.Project.Key;
                var solution = razorDocument.Project.Solution;

                // TODO: A real implementation needs to get the SourceGeneratedDocument from the solution

                var generatedFilePath = filePathService.GetRazorCSharpFilePath(projectKey, razorDocument.FilePath.AssumeNotNull());
                var generatedDocumentId = solution.GetDocumentIdsWithFilePath(generatedFilePath).First(d => d.ProjectId == razorDocument.Project.Id);
                var generatedDocument = solution.GetRequiredDocument(generatedDocumentId);

                var csharpSourceText = await documentContext.GetCSharpSourceTextAsync(cancellationToken).ConfigureAwait(false);

                // HACK: We're not in the same solution fork as the LSP server that provides content for this document
                return generatedDocument.WithText(csharpSourceText);
            });
    }
}
