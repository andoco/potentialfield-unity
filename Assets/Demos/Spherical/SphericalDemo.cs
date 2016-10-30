using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Meshes;
using Andoco.Unity.Framework.PotentialField;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Andoco.Unity.Framework.Core.ObjectPooling.Creator;

namespace Andoco.Unity.Framework.Demos
{
    public class SphericalDemo : MonoBehaviour
    {
        [Inject]
        private IGameObjectCreator goCreator;

        private GameObject icosphereGo;

        public int recursion;
        public Mesh fieldSourceMesh;
        public float radius;
        public Material material;
        public PotentialFieldSystem potentialFieldSystem;
        public GameObject potentialSourcePrefab;
        public GameObject botPrefab;

        public Text addBotsButtonLabel;
        public Slider numBotsSlider;
        public Toggle randomBotPosToggle;

        void Start()
        {
            this.CreateIcosphere();

            var mesh = this.icosphereGo.GetComponent<MeshFilter>().mesh;

            this.SetupSimpleGraphPotentialField(mesh);

            this.OnNumBotsSliderChanged();
        }

        private void CreateIcosphere()
        {
            if (this.fieldSourceMesh != null)
            {
                var verts = this.fieldSourceMesh.vertices;
                var mesh = new Mesh();
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i] *= this.radius;
                }
                mesh.vertices = verts;
                mesh.triangles = this.fieldSourceMesh.triangles;

                this.icosphereGo = new GameObject("Icosphere");
                this.icosphereGo.AddComponent<MeshFilter>().mesh = mesh;
                this.icosphereGo.AddComponent<MeshRenderer>().material = this.material;
                this.icosphereGo.AddComponent<MeshCollider>().sharedMesh = mesh;
            }
            else
            {
                var meshBuilder = new MeshBuilder();
                meshBuilder.BuildIcoSphere(this.recursion, this.radius);
                this.icosphereGo = meshBuilder.CreateGameObject(this.material);
                this.icosphereGo.name = "Icosphere";
            }
        }

        private void SetupSimpleGraphPotentialField(Mesh mesh)
        {
            var graph = mesh.BuildSimpleGraph();
            this.potentialFieldSystem.SetGraph(graph, mesh.vertices);
        }

        public void AddPositiveSource()
        {
            this.AddSource(1f);
        }

        public void AddNegativeSource()
        {
            this.AddSource(-1f);
        }

        private void AddSource(float potential)
        {
            var pos = UnityEngine.Random.onUnitSphere * this.radius;
            var go = (GameObject)GameObject.Instantiate(this.potentialSourcePrefab, pos, Quaternion.identity);
            go.GetComponent<TrackingPotentialFieldSource>().sourceConfigs[0].potential = potential;
        }

        private void AddBot(Vector3 pos)
        {
            var bot = this.goCreator.Create(this.botPrefab, pos, Quaternion.identity);
            var navigator = bot.GetComponent<PotentialFieldNavigator>();
            navigator.StartNavigating();
        }

        public void AddBots()
        {
            var num = (int)this.numBotsSlider.value;

            var ray = UnityEngine.Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            var rayPos = ray.GetHitPoint().Value;

            for (int i = 0; i < num; i++)
            {
                var pos = randomBotPosToggle.isOn
                    ? UnityEngine.Random.onUnitSphere * this.radius
                    : rayPos;

                this.AddBot(pos);
            }
        }

        public void RemoveSources()
        {
            var sources = GameObject.FindObjectsOfType<PotentialFieldSource>();

            for (int i = 0; i < sources.Length; i++)
            {
                GameObject.Destroy(sources[i].gameObject);
            }
        }

        public void OnNumBotsSliderChanged()
        {
            this.addBotsButtonLabel.text = string.Format("Add {0} Bots", this.numBotsSlider.value);
        }
    }
}