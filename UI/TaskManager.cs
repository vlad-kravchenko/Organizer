using System;

namespace UI
{
    public static class TaskManager
    {
        public static void ExecuteTask(Task task, MainWindow parentWindow)
        {
            switch (task.RepeatRule)
            {
                case "Открытая задача":
                case "Один раз":
                    using (var db = new OrganizerContext())
                    {
                        var taskToRemove = db.Tasks.Find(task.Id);
                        if (taskToRemove == null) return;
                        db.Tasks.Remove(taskToRemove);
                        db.SaveChanges();
                        parentWindow?.ReloadTasks();
                    }
                    return;
                case "Каждый день":
                    task.StartDate = task.StartDate.AddDays(1);
                    break;
                case "Каждую неделю":
                    task.StartDate = task.StartDate.AddDays(7);
                    break;
                case "Каждый месяц":
                    task.StartDate = task.StartDate.AddMonths(1);
                    break;
                case "Каждый год":
                    task.StartDate = task.StartDate.AddYears(1);
                    break;
                case "Будни дни":
                    if (DateTime.Today.DayOfWeek != DayOfWeek.Saturday && DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
                    {
                        if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                        {
                            task.StartDate = task.StartDate.AddDays(3);
                        }
                        else
                        {
                            task.StartDate = task.StartDate.AddDays(1);
                        }
                    }
                    break;
                case "Выходные":
                    if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                    {
                        task.StartDate = task.StartDate.AddDays(1);
                    }
                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                    {
                        task.StartDate = task.StartDate.AddDays(6);
                    }
                    break;
            }
            using (var db = new OrganizerContext())
            {
                db.Tasks.AddOrUpdateExtension(task);
                db.SaveChanges();
                parentWindow?.ReloadTasks();
            }
        }
    }
}