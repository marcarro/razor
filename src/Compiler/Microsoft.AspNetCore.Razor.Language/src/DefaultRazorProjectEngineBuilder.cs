﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Razor.Language;

internal class DefaultRazorProjectEngineBuilder : RazorProjectEngineBuilder
{
    public DefaultRazorProjectEngineBuilder(RazorConfiguration configuration, RazorProjectFileSystem fileSystem)
    {
        if (fileSystem == null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        Configuration = configuration;
        FileSystem = fileSystem;
        Features = new List<IRazorFeature>();
        Phases = new List<IRazorEnginePhase>();
    }

    public override RazorConfiguration Configuration { get; }

    public override RazorProjectFileSystem FileSystem { get; }

    public override ICollection<IRazorFeature> Features { get; }

    public override IList<IRazorEnginePhase> Phases { get; }

    public override RazorProjectEngine Build()
    {
        var engineFeatures = Features.OfType<IRazorEngineFeature>().ToArray();
        var phases = Phases.ToArray();
        var engine = new RazorEngine(engineFeatures, phases);

        var projectFeatures = Features.OfType<IRazorProjectEngineFeature>().ToArray();
        var projectEngine = new DefaultRazorProjectEngine(Configuration, engine, FileSystem, projectFeatures);

        return projectEngine;
    }
}
