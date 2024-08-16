﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Razor.Protocol;
using Microsoft.CodeAnalysis.Razor.Protocol.CodeActions;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using StreamJsonRpc;

namespace Microsoft.VisualStudio.Razor.LanguageClient.Endpoints;

internal partial class RazorCustomMessageTarget
{
    [JsonRpcMethod(CustomMessageNames.RazorComponentInfoEndpointName, UseSingleObjectParameterDeserialization = true)]
    public async Task<RazorComponentInfo?> RazorComponentInfoAsync(RazorComponentInfoParams request, CancellationToken cancellationToken)
    {
        var (synchronized, virtualDocumentSnapshot) = await TrySynchronizeVirtualDocumentAsync<CSharpVirtualDocumentSnapshot>(request.HostDocumentVersion, request.Document, cancellationToken);
        if (!synchronized || virtualDocumentSnapshot is null)
        {
            return null;
        }

        request.Document.Uri = virtualDocumentSnapshot.Uri;

        // This endpoint is special because it deals with a file that doesn't exist yet, so there is no document syncing necessary!
        var response = await _requestInvoker.ReinvokeRequestOnServerAsync<RazorComponentInfoParams, RazorComponentInfo?>(
            RazorLSPConstants.RoslynRazorComponentInfoEndpointName,
            RazorLSPConstants.RazorCSharpLanguageServerName,
            request,
            cancellationToken).ConfigureAwait(false);

        return response.Result;
    }
}