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

public class Playtomic_PlayerLevels : Playtomic_Responder
{	
	public Playtomic_PlayerLevels () { }
	
	public IEnumerator Save(Playtomic_PlayerLevel level)
	{
		WWWForm postdata = new WWWForm();
		postdata.AddField("data", level.Data);
		postdata.AddField("playerid", level.PlayerId);
		postdata.AddField("playername", level.PlayerName);
		postdata.AddField("playersource", Playtomic.SourceUrl);
		postdata.AddField("name", level.Name);
		postdata.AddField("nothumb", "y");
		postdata.AddField("customfields", level.CustomData.Count);
		
		var n = 0;
		
		foreach(var key in level.CustomData.Keys)
		{
			postdata.AddField("ckey" + n, key);
			postdata.AddField("cdata" + n, level.CustomData[key]);
			n++;
		}
		
		WWW www = new WWW(Playtomic.APIUrl + "/playerlevels/save.aspx?swfid=" + Playtomic.GameId + "&js=true", postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "Save");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "Save");
			yield break;
		}
		
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		
		if (response.Success)
		{
			Hashtable data = (Hashtable)(results["Data"]);
			response.Data.Add("LevelId", (string)data["LevelId"]);
			response.Data.Add("Name", (string)data["Name"]);
		}
		
		SetResponse(response, "Save");
		yield break;
	}
	
	public IEnumerator Load(string levelid)
	{
		WWWForm postdata = new WWWForm();
		postdata.AddField("unity", 1);
		
		WWW www = new WWW(Playtomic.APIUrl + "/playerlevels/load.aspx?swfid=" + Playtomic.GameId + "&js=y&levelid=" + levelid);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "Load");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "Load");
			yield break;
		}
		
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		response.Levels = new List<Playtomic_PlayerLevel>();
		response.NumItems = 0;
		
		if(response.Success)
		{
			var item = (Hashtable)results["Data"];
	
			var level = new Playtomic_PlayerLevel();
			level.LevelId = (string)item["LevelId"];
			level.PlayerSource = (string)item["PlayerSource"];
			level.PlayerName = WWW.UnEscapeURL((string)item["PlayerName"]);
			level.Name = WWW.UnEscapeURL((string)item["Name"]);
			level.Votes = (int)(double)item["Votes"];
			level.Plays = (int)(double)item["Plays"];
			level.Rating = (double)item["Rating"];
			level.Score = (int)(double)item["Score"];
			level.SDate = DateTime.Parse((string)item["SDate"]);
			level.RDate = WWW.UnEscapeURL((string)item["RDate"]);
			
			if(item.ContainsKey("Data"))
				level.Data = (string)item["Data"];
			
			if(item.ContainsKey("CustomData"))
			{
				Hashtable customdata = (Hashtable)item["CustomData"];
	
				foreach(var key in customdata.Keys)
					level.CustomData.Add((string)key, WWW.UnEscapeURL((string)customdata[key]));
			}
			
			response.Levels = new List<Playtomic_PlayerLevel>();
			response.Levels.Add(level);
		}
		
		SetResponse(response, "Load");
		
		yield break;
	}

	public IEnumerator List(string mode, int page, int perpage)
	{
		return List(mode, page, perpage, false, false, DateTime.MinValue, DateTime.MaxValue);
	}
	
	public IEnumerator List(string mode, int page, int perpage, bool includedata, bool includethumbs)
	{
		return List(mode, page, perpage, includedata, includethumbs, DateTime.MinValue, DateTime.MaxValue);
	}
		
	public IEnumerator List(string mode, int page, int perpage, bool includedata, bool includethumbs, DateTime datemin, DateTime datemax)
	{
		WWWForm postdata = new WWWForm();
		postdata.AddField("unity", 1);
		
		WWW www = new WWW(Playtomic.APIUrl + "/playerlevels/list.aspx?swfid=" + Playtomic.GameId + "&js=y&mode=" + mode + "&page=" + page + "&perpage=" + perpage + 
		                  "&data=" + (includedata ? "y" : "n") + "&thumbs=" + (includethumbs ? "y" : "n") + 
		                  "&datemin=" + (datemin != DateTime.MinValue ? datemin.ToString("MM/dd/yyyy") : "") + "&datemax=" + (datemax != DateTime.MaxValue ? datemax.ToString("MM/dd/yyyy") : ""));
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "List");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "List");
			yield break;
		}
		
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		response.Levels = new List<Playtomic_PlayerLevel>();
		response.NumItems = 0;
		
		if(response.Success)
		{
			var data = (Hashtable)results["Data"];
			var levels = (ArrayList)data["Levels"];
			var len = levels.Count;
			
			response.NumItems = (int)(double)data["NumLevels"];
			
			for(var i=0; i<len; i++)
			{
				Hashtable item = (Hashtable)levels[i];	
				
				var level = new Playtomic_PlayerLevel();
				level.LevelId = (string)item["LevelId"];
				level.PlayerSource = (string)item["PlayerSource"];
				level.PlayerName = WWW.UnEscapeURL((string)item["PlayerName"]);
				level.Name = WWW.UnEscapeURL((string)item["Name"]);
				level.Votes = (int)(double)item["Votes"];
				level.Plays = (int)(double)item["Plays"];
				level.Rating = (double)item["Rating"];
				level.Score = (int)(double)item["Score"];
				level.SDate = DateTime.Parse((string)item["SDate"]);
				level.RDate = WWW.UnEscapeURL((string)item["RDate"]);
				
				if(item.ContainsKey("Data"))
					level.Data = (string)item["Data"];
				
				if(item.ContainsKey("CustomData"))
				{
					Hashtable customdata = (Hashtable)item["CustomData"];
	
					foreach(var key in customdata.Keys)
						level.CustomData.Add((string)key, WWW.UnEscapeURL((string)customdata[key]));
				}
				
				response.Levels.Add(level);
			}
		}
		
		SetResponse(response, "List");
		
		yield break;
	}
	
	public IEnumerator Rate(string levelid, int rating)
	{
		if(rating < 1 || rating > 10)
		{
			SetResponse(Playtomic_Response.Error(401), "Rate");
			yield break;
		}
		
		WWWForm postdata = new WWWForm();
		postdata.AddField("unity", 1);
		
		WWW www = new WWW(Playtomic.APIUrl + "/playerlevels/rate.aspx?swfid=" + Playtomic.GameId + "&js=true&levelid=" + levelid + "&rating=" + rating, postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "Rate");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "Rate");
			yield break;
		}
		
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		SetResponse(response, "Rate");
		yield break;
	}
	
	public void LogStart(string levelid)
	{
		Playtomic.Log.PlayerLevelStart(levelid);
	}
	
	public void LogQuit(string levelid)
	{
		Playtomic.Log.PlayerLevelQuit(levelid);
	}
	
	public void LogWin(string levelid)
	{
		Playtomic.Log.PlayerLevelWin(levelid);
	}
	
	public void LogRetry(string levelid)
	{
		Playtomic.Log.PlayerLevelRetry(levelid);
	}
	
	public void Flag(string levelid)
	{
		Playtomic.Log.PlayerLevelFlag(levelid);
	}
}