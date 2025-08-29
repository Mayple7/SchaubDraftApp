using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2015-2018 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Editor
{
	/// <summary>
	/// Editor for the PlaylistMediaPlayer component
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(PlaylistMediaPlayer))]
	public class PlaylistMediaPlayerEditor : UnityEditor.Editor
	{
		private SerializedProperty _propPlayerA;
		private SerializedProperty _propPlayerB;
		private SerializedProperty _propNextTransition;
		private SerializedProperty _propPlaylist;
		private SerializedProperty _propPlaylistLoopMode;
		private SerializedProperty _propPausePreviousOnTransition;
		private SerializedProperty _propTransitionDuration;
		private SerializedProperty _propTransitionEasing;

		private void OnEnable()
		{
			_propPlayerA = serializedObject.FindProperty("_playerA");
			_propPlayerB = serializedObject.FindProperty("_playerB");
			_propNextTransition = serializedObject.FindProperty("_nextTransition");
			_propTransitionDuration = serializedObject.FindProperty("_transitionDuration");
			_propTransitionEasing = serializedObject.FindProperty("_transitionEasing.preset");
			_propPausePreviousOnTransition = serializedObject.FindProperty("_pausePreviousOnTransition");
			_propPlaylist = serializedObject.FindProperty("_playlist._items");
			_propPlaylistLoopMode = serializedObject.FindProperty("_playlistLoopMode");
		}

		public override bool RequiresConstantRepaint()
		{
			PlaylistMediaPlayer media = (this.target) as PlaylistMediaPlayer;
			return (media.Control != null && media.isActiveAndEnabled);
		}

		public override void OnInspectorGUI()
		{
			PlaylistMediaPlayer media = (this.target) as PlaylistMediaPlayer;

			serializedObject.Update();

			if (media == null || _propPlayerA == null)
			{
				return;
			}

			EditorGUILayout.PropertyField(_propPlayerA);
			EditorGUILayout.PropertyField(_propPlayerB);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUILayout.Label("Playlist", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_propPlaylistLoopMode, new GUIContent("Loop Mode"));
			EditorGUILayout.PropertyField(_propPlaylist, new GUIContent("Items"), true);
			EditorGUILayout.Space(); 
			EditorGUILayout.Space();
			GUILayout.Label("Transition", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_propNextTransition, new GUIContent("Next"));
			EditorGUILayout.PropertyField(_propTransitionEasing, new GUIContent("Easing"));
			EditorGUILayout.PropertyField(_propTransitionDuration, new GUIContent("Duration"));
			EditorGUILayout.PropertyField(_propPausePreviousOnTransition, new GUIContent("Pause Previous"));
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (Application.isPlaying)
			{
				IMediaProducer textureSource = media.TextureProducer;

				Texture texture = null;
				if (textureSource != null)
				{
					texture = textureSource.GetTexture();
				}
				if (texture == null)
				{
					texture = EditorGUIUtility.whiteTexture;
				}

				float ratio = (float)texture.width / (float)texture.height;

				// Reserve rectangle for texture
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				Rect textureRect;
				Rect alphaRect = new Rect(0f, 0f, 1f, 1f);
				if (texture != EditorGUIUtility.whiteTexture)
				{
					textureRect = GUILayoutUtility.GetRect(Screen.width / 2, Screen.width / 2, (Screen.width / 2) / ratio, (Screen.width / 2) / ratio);
				}
				else
				{
					textureRect = GUILayoutUtility.GetRect(1920f / 40f, 1080f / 40f);
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				string rateText = "0";
				string playerText = string.Empty;
				if (media.Info != null)
				{
					rateText = media.Info.GetVideoDisplayRate().ToString("F2");
					playerText = media.Info.GetPlayerDescription();
				}

				EditorGUILayout.LabelField("Display Rate", rateText);
				EditorGUILayout.LabelField("Using", playerText);
								
				// Draw the texture
				Matrix4x4 prevMatrix = GUI.matrix;
				if (textureSource != null && textureSource.RequiresVerticalFlip())
				{
					GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, textureRect.y + (textureRect.height / 2)));
				}

				if (!GUI.enabled)
				{
					GUI.color = Color.grey;
					GUI.DrawTexture(textureRect, texture, ScaleMode.ScaleToFit, false);
					GUI.color = Color.white;
				}
				else
				{
					{
						GUI.DrawTexture(textureRect, texture, ScaleMode.ScaleToFit, false);
						EditorGUI.DrawTextureAlpha(alphaRect, texture, ScaleMode.ScaleToFit);
					}
				}
				GUI.matrix = prevMatrix;
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUI.BeginDisabledGroup(!Application.isPlaying);

			GUILayout.Label("Current Item: " + media.PlaylistIndex + " / " + Mathf.Max(0, media.Playlist.Items.Count - 1) );

			GUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(!media.CanJumpToItem(media.PlaylistIndex - 1));
			if (GUILayout.Button("Prev"))
			{
				media.PrevItem();
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(!media.CanJumpToItem(media.PlaylistIndex + 1));
			if (GUILayout.Button("Next"))
			{
				media.NextItem();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}
	}
}