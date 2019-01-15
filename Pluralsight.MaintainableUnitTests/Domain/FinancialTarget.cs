using System;

namespace Pluralsight.MaintainableUnitTests
{
    public class FinancialTarget
    {
        public int SummerSeasonFirstMonth => 5;
        public int SummerSeasonLastMonth => 8;
        private ITimeServer TimeServer { get; }

        public FinancialTarget(ITimeServer timeServer)
        {
            TimeServer = timeServer;
        }

        public void AddTargetPoints(IMyList toArray, int count)
        {
            if (count < 0)
                throw new ArgumentException($"{nameof(count)} cannot be negative.");

            int month = DateTime.Today.Month;

            var factor = CalculateScalingFactor();

            int[] points = new int[count];

            for (int i = 0; i < count; i++)
                points[i] = 3 + factor * i;

            toArray.AppendMany(points);
        }

        public int GetGoldenTarget(IMyList targetPoints)
        { 
            if (targetPoints.Count < 2)
                throw new ArgumentException("points.Count < 2", nameof(targetPoints));

            return (targetPoints.GetFirst() + targetPoints.ElementAt(targetPoints.Count - 1)) / 2;
        }

        private int CalculateScalingFactor()
        {
            if (IsSummerSeason())
            {
                return 1;
            }

            return 2;
        }

        private bool IsSummerSeason()
        {
            int month = TimeServer.GetCurrentMonth();
            return month >= SummerSeasonFirstMonth &&
                            month <= SummerSeasonLastMonth;
        }

        public MyArray InitializePoints()
        {
            var array = new MyArray();
            AddTargetPoints(array, 1);
            return array;
        }
    }
}
