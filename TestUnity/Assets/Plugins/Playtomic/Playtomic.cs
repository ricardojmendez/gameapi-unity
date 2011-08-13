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

public class Playtomic : MonoBehaviour
{
	private long gameid;
	private string gameguid;
	private string sourceUrl;
	private Playtomic_Log log;
	private Playtomic_Data data;
	private Playtomic_Leaderboards leaderboards;
	private Playtomic_PlayerLevels playerlevels;
	private Playtomic_GeoIP geoip;
	private Playtomic_Link link;
	private Playtomic_GameVars gamevars;
	private Playtomic_Parse parse;
	
	private static Playtomic _instance = null;
	
	/// <summary>
	/// Initialises the API.  You must do this before anything else.  Get your credentials from the Playtomic dashboard.
	/// </summary>
	/// <param name="gameid">
	/// A <see cref="System.Int64"/>
	/// </param>
	/// <param name="gameguid">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="apikey">
	/// A <see cref="System.String"/>
	/// </param>
	public static void Initialise(long gameid, string gameguid, string apikey)
	{
		if(_instance != null)
			return;
			
		var go = new GameObject("playtomic");
		GameObject.DontDestroyOnLoad(go);
			
		_instance = go.AddComponent("Playtomic") as Playtomic;
		_instance.gameid = gameid;
		_instance.gameguid = gameguid;
		_instance.sourceUrl = string.IsNullOrEmpty(Application.absoluteURL) ? "http://localhost/" : Application.absoluteURL;
		_instance.log = new Playtomic_Log();
		_instance.data = new Playtomic_Data();
		_instance.leaderboards = new Playtomic_Leaderboards();
		_instance.playerlevels = new Playtomic_PlayerLevels();
		_instance.geoip = new Playtomic_GeoIP();
		_instance.link = new Playtomic_Link();
		_instance.gamevars = new Playtomic_GameVars();
		_instance.parse = new Playtomic_Parse();
		
		Playtomic_Request.Initialise();
		Playtomic_Data.Initialise(apikey);
		Playtomic_GameVars.Initialise(apikey);
		Playtomic_Leaderboards.Initialise(apikey);
		Playtomic_GeoIP.Initialise(apikey);
		Playtomic_PlayerLevels.Initialise(apikey);
		Playtomic_Parse.Initialise(apikey);
	}
	
	public static long GameId
	{
		get { return _instance.gameid; }
	}
	
	public static string GameGuid
	{
		get { return _instance.gameguid; }
	}
	
	public static string SourceUrl
	{
		get { return _instance.sourceUrl; }
	}
	
	public static Playtomic API
	{
		get { return _instance; }
	}
	
	public static Playtomic_Log Log
	{
		get { return _instance.log; }
	}
	
	public static Playtomic_Data Data
	{
		get { return _instance.data; }
	}
	
	public static Playtomic_Leaderboards Leaderboards
	{
		get  { return _instance.leaderboards; }
	}
	
	public static Playtomic_PlayerLevels PlayerLevels
	{
		get { return _instance.playerlevels; }
	}
	
	public static Playtomic_GeoIP GeoIP
	{
		get { return _instance.geoip; }
	}
	
	public static Playtomic_Link Link
	{
		get { return _instance.link; }
	}
	
	public static Playtomic_GameVars GameVars
	{
		get { return _instance.gamevars; }
	}
	
	public static Playtomic_Parse Parse
	{
		get { return _instance.parse; }
	}
}