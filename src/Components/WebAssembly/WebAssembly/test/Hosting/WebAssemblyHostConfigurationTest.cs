// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    public class WebAssemblyHostConfigurationTest
    {
        [Fact]
        public void AddsSourcesToPublicProperty()
        {
            // Arrange
            var initialData = new Dictionary<string, string>() { { "color", "blue" } };
            var memoryConfig = new MemoryConfigurationSource { InitialData = initialData };
            var configuration = new WebAssemblyHostConfiguration();

            // Act
            configuration.Add(memoryConfig);

            // Assert
            Assert.Contains(memoryConfig, configuration.Sources);
        }

        [Fact]
        public void CanSetAndGetConfigurationValue()
        {
            // Arrange
            var initialData = new Dictionary<string, string>() { { "color", "blue" } };
            var memoryConfig = new MemoryConfigurationSource { InitialData = initialData };
            var configuration = new WebAssemblyHostConfiguration();

            // Act
            configuration.Add(memoryConfig);
            configuration["type"] = "car";

            // Assert
            Assert.Equal("car", configuration["type"]);
            Assert.Equal("blue", configuration["color"]);
        }

        [Fact]
        public void SettingValueUpdatesAllProviders()
        {
            // Arrange
            var initialData = new Dictionary<string, string>() { { "color", "blue" } };
            var source1 = new MemoryConfigurationSource { InitialData = initialData };
            var source2 = new MemoryConfigurationSource { InitialData = initialData };
            var configuration = new WebAssemblyHostConfiguration();

            // Act
            configuration.Add(source1);
            configuration.Add(source2);
            configuration["type"] = "car";

            // Assert
            Assert.Equal("car", configuration["type"]);
            Assert.True(configuration.Providers.All(provider =>
            {
                provider.TryGet("type", out var value);
                return value == "car";
            }));
        }

        [Fact]
        public void CanGetChildren()
        {
            // Arrange
            var initialData = new Dictionary<string, string>() { { "color", "blue" } };
            var memoryConfig = new MemoryConfigurationSource { InitialData = initialData };
            var configuration = new WebAssemblyHostConfiguration();

            // Act
            configuration.Add(memoryConfig);
            var children = configuration.GetChildren();

            // Assert
            Assert.NotNull(children);
            Assert.NotEmpty(children);
        }

        [Fact]
        public void CanGetSection()
        {
            // Arrange
            var initialData = new Dictionary<string, string>() {
                { "color", "blue" },
                { "type", "car" },
                { "wheels:year", "2008" },
                { "wheels:count", "4" },
                { "wheels:brand", "michelin" },
                { "wheels:brand:type", "rally" },
            };
            var memoryConfig = new MemoryConfigurationSource { InitialData = initialData };
            var configuration = new WebAssemblyHostConfiguration();

            // Act
            configuration.Add(memoryConfig);
            var section = configuration.GetSection("wheels").AsEnumerable(makePathsRelative: true).ToDictionary(k => k.Key, v => v.Value);

            // Assert
            Assert.Equal(4, section.Count);
            Assert.Equal("2008", section["year"]);
            Assert.Equal("4", section["count"]);
            Assert.Equal("michelin", section["brand"]);
            Assert.Equal("rally", section["brand:type"]);
        }

        [Fact]
        public void CanDisposeProviders()
        {
            // Arrange
            var initialData = new Dictionary<string, string>() { { "color", "blue" } };
            var memoryConfig = new MemoryConfigurationSource { InitialData = initialData };
            var configuration = new WebAssemblyHostConfiguration();

            // Act
            configuration.Add(memoryConfig);
            Assert.Equal("blue", configuration["color"]);
            var exception = Record.Exception(() => configuration.Dispose());

            // Assert
            Assert.Null(exception);
        }
    }
}
