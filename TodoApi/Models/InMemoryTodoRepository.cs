using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public class InMemoryTodoRepository : ITodoRepository
    {
        private static ConcurrentDictionary<string, TodoItem> _todos =
              new ConcurrentDictionary<string, TodoItem>();

        public InMemoryTodoRepository()
        {
            Add(new TodoItem { Name = "Walk dog", IsComplete = false, Id = "9e882920-78f2-4aaa-a14a-b062345bc391" });
            Add(new TodoItem { Name = "Walk dog 2", IsComplete = false, Id = "9e882920-78f2-4aaa-a14a-b062345bc392" });
            Add(new TodoItem { Name = "Walk dog 3", IsComplete = true, Id = "9e882920-78f2-4aaa-a14a-b062345bc393" });
            Add(new TodoItem { Name = "Walk dog 4", IsComplete = true, Id = "9e882920-78f2-4aaa-a14a-b062345bc394" });
        }

        public IEnumerable<TodoItem> GetAll()
        {
            //var results = _todos.Values;
            //return results;
            return _todos.Values;
        }

        public bool Add(TodoItem item)
        {
            //item.Id = Guid.NewGuid().ToString();
            var res = _todos.TryAdd(item.Id, item);
            //_todos[item.Id] = item;
            return res;
        }

        public TodoItem Find(string key)
        {
            TodoItem item;
            _todos.TryGetValue(key, out item);
            return item;
        }

        public TodoItem Remove(string key)
        {
            TodoItem item;
            _todos.TryGetValue(key, out item);
            _todos.TryRemove(key, out item);
            return item;
        }

        public void Update(TodoItem item)
        {
            _todos[item.Id] = item;
        }
    }
}
