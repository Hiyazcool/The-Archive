using System;
using Unity.Collections;
using UnityEngine;
using Hiyazcool;
using static Hiyazcool.Inventory.Item;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;

namespace Hiyazcool
{
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
         * Find a solution to equip Items and Interact Items using IIventoryItem 
         *      Maybe using an Action??
         *      Alternatively
         *          Have the Data in the struct then pass it to a method outside the struct determining what to do with it
         *          Should make it simpler to implement multiple controls like Mouse Dragging and Rightclicking a dropdown menu easier to work simultaneously
         *          Need to define a simple struct That contains the Information to relay to reciever 
         *              Reciever need to unpack small size Container that links to Information like a Dictionary *Tada I finally get to make one probably*
         *              
         */
        #region Container Section
        public class InventoryContainer : IEnumerable
        {
            private event Action<SlotContext> SlotChangeEvent;
            private NativeArray<ItemSlot> ItemSlots;
            private int x, y;
            private int inventorySize;
            public InventoryContainer(int x, int y)
            {
                ItemSlots = new NativeArray<ItemSlot>(x * y, Allocator.Persistent);
                this.x = x;
                this.y = y;
                inventorySize = x * y;
            }
            public void SubscribeToSlotChange(Action<SlotContext> action) => SlotChangeEvent += action;
            public void UnSubscribeToSlotChange(Action<SlotContext> action) => SlotChangeEvent -= action;
            public int GetSize() => inventorySize;
            public int2 GetAxis() => new(this.x, this.y);
            public int GetX() => this.x;
            public int GetY() => this.y;
            public struct SlotContext
            {

            }
            /* Things to Implement 
             *      - Access Slot Function 
             *          Invokes SlotChange event
             *      - Save/Load Feature Low Priority
             *      - Implement Safety Features for Subscribe Methods
             *      - Safety for Access Function
             */
            public IEnumerator GetEnumerator()
            {
                return new InventoryEnum(ItemSlots);
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }
        }
        public class InventoryEnum : IEnumerator
        {
            public NativeArray<ItemSlot> ItemSlots;
            public int position = -1;
            public InventoryEnum(NativeArray<ItemSlot> _itemSlots) => ItemSlots = _itemSlots;
            public object Current
            {
                get
                {
                    try
                    {
                        return ItemSlots[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            public bool MoveNext()
            {
                position++;
                return (position < ItemSlots.Length);
            }
            public void Reset() => position = -1;
        }
        // Enumerator
        // Dispose function
        // Enumeration to allow iteration through inventory
        // Accessor Methods */
        #endregion
        public static class Item
        {
            public static void SwapSlots(ref ItemSlot slot1, ref ItemSlot slot2)
            {
                ItemSlot temp = slot1;
                slot1 = slot2;
                slot2 = temp;
                return;
            }
            public struct ItemSlot
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
                private bool AddAmount(ref int amount, out int successCount)
                {
                    successCount = (amount <= SlotSize - ItemCount) ? amount : SlotSize - ItemCount;
                    ItemCount += successCount;
                    UpdateSlot();
                    return ((amount -= successCount) == 0);
                }
                private bool AddAmount(ref int amount)
                {
                    int count = (amount <= SlotSize - ItemCount) ? amount : SlotSize - ItemCount;
                    ItemCount += count;
                    UpdateSlot();
                    return ((amount -= count) == 0);
                }
                public bool AddAmount(ref ItemSlot slot)
                {
                    int successCount = 0;
                    int count = slot.ItemCount;
                    var isSuccess = slot.Item == Item ? AddAmount(ref count, out successCount) : Item == null ? AddItem(slot.Item, ref count) : false;
                    if (count != slot.ItemCount)
                        slot.SubtractAmount(ref successCount);
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
                private bool SubtractAmount(ref int amount, out int successCount)
                {
                    successCount = (amount <= ItemCount) ? amount : ItemCount;
                    ItemCount -= successCount;
                    UpdateSlot();
                    return (amount -= successCount) == 0 ? true : false;
                }
                private bool SubtractAmount(ref int amount)
                {
                    int successCount = (amount <= ItemCount) ? amount : ItemCount;
                    ItemCount -= successCount;
                    UpdateSlot();
                    return (amount -= successCount) == 0 ? true : false;
                }
                public bool SubtractAmount(ref ItemSlot slot)
                {
                    int successCount = 0;
                    int count = slot.ItemCount;
                    var isSuccess = slot.Item == Item ? SubtractAmount(ref count, out successCount) : false;
                    if (count != slot.ItemCount)
                        slot.AddAmount(ref successCount);
                    return isSuccess;
                }
                public bool SubtractAmount(IInventoryItem item, ref int amount)
                {
                    return item == Item ? SubtractAmount(ref amount) : false;
                }
                public ItemSlot Split(int amount)
                {
                    int count = ItemCount > 0 ? amount : 0;
                    SubtractAmount(ref count);
                    UpdateSlot();
                    ItemSlot slotValue = new ItemSlot
                    {
                        Item = this.Item,
                        SlotSize = this.SlotSize,
                        ItemCount = amount - count
                    };
                    slotValue.UpdateSlot();
                    return slotValue;

                }
                public ItemSlot Split()
                {
                    int amount = ItemCount > 1 ? ItemCount / 2 : ItemCount == 1 ? 1 : 0;
                    if (amount == 0)
                        return new ItemSlot { };
                    int count = amount;
                    SubtractAmount(ref count);
                    UpdateSlot();
                    ItemSlot slotValue = new ItemSlot
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
                    if (ItemCount > 0)
                    {
                        ItemStackValue = Item.Value * ItemCount;
                        return;
                    }
                    else if (ItemCount == 0)
                    {
                        EmptySlot();
                        return;
                    }
                    else if (ItemCount < 0 || ItemCount > SlotSize)
                        throw new Exception("Item Count out of Valid Range");
                } // Implement a Event?.Invoke
                private void EmptySlot() => this = ItemSlot.CreateEmptySlot();
                public static ItemSlot CreateEmptySlot() => new ItemSlot()
                {
                    Item = null,
                    SlotSize = 0,
                    ItemCount = 0,
                    ItemStackValue = 0,
                };
            }
        }
        [Serializable]
        public abstract class ItemContext { }
        public interface IInventoryItem : IEquatable<IInventoryItem>
        {
            public string Name { get; init; }
            public int MaxSize { get; }
            public int Value { get; init; }
            public ItemTag Tag { get; }
            public IItemAction[] itemActions { get; init; }
        }
        [Serializable]
        public struct JunkItem : IInventoryItem
        {
            public string Name { get; init; }
            public int MaxSize { get; init; }
            public int Value { get; init; }
            public readonly ItemTag Tag { get { return ItemTag.Junk; } }
            public IItemAction[] itemActions { get; init; }

            public bool Equals(IInventoryItem other)
            {
                return other != null && other.Name == Name && other.Value == Value && other.Tag == Tag && other.MaxSize == MaxSize;
            }
        }
        [Serializable]
        public struct WeaponItem : IInventoryItem
        {
            public string Name { get; init; }
            public readonly int MaxSize { get { return 1; } }
            public int Value { get; init; }
            // Static Lookup Blueprint for Weapon Stats 
            public readonly ItemTag Tag { get { return ItemTag.Weapon; } }
            public IItemAction[] itemActions { get; init; }
            public bool Equals(IInventoryItem other)
            {
                return other != null && other.Name == Name && other.Value == Value && other.Tag == Tag && other.MaxSize == MaxSize;
            }

        }
        [Serializable]
        public struct ArmorItem : IInventoryItem
        {
            public string Name { get; init; }
            public readonly int MaxSize { get { return 1; } }
            public int Value { get; init; }
            public int Armour { get; init; }
            // Static Lookup Blueprint for Armor stats
            public readonly ItemTag Tag { get { return ItemTag.Armor; } }
            public IItemAction[] itemActions { get; init; }
            public bool Equals(IInventoryItem other)
            {
                return other != null && other.Name == Name && other.Value == Value && other.Tag == Tag && other.MaxSize == MaxSize;
            }
        }
        [Serializable]
        public enum ItemTag
        {
            Weapon,
            Armor,
            Junk
        }
        public interface IItemAction
        {
            public abstract void Interact();
        }
    }
}
