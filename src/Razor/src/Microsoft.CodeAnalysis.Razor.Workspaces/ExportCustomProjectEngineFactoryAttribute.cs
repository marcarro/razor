﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

#nullable disable

using System;
using System.Composition;

namespace Microsoft.CodeAnalysis.Razor
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExportCustomProjectEngineFactoryAttribute : ExportAttribute, ICustomProjectEngineFactoryMetadata
    {
        public ExportCustomProjectEngineFactoryAttribute(string configurationName!!)
            : base(typeof(IProjectEngineFactory))
        {
            ConfigurationName = configurationName;
        }

        public string ConfigurationName { get; }

        public bool SupportsSerialization { get; set; }
    }
}
