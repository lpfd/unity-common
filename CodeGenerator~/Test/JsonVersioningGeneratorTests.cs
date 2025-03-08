using System;
using Xunit;

namespace Test;

public class JsonVersioningGeneratorTests
{
    [Fact]
    public void CustomConverter()
    {
        var testObject = new FieldWithCustomConverterCollection() {CustomSerializedField = new System.DateTime(1999,1,1) };
        var token = testObject.ToJToken();

        var restoredObject = new FieldWithCustomConverterCollection();
        restoredObject.FromJToken(token);
        Assert.Equal(testObject.CustomSerializedField, restoredObject.CustomSerializedField);
    }

    [Fact]
    public void IntCollection()
    {
        var testObject = new IntCollection() { ListOfInts = { 42, 1} };
        var token = testObject.ToJToken();

        var restoredObject = new IntCollection();
        restoredObject.FromJToken(token);
        Assert.Equal(testObject.ListOfInts, restoredObject.ListOfInts);
    }
}
