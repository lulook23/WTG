﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ItemsDatabase : EditorWindow
{
	[MenuItem ("Window/ItemsDatabase")]
	private static void ShowWindow () {
		EditorWindow.GetWindow<ItemsDatabase> ();
	}

	public static string path {
		get {
			return Application.streamingAssetsPath + "/ItemsDatabase/ItemsData.cfg";
		}
	}

	public static bool Exist () {
		return File.Exists (path);
	}

	public static IItem[] items = new IItem[0];

	public static IItem[] LoadBasicItems () {
		return IItemAsset.items;
	}

	public static IItem[] LoadSaved () {
		IItem[] its = new IItem[0];
		if (Exist()) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);
			its = (IItem[])bf.Deserialize (file);
			file.Close ();
		}
		return its;
	}

	public static void Save () {
		Directory.CreateDirectory (Application.streamingAssetsPath + "/ItemsDatabase");
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (path);
		bf.Serialize (file, items);
		file.Close ();
		Debug.Log ("Saved at : " + path);
	}

	public static void AddItem () {
		IItem[] its = new IItem[items.Length + 1];
		for (int i = 0; i < items.Length; i++) {
			its [i] = items [i];
		}
		its [its.Length - 1] = new IItem (0, 0, "no name", IItemType.Armor);
		items = its;
	}
	public static void RemoveItem (int index) {
		List<IItem> its = new List<IItem> ();
		its.AddRange (items);
		its.RemoveAt (index);
		items = its.ToArray ();
	}

	public Vector2 scroll;

	private void OnGUI () {
		IItem[] basicItems = LoadBasicItems ();
		if (Exist ()) {
			if (GUILayout.Button ("LoadFromFile")) {
				items = LoadSaved ();
			}
		}
		using (EditorGUILayout.ScrollViewScope s = new EditorGUILayout.ScrollViewScope (scroll)) {
			scroll = s.scrollPosition;
			GUILayout.Label ("Basic items : ");
			for (int i = 0; i < basicItems.Length; i++) {
				GUILayout.Box ("" + '\n' + basicItems [i].name);
				GUILayout.Box (IItemAsset.LoadTexture(IItemAsset.GetIDByName(basicItems[i].name)));
				GUILayout.Label (basicItems [i].intro);
				GUILayout.Label ("Cost : " + basicItems [i].sellCount);
				GUILayout.Label ("ID : " + i);
				GUILayout.Label ("Type : " + basicItems [i].type);
				GUILayout.Label ("Value : " + basicItems [i].value);
			}
			GUILayout.Label ("" + '\n' + "Available to edit items : " + '\n');
			if (GUILayout.Button("Add item")) {
				AddItem ();
			}
			for (int i = 0; i < items.Length; i++) {
				items [i].name = EditorGUILayout.TextField ("Item name", items [i].name);
				items [i].intro = EditorGUILayout.TextArea (items [i].intro);
				items [i].sellCount = EditorGUILayout.IntField ("Item cost", items [i].sellCount);
				items [i].value = EditorGUILayout.IntField ("Item value", items [i].value);
				items [i].type = (IItemType)EditorGUILayout.EnumPopup (items [i].type);
				if (GUILayout.Button("Remove item")) {
					RemoveItem (i);
				}
			}
		}
		if (GUILayout.Button("Save items")) {
			Save ();
		}
	}
}