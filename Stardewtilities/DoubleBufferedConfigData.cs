namespace BeardPhantom.Stardewtilities
{
    public class DoubleBufferedConfigData<T> where T : new()
    {
        public T RealtimeData { get; private set; } = new();

        public T PersistedData { get; private set; } = new();
    }
}