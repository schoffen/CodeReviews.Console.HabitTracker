# Console Habit Tracker
This was my first CRUD application, it's used to track habits. Made with C# and SQLite.

### Requirements:
- This is an application where you’ll log occurrences of a habit
- This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)
- Users need to be able to input the date of the occurrence of the habit
- The application should store and retrieve data from a real database
- When the application starts, it should create a sqlite database, if one isn’t present
- It should also create a table in the database, where the habit will be logged
- The users should be able to insert, delete, update and view their logged habit
- You should handle all possible errors so that the application never crashes
- You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper
- Follow the DRY Principle, and avoid code repetition

### Optional Requirements:
- Use parameterized queries
- Let users create their own habits, with their own measurement unity
- Create a report functionality (I didn't complete this one)

### Features
- Clean console UI using Spectre.Console prompts
- Full CRUD operations with Dapper
- Clear separation between controller, service, and repository layers
- Custom exception handling

### Lessons Learned:
This project helped me better understand responsibility boundaries in a growing codebase. Separating concerns is not trivial, especially as new features are added, and I often found myself refactoring code to improve structure and readability.

I also realized that I tend to overcomplicate simple solutions, which taught me the importance of balancing clean architecture with pragmatism. Along the way, I learned and practiced concepts such as generic types, interfaces, actions, exception handling, and working with third-party libraries.
