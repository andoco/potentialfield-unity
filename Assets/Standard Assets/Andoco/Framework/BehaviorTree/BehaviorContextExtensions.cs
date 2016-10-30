namespace Andoco.Unity.Framework.BehaviorTree
{
	using System.Collections.Generic;
	using Andoco.BehaviorTree;
	using UnityEngine;
	
	public static class BehaviorContextExtensions
	{
		public static GameObject GetGameObject(this IBehaviorContext ctx, string key = null)
		{
			key = key ?? "gameObject";
			return ctx.GetItem<GameObject>(key);
		}

        public static T GetComponent<T>(this IBehaviorContext ctx) where T : Component
        {
            return ctx.GetGameObject().GetComponent<T>();
        }
		
		/// <summary>
		/// Gets the game objects identified by the supplied ctx key.
		/// </summary>
		/// <remarks>
		/// Automatically checks if the key is for a single GameObject, a collection of GameObjects,
		/// or if no key has been supplied. When no key is supplied, the current GameObject is used.
		/// </remarks>
		/// <returns>
		/// The game objects.
		/// </returns>
		/// <param name='ctx'>
		/// The data for the tree.
		/// </param>
		/// <param name='key'>
		/// Key.
		/// </param>
		public static IEnumerable<GameObject> GetGameObjects(this IBehaviorContext ctx, string key)
		{
			var targets = new List<GameObject>();
			
			if (string.IsNullOrEmpty(key))
			{
				targets.Add(ctx.GetGameObject());
			}
			else
			{
				var targetObj = ctx.GetItemOrDefault<object>(key);
				
				if (targetObj is IEnumerable<GameObject>)
				{
					targets.AddRange((IEnumerable<GameObject>)targetObj);
				}
				else if (targetObj is GameObject)
				{
					targets.Add((GameObject)targetObj);
				}
			}
			
			return targets;
		}
	}
}
