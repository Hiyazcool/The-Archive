using System;
using Unity.Collections;
using UnityEngine;
using Hiyazcool;
namespace Inventory
{
    /* Priority High
     * Put this into Hiyazcool Namespace
     * Flesh everything out and Test
     * make events to signal UI and other classes of changes
     * implement Input
     * AddAmount and Subtract Amounts will not work correctly I believe
     * Due to Ref not carrying thruogh all of them
     * Private Methods should work but Rework Exposed to have an in / out? or figure how to persist the reference maybe by modifing the original after the calc
     * Make it easily serializable?
     * Finish Implementing IInvetoryItem Structs
     * 
     * 
     */
    public static class Inventory
    {
        public static void SwapSlots(ref InventorySlot slot1, ref InventorySlot slot2)
        {
            InventorySlot temp = slot1;
            slot1 = slot2;
            slot2 = temp;
            return;
        }
        public struct InventorySlot
        {
            private IInventoryItem _item;
            public IInventoryItem Item
            {
                get { return _item; }
                private set
                {
                    _item = value;
                    SlotSize = value.MaxSize;
                }
            }
            public int SlotSize
            { get; private set; }
            public int ItemCount
            { get; private set; }
            public int ItemStackValue
            { get; private set; }
            private bool AddAmount(ref int amount)
            {
                int addAmount = (amount <= SlotSize - ItemCount) ? amount : SlotSize - ItemCount;
                ItemCount += addAmount;
                UpdateSlot();
                return (amount -= addAmount) == 0 ? true : false;
            }
            public bool AddAmount(ref InventorySlot slot)
            {
                int count = slot.ItemCount;
                var isSuccess = slot.Item == Item ? AddAmount(ref count) : Item == null ? AddItem(slot.Item, ref count) : false;
                if (count != slot.ItemCount)
                    slot.SubtractAmount(ref count);
                return isSuccess;
            }
            public bool AddAmount(IInventoryItem item, ref int amount)
            {
                return item == Item ? AddAmount(ref amount) : Item == null ? AddItem(item, ref amount) : false;
            }
            private bool AddItem(IInventoryItem item, ref int amount)
            {
                Item = item;
                return AddAmount(ref amount);
            }
            private bool SubtractAmount(ref int amount)
            {
                int SubtractAmount = (amount <= ItemCount) ? amount : ItemCount;
                ItemCount -= SubtractAmount;
                UpdateSlot();
                return (amount -= SubtractAmount) == 0 ? true : false;
            }
            public bool SubtractAmount(ref InventorySlot slot)
            {
                int count = slot.ItemCount;
                var isSuccess = slot.Item == Item ? SubtractAmount(ref count) : false;
                if (count != slot.ItemCount)
                    slot.AddAmount(ref count);
                return isSuccess;
            }
            public bool SubtractAmount(IInventoryItem item, ref int amount)
            {
                return item == Item ? SubtractAmount(ref amount) : false;
            }
            public InventorySlot Split(int amount)
            {
                int count = ItemCount != 0 ? amount : 0;
                SubtractAmount(ref count);
                UpdateSlot();
                InventorySlot slotValue = new InventorySlot
                {
                    Item = this.Item,
                    SlotSize = this.SlotSize,
                    ItemCount = amount - count
                };
                slotValue.UpdateSlot();
                return slotValue;

            }
            public InventorySlot Split()
            {
                int amount = ItemCount > 1 ? ItemCount / 2 : ItemCount == 1 ? 1 : 0;
                if (amount == 0)
                    return new InventorySlot { };
                int count = amount;
                SubtractAmount(ref count);
                UpdateSlot();
                InventorySlot slotValue = new InventorySlot
                {
                    Item = this.Item,
                    SlotSize = this.SlotSize,
                    ItemCount = amount - count
                };
                slotValue.UpdateSlot();
                return slotValue;
            }
            public void UpdateSlot()
            {
                if (ItemCount <= 0)
                {
                    Item = null;
                    SlotSize = 0;
                    ItemCount = 0;
                    ItemStackValue = 0;
                    return;
                }
                else
                {
                    ItemStackValue = Item.Value * ItemCount;
                    return;
                }
            }
            private void EmptySlot() => this = InventorySlot.CreateEmptySlot();
            public static InventorySlot CreateEmptySlot() => new InventorySlot()
            {
                Item = null,
                SlotSize = 0,
                ItemCount = 0,
                ItemStackValue = 0,
            };
        }
        public abstract class ItemContext { } // This is for possible future functionality
        public static void test(IInventoryItem item)
        {
            if (item.Tag == ItemTag.Weapon)
            {
                WeaponItem weaponItem = (WeaponItem)item;  
            }
        }
        [Serializable]
        public struct InventoryContainer
        {
            private NativeArray<InventorySlot> inventorySlots;
            private int x, y;
            public InventoryContainer(int x, int y)
            {
                inventorySlots = new NativeArray<InventorySlot>(x * y, Allocator.Persistent);
                this.x = x;
                this.y = y;
            }
            // Dispose function
            // Enumeration to allow iteration through inventory
            // Accessor Methods
        }
    }
    public abstract class ItemContext { }
    public interface IInventoryItem
    {
        public string Name { get; init; }
        public int MaxSize { get; init; }
        public int Value { get; init; }
        public ItemTag Tag { get; }
        public abstract void Interact(ItemContext context);
        public abstract void SecondaryInteract(ItemContext context);
    }
    [Serializable]
    public struct JunkItem : IInventoryItem
    {
        public string Name { get; init; }
        public int MaxSize { get; init; }
        public int Value { get; init; }
        public ItemTag Tag { get { return ItemTag.Junk; } }

        public void Interact(ItemContext context)
        {
            throw new System.NotImplementedException();
        }

        void IInventoryItem.SecondaryInteract(ItemContext context)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public struct WeaponItem : IInventoryItem
    {
        public string Name { get; init; }
        public int MaxSize { get; init; }
        public int Value { get; init; }
        public IWeaponAbility weaponAbility { get; init; }

        public ItemTag Tag { get { return ItemTag.Weapon; } }

        public void Interact(ItemContext context)
        {
            throw new System.NotImplementedException();
        }

        void IInventoryItem.SecondaryInteract(ItemContext context)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public struct ArmourItem : IInventoryItem
    {
        public string Name { get; init; }
        public int MaxSize { get; init; }
        public int Value { get; init; }
        public int Armour { get; init; }
        public ItemTag Tag { get { return ItemTag.Armour; } }

        public void Interact(ItemContext context)
        {
            throw new System.NotImplementedException();
        }
        void IInventoryItem.SecondaryInteract(ItemContext context)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public enum ItemTag
    {
        Weapon,
        Armour,
        Junk
    }
    public struct WeaponAbility : IWeaponAbility
    {

    }
    public interface IWeaponAbility
    {

    }


}