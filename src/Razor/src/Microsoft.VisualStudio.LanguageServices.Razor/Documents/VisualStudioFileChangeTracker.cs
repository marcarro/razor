﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.CodeAnalysis.Razor.Workspaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace Microsoft.VisualStudio.Editor.Razor.Documents
{
    internal class VisualStudioFileChangeTracker : FileChangeTracker, IVsFreeThreadedFileChangeEvents2
    {
        private const _VSFILECHANGEFLAGS FileChangeFlags = _VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Del | _VSFILECHANGEFLAGS.VSFILECHG_Add;

        private readonly ErrorReporter _errorReporter;
        private readonly IVsAsyncFileChangeEx _fileChangeService;
        private readonly ProjectSnapshotManagerDispatcher _projectSnapshotManagerDispatcher;
        private readonly JoinableTaskContext _joinableTaskContext;

        // Internal for testing
        internal JoinableTask<uint> _fileChangeAdviseTask;
        internal JoinableTask _fileChangeUnadviseTask;
        internal JoinableTask _fileChangedTask;

        public override event EventHandler<FileChangeEventArgs> Changed;

        public VisualStudioFileChangeTracker(
            string filePath,
            ErrorReporter errorReporter,
            IVsAsyncFileChangeEx fileChangeService,
            ProjectSnapshotManagerDispatcher projectSnapshotManagerDispatcher,
            JoinableTaskContext joinableTaskContext)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(filePath));
            }

            if (errorReporter is null)
            {
                throw new ArgumentNullException(nameof(errorReporter));
            }

            if (fileChangeService is null)
            {
                throw new ArgumentNullException(nameof(fileChangeService));
            }

            if (projectSnapshotManagerDispatcher is null)
            {
                throw new ArgumentNullException(nameof(projectSnapshotManagerDispatcher));
            }

            if (joinableTaskContext is null)
            {
                throw new ArgumentNullException(nameof(joinableTaskContext));
            }

            FilePath = filePath;
            _errorReporter = errorReporter;
            _fileChangeService = fileChangeService;
            _projectSnapshotManagerDispatcher = projectSnapshotManagerDispatcher;
            _joinableTaskContext = joinableTaskContext;
        }

        public override string FilePath { get; }

        public override void StartListening()
        {
            _projectSnapshotManagerDispatcher.AssertDispatcherThread();

            if (_fileChangeUnadviseTask?.IsCompleted == false)
            {
                // An unadvise operation is still processing, block the project snapshot manager's thread until it completes.
                _fileChangeUnadviseTask.Join();
            }

            if (_fileChangeAdviseTask != null)
            {
                // Already listening
                return;
            }

            _fileChangeAdviseTask = _joinableTaskContext.Factory.RunAsync(async () =>
            {
                try
                {
                    return await _fileChangeService.AdviseFileChangeAsync(FilePath, FileChangeFlags, this).ConfigureAwait(true);
                }
                catch (PathTooLongException)
                {
                    // Don't report PathTooLongExceptions but don't fault either.
                }
                catch (Exception exception)
                {
                    // Don't explode on actual exceptions, just report gracefully.
                    _errorReporter.ReportError(exception);
                }

                return VSConstants.VSCOOKIE_NIL;
            });
        }

        public override void StopListening()
        {
            _projectSnapshotManagerDispatcher.AssertDispatcherThread();

            if (_fileChangeAdviseTask == null || _fileChangeUnadviseTask?.IsCompleted == false)
            {
                // Already not listening or trying to stop listening
                return;
            }

            _fileChangeUnadviseTask = _joinableTaskContext.Factory.RunAsync(async () =>
            {
                try
                {
                    var fileChangeCookie = await _fileChangeAdviseTask;

                    if (fileChangeCookie == VSConstants.VSCOOKIE_NIL)
                    {
                        // Wasn't able to listen for file change events. This typically happens when some sort of exception (i.e. access exceptions)
                        // is thrown when attempting to listen for file changes.
                        return;
                    }

                    await _fileChangeService.UnadviseFileChangeAsync(fileChangeCookie).ConfigureAwait(true);
                    _fileChangeAdviseTask = null;
                }
                catch (PathTooLongException)
                {
                    // Don't report PathTooLongExceptions but don't fault either.
                }
                catch (Exception exception)
                {
                    // Don't explode on actual exceptions, just report gracefully.
                    _errorReporter.ReportError(exception);
                }
            });
        }

        public int FilesChanged(uint fileCount, string[] filePaths, uint[] fileChangeFlags)
        {
            // Capturing task for testing purposes
            _fileChangedTask = _joinableTaskContext.Factory.RunAsync(async () =>
            {
                await _joinableTaskContext.Factory.SwitchToMainThreadAsync();

                foreach (var fileChangeFlag in fileChangeFlags)
                {
                    var fileChangeKind = FileChangeKind.Changed;
                    var changeFlag = (_VSFILECHANGEFLAGS)fileChangeFlag;
                    if ((changeFlag & _VSFILECHANGEFLAGS.VSFILECHG_Del) == _VSFILECHANGEFLAGS.VSFILECHG_Del)
                    {
                        fileChangeKind = FileChangeKind.Removed;
                    }
                    else if ((changeFlag & _VSFILECHANGEFLAGS.VSFILECHG_Add) == _VSFILECHANGEFLAGS.VSFILECHG_Add)
                    {
                        fileChangeKind = FileChangeKind.Added;
                    }

                    // Purposefully not passing through the file paths here because we know this change has to do with this trackers FilePath.
                    // We use that FilePath instead so any path normalization the file service did does not impact callers.
                    OnChanged(fileChangeKind);
                }
            });

            return VSConstants.S_OK;
        }

        public int DirectoryChanged(string pszDirectory) => VSConstants.S_OK;

        public int DirectoryChangedEx(string pszDirectory, string pszFile) => VSConstants.S_OK;

        public int DirectoryChangedEx2(string pszDirectory, uint cChanges, string[] rgpszFile, uint[] rggrfChange) => VSConstants.S_OK;

        private void OnChanged(FileChangeKind changeKind)
        {
            _joinableTaskContext.AssertUIThread();

            if (Changed == null)
            {
                return;
            }

            var args = new FileChangeEventArgs(FilePath, changeKind);
            Changed.Invoke(this, args);
        }
    }
}