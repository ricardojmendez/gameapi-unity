//  This file is part of the official Playtomic API for Unity games.  
//  Playtomic is a real time analytics platform for casual games 
//  and services that go in casual games.  If you haven't used it 
//  before check it out:
//  http://playtomic.com/
//
//  Created by ben at the above domain on 2/25/11.
//  Copyright 2011 Playtomic LLC. All rights reserved.
//
//  Documentation is available at:
//  http://playtomic.com/api/unity
//
// PLEASE NOTE:
// You may modify this SDK if you wish but be kind to our servers.  Be
// careful about modifying the analytics stuff as it may give you 
// borked reports.
//
// If you make any awesome improvements feel free to let us know!
//
// -------------------------------------------------------------------------
// THIS SOFTWARE IS PROVIDED BY PLAYTOMIC, LLC "AS IS" AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class Playtomic_PlayerScore
{
	public string Name;
	public string FBUserId;
	public double Points;
	public string Website;
	public DateTime SDate;
	public string RDate;
	public Dictionary<string, string> CustomData = new Dictionary<string, string>();

	public Playtomic_Result Result;
	
	public Playtomic_PlayerScore() 
	{ 
		SDate = new DateTime();
		RDate = "Just now";
	}
	
	// loading
	public IEnumerator Load(string levelid)
	{
		WWWForm postdata = new WWWForm(); // need a WWWForm to force WWW to send as POST
		postdata.AddField("unity", "1");
		
		WWW www = new WWW("http://g" + Playtomic.GameGuid + ".api.playtomic.com/playerlevels/load.aspx?swfid=" + Playtomic.GameId + "&levelid=" + WWW.EscapeURL(levelid) + "&js=true", postdata);
		yield return www;
		
		if (www.error != null)
		{
			Result = new Playtomic_Result(false, -1, www.error);
			yield break;
		}
		
		//Debug.Log("LOADED! Results are: " + www.text);
		
		if (www.text == "")
		{
			Result = new Playtomic_Result(false, 1, "Playtomic returned null");
			yield break;
		}
		
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		int status = (int)(double)results["Status"];
		int errorCode = (int)(double)results["ErrorCode"];
		
		if (status != 1)
		{
			Result = new Playtomic_Result(false, errorCode);
			yield break;
		}
		
		Hashtable data = (Hashtable)results["Data"];
		LoadFromHashtable(data);
		yield break;
	}		
	
	
	public void LoadFromHashtable(Hashtable data)
	{
		Name = WWW.UnEscapeURL((string)data["Name"]);
		FBUserId = null;
		if (data.ContainsKey("FBUserId"))
			FBUserId = WWW.UnEscapeURL((string)data["FBUserId"]);
		Points = (int)(double)data["Points"];
		Website = WWW.UnEscapeURL((string)data["Website"]);
		
		IFormatProvider culture = new CultureInfo("en-US", true);
		SDate = DateTime.Parse(WWW.UnEscapeURL((string)data["SDate"]), culture);
		RDate = WWW.UnEscapeURL((string)data["RDate"]);
			
		CustomData = new Dictionary<string, string>();
		Hashtable custom = (Hashtable)data["CustomData"];
		foreach (string key in custom.Keys)
		{
			CustomData[key] = WWW.UnEscapeURL((string)custom[key]);
			//Debug.Log("Added " + key + " = " + CustomData[key]);
		}
		
		//Debug.Log(LevelId + ", " + PlayerName + ", " + PlayerId + ", " + Name + ", " + Score + ", " + Votes + ", " + Plays + ", " + Rating + ", " + Data + ", " + Thumbnail);
	}

	
	// saving
	public IEnumerator Save()
	{
		yield break;
		/*
		WWWForm postdata = new WWWForm();
		postdata.AddField("data", Data);
		postdata.AddField("playerid", PlayerId);
		postdata.AddField("playersource", PlayerSource);
		postdata.AddField("playername", PlayerName);
		postdata.AddField("name", Name);
		postdata.AddField("nothumb", "true");
		postdata.AddField("unity", "1");
		
		int c = 0;
		foreach (KeyValuePair<string, string> kvp in CustomData)
		{
			postdata.AddField("ckey" + c, kvp.Key);
			postdata.AddField("cdata" + c, kvp.Value);
			c++;
		}
		
		postdata.AddField("customfields", c);
		
		WWW www = new WWW("http://g" + Playtomic.GameGuid + ".api.playtomic.com/playerlevels/save.aspx?swfid=" + Playtomic.GameId + "&js=true", postdata);

		yield return www;
		
		if (www.error != null)
		{
			Result = new Playtomic_Result(false, -1, www.error);
			yield break;
		}
		
		//Debug.Log("SAVED! Results are: " + www.text);
		
		if (www.text == "")
		{
			Result = new Playtomic_Result(false, 1, "Playtomic returned null");
			yield break;
		}
		
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		int status = (int)(double)results["Status"];
		int errorCode = (int)(double)results["ErrorCode"];
		
		Hashtable data = (Hashtable)results["Data"];
		LevelId = (string)data["LevelId"];
		
		Result = new Playtomic_Result((status == 1), errorCode);
		*/
	}
	
	
	public IEnumerator Rate(int rating)
	{
		yield break;
		/*
		if (PlayerPrefs.HasKey("playtomic-ratings-" + LevelId))
		{
			Result = new Playtomic_Result(false, 402);
			yield break;
		}
		
		if(rating < 0 || rating > 10)
		{
			Result = new Playtomic_Result(false, 401);
			yield break;
		}
		
		WWWForm postdata = new WWWForm(); // need a WWWForm to force WWW to send as POST
		postdata.AddField("unity", "1");
		
		WWW www = new WWW("http://g" + Playtomic.GameGuid + ".api.playtomic.com/playerlevels/rate.aspx?swfid=" + Playtomic.GameId + "&js=true&levelid=" + LevelId, postdata);

		yield return www;
		
		//Debug.Log("RATED! Results are: " + www.text);
		
		if (www.text == "")
		{
			Result = new Playtomic_Result(false, 1, "Playtomic returned null");
			yield break;
		}
		
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		int status = (int)(double)results["Status"];
		int errorCode = (int)(double)results["ErrorCode"];
		
		if (status == 1)
		{
			PlayerPrefs.SetInt("playtomic-ratings-" + LevelId, 1);
		}
		
		Result = new Playtomic_Result((status == 1), errorCode);
		*/
	}
	
}
