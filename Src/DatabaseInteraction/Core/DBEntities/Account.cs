using System;

namespace BMBank.Src.DatabaseInteraction.Core.DBEntities
{
    public class Account : IDBEntity
    {
        public int ID { get; }
        public int AccountNumber { get; }
        public long Balance { get; }
        public bool IsActive { get; }
        public DateTime CreatedAtDate { get; }
        public DateTime? ClosedAtDate { get; }
    }
}