// ========================================================
// des：
// author: 
// time：2020-12-29 15:56:42
// version：1.0
// ========================================================

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Game
{
    [Serializable]
    public class SkillShot : PlayableAsset
	{
		public SkillShotPlayable template = new SkillShotPlayable();
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
			var playable = ScriptPlayable<SkillShotPlayable>.Create(graph, template);
			playable.GetBehaviour().graph = graph;

			return playable;
        }
	}
}
