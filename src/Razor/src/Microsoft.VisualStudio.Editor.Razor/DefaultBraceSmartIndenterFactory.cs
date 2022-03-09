﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

#nullable disable

using System;
using Microsoft.CodeAnalysis.Razor.Workspaces.Extensions;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Threading;

namespace Microsoft.VisualStudio.Editor.Razor
{
    internal class DefaultBraceSmartIndenterFactory : BraceSmartIndenterFactory
    {
        private readonly IEditorOperationsFactoryService _editorOperationsFactory;
        private readonly JoinableTaskContext _joinableTaskContext;
        private readonly TextBufferCodeDocumentProvider _codeDocumentProvider;

        public DefaultBraceSmartIndenterFactory(
            JoinableTaskContext joinableTaskContext!!,
            TextBufferCodeDocumentProvider codeDocumentProvider!!,
            IEditorOperationsFactoryService editorOperationsFactory!!)
        {
            _joinableTaskContext = joinableTaskContext;
            _codeDocumentProvider = codeDocumentProvider;
            _editorOperationsFactory = editorOperationsFactory;
        }

        public override BraceSmartIndenter Create(VisualStudioDocumentTracker documentTracker!!)
        {
            _joinableTaskContext.AssertUIThread();

            var braceSmartIndenter = new BraceSmartIndenter(_joinableTaskContext, documentTracker, _codeDocumentProvider, _editorOperationsFactory);

            return braceSmartIndenter;
        }
    }
}
