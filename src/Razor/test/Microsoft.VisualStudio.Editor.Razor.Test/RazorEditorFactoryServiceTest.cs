﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Razor.Test.Common;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.VisualStudio.Editor.Razor;

public class RazorEditorFactoryServiceTest(ITestOutputHelper testOutput) : ToolingTestBase(testOutput)
{
    private static readonly IContentType s_razorCoreContentType = Mock.Of<IContentType>(
        c => c.IsOfType(RazorLanguage.CoreContentType) == true,
        MockBehavior.Strict);
    private static readonly IContentType s_nonRazorCoreContentType = Mock.Of<IContentType>(
        c => c.IsOfType(It.IsAny<string>()) == false,
        MockBehavior.Strict);

    [Fact]
    public void TryGetDocumentTracker_ForRazorTextBuffer_ReturnsTrue()
    {
        // Arrange
        var expectedDocumentTracker = Mock.Of<IVisualStudioDocumentTracker>(MockBehavior.Strict);
        var factoryService = CreateFactoryService(expectedDocumentTracker);
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryGetDocumentTracker(textBuffer, out var documentTracker);

        // Assert
        Assert.True(result);
        Assert.Same(expectedDocumentTracker, documentTracker);
    }

    [Fact]
    public void TryGetDocumentTracker_NonRazorBuffer_ReturnsFalse()
    {
        // Arrange
        var factoryService = CreateFactoryService();
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_nonRazorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryGetDocumentTracker(textBuffer, out var documentTracker);

        // Assert
        Assert.False(result);
        Assert.Null(documentTracker);
    }

    [Fact]
    public void TryInitializeTextBuffer_StoresTracker_ReturnsTrue()
    {
        // Arrange
        var expectedDocumentTracker = Mock.Of<IVisualStudioDocumentTracker>(MockBehavior.Strict);
        var factoryService = CreateFactoryService(expectedDocumentTracker);
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryInitializeTextBuffer(textBuffer);

        // Assert
        Assert.True(result);
        Assert.True(textBuffer.Properties.TryGetProperty(typeof(IVisualStudioDocumentTracker), out IVisualStudioDocumentTracker documentTracker));
        Assert.Same(expectedDocumentTracker, documentTracker);
    }

    [Fact]
    public void TryInitializeTextBuffer_OnlyStoresTrackerOnTextBufferOnce_ReturnsTrue()
    {
        // Arrange
        var factoryService = CreateFactoryService();
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);
        factoryService.TryInitializeTextBuffer(textBuffer);
        var expectedDocumentTracker = textBuffer.Properties[typeof(IVisualStudioDocumentTracker)];

        // Create a second factory service so it generates a different tracker
        factoryService = CreateFactoryService();

        // Act
        var result = factoryService.TryInitializeTextBuffer(textBuffer);

        // Assert
        Assert.True(result);
        Assert.True(textBuffer.Properties.TryGetProperty(typeof(IVisualStudioDocumentTracker), out IVisualStudioDocumentTracker documentTracker));
        Assert.Same(expectedDocumentTracker, documentTracker);
    }

    [Fact]
    public void TryGetParser_ForRazorTextBuffer_ReturnsTrue()
    {
        // Arrange
        var expectedParser = Mock.Of<IVisualStudioRazorParser>(MockBehavior.Strict);
        var factoryService = CreateFactoryService(parser: expectedParser);
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryGetParser(textBuffer, out var parser);

        // Assert
        Assert.True(result);
        Assert.Same(expectedParser, parser);
    }

    [Fact]
    public void TryGetParser_NonRazorBuffer_ReturnsFalse()
    {
        // Arrange
        var factoryService = CreateFactoryService();
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_nonRazorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryGetParser(textBuffer, out var parser);

        // Assert
        Assert.False(result);
        Assert.Null(parser);
    }

    [Fact]
    public void TryInitializeTextBuffer_StoresParser_ReturnsTrue()
    {
        // Arrange
        var expectedParser = Mock.Of<IVisualStudioRazorParser>(MockBehavior.Strict);
        var factoryService = CreateFactoryService(parser: expectedParser);
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryInitializeTextBuffer(textBuffer);

        // Assert
        Assert.True(result);
        Assert.True(textBuffer.Properties.TryGetProperty(typeof(IVisualStudioRazorParser), out IVisualStudioRazorParser parser));
        Assert.Same(expectedParser, parser);
    }

    [Fact]
    public void TryInitializeTextBuffer_OnlyStoresParserOnTextBufferOnce_ReturnsTrue()
    {
        // Arrange
        var factoryService = CreateFactoryService();
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);
        factoryService.TryInitializeTextBuffer(textBuffer);
        var expectedParser = textBuffer.Properties[typeof(IVisualStudioRazorParser)];

        // Create a second factory service so it generates a different parser
        factoryService = CreateFactoryService();

        // Act
        var result = factoryService.TryInitializeTextBuffer(textBuffer);

        // Assert
        Assert.True(result);
        Assert.True(textBuffer.Properties.TryGetProperty(typeof(IVisualStudioRazorParser), out IVisualStudioRazorParser parser));
        Assert.Same(expectedParser, parser);
    }

    [Fact]
    public void TryGetSmartIndenter_ForRazorTextBuffer_ReturnsTrue()
    {
        // Arrange
        var expectedSmartIndenter = Mock.Of<BraceSmartIndenter>(MockBehavior.Strict);
        var factoryService = CreateFactoryService(smartIndenter: expectedSmartIndenter);
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryGetSmartIndenter(textBuffer, out var smartIndenter);

        // Assert
        Assert.True(result);
        Assert.Same(expectedSmartIndenter, smartIndenter);
    }

    [Fact]
    public void TryGetSmartIndenter_NonRazorBuffer_ReturnsFalse()
    {
        // Arrange
        var factoryService = CreateFactoryService();
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_nonRazorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryGetSmartIndenter(textBuffer, out var smartIndenter);

        // Assert
        Assert.False(result);
        Assert.Null(smartIndenter);
    }

    [Fact]
    public void TryInitializeTextBuffer_StoresSmartIndenter_ReturnsTrue()
    {
        // Arrange
        var expectedSmartIndenter = Mock.Of<BraceSmartIndenter>(MockBehavior.Strict);
        var factoryService = CreateFactoryService(smartIndenter: expectedSmartIndenter);
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);

        // Act
        var result = factoryService.TryInitializeTextBuffer(textBuffer);

        // Assert
        Assert.True(result);
        Assert.True(textBuffer.Properties.TryGetProperty(typeof(BraceSmartIndenter), out BraceSmartIndenter smartIndenter));
        Assert.Same(expectedSmartIndenter, smartIndenter);
    }

    [Fact]
    public void TryInitializeTextBuffer_OnlyStoresSmartIndenterOnTextBufferOnce_ReturnsTrue()
    {
        // Arrange
        var factoryService = CreateFactoryService();
        var textBuffer = Mock.Of<ITextBuffer>(b =>
            b.ContentType == s_razorCoreContentType &&
            b.Properties == new PropertyCollection(),
            MockBehavior.Strict);
        factoryService.TryInitializeTextBuffer(textBuffer);
        var expectedSmartIndenter = textBuffer.Properties[typeof(BraceSmartIndenter)];

        // Create a second factory service so it generates a different smart indenter
        factoryService = CreateFactoryService();

        // Act
        var result = factoryService.TryInitializeTextBuffer(textBuffer);

        // Assert
        Assert.True(result);
        Assert.True(textBuffer.Properties.TryGetProperty(typeof(BraceSmartIndenter), out BraceSmartIndenter smartIndenter));
        Assert.Same(expectedSmartIndenter, smartIndenter);
    }

    private static RazorEditorFactoryService CreateFactoryService(
        IVisualStudioDocumentTracker? documentTracker = null,
        IVisualStudioRazorParser? parser = null,
        BraceSmartIndenter? smartIndenter = null)
    {
        documentTracker ??= Mock.Of<IVisualStudioDocumentTracker>(MockBehavior.Strict);
        parser ??= Mock.Of<IVisualStudioRazorParser>(MockBehavior.Strict);
        smartIndenter ??= Mock.Of<BraceSmartIndenter>(MockBehavior.Strict);

        var documentTrackerFactory = Mock.Of<IVisualStudioDocumentTrackerFactory>(f =>
            f.Create(It.IsAny<ITextBuffer>()) == documentTracker,
            MockBehavior.Strict);
        var parserFactory = Mock.Of<IVisualStudioRazorParserFactory>(f =>
            f.Create(It.IsAny<IVisualStudioDocumentTracker>()) == parser,
            MockBehavior.Strict);
        var smartIndenterFactory = Mock.Of<IBraceSmartIndenterFactory>(f =>
            f.Create(It.IsAny<IVisualStudioDocumentTracker>()) == smartIndenter,
            MockBehavior.Strict);

        var factoryService = new RazorEditorFactoryService(documentTrackerFactory, parserFactory, smartIndenterFactory);

        return factoryService;
    }
}