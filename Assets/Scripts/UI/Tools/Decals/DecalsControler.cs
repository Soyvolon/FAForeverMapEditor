﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OzoneDecals;

public class DecalsControler : MonoBehaviour {

	public static DecalsControler Current;

	public List<OzoneDecal> AllDecals;

	private void Awake()
	{
		Current = this;
	}

	public static List<Decal> GetAllDecals()
	{
		int Count = Current.AllDecals.Count;
		List<Decal> ToReturn = new List<Decal>(Count);

		for (int i = 0; i < Count; i++)
		{
			Current.AllDecals[i].Bake();
			ToReturn.Add(Current.AllDecals[i].Component);
		}

		return ToReturn;
	}


	public static void AddDecal(OzoneDecal dc)
	{
		if (!Current.AllDecals.Contains(dc))
		{
			Current.AllDecals.Add(dc);
			OzoneDecalRenderer.AddAlbedoDecal(dc);
		}
	}

	public static void RemoveDecal(OzoneDecal dc)
	{
		if (Current.AllDecals.Contains(dc))
		{
			Current.AllDecals.Remove(dc);
			OzoneDecalRenderer.RemoveAlbedoDecal(dc);
		}
	}

	private void Update()
	{
		int count = AllDecals.Count;

		for(int i = 0; i < count; i++)
		{
			AllDecals[i].LastDistance = OzoneDecalRenderer.DecalDist(AllDecals[i].tr);
		}
	}


	public void UnloadDecals()
	{
		if (UpdateProcess != null)
			StopCoroutine(UpdateProcess);

		for (int d = 0; d < AllDecals.Count; d++)
		{
			if (AllDecals[d] != null)
				Destroy(AllDecals[d].gameObject);
		}

		//AllDecals = new List<OzoneDecal>();
	}

	public static bool IsUpdating
	{
		get
		{
			return Updating;
		}
	}

	static bool Updating = false;
	static bool BufforUpdate = false;
	static Coroutine UpdateProcess;
	public static void UpdateDecals()
	{
		if (!Updating)
		{
			Updating = true;
			UpdateProcess = Current.StartCoroutine(Current.UpdatingDecals());
		}
		else
			BufforUpdate = true;
	}

	IEnumerator UpdatingDecals()
	{
		const int BreakEvery = 50;
		int UpdateCount = 0;

		for (int d = 0; d < AllDecals.Count; d++)
		{
			Vector3 Pos = AllDecals[d].GetPivotPoint();
			Pos.y = ScmapEditor.Current.Teren.SampleHeight(Pos);
			AllDecals[d].MovePivotPoint(Pos);

			//AllDecals[d].tr.position = Pos;// + (AllDecals[d].tr.forward * AllDecals[d].tr.localScale.z * 0.5f) - (AllDecals[d].tr.right * AllDecals[d].tr.localScale.x * 0.5f);


			UpdateCount++;
			if (UpdateCount > BreakEvery)
			{
				UpdateCount = 0;
				yield return null;
			}
		}


		yield return null;
		UpdateProcess = null;
		Updating = false;
		if (BufforUpdate)
		{
			BufforUpdate = false;
			UpdateDecals();
		}
	}

}
