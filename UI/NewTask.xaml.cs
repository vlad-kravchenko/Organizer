using System;
using System.Collections.Generic;
using System.Windows;

namespace UI
{
    public partial class NewTask : Window
    {
        Task task = null;

        List<string> rules = new List<string>()
        {
            "Открытая задача",
            "Один раз",
            "Каждый день",
            "Каждую неделю",
            "Каждый месяц",
            "Каждый год",
            "Будни дни",
            "Выходные"
        };

        public NewTask(Task task)
        {
            this.task = task;
            InitializeComponent();
            TaskRepeatRule.ItemsSource = rules;
            for (int i = 0; i < 24; i++)
            {
                string hour = i.ToString();
                if (i < 10) hour = "0" + hour;
                TaskHour.Items.Add(hour);
            }
            for (int i = 0; i < 60; i++)
            {
                string min = i.ToString();
                if (i < 10) min = "0" + min;
                TaskMinute.Items.Add(min);
            }
            TaskFixedTime.IsChecked = false;
            TaskRepeatRule.SelectedIndex = 0;
            TaskStartDate.SelectedDate = DateTime.Today;
            TaskText.Text = "Новая задача";

            if (task != null)
            {
                TaskText.Text = task.Text;
                TaskRepeatRule.SelectedItem = task.RepeatRule;
                TaskFixedTime.IsChecked = task.FixedTime;
                TaskHour.SelectedItem = (task.Hour > 9 ? task.Hour.ToString() : "0" + task.Hour);
                TaskMinute.SelectedItem = (task.Minute > 9 ? task.Minute.ToString() : "0" + task.Minute);
                TaskStartDate.SelectedDate = task.StartDate;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskText.Text) || TaskStartDate.SelectedDate == null) return;
            if (task == null)
            {
                task = new Task
                {
                    Text = TaskText.Text,
                    RepeatRule = TaskRepeatRule.Text,
                    FixedTime = TaskFixedTime.IsChecked.Value,
                    StartDate = TaskStartDate.SelectedDate.Value
                };
            }
            else
            {
                task.Text = TaskText.Text;
                task.RepeatRule = TaskRepeatRule.Text;
                task.FixedTime = TaskFixedTime.IsChecked.Value;
                task.StartDate = TaskStartDate.SelectedDate.Value;
            }
            if (TaskFixedTime.IsChecked.Value)
            {
                task.Hour = GetVal(TaskHour.Text);
                task.Minute = GetVal(TaskMinute.Text);
            }
            using (var db = new OrganizerContext())
            {
                db.Tasks.AddOrUpdateExtension(task);
                db.SaveChanges();
            }
            DialogResult = true;
            Close();
        }

        private int GetVal(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            if (text[0] == '0')
            {
                text = text.Remove(0, 1);
                int res = 0;
                if (int.TryParse(text, out res)) return res;
            }
            else if (int.TryParse(text, out int res)) return res;
            return 0;
        }
    }
}
