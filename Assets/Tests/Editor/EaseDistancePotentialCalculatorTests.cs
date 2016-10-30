namespace Andoco.Unity.Framework.Tests.UnitTests.PotentialField
{
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.PotentialField;
    using NUnit.Framework;
    using UnityEngine;

    [TestFixture]
    public class EaseDistancePotentialCalculatorTests
    {
        [Test]
        [TestCase(0f, 0f, 0f, Interpolate.EaseType.Linear, 0f)]
        [TestCase(0f, 0f, 1f, Interpolate.EaseType.Linear, 1f)]

        [TestCase(10f, 10f, 1f, Interpolate.EaseType.Linear, 0f)]
        [TestCase(5f, 10f, 1f, Interpolate.EaseType.Linear, 0.5f)]
        [TestCase(0f, 10f, 1f, Interpolate.EaseType.Linear, 1f)]
        [TestCase(10f, 10f, -1f, Interpolate.EaseType.Linear, 0f)]
        [TestCase(5f, 10f, -1f, Interpolate.EaseType.Linear, -0.5f)]
        [TestCase(0f, 10f, -1f, Interpolate.EaseType.Linear, -1f)]
        public void GetPotential_ShouldCalculateCorrectPotential(
            float distance,
            float maxDistance,
            float sourcePotential, 
            Interpolate.EaseType easeType,
            float expectedPotential)
        {
            var go = this.CreateGameObject();
            var calculator = go.AddComponent<EaseDistancePotentialCalculator>();
            calculator.easeType = easeType;
            calculator.maxDistance = maxDistance;

            var sourcePos = Vector3.zero;
            var samplePos = Vector3.forward * distance;

            var potential = calculator.GetPotential(samplePos, sourcePos, sourcePotential);

            GameObject.DestroyImmediate(go);

            Assert.That(potential, Is.EqualTo(expectedPotential));
        }

        private GameObject CreateGameObject(string name = null)
        {
            return new GameObject(name);
        }
    }
}
