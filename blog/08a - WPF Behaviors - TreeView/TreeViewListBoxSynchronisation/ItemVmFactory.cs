using Peregrine.Library;
using System;
using System.Collections.Generic;

namespace TreeViewListBoxSynchronisation
{
    public static class ItemVmFactory
    {
        private static readonly Random Random = new Random(1234); // generate the same "random" sequence each time the application is run

        private static int _nextItemId = 1;

        public static IEnumerable<ItemVm> CreateItemVms(string baseCaption, int level)
        {
            var result = new List<ItemVm>();
            var itemCount = 3 + Random.Next(4);

            for (var i = 1; i <= itemCount; i++)
            {
                var caption = baseCaption + (level == 0 ? "" : ".") + i;
                var item = new ItemModel(_nextItemId++, caption, level);
                var vm = new ItemVm(item)
                {
                    IsEnabled = Random.Next(10) != 0 // random 10% chance of being disabled
                };

                result.Add(vm);
            }

            AllItemVmsList.AddRange(result, true);

            return result;
        }

        private static readonly perObservableCollection<ItemVm> AllItemVmsList = new perObservableCollection<ItemVm>();

        public static IEnumerable<ItemVm> AllItemVms => AllItemVmsList;
    }
}