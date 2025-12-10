namespace schoffen.habitTracker;

public class Record(string date, int amount, Habit habit, int id = 0)
{
    public int Id { get; set; } = id;
    public string Date { get; set; } = date;
    public int Amount { get; set; } = amount;
    public Habit Habit { get; set; } = habit;
    
    public override string ToString() => $"{Date} - {Amount} {Habit.MeasurementUnity}";
}