using Microsoft.Data.Sqlite;

namespace schoffen.habitTracker;

public class Database
{
    private const string ConnectionString = @"Data Source=habit-tracker.db";
    private const string HabitsTableName = "habits";
    private const string RecordsTableName = "habit_logs";

    public Database()
    {
        CreateStartUpTables();
    }

    private void CreateStartUpTables()
    {
        using var connection = GetConnection();
        connection.Open();

        var habitsTableCommand = connection.CreateCommand();
        habitsTableCommand.CommandText = $"""
                                          CREATE TABLE IF NOT EXISTS {HabitsTableName} (
                                              id INTEGER PRIMARY KEY AUTOINCREMENT,
                                              name TEXT,
                                              measurement_unit TEXT
                                          );
                                          """;

        habitsTableCommand.ExecuteNonQuery();

        var habitsLogsTableCommand = connection.CreateCommand();
        habitsLogsTableCommand.CommandText = $"""
                                              CREATE TABLE IF NOT EXISTS {RecordsTableName} (
                                                  id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                  log_date TEXT,
                                                  quantity INTEGER,
                                                  habit_id INTEGER,
                                                  FOREIGN KEY (habit_id) REFERENCES habits(id) ON DELETE CASCADE
                                              );
                                              """;

        habitsLogsTableCommand.ExecuteNonQuery();

        connection.Close();
    }

    private SqliteConnection GetConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    internal void AddHabit(Habit habit)
    {
        using var connection = GetConnection();
        connection.Open();

        var addCmd = connection.CreateCommand();
        addCmd.CommandText = $"""
                              INSERT INTO {HabitsTableName} (name, measurement_unit)
                              VALUES (@Name, @Measurement);
                              """;

        addCmd.Parameters.Add("@Name", SqliteType.Text).Value = habit.Name;
        addCmd.Parameters.Add("@Measurement", SqliteType.Integer).Value = habit.MeasurementUnity;

        addCmd.ExecuteNonQuery();
    }

    internal List<Habit> GetAllHabits()
    {
        using var connection = GetConnection();
        connection.Open();

        var getCmd = connection.CreateCommand();
        getCmd.CommandText = $"SELECT * FROM {HabitsTableName};";

        using var reader = getCmd.ExecuteReader();

        List<Habit> habits = [];

        while (reader.Read())
        {
            habits.Add(new Habit(reader.GetString(1),
                reader.GetString(2), reader.GetInt32(0)));
        }

        reader.Close();
        connection.Close();

        return habits;
    }

    internal void UpdateHabitName(int id, string name)
    {
        using var connection = GetConnection();
        connection.Open();

        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"""
                                    UPDATE {HabitsTableName}
                                    SET name=@Name
                                    WHERE id=@Id;
                                 """;

        updateCmd.Parameters.Add("@Name", SqliteType.Text).Value = name;
        updateCmd.Parameters.Add("@Id", SqliteType.Integer).Value = id;

        updateCmd.ExecuteNonQuery();
        connection.Close();
    }
    
    internal void UpdateHabitMeasurementUnity(int id, string measurementUnity)
    {
        using var connection = GetConnection();
        connection.Open();

        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"""
                                    UPDATE {HabitsTableName}
                                    SET measurement_unit=@MeasurementUnity
                                    WHERE id=@Id;
                                 """;

        updateCmd.Parameters.Add("@MeasurementUnity", SqliteType.Text).Value = measurementUnity;
        updateCmd.Parameters.Add("@Id", SqliteType.Integer).Value = id;

        updateCmd.ExecuteNonQuery();
        connection.Close();
    }
    
    internal void DeleteHabit(int id)
    {
        using var connection = GetConnection();
        connection.Open();

        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = $"DELETE FROM {HabitsTableName} WHERE id=@HabitID";
        deleteCmd.Parameters.Add("HabitID", SqliteType.Integer).Value = id;
        
        deleteCmd.ExecuteNonQuery();
    }

    internal void AddRecord(Record record)
    {
        using var connection = GetConnection();
        connection.Open();

        var addCmd = connection.CreateCommand();
        addCmd.CommandText = $"""
                                 INSERT INTO {RecordsTableName} (log_date, quantity, habit_id)
                                 VALUES (@LogDate, @Quantity, @HabitId);
                              """;

        addCmd.Parameters.Add("@LogDate", SqliteType.Text).Value = record.Date;
        addCmd.Parameters.Add("@Quantity", SqliteType.Integer).Value = record.Amount;
        addCmd.Parameters.Add("@HabitId", SqliteType.Integer).Value = record.Habit.Id;

        addCmd.ExecuteNonQuery();
    }
    
    internal List<Record> GetAllRecordsFromHabit(Habit habit)
    {
        using var connection = GetConnection();
        connection.Open();

        var getCmd = connection.CreateCommand();
        getCmd.CommandText = $"""
                                SELECT {RecordsTableName}.id, 
                                  {RecordsTableName}.log_date,
                                  {RecordsTableName}.quantity,
                                  {HabitsTableName}.id,
                                  {HabitsTableName}.name,
                                  {HabitsTableName}.measurement_unit
                                  FROM {RecordsTableName} 
                                  JOIN {HabitsTableName} ON {RecordsTableName}.habit_id = {HabitsTableName}.id
                                  WHERE {RecordsTableName}.habit_id=@HabitID;
                              """;

        getCmd.Parameters.Add("@HabitID", SqliteType.Integer).Value = habit.Id; 

        using var reader = getCmd.ExecuteReader();

        List<Record> records = [];

        while (reader.Read())
        {
            var record = new Record(
                reader.GetString(1),
                reader.GetInt32(2),
                habit,
                reader.GetInt32(0)
            );
            
            records.Add(record);
        }
        
        reader.Close();
        connection.Close();

        return records;
    }
    
    internal void UpdateRecordDate(int id, string date)
    {
        using var connection = GetConnection();
        connection.Open();

        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"""
                                    UPDATE {RecordsTableName}
                                    SET log_date=@Date
                                    WHERE id=@Id;
                                 """;

        updateCmd.Parameters.Add("@Date", SqliteType.Text).Value = date;
        updateCmd.Parameters.Add("@Id", SqliteType.Integer).Value = id;

        updateCmd.ExecuteNonQuery();
        connection.Close();
    }
    
    internal void UpdateRecordQuantity(int id, int quantity)
    {
        using var connection = GetConnection();
        connection.Open();

        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"""
                                    UPDATE {RecordsTableName}
                                    SET quantity=@Quantity
                                    WHERE id=@Id;
                                 """;

        updateCmd.Parameters.Add("@Quantity", SqliteType.Integer).Value = quantity;
        updateCmd.Parameters.Add("@Id", SqliteType.Integer).Value = id;

        updateCmd.ExecuteNonQuery();
        connection.Close();
    }
    
    internal void DeleteRecord(int recordId)
    {
        using var connection = GetConnection();
        connection.Open();

        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = $"""
                                DELETE FROM {RecordsTableName}
                                WHERE id=@RecordId;
                                """;

        deleteCmd.Parameters.Add("@RecordId", SqliteType.Integer).Value = recordId;

        deleteCmd.ExecuteNonQuery();
        connection.Close();
    }
}