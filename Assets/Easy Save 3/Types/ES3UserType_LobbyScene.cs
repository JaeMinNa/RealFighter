using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_LobbyScene : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_LobbyScene() : base(typeof(LobbyScene)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (LobbyScene)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (LobbyScene)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_LobbySceneArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_LobbySceneArray() : base(typeof(LobbyScene[]), ES3UserType_LobbyScene.Instance)
		{
			Instance = this;
		}
	}
}