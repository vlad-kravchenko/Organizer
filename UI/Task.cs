using System;
using System.ComponentModel.DataAnnotations;

namespace UI
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string RepeatRule { get; set; }
        public bool FixedTime { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public DateTime StartDate { get; set; }
    }
}