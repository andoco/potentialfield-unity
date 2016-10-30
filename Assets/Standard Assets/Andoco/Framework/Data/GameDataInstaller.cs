using Zenject;
using UnityEngine;

namespace Andoco.Unity.Framework.Data
{
    public class GameDataInstaller : Installer
    {
        public override void InstallBindings()
        {
            var existingGameData = GameObject.FindObjectOfType<GameData>();

            if (existingGameData == null)
            {
                var go = new GameObject("GameData");
                existingGameData = go.AddComponent<GameData>();
            }

            existingGameData.transform.parent = null;

            Container.BindAllInterfacesAndSelf<GameData>().FromInstance(existingGameData);
        }
    }
}
