using System.Collections.Immutable;

namespace EventSourcing
{
    public class ExampleState
    {
        private readonly ImmutableList<string> _events;

        public ExampleState(ImmutableList<string> events)
        {
            _events = events;
        }

        public ExampleState() : this(ImmutableList.Create<string>())
        {
        }

        public ExampleState Updated(Evt evt)
        {
            return new ExampleState(_events.Add(evt.Data));
        }

        public int Size => _events.Count;

        public override string ToString()
        {
            return string.Join(", ", _events.Reverse());
        }
    }
}
