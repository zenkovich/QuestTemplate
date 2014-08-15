using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class PlayAnimQuestAction : IQuestAction {

	public Animation animation;
	public AnimationClip clip;

	public PlayAnimQuestAction(): base() {
		caption = "Play animation";
	}

	public override void Action() {
		if (animation.GetClip(clip.name) == null)
			animation.AddClip(clip, clip.name);

		animation.Play(clip.name);

		Debug.Log ("Play animation " + clip.name + " on " + animation.gameObject);
	}
}
