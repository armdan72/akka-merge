using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing
{
    public class PersistentActor : UntypedPersistentActor
    {
        private ExampleState _state = new ExampleState();

        private void UpdateState(Evt evt)
        {
            _state = _state.Updated(evt);
        }

        private int NumEvents => _state.Size;

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case Evt evt:
                    UpdateState(evt);
                    break;
                case SnapshotOffer snapshot when snapshot.Snapshot is ExampleState:
                    _state = (ExampleState)snapshot.Snapshot;
                    break;
            }
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case Cmd cmd:
                    Persist(new Evt($"{cmd.Data}-{NumEvents}"), UpdateState);
                    //Persist(new Evt($"{cmd.Data}-{NumEvents + 1}"), evt =>
                    //{
                    //    UpdateState(evt);
                    //    Context.System.EventStream.Publish(evt);
                    //});
                    break;
                case "snap":
                    SaveSnapshot(_state);
                    break;
                case "print":
                    Console.WriteLine(_state);
                    break;
            }
        }

        public override string PersistenceId { get; } = "sample-id-1";
    }
}
