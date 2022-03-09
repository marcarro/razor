﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

#nullable disable

using System;
using System.Composition;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.VisualStudio.Editor.Razor;

namespace Microsoft.VisualStudio.Mac.LanguageServices.Razor.Editor
{
    [Shared]
    [ExportLanguageServiceFactory(typeof(VisualStudioCompletionBroker), RazorLanguage.Name, ServiceLayer.Default)]
    internal class DefaultVisualStudioCompletionBrokerFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices languageServices!!)
        {
            return new DefaultVisualStudioCompletionBroker();
        }
    }
}
