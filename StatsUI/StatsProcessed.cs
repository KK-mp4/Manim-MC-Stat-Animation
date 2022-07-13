namespace StatsUI
{
    using System;

    public class StatsProcessed
    {
        public DateTime timestamp { get; set; }

        public int amount { get; set; }

        public string FullInfo
        {
            get
            {
                return $"{ timestamp.ToShortDateString() }: { amount.ToString("### ### ###") }";
            }
        }
    }
}
