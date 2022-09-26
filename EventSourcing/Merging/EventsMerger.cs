using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Akka.Streams;
using Akka.IO;
using Akka.Persistence.Query.Sql;

namespace EventSourcing.Merging;

public static class EventMerger
{
    public static async Task Merge(string inputDbConnectionString1, string inputDbConnectionString2)
    {
        using var readSystem1 = GetReadActorSystem(inputDbConnectionString1, 1);
        using var readSystem2 = GetReadActorSystem(inputDbConnectionString2, 2);

        var actorMaterializer1 = ActorMaterializer.Create(readSystem1, ActorMaterializerSettings.Create(readSystem1).WithDebugLogging(true));
        var readJournal1 = PersistenceQuery.Get(readSystem1).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
        var source1 = readJournal1.CurrentEventsByPersistenceId("sample-id-1", 0L, long.MaxValue);
        await source1.Select(x => ByteString.FromString($"{x.Timestamp.ToString()}{Environment.NewLine}")).RunWith(FileIO.ToFile(new FileInfo("c:\\tmp\\merge1.txt")), actorMaterializer1);

        // just creating the materializer changes the source!!!
        var actorMaterializer2 = ActorMaterializer.Create(readSystem2, ActorMaterializerSettings.Create(readSystem1).WithDebugLogging(true));
        var readJournal2 = PersistenceQuery.Get(readSystem2).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
        var source2 = readJournal2.CurrentEventsByPersistenceId("sample-id-1", 0L, long.MaxValue);
        await source2.Select(x => ByteString.FromString($"{x.Timestamp.ToString()}{Environment.NewLine}")).RunWith(FileIO.ToFile(new FileInfo("c:\\tmp\\merge2.txt")), actorMaterializer2);

        var source = source1.MergeSorted(source2, new EventEnvelopComparer());
        await source.Select(x => ByteString.FromString($"{x.Timestamp.ToString()}{Environment.NewLine}")).RunWith(FileIO.ToFile(new FileInfo("c:\\tmp\\merge.txt")), actorMaterializer1);
        
        await readSystem1.WhenTerminated;
        await readSystem2.WhenTerminated;
    }

    public static ActorSystem GetReadActorSystem(
    string inputDbConnectionString,
    int number
    )
    {
        var readConfig = AkkaConfigurationHelper.CreateSqlLiteConfig(inputDbConnectionString, true);
        var actorSystem = ActorSystem.Create($"read-system{number}", readConfig);
        return actorSystem;
    }
}
