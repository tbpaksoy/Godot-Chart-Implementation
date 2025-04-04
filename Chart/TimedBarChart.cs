using Godot;
using System;
using System.Collections.Generic;
[GlobalClass, Tool]
public partial class TimedBarChart : BarChart
{
    #region display options
    [Export]
    private ChartDataSourceWithDate.Unit unit;
    #endregion

    #region  Godot methods
    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == NotificationParented)
        {
            for (int i = 0; i < GetChildCount(); i++)
            {
                Node dataSource = GetChild(i);
                if (dataSource is not ChartDataSourceWithDate)
                {
                    GD.PushWarning($"TimedBarChart: Child {i} is not a ChartDataSourceWithDate.");
                    dataSource.QueueFree();
                }
            }
        }
    }
    public override void _Draw()
    {
        if (data == null || data.Count == 0 || colors == null || colors.Length == 0) return;
        float positiveRatio = 0f;
        int[] order = GetDateOrder();
        switch (unit)
        {
            case ChartDataSourceWithDate.Unit.Day:
                positiveRatio = (max <= 0 && min >= 0f) ? 1f : max / (max - min);
                positiveRatio = Mathf.Clamp(positiveRatio, 0f, 1f);
                if (min < 0f)
                {
                    DrawLine(new Vector2(0f, positiveRatio * Size.Y), new Vector2(Size.X, Size.Y * positiveRatio), new Color(1f, 0f, 0f));
                }

                int interval = _maxDate.DayNumber - _minDate.DayNumber + 1;

                float xPerColumn = Size.X / interval, yPerValue = Size.Y / (Mathf.Max(max, 0f) - Mathf.Min(min, 0f));


                for (int i = 0, j = 0; i < interval; i++)
                {
                    if (!dates.Contains(DateOnly.FromDayNumber(i))) continue;
                    int index = order[j];
                    Vector2 begin = new Vector2(i * xPerColumn, Size.Y * positiveRatio);
                    Vector2 offset = new Vector2(xPerColumn, -data[index] * yPerValue);
                    DrawColumn(begin, offset, colors[j++ % colors.Length], out Vector2 top, out Vector2 middle, out Vector2 down);
                    string[] texts = new string[] { null, null, null };
                    if (writeValues)
                        switch (data[index])
                        {
                            case < 0:
                                texts[2] = data[index].ToString();
                                break;
                            case > 0:
                                texts[0] = data[index].ToString();
                                break;
                        }
                    if (writeDates)
                        texts[1] = dates[index].ToString();
                    if (data[index] < 0) texts[0] = names[index].ToString();
                    else if (data[index] > 0) texts[2] = names[index].ToString();
                    DrawTexts(texts[0], texts[1], texts[2], top, middle, down);
                }
                break;
            case ChartDataSourceWithDate.Unit.Month:
                Dictionary<int, float> monthlyData = CalculateMonthlyData();

                if (monthlyData.Count < 2) return;

                float montlyMax = float.MinValue, montlyMin = float.MaxValue;
                foreach (var value in monthlyData.Values)
                {
                    if (value > montlyMax) montlyMax = value;
                    if (value < montlyMin) montlyMin = value;
                }
                positiveRatio = (montlyMax <= 0 && montlyMin >= 0f) ? 1f : montlyMax / (montlyMax - montlyMin);
                positiveRatio = Mathf.Clamp(positiveRatio, 0f, 1f);
                if (montlyMin < 0f)
                {
                    DrawLine(new Vector2(0f, positiveRatio * Size.Y), new Vector2(Size.X, Size.Y * positiveRatio), new Color(1f, 0f, 0f));
                }

                int intervalMonth = _maxDate.Month + _maxDate.Year * 12 - (_minDate.Month + _minDate.Year * 12) + 1;
                float xPerColumnMonth = Size.X / intervalMonth, yPerValueMonth = Size.Y / (Mathf.Max(max, 0f) - Mathf.Min(min, 0f));
                for (int i = _minDate.Month + _minDate.Year * 12, k = 0, j = 0; i <= _maxDate.Month + _maxDate.Year * 12; i++, j++)
                {
                    if (!monthlyData.ContainsKey(i)) continue;
                    Vector2 begin = new Vector2(j * xPerColumnMonth, Size.Y * positiveRatio);
                    Vector2 offset = new Vector2(xPerColumnMonth, -monthlyData[i] * yPerValueMonth);
                    DrawColumn(begin, offset, colors[k++ % colors.Length], out Vector2 top, out Vector2 middle, out Vector2 down);
                    string[] texts = new string[] { null, null, null };
                    if (writeValues)
                        switch (monthlyData[i])
                        {
                            case < 0:
                                texts[2] = monthlyData[i].ToString();
                                break;
                            case > 0:
                                texts[0] = monthlyData[i].ToString();
                                break;
                        }
                }

                break;
        }

    }
    #endregion

    private Dictionary<int, float> CalculateMonthlyData()
    {
        Dictionary<int, float> monthlyData = new Dictionary<int, float>();
        for (int i = 0; i < data.Count; i++)
        {
            int month = dates[i].Month + 12 * dates[i].Year;
            if (!monthlyData.ContainsKey(month))
            {
                monthlyData.Add(month, data[i]);
            }
            else
            {
                monthlyData[month] += data[i];
            }
        }
        return monthlyData;
    }
    private Dictionary<int, float> CalculateYearlyData()
    {
        Dictionary<int, float> yearlyData = new Dictionary<int, float>();
        for (int i = 0; i < data.Count; i++)
        {
            int year = dates[i].Year;
            if (!yearlyData.ContainsKey(year))
            {
                yearlyData.Add(year, data[i]);
            }
            else
            {
                yearlyData[year] += data[i];
            }
        }
        return yearlyData;
    }
}
