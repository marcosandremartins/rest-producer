namespace KafkaRestProducer.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using KafkaRestProducer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

public class TopicsBulkTests : BaseTestServer
{
    [Fact]
    public async Task PostTopicsBulk_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var messageRequest = new MessageRequestBulk()
        {
            Topic = "topic",
            Serializer = SerializerType.Avro,
            Contract = "FakeContract",
            NumberOfMessages = 5
        };

        var contentRequest = new StringContent(
            JsonConvert.SerializeObject(messageRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("/topicsBulk"),
            Method = HttpMethod.Post,
            Content = contentRequest
        };

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
