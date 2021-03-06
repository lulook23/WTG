﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IUsable : MonoBehaviour {

	public Transform trans;

	public static List<IUsable> usablesAll = new List<IUsable> ();

	private void Start () {
		trans = transform;
		usablesAll.Add (this);
	}

	public static string GetUseText (IUsable toUse)
	{
		string t = "Взаимодействовать : " + toUse.GetType ().Name;
		if (toUse is IDoor) {
			t = "Дверь";
		}
		if (toUse is IChest) {
			t = "Сундук";
		}
		if (toUse is ILocationTransition) {
			t = "Покинуть локацию";
		}
		if (toUse is IItemObject) {
			t = IItemAsset.items [((IItemObject)toUse).indentification].name;
		}

		return t;
	}

	public static IUsable GetNearestToPosition (Vector3 point) {
		IUsable[] usables = IUsable.usablesAll.ToArray ();
		IUsable usable = null;

		float dist = 2;

		usables = usables.Where ((IUsable arg) => ((arg.position - point).magnitude) < dist &&
		!Physics.Linecast (point + Vector3.up, arg.position + Vector3.up, LayerMask.GetMask ("Default")))
			.OrderBy ((IUsable arg) => ((arg.position - point).magnitude)).ToArray ();

		if (usables.Length > 0) {
			usable = usables [0];
		}
		return usable;
	}
	public static Vector3 GetPosition (IUsable us) {
		BoxCollider coll = us.GetComponent<BoxCollider> ();
		Vector3 pos = us.trans.position;
		if (coll) {
			pos = coll.bounds.center;
		}
		return pos;
	}
	public Vector3 position
	{
		get
		{
			return GetPosition (this);
		}
	}
	public static IUsable GetNearestToPositionAndWithDirection (Vector3 point, Vector3 direction) {
		IUsable[] usables = IUsable.usablesAll.ToArray ();
		IUsable usable = null;

		float dist = 2;
		float angle = 60;

		Vector3 pos = point + IControl.headHeight;

		foreach (var item in usables) {
			if (item is IDoor) {
				Debug.DrawLine (pos, item.position, Color.blue);
			}
			if (item is IChest) {
				Debug.DrawLine (pos, item.position, Color.red);
			}
			if (item is IItemObject) {
				Debug.DrawLine (pos, item.position, Color.green);
			}
			if (item is ILocationTransition) {
				Debug.DrawLine (pos, item.position, Color.magenta);
			}
		}

		usables = usables.Where ((IUsable arg) => ((arg.position - pos).magnitude) < dist &&
		Vector3.Angle ((arg.position - pos), direction) < angle
		&&
		!Physics.Linecast (pos, arg.position, LayerMask.GetMask ("Default")))
			.OrderBy ((IUsable arg) => ((pos - point).magnitude)).OrderBy (
			(IUsable arg) => Vector3.Angle ((arg.position - pos), direction)).ToArray ();
		if (usables.Length > 0) {
			usable = usables [0];
		}

		return usable;
	}
}