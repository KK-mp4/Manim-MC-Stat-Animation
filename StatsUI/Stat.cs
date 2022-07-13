namespace StatsUI
{
    using System;

    public class Stat
    {
        public string uuid { get; set; }

        public int stat_id { get; set; }

        public int amount { get; set; }

        public DateTime timestamp { get; set; }

        public string FullInfo
        {
            get
            {
                return $"{ uuid } { stat_id } { amount } { timestamp.ToShortDateString() }";
            }
        }
    }
}
