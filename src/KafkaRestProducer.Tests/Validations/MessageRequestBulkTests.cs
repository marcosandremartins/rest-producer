namespace KafkaRestProducer.Tests.Validations;

using AutoFixture;
using FluentAssertions;
using KafkaRestProducer.Models;
using Xunit;

public class MessageRequestBulkTests
{
    private readonly Fixture fixture;

    public MessageRequestBulkTests()
    {
        this.fixture = new Fixture();
    }

    [Fact]
    public void OnMessageRequestBulk_MissingTopicProperty_ShouldThrowArgumentException()
    {
        // Arrange
        var messageRequest = this.fixture
            .Build<MessageRequestBulk>()
            .With(p => p.Serializer, SerializerType.Protobuf)
            .Without(p => p.Topic)
            .Create();

        // Act
        var result = () => messageRequest.Validate();

        // Assert
        result.Should().Throw<ArgumentException>().WithMessage("Property 'Topic' is Mandatory.");
    }

    [Fact]
    public void OnMessageRequestBulk_MissingContractProperty_ShouldThrowArgumentException()
    {
        // Arrange
        var messageRequest = this.fixture
            .Build<MessageRequestBulk>()
            .With(p => p.Serializer, SerializerType.Protobuf)
            .Without(p => p.Contract)
            .Create();

        // Act
        var result = () => messageRequest.Validate();

        // Assert
        result.Should().Throw<ArgumentException>().WithMessage("Property 'Contract' is Mandatory.");
    }

    [Fact]
    public void OnMessageRequestBulk_WithInvalidNumberOfMessages_ShouldThrowArgumentException()
    {
        // Arrange
        var messageRequest = this.fixture
            .Build<MessageRequestBulk>()
            .With(p => p.Serializer, SerializerType.Avro)
            .With(p => p.NumberOfMessages, 0)
            .Create();

        // Act
        var result = () => messageRequest.Validate();

        // Assert
        result.Should().Throw<ArgumentException>().WithMessage("'NumberOfMessages' must be higher than 0.");
    }

    [Fact]
    public void OnMessageRequestBulk_NoErrors_ShouldNotThrowException()
    {
        // Arrange
        var messageRequest = this.fixture
            .Build<MessageRequestBulk>()
            .With(p => p.Serializer, SerializerType.Avro)
            .Create();

        // Act
        var result = () => messageRequest.Validate();

        // Assert
        result.Should().NotThrow();
    }
}
