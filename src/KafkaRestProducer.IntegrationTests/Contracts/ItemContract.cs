namespace KafkaRestProducer.IntegrationTests.Contracts;

using Avro;
using Avro.Specific;

public class ItemContract : ISpecificRecord
{
    private Avro.AvroDecimal _price;
    private string _name;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public Avro.AvroDecimal Price
    {
        get => _price;
        set => _price = value;
    }

    public Schema Schema => throw new NotImplementedException();

    public object Get(int fieldPos)
    {
        throw new NotImplementedException();
    }

    public void Put(int fieldPos, object fieldValue)
    {
        throw new NotImplementedException();
    }
}