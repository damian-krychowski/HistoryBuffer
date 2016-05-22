using System;
using System.Diagnostics.Contracts;

namespace HistoryBuffer
{
    [Pure]
    public class HistoryEventArgs<T> : EventArgs
    {
        private readonly T _currentItem;
        private readonly bool _hasCurrentItem;
        private readonly bool _hasPreviousItem;
        private readonly T _previousItem;
        
        private HistoryEventArgs(
            bool hasCurrentItem, 
            T currentItem,
            bool hasPreviousItem,
            T previousItem)
        {
            _hasCurrentItem = hasCurrentItem;
            _currentItem = currentItem;
            _hasPreviousItem = hasPreviousItem;
            _previousItem = previousItem;
        }

        public static HistoryEventArgs<T> OnlyCurrentItem(T currentItem)
        {
            return new HistoryEventArgs<T>(true, currentItem, false, default(T));
        }

        public static HistoryEventArgs<T> OnlyPreviousItem(T previousItem)
        {
            return new HistoryEventArgs<T>(false, default(T),true, previousItem);
        }

        public static HistoryEventArgs<T> BothItems(T currentItem, T previousItem)
        {
            return new HistoryEventArgs<T>(true, currentItem, true, previousItem);
        } 

        public bool TryGetCurrentItem(out T currentItem)
        {
            currentItem = _hasCurrentItem ? _currentItem : default(T);
            return _hasCurrentItem;
        }

        public bool TryGetPreviousItem(out T previousItem)
        {
            previousItem = _hasPreviousItem ? _previousItem : default(T);
            return _hasPreviousItem;
        }
    }
}