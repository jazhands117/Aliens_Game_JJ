namespace Lab08.GameDesign
{
    public class Inventory
    {
        private readonly List<IItem> _items = new List<IItem>();
        public IReadOnlyList<IItem> Items => _items;

        public IItem? GetItemByName(string name)
        {
            return _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        public void AddItem(IItem newItem)
        {
            var existingItem = _items.FirstOrDefault(i => i.Name == newItem.Name);
            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                _items.Add(newItem);
            }
        }
        public void RemoveItem(IItem itemToRemove, int quantity)
        {
            var existingItem = _items.FirstOrDefault(i => i.Name == itemToRemove.Name);
            if (existingItem != null)
            {
                existingItem.Quantity -= quantity;
                if (existingItem.Quantity <= 0)
                {
                    _items.Remove(existingItem);
                }
            }
        }
        public void RemoveStack(IItem itemToRemove)
        {
            var existingItem = _items.FirstOrDefault(i => i.Name == itemToRemove.Name);
            if (existingItem != null)
            {
                _items.Remove(existingItem);
            }
        }
        public bool Contains(string item)
        {
            return _items.Any(i => i.Name.Equals(item, StringComparison.OrdinalIgnoreCase));
        }
        public bool HasItem(IItem item)
        {
            return _items.Contains(item);
        }
    }
}