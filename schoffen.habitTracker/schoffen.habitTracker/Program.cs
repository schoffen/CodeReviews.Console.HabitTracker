using System.Globalization;
using Spectre.Console;

namespace schoffen.habitTracker;

internal abstract class Program
{
    private static void Main()
    {
        var db = new Database();

        MainMenu(db);
    }

    private static void MainMenu(Database database)
    {
        Console.Clear();

        var option = AnsiConsole.Prompt(
            new SelectionPrompt<MainMenuOptions>()
                .Title("Habit Tracker\nWhat would you like to do?")
                .AddChoices(Enum.GetValues<MainMenuOptions>()));

        switch (option)
        {
            case MainMenuOptions.Habits:
                HabitsMenu(database);
                break;
            case MainMenuOptions.Records:
                RecordsMenu(database);
                break;
            case MainMenuOptions.Exit:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void HabitsMenu(Database database)
    {
        while (true)
        {
            Console.Clear();
            
            var habits = database.GetAllHabits();

            var option = AnsiConsole.Prompt(new SelectionPrompt<ProgramOptions>()
                .Title("My Habits")
                .AddChoices(Enum.GetValues<ProgramOptions>()));

            switch (option)
            {
                case ProgramOptions.ShowAll:
                    if (habits.Count == 0)
                    {
                        Console.WriteLine("\nNo habits found, start by adding one!\nPress any key to return");
                        Console.ReadKey();
                        continue;
                    }

                    var table = new Table();
                    table.AddColumns(["Habit", "Unity of Measurement"]);

                    foreach (var habit in habits)
                    {
                        table.AddRow(habit.Name, habit.MeasurementUnity);
                    }

                    AnsiConsole.Write(table);

                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    continue;
                case ProgramOptions.New:
                    var name = AnsiConsole.Prompt(
                        new TextPrompt<string>("Habit name: "));

                    var measurement = AnsiConsole.Prompt(
                        new TextPrompt<string>("Unity of measurement: "));

                    database.AddHabit(new Habit(name, measurement));

                    Console.WriteLine($"Habit {name} has been added.\nPress any key to return");
                    continue;
                case ProgramOptions.Delete:
                    Console.WriteLine("\n\nWARNING: Delete a habit is going to delete every record associated with it!!!\n\n");
                    
                    var habitToDelete = AnsiConsole.Prompt(
                        new SelectionPrompt<Habit>()
                            .Title("Select Habit to delete it: ")
                            .AddChoices(habits));
                    
                    if (ConfirmationInput())
                    {
                        database.DeleteHabit(habitToDelete.Id);
                        Console.WriteLine("Habit was deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Operation was canceled.\n");
                    }
                    
                    Console.WriteLine("\nPress any key to continue");
                    Console.ReadKey();
                    continue;
                
                case ProgramOptions.Update:
                    var habitToUpdate = AnsiConsole.Prompt(
                        new SelectionPrompt<Habit>()
                            .Title("Select Habit to delete it: ")
                            .AddChoices(habits));

                    var propertyToUpdate = AnsiConsole.Prompt(
                        new SelectionPrompt<HabitUpdateProperty>()
                            .Title("Which property you want to update? ")
                            .AddChoices(Enum.GetValues<HabitUpdateProperty>()));

                    switch (propertyToUpdate)
                    {
                        case HabitUpdateProperty.Name:
                            var newName = AnsiConsole.Prompt(new TextPrompt<string>("Enter new name: "));

                            if (!ConfirmationInput())
                                break;
                            
                            database.UpdateHabitName(habitToUpdate.Id, newName);
                            Console.WriteLine("Name updated.");
                            break;
                        
                        case HabitUpdateProperty.MeasurementUnity:
                            var newUnity = AnsiConsole.Prompt(new TextPrompt<string>("Enter new measurement unity: "));

                            if (!ConfirmationInput())
                                break;
                            
                            database.UpdateHabitMeasurementUnity(habitToUpdate.Id, newUnity);
                            Console.WriteLine("Measurement unity updated.");
                            break;
                    }
                    
                    HoldInput();
                    continue;
                
                case ProgramOptions.Return:
                    MainMenu(database);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            break;
        }
    }

    private static void RecordsMenu(Database database)
    {
        Console.Clear();
            
        var habits = database.GetAllHabits();
        if (habits.Count == 0)
        {
            Console.WriteLine("No habits found, you must add one to start tracking.\n");
            HoldInput();
            MainMenu(database);
        }
            
        var habit = AnsiConsole.Prompt(new SelectionPrompt<Habit>().
            Title("Which habit you want to use? ")
            .AddChoices(habits)
        );
        
        while (true)
        {
            Console.Clear();
            
            var option = AnsiConsole.Prompt(new SelectionPrompt<ProgramOptions>()
                .Title($"{habit.Name} Records\nWhat would you like to do?")
                .AddChoices(Enum.GetValues<ProgramOptions>()));
            
            var records = database.GetAllRecordsFromHabit(habit);
            
            switch (option)
            {
                case ProgramOptions.ShowAll:
                    if (records.Count > 0)
                    {
                        var table = new Table();
                        table.AddColumns(["Date", $"{habit.MeasurementUnity}"]);

                        foreach (var record in records)
                        {
                            table.AddRow(record.Date, record.Amount.ToString());
                        }

                        AnsiConsole.Write(table);
                    }
                    else
                    {
                        Console.WriteLine($"No records found for {habit.Name}.");
                    }

                    HoldInput();
                    
                    continue;
                case ProgramOptions.New:
                    Console.WriteLine($"Tracking {habit.Name}\n");
                    
                    var date = GetDateInput();
                    var amount = AnsiConsole.Prompt(new TextPrompt<int>($"Amount of {habit.MeasurementUnity}?"));
                    
                    database.AddRecord(new Record(date, amount, habit));

                    Console.WriteLine($"""

                                       Date: {date}
                                       Habit: {habit.Name}
                                       {habit.MeasurementUnity}: {amount}

                                       """);
                    Console.WriteLine("Record registered.\nPress any key to continue");
                    Console.ReadKey();
                    continue;
                
                case ProgramOptions.Update:
                    
                    var recordToUpdate = AnsiConsole.Prompt(
                        new SelectionPrompt<Record>()
                            .Title("Select a record to update.")
                            .AddChoices(records));
                    
                    var propertyToUpdate = AnsiConsole.Prompt(
                        new SelectionPrompt<RecordUpdateProperty>()
                            .Title("Which property you want to update? ")
                            .AddChoices(Enum.GetValues<RecordUpdateProperty>()));

                    switch (propertyToUpdate)
                    {
                        case RecordUpdateProperty.Date:
                            var newDate = GetDateInput();

                            if (!ConfirmationInput())
                                break;
                            
                            database.UpdateRecordDate(recordToUpdate.Id, newDate);
                            Console.WriteLine("Date updated.");
                            break;
                        
                        case RecordUpdateProperty.Quantity:
                            var newQuantity = AnsiConsole.Prompt(new TextPrompt<int>("Enter new quantity: "));

                            if (!ConfirmationInput())
                                break;
                            
                            database.UpdateRecordQuantity(recordToUpdate.Id, newQuantity);
                            Console.WriteLine("Quantity updated");
                            break;
                    }
                    
                    HoldInput();
                    continue;
                
                case ProgramOptions.Delete:
                    var recordChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<Record>()
                            .Title("Select a record to delete:")
                            .AddChoices(records));
                    
                    if (ConfirmationInput())
                    {
                        database.DeleteRecord(recordChoice.Id);
                        Console.WriteLine("Record was deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Operation was canceled.\n");
                    }
                    
                    HoldInput();
                    continue;
                case ProgramOptions.Return:
                    MainMenu(database);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            break;
        }
    }

    private static string GetDateInput()
    {
        Console.WriteLine("Type date in this format [dd-mm-yyyy]. Or leave empty for today.");

        var dateInput = Console.ReadLine();

        if (dateInput == "")
            return DateTime.Now.ToString("dd-MM-yyyy");

        while (!DateTime.TryParseExact(dateInput, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            Console.WriteLine("\nInvalid date, please follow the format [dd-MM-yyyy]");
            dateInput = Console.ReadLine();
        }

        return dateInput;
    }

    private static bool ConfirmationInput()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<bool>("Are you sure you want to continue?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));
    }

    private static void HoldInput()
    {
        Console.WriteLine("\nPress any key to continue");
        Console.ReadKey();
    }
}