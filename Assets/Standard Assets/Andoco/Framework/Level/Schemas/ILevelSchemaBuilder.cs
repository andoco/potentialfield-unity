namespace Andoco.Unity.Framework.Level.Schemas
{
    public interface ILevelSchemaBuilder
    {
        ILevelSchema BuildSchema(float rating);

        void BuildLevel(ILevelSchema schema);
    }
}
