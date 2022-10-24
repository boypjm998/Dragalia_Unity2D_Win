using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Trail2D))]
[ExecuteInEditMode]
public class Trail2DEditor : MonoBehaviour
{
	public bool preview = false;
	bool previousPreview = false;
	Trail2D trail2D;
	#if UNITY_EDITOR
		void Start(){
			if(EditorApplication.isPlaying){
				DestroyImmediate(this);
			}
		}
		void OnEnable()
		{
			trail2D = GetComponent<Trail2D>();
			trail2D.reset = true;
			EditorApplication.update += EditorUpdate;
		}
		void OnDisable()
		{
			EditorApplication.update -= EditorUpdate;
			trail2D = null;
		}

		void EditorUpdate()
		{
			if(preview){
				if(previousPreview == false){
					trail2D.reset = true;
					previousPreview = preview;
				}
				trail2D.MeshTrail();
			} else {
				previousPreview = false;
			}
		}
	#else
	
		void Start(){
			DestroyImmediate(this);
		}
	
	#endif
}
