namespace BMBank.Src.DatabaseInteraction.Core.DBEntities
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="IDBEntity"]/*'/>
    public interface IDBEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        /// <remarks>
        /// The ID property should be a non-negative integer that
        /// uniquely identifies the entity within the database.
        /// If it is set to zero, it indicates that the entity has not yet
        /// been added to the database and that it will be assigned a valid ID upon insertion.
        /// </remarks>>
        public int ID { get; }
    }

}
