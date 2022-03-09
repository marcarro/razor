﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

#nullable disable

using System;
using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.Razor.Tooltip
{
    internal class AggregateBoundElementDescription
    {
        public static readonly AggregateBoundElementDescription Default = new AggregateBoundElementDescription(Array.Empty<BoundElementDescriptionInfo>());

        public AggregateBoundElementDescription(IReadOnlyList<BoundElementDescriptionInfo> associatedTagHelperDescriptions!!)
        {
            AssociatedTagHelperDescriptions = associatedTagHelperDescriptions;
        }

        public IReadOnlyList<BoundElementDescriptionInfo> AssociatedTagHelperDescriptions { get; }
    }
}
