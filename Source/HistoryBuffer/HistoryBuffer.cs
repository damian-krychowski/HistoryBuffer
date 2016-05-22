using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoryBuffer
{
    public delegate void HistoryOverflowedEventHandler<T>(object sender, HistoryEventArgs<T> e);
    public delegate void NewItemRememberEventHandler<T>(object sender, HistoryEventArgs<T> e);
    public delegate void CurrentItemForgottenEventHandler<T>(object sender, HistoryEventArgs<T> e);
    public delegate void HistoryUndidEventHandler<T>(object sender, HistoryEventArgs<T> e);
    public delegate void HisotryRepeatedEventHandler<T>(object sender, HistoryEventArgs<T> e);
    
    public class HistoryBuffer<T>
    {
        private readonly List<T> _history;

        public int? MaxSize { get; set; }

        public event HistoryOverflowedEventHandler<T> HistoryOverflowed;
        public event NewItemRememberEventHandler<T> NewItemRemembered;
        public event CurrentItemForgottenEventHandler<T> CurrentItemForgotten;
        public event HistoryUndidEventHandler<T> HistoryUndid;
        public event HisotryRepeatedEventHandler<T> HistoryRepeated;

        public T CurrentItem
        {
            get
            {
                if(CurrentIndex == -1) throw new HistoryIsEmptyException("There are no items in the history.");
                return _history[CurrentIndex];
            }
        }
        public int Count => _history.Count;

        private int CurrentIndex { get; set; } = -1;
        private int NextIndex => CurrentIndex + 1;
        private T NextItem => _history[NextIndex];

        private int PreviousIndex => CurrentIndex - 1;
        private T PreviousItem => _history[PreviousIndex];

        private int MaxHistoryIndex => _history.MaxIndex();
        
        public HistoryBuffer()
        {
          _history = new List<T>();   
        }

        public HistoryBuffer(int maxSize)
        {
            MaxSize = maxSize;
            _history = new List<T>(maxSize);
        }

        public void RememberNew(T item)
        {
            var eventArgs = _history.IsEmpty()
                ? HistoryEventArgs<T>.OnlyCurrentItem(
                    currentItem: item)
                : HistoryEventArgs<T>.BothItems(
                    currentItem: item,
                    previousItem: CurrentItem);

            AddNewItem(item);
            NewItemRemembered?.Invoke(this, eventArgs);

            OverflowIfNecessary(eventArgs);
        }

        private void OverflowIfNecessary(HistoryEventArgs<T> eventArgs)
        {
            if (!MaxSize.HasValue || CurrentIndex < MaxSize.Value) return;

            RemoveOldestItem();
            HistoryOverflowed?.Invoke(this, eventArgs);
        }

        private void AddNewItem(T item)
        {
            CurrentIndex++;
            _history.RemoveAllStartingFrom(CurrentIndex);
            _history.Add(item);
        }

        private void RemoveOldestItem()
        {
            _history.RemoveAt(0);
            CurrentIndex --;
        }

        public void ForgetCurrent()
        {
            if (_history.IsEmpty()) return;

            var eventArgs = CurrentIndex == 0
                ? HistoryEventArgs<T>.OnlyPreviousItem(
                    previousItem: CurrentItem)
                : HistoryEventArgs<T>.BothItems(
                    currentItem: PreviousItem,
                    previousItem: CurrentItem);

            RemoveCurrentItem();
            CurrentItemForgotten?.Invoke(this, eventArgs);
        }

        private void RemoveCurrentItem()
        {
            _history.RemoveAt(CurrentIndex);
            CurrentIndex--;
        }

        public void Repeat()
        {
            if (CurrentIndex >= MaxHistoryIndex) return;

            var eventArgs = HistoryEventArgs<T>.BothItems(
                currentItem: NextItem,
                previousItem: CurrentItem);

            CurrentIndex ++;
            HistoryRepeated?.Invoke(this, eventArgs);
        }

        public void Undo()
        {
            if (CurrentIndex <= 0) return;

            var eventArgs = HistoryEventArgs<T>.BothItems(
                currentItem: PreviousItem,
                previousItem: CurrentItem);

            CurrentIndex --;
            HistoryUndid?.Invoke(this, eventArgs);
        }

        public IEnumerable<T> GetAll()
        {
            return _history.ToList();
        }
    }
}
