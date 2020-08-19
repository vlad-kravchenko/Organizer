using System;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class TaskListItem : UserControl
    {
        public Task Task { get; set; }
        public MainWindow ParentWindow { get; set; }

        public TaskListItem()
        {
            InitializeComponent();
        }

        public void UpdateGraphics()
        {
            TaskText.Text = Task.Text;
            TaskRepeatRule.Text = Task.RepeatRule;
            if (Task.FixedTime)
            {
                Remind.Visibility = Visibility.Visible;
                TaskFixedTime.IsChecked = Task.FixedTime;
                TaskFixedTime.Content = GetTaskTime();
            }
            Next.Text = Task.StartDate.ToShortDateString();
        }

        private string GetTaskTime()
        {
            return TaskFixedTime.Content + " в " + (Task.Hour > 9 ? Task.Hour.ToString() : "0" + Task.Hour) + ":" +
                            (Task.Minute > 9 ? Task.Minute.ToString() : "0" + Task.Minute);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewTask(Task);
            dialog.Title = "Редактировать задачу";
            if (dialog.ShowDialog() == true)
            {
                using (var db = new OrganizerContext())
                {
                    db.Tasks.AddOrUpdateExtension(Task);
                    db.SaveChanges();
                    ParentWindow.ReloadTasks();
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            RemoveTask();
        }

        private void RemoveTask()
        {
            using (var db = new OrganizerContext())
            {
                db.Tasks.Remove(db.Tasks.Find(Task.Id));
                db.SaveChanges();
                ParentWindow.ReloadTasks();
            }
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            TaskManager.ExecuteTask(Task, ParentWindow);
        }
    }
}