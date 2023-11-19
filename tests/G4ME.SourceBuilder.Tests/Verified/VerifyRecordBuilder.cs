using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace G4ME.SourceBuilder.Tests.Verified;

[UsesVerify]
public class VerifyRecordBuilder
{
    [Fact]
    public async Task Create_SimpleRecord_Verified()
    {
        var recordBuilder = new RecordBuilder("SimpleRecord");

        await BuildAndVerify(recordBuilder);
    }

    [Fact]
    public async Task Create_RecordWithPrimaryConstructor_Verified()
    {
        var recordBuilder = new RecordBuilder("SimpleRecord");

        recordBuilder.Parameter<string>("FirstName")
                     .Parameter<string>("LastName");

        await BuildAndVerify(recordBuilder);
    }

    [Fact]
    public async Task Create_RecordWithAttribute_Verified()
    {
        RecordBuilder recordBuilder = new("TestRecord");
        
        recordBuilder.Attributes(a => a
                         .Add<SerializableAttribute>());

        await BuildAndVerify(recordBuilder);
    }

    //[Fact]
    //public async Task Create_RecordWithProperties_Verified()
    //{
    //    var recordBuilder = new RecordBuilder("TestRecord")
    //                            .Properties(p => p.Add<string>("Name").Get().Set()); // TODO: Support Init()

    //    await BuildAndVerify(recordBuilder);
    //}

    //[Fact]
    //public async Task Create_RecordWithMethod_Verified()
    //{
    //    var recordBuilder = new RecordBuilder("TestRecord")
    //                            .AddMethod("TestMethod", m => m
    //                                .Body(b => b
    //                                    .AddLine(@"Console.WriteLine(""Hello World!"")")));

    //    await BuildAndVerify(recordBuilder);
    //}

    [Fact]
    public async Task Create_RecordImplementsInterface_Verified()
    {
        var recordBuilder = new RecordBuilder("TestRecord");
        recordBuilder.Implements<ISomeInterface>();

        await BuildAndVerify(recordBuilder);
    }

    [Fact]
    public async Task Create_RecordExtendsBase_Verified()
    {
        var recordBuilder = new RecordBuilder("TestRecord");
        recordBuilder.Extends<BaseRecord>();

        await BuildAndVerify(recordBuilder);
    }

    private static async Task BuildAndVerify(RecordBuilder recordBuilder)
    {
        var generatedRecord = recordBuilder.Build();

        await Verify(generatedRecord.NormalizeWhitespace().ToFullString());
    }
}
