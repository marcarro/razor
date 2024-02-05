﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Test.Common;
using Microsoft.AspNetCore.Razor.Test.Common.Editor;
using Microsoft.CodeAnalysis.Razor.ProjectSystem;
using Microsoft.VisualStudio.Editor.Razor.Documents;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.VisualStudio.Editor.Razor;

public class ImportDocumentManagerTest : ProjectSnapshotManagerDispatcherTestBase
{
    private readonly string _projectPath;
    private readonly string _directoryPath;
    private readonly RazorProjectFileSystem _fileSystem;
    private readonly RazorProjectEngine _projectEngine;

    public ImportDocumentManagerTest(ITestOutputHelper testOutput)
        : base(testOutput)
    {
        _projectPath = TestProjectData.SomeProject.FilePath;
        _directoryPath = Path.GetDirectoryName(_projectPath);

        _fileSystem = RazorProjectFileSystem.Create(Path.GetDirectoryName(_projectPath));

        // These tests rely on MVC's import behavior.
        _projectEngine = RazorProjectEngine.Create(
            FallbackRazorConfiguration.MVC_2_1,
            _fileSystem,
            AspNetCore.Mvc.Razor.Extensions.RazorExtensions.Register);
    }

    [UIFact]
    public void OnSubscribed_StartsFileChangeTrackers()
    {
        // Arrange
        var tracker = Mock.Of<IVisualStudioDocumentTracker>(
            t => t.FilePath == Path.Combine(_directoryPath, "Views", "Home", "file.cshtml") &&
            t.ProjectPath == _projectPath &&
            t.ProjectSnapshot == Mock.Of<IProjectSnapshot>(p => p.GetProjectEngine() == _projectEngine && p.GetDocument(It.IsAny<string>()) == null, MockBehavior.Strict), MockBehavior.Strict);

        var fileChangeTrackerFactory = new Mock<IFileChangeTrackerFactory>(MockBehavior.Strict);
        var fileChangeTracker1 = new Mock<IFileChangeTracker>(MockBehavior.Strict);
        fileChangeTracker1
            .Setup(f => f.StartListening())
            .Verifiable();
        fileChangeTrackerFactory
            .Setup(f => f.Create(Path.Combine(_directoryPath, "Views", "Home", "_ViewImports.cshtml")))
            .Returns(fileChangeTracker1.Object)
            .Verifiable();
        var fileChangeTracker2 = new Mock<IFileChangeTracker>(MockBehavior.Strict);
        fileChangeTracker2
            .Setup(f => f.StartListening())
            .Verifiable();
        fileChangeTrackerFactory
            .Setup(f => f.Create(Path.Combine(_directoryPath, "Views", "_ViewImports.cshtml")))
            .Returns(fileChangeTracker2.Object)
            .Verifiable();
        var fileChangeTracker3 = new Mock<IFileChangeTracker>(MockBehavior.Strict);
        fileChangeTracker3.Setup(f => f.StartListening()).Verifiable();
        fileChangeTrackerFactory
            .Setup(f => f.Create(Path.Combine(_directoryPath, "_ViewImports.cshtml")))
            .Returns(fileChangeTracker3.Object)
            .Verifiable();

        var manager = new ImportDocumentManager(Dispatcher, fileChangeTrackerFactory.Object);

        // Act
        manager.OnSubscribed(tracker);

        // Assert
        fileChangeTrackerFactory.Verify();
        fileChangeTracker1.Verify();
        fileChangeTracker2.Verify();
        fileChangeTracker3.Verify();
    }

    [UIFact]
    public void OnSubscribed_AlreadySubscribed_DoesNothing()
    {
        // Arrange
        var tracker = Mock.Of<IVisualStudioDocumentTracker>(
            t => t.FilePath == Path.Combine(_directoryPath, "file.cshtml") &&
            t.ProjectPath == _projectPath &&
            t.ProjectSnapshot == Mock.Of<IProjectSnapshot>(p => p.GetProjectEngine() == _projectEngine && p.GetDocument(It.IsAny<string>()) == null, MockBehavior.Strict), MockBehavior.Strict);

        var anotherTracker = Mock.Of<IVisualStudioDocumentTracker>(
            t => t.FilePath == Path.Combine(_directoryPath, "anotherFile.cshtml") &&
            t.ProjectPath == _projectPath &&
            t.ProjectSnapshot == Mock.Of<IProjectSnapshot>(p => p.GetProjectEngine() == _projectEngine && p.GetDocument(It.IsAny<string>()) == null, MockBehavior.Strict), MockBehavior.Strict);

        var callCount = 0;
        var fileChangeTrackerFactory = new Mock<IFileChangeTrackerFactory>(MockBehavior.Strict);
        var fileChangeTracker = new Mock<IFileChangeTracker>(MockBehavior.Strict);
        fileChangeTracker.Setup(t => t.StartListening()).Verifiable();
        fileChangeTrackerFactory
            .Setup(f => f.Create(It.IsAny<string>()))
            .Returns(fileChangeTracker.Object)
            .Callback(() => callCount++);

        var manager = new ImportDocumentManager(Dispatcher, fileChangeTrackerFactory.Object);
        manager.OnSubscribed(tracker); // Start tracking the import.

        // Act
        manager.OnSubscribed(anotherTracker);

        // Assert
        Assert.Equal(1, callCount);
    }

    [UIFact]
    public void OnUnsubscribed_StopsFileChangeTracker()
    {
        // Arrange
        var tracker = Mock.Of<IVisualStudioDocumentTracker>(
            t => t.FilePath == Path.Combine(_directoryPath, "file.cshtml") &&
            t.ProjectPath == _projectPath &&
            t.ProjectSnapshot == Mock.Of<IProjectSnapshot>(p => p.GetProjectEngine() == _projectEngine && p.GetDocument(It.IsAny<string>()) == null, MockBehavior.Strict), MockBehavior.Strict);

        var fileChangeTrackerFactory = new Mock<IFileChangeTrackerFactory>(MockBehavior.Strict);
        var fileChangeTracker = new Mock<IFileChangeTracker>(MockBehavior.Strict);
        fileChangeTracker.Setup(f => f.StartListening()).Verifiable();
        fileChangeTracker
            .Setup(f => f.StopListening())
            .Verifiable();
        fileChangeTrackerFactory
            .Setup(f => f.Create(Path.Combine(_directoryPath, "_ViewImports.cshtml")))
            .Returns(fileChangeTracker.Object)
            .Verifiable();

        var manager = new ImportDocumentManager(Dispatcher, fileChangeTrackerFactory.Object);
        manager.OnSubscribed(tracker); // Start tracking the import.

        // Act
        manager.OnUnsubscribed(tracker);

        // Assert
        fileChangeTrackerFactory.Verify();
        fileChangeTracker.Verify();
    }

    [UIFact]
    public void OnUnsubscribed_AnotherDocumentTrackingImport_DoesNotStopFileChangeTracker()
    {
        // Arrange
        var tracker = Mock.Of<IVisualStudioDocumentTracker>(
            t => t.FilePath == Path.Combine(_directoryPath, "file.cshtml") &&
            t.ProjectPath == _projectPath &&
            t.ProjectSnapshot == Mock.Of<IProjectSnapshot>(p => p.GetProjectEngine() == _projectEngine && p.GetDocument(It.IsAny<string>()) == null, MockBehavior.Strict), MockBehavior.Strict);

        var anotherTracker = Mock.Of<IVisualStudioDocumentTracker>(
            t => t.FilePath == Path.Combine(_directoryPath, "anotherFile.cshtml") &&
            t.ProjectPath == _projectPath &&
            t.ProjectSnapshot == Mock.Of<IProjectSnapshot>(p => p.GetProjectEngine() == _projectEngine && p.GetDocument(It.IsAny<string>()) == null, MockBehavior.Strict), MockBehavior.Strict);

        var fileChangeTrackerFactory = new Mock<IFileChangeTrackerFactory>(MockBehavior.Strict);
        var fileChangeTracker = new Mock<IFileChangeTracker>(MockBehavior.Strict);
        fileChangeTracker.Setup(f => f.StartListening()).Verifiable();
        fileChangeTracker
            .Setup(f => f.StopListening())
            .Throws(new InvalidOperationException());
        fileChangeTrackerFactory
            .Setup(f => f.Create(It.IsAny<string>()))
            .Returns(fileChangeTracker.Object);

        var manager = new ImportDocumentManager(Dispatcher, fileChangeTrackerFactory.Object);
        manager.OnSubscribed(tracker); // Starts tracking import for the first document.

        manager.OnSubscribed(anotherTracker); // Starts tracking import for the second document.

        // Act & Assert (Does not throw)
        manager.OnUnsubscribed(tracker);
        manager.OnUnsubscribed(tracker);
    }
}