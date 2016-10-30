using System.Collections.Generic;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.ObjectPooling.Creator;
using Andoco.Unity.Framework.Data;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.Level.Schemas
{

    public class LevelSchemaSystem : MonoBehaviour, ILevelSchemaSystem
    {
        LevelSchemaData data;

        ILevelSchemaBuilder[] builders;

        [Inject]
        IGameData gameData;

        [Inject]
        IGameObjectCreator goCreator;

        public string dataKey = "levelSchemas";
        public SchemaPrefabConfig[] schemaPrefabs;
        public bool reseedOnBuild = true;

        public ILevelSchemaBuilder[] Builders { get { return GetAllBuilders(); } }

        public IList<ILevelSchema> Schemas { get { return data.Schemas; } }

        public ILevelSchema CurrentSchema { get { return data.Schemas[data.CurrentSchema]; } }

        [Inject]
        void OnPostInject()
        {
            data = gameData.GetOrAdd<LevelSchemaData>(dataKey);
            //playerData = data.Get<PlayerData>(Constants.DataKeys.PlayerData);
        }

        public void BuildRandomSchema()
        {
            var candidates = GetAllBuilders();
            var builder = candidates[Random.Range(0, candidates.Length)];
            var schema = builder.BuildSchema(0f);

            data.Schemas.Add(schema);
        }

        public void AddSchema(ILevelSchema schema)
        {
            data.Schemas.Add(schema);
        }

        public void SelectSchema(ILevelSchema schema)
        {
            this.data.CurrentSchema = this.data.Schemas.IndexOf(schema);
        }

        public void BuildLevel()
        {
            // Set a new seed at the start of every level as the previous build actions may set it to constant value.
            if (reseedOnBuild)
            {
                RandomHelper.Reseed();
            }

            var schema = this.data.Schemas[this.data.CurrentSchema];

            var builder = CreateBuilder(schema.BuilderIndex, GetValidSchemaPrefabs());
            builder.BuildLevel(schema);
        }

        GameObject[] GetValidSchemaPrefabs()
        {
            var results = new List<GameObject>();

            foreach (var conf in schemaPrefabs)
            {
                if (conf != null &&
                    conf.prefab != null &&
                    conf.enabled)
                {
                    results.Add(conf.prefab);
                }
            }

            return results.ToArray();
        }

        ILevelSchemaBuilder[] GetAllBuilders()
        {
            if (builders == null)
            {
                var prefabs = GetValidSchemaPrefabs();

                builders = new ILevelSchemaBuilder[prefabs.Length];

                for (int i = 0; i < prefabs.Length; i++)
                {
                    var builder = CreateBuilder(i, prefabs);
                    builders[i] = builder;
                }
            }

            return builders;
        }

        ILevelSchemaBuilder CreateBuilder(int builderIndex, GameObject[] prefabs)
        {
            var prefab = prefabs[builderIndex];
            var go = goCreator.Create(prefab, Vector3.zero, Quaternion.identity);
            go.transform.parent = transform;

            return go.GetComponent<ILevelSchemaBuilder>();
        }

        #region Types

        [System.Serializable]
        class LevelSchemaData
        {
            public LevelSchemaData()
            {
                Schemas = new List<ILevelSchema>();
            }

            public IList<ILevelSchema> Schemas { get; private set; }

            public int CurrentSchema { get; set; }
        }

        [System.Serializable]
        public class SchemaPrefabConfig
        {
            public GameObject prefab;
            public bool enabled;
        }

        #endregion
    }
}
