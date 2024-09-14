namespace TimescaleDisplay
{
    [Serializable]
    public struct TimescaleChangeMessage
    {
        public float AvgTimeScale { get; set; }

        public TimescaleChangeMessage(float avgTimeScale)
        {
            AvgTimeScale = avgTimeScale;
        }
    }
}