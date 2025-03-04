﻿using System.Linq;

namespace ReactNativeTizen.ElmSharp.Extension
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    internal class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <remarks>
        /// Fisrt remove all items, send CollectionChanged event with Remove Action
        /// Second call ClearItems of base
        /// </remarks>
        protected override void ClearItems()
        {
            var oldItems = Items.ToList();
            Items.Clear();
            using (BlockReentrancy())
            {
                // TODO: changed notification here, do not forget ...
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
            }
            base.ClearItems();
        }
    }
}