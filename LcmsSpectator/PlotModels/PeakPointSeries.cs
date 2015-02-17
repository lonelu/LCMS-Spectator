﻿using OxyPlot.Series;

namespace LcmsSpectator.PlotModels
{
    public class PeakPointSeries: StemSeries, IDataPointSeries
    {
        public double GetMaxYInRange(double minX, double maxX)
        {
            double maxY = 0.0;
            var dataPoints = ActualPoints;
            if (dataPoints != null && dataPoints.Count > 0 && IsVisible)
            {
                foreach (var point in dataPoints)
                {
                    if (point.X >= minX && point.X <= maxX && point.Y >= maxY)
                        maxY = point.Y;
                }
            }
            return maxY;
        }
    }
}