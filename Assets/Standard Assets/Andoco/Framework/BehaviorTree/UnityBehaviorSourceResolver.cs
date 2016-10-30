namespace Andoco.Unity.Framework.BehaviorTree
{
    using System;
    using System.IO;
    using Andoco.BehaviorTree.Reader.Source;
    using UnityEngine;

	public class UnityBehaviorSourceResolver : IBehaviorSourceResolver {
		
		public Stream GetStream(string name)
		{
			//Debug.Log(string.Format("Getting stream for {0}", name));
			var behaviorResource = LoadBehavior(name);
			
			return new MemoryStream(behaviorResource);
		}
		
		private static byte[] LoadBehavior(string behaviorFile)
		{
			var resourceName = System.IO.Path.GetFileNameWithoutExtension(behaviorFile);
			//Debug.Log(string.Format("Loading behavior resource {0}", resourceName));
			
			var text = Resources.Load(resourceName) as TextAsset;
			
			if (text == null)
				throw new InvalidOperationException(string.Format("Could not load behavior resource {0}", resourceName));
			
			var bytes = System.Text.Encoding.UTF8.GetBytes(text.text);
			
			return bytes;
		}
	}
}