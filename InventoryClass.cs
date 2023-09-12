using System;
using Unity.Collections;
using UnityEngine;
using static InventoryClass;
// "init" keyword is broken atm with unity so made a work around in "Utilities.cs" to fix it
// This is Old Needs to be double checked that all relevant Info/Code is migrated to proper file
public static class InventoryClass // Version 1
{
    public static void SwapSlots(ref InventorySlot slot1, ref InventorySlot slot2)
    {
        InventorySlot temp = slot1;
        slot1 = slot2;
        slot2 = temp;
        return;
    }
    [Serializable]
    public struct InventorySlot
    {
        public InventoryItem Item
        {
            get { return Item; }
            private set
            {
                Item = value;
                SlotSize = value.StackSize;
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
            if ((amount -= addAmount) == 0)
                return true;
            else
                return false;
        }
        public bool AddAmount(ref InventorySlot slot)
        {
            int count = slot.ItemCount;
            var isSuccess = slot.Item.Name == Item.Name ? AddAmount(ref count) : Item.Name == null ? AddItem(slot.Item, ref count) : false;
            if (count != slot.ItemCount)  
                slot.SubtractAmount(ref count);
            return isSuccess;
        }
        public bool AddAmount(InventoryItem item, ref int amount)
        {
            return item.Name == Item.Name ? AddAmount(ref amount) : Item.Name == null ? AddItem(item, ref amount) : false;
        }
        private bool AddItem(InventoryItem item, ref int amount)
        {
            Item = item;
            return AddAmount(ref amount);
        }
        private bool SubtractAmount(ref int amount)
        {
            int SubtractAmount = (amount <= ItemCount) ? amount : ItemCount;
            ItemCount -= SubtractAmount;
            UpdateSlot();
            if ((amount -= SubtractAmount) == 0)
                return true;
            else
            return false;
        }
        public bool SubtractAmount(ref InventorySlot slot)
        {
            int count = slot.ItemCount;
            var isSuccess = slot.Item.Name == Item.Name ? SubtractAmount(ref count) : false;
            if (count != slot.ItemCount)
                slot.AddAmount(ref count);
            return isSuccess;
        }
        public bool SubtractAmount(InventoryItem item, ref int amount)
        {
            return item.Name == Item.Name ? SubtractAmount(ref amount) : false;
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
                Item = new InventoryItem();
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
        public void EmptySlot() => this = InventorySlot.Empty;
        public static InventorySlot Empty = new InventorySlot() { 
            Item = new InventoryItem(),
            SlotSize = 0,
            ItemCount = 0,
            ItemStackValue = 0,
        };
    }
    public abstract class ItemContext { } // This is for possible future functionality
    [Serializable]
    public struct Inventory  
    {
        private NativeArray<InventorySlot> inventorySlots;
        private int x, y;
        public Inventory(int x, int y)
        {
            inventorySlots = new NativeArray<InventorySlot>(x * y, Allocator.Persistent);
            this.x = x;
            this.y = y; 
        }
        // Dispose function
        // Enumeration to allow iteration through inventory
        // Accessor Methods
    }
    #region ItemTemplates
    [Serializable]
    public struct InventoryItem
    {
        public string Name { get; init; }
        public Sprite Icon { get; init; }
        public int StackSize { get; init; }
        public int Value { get; init; }
        public ItemTag Tag { get; }
        public IInteractable[] Interactions { get; set; }
        public IEquipable equipable { get; set; }
        
    }
    public interface IEquipable
    {
        public IEquipStats equipStats { get; set; }
        public abstract void Equip(); // Return Stats and Damage Modifers/Abilities for it to store, or some variation of this for different Items
    }
    public interface IInteractable 
    {
        public abstract void Interact();
    }
    public interface IEquipStats
    {

    }
    #endregion
    [Serializable]
    public enum ItemTag
    {
        Weapon,
        Armour,
        Junk
    }
}