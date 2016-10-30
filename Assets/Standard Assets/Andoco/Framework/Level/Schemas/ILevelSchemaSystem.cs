using System.Collections.Generic;

namespace Andoco.Unity.Framework.Level.Schemas
{
    public interface ILevelSchemaSystem
    {
        ILevelSchemaBuilder[] Builders { get; }

        IList<ILevelSchema> Schemas { get; }

        ILevelSchema CurrentSchema { get; }

        void AddSchema(ILevelSchema schema);

        void BuildRandomSchema();

        void SelectSchema(ILevelSchema schema);

        /// <summary>
        /// Builds the level based on the currently selected level schema.
        /// </summary>
        void BuildLevel();
    }
}
