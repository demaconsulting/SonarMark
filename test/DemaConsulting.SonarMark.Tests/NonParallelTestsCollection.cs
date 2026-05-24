// Copyright (c) DemaConsulting. All rights reserved.
// Licensed under the MIT License.

using Xunit;

namespace DemaConsulting.SonarMark.Tests;

/// <summary>
///     Defines a test collection for tests that manipulate shared process state
///     (Console.Out / Console.Error) and therefore must not run in parallel.
/// </summary>
[CollectionDefinition(Name)]
public sealed class NonParallelTestsCollection : ICollectionFixture<NonParallelTestsCollection>
{
    /// <summary>Collection name used by [Collection] attributes.</summary>
    public const string Name = "NonParallelTests";
}
