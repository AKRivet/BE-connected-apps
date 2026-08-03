using BeConnectedAppsTesting.Api.Tests.Helpers;
using Xunit;

namespace BeConnectedAppsTesting.Api.Tests;

[CollectionDefinition("ApiTests")]
public sealed class ApiTestsCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
