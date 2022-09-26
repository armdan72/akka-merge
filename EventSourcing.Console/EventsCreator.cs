using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing.Console
{
    public static class EventsCreator
    {
        public static async Task CreateEvent(string dbConnectionString)
        {
            var actorSystem = ActorSystem.Create("ESActorSystem", AkkaConfigurationHelper.CreateSqlLiteConfig(dbConnectionString, true));
            var repo = actorSystem.ActorOf(Props.Create<PersistentActor>(), "repo");

            var command = new Cmd($"Command created at: {DateTime.Now}");
            var result = await repo.Ask(command);

            await actorSystem.Terminate();
        }
    }
}
