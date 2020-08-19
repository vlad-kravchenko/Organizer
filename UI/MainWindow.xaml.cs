using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace UI
{
    public partial class MainWindow : Window
    {
        NotifyIcon ni = new NotifyIcon();
        bool separatorAdded = false;

        public MainWindow()
        {
            InitializeComponent();
            ni.Icon = Properties.Resources.icon;
            ni.Visible = true;
            ReloadTasks();
            StartMonitor();
        }

        private void StartMonitor()
        {
            new System.Threading.Tasks.TaskFactory().StartNew(() =>
            {
                while (true)
                {
                    using (var db = new OrganizerContext())
                    {
                        foreach (var task in db.Tasks.Where(t => t.StartDate == DateTime.Today && t.FixedTime).ToList())
                        {
                            if (DateTime.Now.Minute == task.Minute && DateTime.Now.Hour == task.Hour)
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ni.ShowBalloonTip(5000, "Напоминание", task.Text, ToolTipIcon.Warning);
                                    TaskManager.ExecuteTask(task, this);
                                }));
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void ReloadTasks()
        {
            separatorAdded = false;
            TaskList.Items.Clear();
            using (var db = new OrganizerContext())
            {
                foreach (var task in db.Tasks.OrderBy(t => t.StartDate).ThenBy(t => t.Hour).ThenBy(t => t.Minute))
                {
                    if (task.StartDate > DateTime.Today) AddSeparator();
                    var item = new TaskListItem();
                    item.Task = task;
                    item.ParentWindow = this;
                    item.UpdateGraphics();
                    TaskList.Items.Add(item);
                }
            }
        }

        private void AddSeparator()
        {
            if (separatorAdded) return;
            var block = new TextBlock();
            block.Text = "Будущие задачи";
            block.TextAlignment = TextAlignment.Center;
            block.FontSize = 20;
            TaskList.Items.Add(block);
            separatorAdded = true;
        }

        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewTask(null);
            dialog.Title = "Новая задача";
            if (dialog.ShowDialog() == true)
            {
                ReloadTasks();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadTasks();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            ni.DoubleClick += (sndr, args) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
            ni.BalloonTipClicked += (sndr, args) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ni.Visible = false;
        }
    }
}