using System.Collections.Generic;

namespace TodoApi.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<TodoItem> TodoItems { get; set; }
    }
}