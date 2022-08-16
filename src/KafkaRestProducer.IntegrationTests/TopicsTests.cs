namespace KafkaRestProducer.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using KafkaRestProducer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

public class TopicsTests : BaseTestServer
{
    [Fact]
    public async Task PostTopics_ValidRequest_ReturnsAccepted()
    {
        // Arrange
        var messageRequest = new MessageRequest
        {
            Topic = "topic",
            Key = "key",
            Serializer = SerializerType.Json,
            Payload = new {Id = 1, Name = "SomeOne"}
        };

        var contentRequest = new StringContent(
            JsonConvert.SerializeObject(messageRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("/topics"),
            Method = HttpMethod.Post,
            Content = contentRequest
        };

        // Act
        var response = await this.TestHttpClient.SendAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task PostTopics_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var messageRequest = new MessageRequest
        {
            Topic = "topic",
            Key = "key",
            Serializer = SerializerType.Protobuf,
            Contract = "FakeContract"
        };

        var contentRequest = new StringContent(
            JsonConvert.SerializeObject(messageRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("/topics"),
            Method = HttpMethod.Post,
            Content = contentRequest
        };

        request.Headers.Add("Auto-Generate-Payload", "true");

        // Act
        var response = await this.TestHttpClient.SendAsync(request);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content?.Status.Should().Be(400);
        content?.Type.Should().Contain(nameof(DllNotFoundException));
        content?.Detail.Should().Be("Contract 'FakeContract' not found.");
    }
}
