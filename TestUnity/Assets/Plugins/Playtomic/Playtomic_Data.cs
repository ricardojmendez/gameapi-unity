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

public class Playtomic_Data : Playtomic_Responder
{
	public Playtomic_Data() { }
	
	// general metrics
	public IEnumerator Views()
	{
		return Views(0, 0, 0);
	}
	
	public IEnumerator Views(int month, int year)
	{
		return Views(0, month, year);
	}
	
	public IEnumerator Views(int day, int month, int year)
	{
		return General("Views", day, month, year);
	}
	
	public IEnumerator Plays()
	{
		return Plays(0, 0, 0);
	}
	
	public IEnumerator Plays(int month, int year)
	{
		return Plays(0, month, year);
	}
	
	public IEnumerator Plays(int day, int month, int year)
	{
		return General("Plays", day, month, year);
	}
	
	public IEnumerator PlayTime()
	{
		return PlayTime(0, 0, 0);
	}
	
	public IEnumerator PlayTime(int month, int year)
	{
		return PlayTime(0, month, year);
	}
	
	public IEnumerator PlayTime(int day, int month, int year)
	{
		return General("PlayTime", day, month, year);
	}
	
	private IEnumerator General(string type, int day, int month, int year)
	{
		return GetData(type, Playtomic.APIUrl + "/data/" + type + ".aspx?swfid=" + Playtomic.GameId + "&js=y&day=" + day + "&month=" + month + "&year=" + year);
	}
	
	// custom metrics
	public IEnumerator CustomMetric(string metric)
	{
		return CustomMetric(metric, 0, 0, 0);
	}
	
	public IEnumerator CustomMetric(string metric, int month, int year)
	{
		return CustomMetric(metric, 0, month, year);
	}
	
	public IEnumerator CustomMetric(string metric, int day, int month, int year)
	{
		return GetData("CustomMetric", Playtomic.APIUrl + "/data/custommetric.aspx?swfid=" + Playtomic.GameId + "&metric=" + metric + "&js=y&day=" + day + "&month=" + month + "&year=" + year);
	}
	
	// level metrics
	public IEnumerator LevelCounter(string metric, string level)
	{
		return LevelCounter(metric, level, 0, 0, 0);
	}
	
	public IEnumerator LevelCounter(string metric, int level)
	{
		return LevelCounter(metric, level.ToString(), 0, 0, 0);
	}
	
	public IEnumerator LevelCounter(string metric, string level, int month, int year)
	{
		return LevelCounter(metric, level, 0, month, year);
	}
	
	public IEnumerator LevelCounter(string metric, int level, int month, int year)
	{
		return LevelCounter(metric, level.ToString(), 0, month, year);
	}
	
	public IEnumerator LevelCounter(string metric, int level, int day, int month, int year)
	{
		return LevelCounter(metric, level.ToString(), day, month, year);
	}
	
	public IEnumerator LevelCounter(string metric, string level, int day, int month, int year)
	{
		return LevelMetric("Counter", metric, level, day, month, year);
	}
	
	public IEnumerator LevelAverage(string metric, string level)
	{
		return LevelAverage(metric, level, 0, 0, 0);
	}
	
	public IEnumerator LevelAverage(string metric, int level)
	{
		return LevelAverage(metric, level.ToString(), 0, 0, 0);
	}
	
	public IEnumerator LevelAverage(string metric, string level, int month, int year)
	{
		return LevelAverage(metric, level, 0, month, year);
	}
	
	public IEnumerator LevelAverage(string metric, int level, int month, int year)
	{
		return LevelAverage(metric, level.ToString(), 0, month, year);
	}
	
	public IEnumerator LevelAverage(string metric, int level, int day, int month, int year)
	{
		return LevelAverage(metric, level.ToString(), day, month, year);
	}
	
	public IEnumerator LevelAverage(string metric, string level, int day, int month, int year)
	{
		return LevelMetric("Average", metric, level, day, month, year);
	}
		
	public IEnumerator LevelRanged(string metric, string level)
	{
		return LevelRanged(metric, level, 0, 0, 0);
	}
	
	public IEnumerator LevelRanged(string metric, int level)
	{
		return LevelRanged(metric, level.ToString(), 0, 0, 0);
	}
	
	public IEnumerator LevelRanged(string metric, string level, int month, int year)
	{
		return LevelRanged(metric, level, 0, month, year);
	}
	
	public IEnumerator LevelRanged(string metric, int level, int month, int year)
	{
		return LevelRanged(metric, level.ToString(), 0, month, year);
	}
	
	public IEnumerator LevelRanged(string metric, int level, int day, int month, int year)
	{
		return LevelRanged(metric, level.ToString(), day, month, year);
	}
	
	public IEnumerator LevelRanged(string metric, string level, int day, int month, int year)
	{
		return LevelMetric("Ranged", metric, level, day, month, year);
	}
	
	private IEnumerator LevelMetric(string type, string metric, string level, int day, int month, int year)
	{
		return GetData("Level" + type, Playtomic.APIUrl + "/data/levelmetric" + type + ".aspx?swfid=" + Playtomic.GameId + "&metric=" + metric + "&level=" + level + "&js=y&day=" + day + "&month=" + month + "&year=" + year);
	}
	
	private IEnumerator GetData(string identifier, string url)
	{
		WWWForm postdata = new WWWForm();
		postdata.AddField("unity", 1);
		
		WWW www = new WWW(url, postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), identifier);
			yield break;
		}

		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), identifier);
			yield break;
		}
		
		var results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		
		if (response.Success)
		{
			var data = (Hashtable)(results["Data"]);
			
			foreach(string key in data.Keys)
				response.Data.Add(key, (string)data[key]);
		}
		
		SetResponse(response, identifier);
		yield break;
	}
}