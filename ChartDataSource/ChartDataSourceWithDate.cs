using Godot;
using Date = System.DateOnly;
[GlobalClass, Tool]
public partial class ChartDataSourceWithDate : ChartDataSource
{
    private enum Unit
    {
        Day,
        Month,
        Year
    }
    public Date date { get; private set; }
    [Export(hint: PropertyHint.Range, hintString: "1,31")]
    public int Day
    {
        get => date.Day;
        set
        {
            Update(Unit.Day, value);
            target.QueueRedraw();
        }
    }
    [Export(hint: PropertyHint.Range, hintString: "1,12")]
    public int Month
    {
        get => date.Month;
        set
        {
            Update(Unit.Month, value);
            target.QueueRedraw();
        }
    }
    [Export(hint: PropertyHint.Range, hintString: "1,9999")]
    public int Year
    {
        get => date.Year;
        set
        {
            Update(Unit.Year, value);
            target.QueueRedraw();
        }
    }
    private void Update(Unit unit, int value)
    {

        try
        {
            Date date = default;
            switch (unit)
            {
                case Unit.Day:
                    date = new Date(Year, Month, value);
                    break;
                case Unit.Month:
                    date = new Date(Year, value, Day);
                    break;
                case Unit.Year:
                    date = new Date(value, Month, Day);
                    break;

            }
            this.date = date;
        }
        catch (System.ArgumentOutOfRangeException)
        {
            GD.PushError("Invalid date");
        }
    }
}