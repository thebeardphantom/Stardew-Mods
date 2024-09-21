namespace TimescaleDisplay
{
    [Serializable]
    public struct ChangeNetworkMessage
    {
        public float AvgTimeScale { get; set; }

        public ChangeNetworkMessage(float avgTimeScale)
        {
            AvgTimeScale = avgTimeScale;
        }
    }
}