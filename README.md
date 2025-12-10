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
- SQLite initialization: The program creates two tables (habits and records) when it starts
- Spectre.Console prompts: Provides cleaner console output and easier user selection
- CRUD operations: Add, update, read, and delete habits and records

### Lessons Learned:
This project involved a lot of repetition, which helped me understand how to open SQLite connections and manage a database using queries.
It was also my largest project so far, and I had to think carefully about how to organize my code and avoid repetition. Some parts still need improvement, especially in structure and organization.

I realized that understanding the problem before coding saves a lot of time. At some point the project grew so much that finding things became difficult, and refactoring felt overwhelming because I was learning many new concepts at the same time.
