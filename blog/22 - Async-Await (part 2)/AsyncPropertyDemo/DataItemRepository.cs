using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncPropertyDemo
{
    public class DataItemRepository
    {
        public async Task<IReadOnlyCollection<DataItemViewModel>> LoadData()
        {
            // simulate delay in calling a database or web service.
            await Task.Delay(TimeSpan.FromSeconds(1));

            var vms = new List<DataItemViewModel>
            {
                new DataItemViewModel(@".\images\apple.png", "Apple"),
                new DataItemViewModel(@".\images\arrows.png", "Arrows"),
                new DataItemViewModel(@".\images\atom.png", "Atom"),
                new DataItemViewModel(@".\images\balloons.png", "Balloons"),
                new DataItemViewModel(@".\images\clipboard.png", "Clipboard"),
                new DataItemViewModel(@".\images\flag.png", "Flag"),
                new DataItemViewModel(@".\images\floppy_disks.png", "Floppy Disks"),
                new DataItemViewModel(@".\images\hand.png", "Hand"),
                new DataItemViewModel(@".\images\key.png", "Key"),
                new DataItemViewModel(@".\images\lightbulb.png", "Light Bulb"),
                new DataItemViewModel(@".\images\loudspeaker.png", "Loudspeaker"),
                new DataItemViewModel(@".\images\masks.png", "Masks"),
                new DataItemViewModel(@".\images\plant.png", "Plant"),
                new DataItemViewModel(@".\images\playing_cards.png", "Playing Cards"),
                new DataItemViewModel(@".\images\shopping_cart.png", "Shopping Cart"),
                new DataItemViewModel(@".\images\tap.png", "Tap"),
                new DataItemViewModel(@".\images\thread.png", "Thread"),
                new DataItemViewModel(@".\images\tractor.png", "Tractor"),
                new DataItemViewModel(@".\images\tree.png", "Tree"),
                new DataItemViewModel(@".\images\truck.png", "Truck"),
                new DataItemViewModel(@".\images\wine.png", "Wine")
            };

            return vms.AsReadOnly();
        }
    }
}