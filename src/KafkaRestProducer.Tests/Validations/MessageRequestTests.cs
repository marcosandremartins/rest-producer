namespace KafkaRestProducer.Tests.Validations;

using AutoFixture;
using FluentAssertions;
using KafkaRestProducer.Models;
using Xunit;

public class MessageRequestTests
{
    private readonly Fixture fixture;

    public MessageRequestTests()
    {
        this.fixture = new Fixture();
    }

    [Fact]
    public void OnMessageRequest_MissingTopicProperty_ShouldThrowArgumentException()
    {
        // Arrange
        const bool autoGeneratePayload = false;

        var messageRequest = this.fixture
            .Build<MessageRequest>()
            .Without(p => p.Topic)
            .Create();

        // Act
        var result = () => messageRequest.Validate(autoGeneratePayload);

        // Assert
        result.Should().Throw<ArgumentException>().WithMessage("Property 'Topic' is Mandatory.");
    }

    [Fact]
    public void OnMessageRequest_MissingKeyProperty_ShouldThrowArgumentException()
    {
        // Arrange
        const bool autoGeneratePayload = false;

        var messageRequest = this.fixture
            .Build<MessageRequest>()
            .Without(p => p.Key)
            .Create();

        // Act
        var result = () => messageRequest.Validate(autoGeneratePayload);

        // Assert
        result.Should().Throw<ArgumentException>().WithMessage("Property 'Key' is Mandatory.");
    }

    [Fact]
    public void OnMessageRequest_WithoutContractProperty_AndAutoGeneratePayload_ShouldThrowArgumentException()
    {
        // Arrange
        const bool autoGeneratePayload = true;

        var messageRequest = this.fixture
            .Build<MessageRequest>()
            .Without(p => p.Contract)
            .Create();

        // Act
        var result = () => messageRequest.Validate(autoGeneratePayload);

        // Assert
        result.Should().Throw<ArgumentException>()
            .WithMessage("'Contract' is required when auto generate set to true.");
    }

    [Fact]
    public void OnMessageRequest_WithoutContractProperty_AndNoAutoGeneratePayload_ShouldNotThrowException()
    {
        // Arrange
        const bool autoGeneratePayload = false;

        var messageRequest = this.fixture
            .Build<MessageRequest>()
            .Without(p => p.Contract)
            .Create();

        // Act
        var result = () => messageRequest.Validate(autoGeneratePayload);

        // Assert
        result.Should().NotThrow();
    }

    [Fact]
    public void OnMessageRequest_MissingPayloadProperty_WithoutAutoGeneratedPayload_ShouldThrowArgumentException()
    {
        // Arrange
        const bool autoGeneratePayload = false;

        var messageRequest = this.fixture
            .Build<MessageRequest>()
            .Without(p => p.Payload)
            .Create();

        // Act
        var result = () => messageRequest.Validate(autoGeneratePayload);

        // Assert
        result.Should().Throw<ArgumentException>().WithMessage("Property 'Payload' is required.");
    }

    [Fact]
    public void OnMessageRequest_MissingPayloadProperty_WithAutoGeneratedPayload_ShouldNotThrowException()
    {
        // Arrange
        const bool autoGeneratePayload = true;

        var messageRequest = this.fixture
            .Build<MessageRequest>()
            .With(p => p.Serializer, SerializerType.Protobuf)
            .Without(p => p.Payload)
            .Create();

        // Act
        var result = () => messageRequest.Validate(autoGeneratePayload);

        // Assert
        result.Should().NotThrow();
    }
}
