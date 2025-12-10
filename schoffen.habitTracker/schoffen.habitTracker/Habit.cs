namespace schoffen.habitTracker;

public class Habit(string name, string measurementUnity, int id = 0)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string MeasurementUnity { get; set; } = measurementUnity;

    public override string ToString() => $"{Name}";
}