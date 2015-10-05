/*
	Custom Markup Language (CML) V2.0 Copyright @ myBad Studios 2012
	http://www.mybadstudios.com

	CML was created to provide a simple format for storing, retrieving and navigating
	sets of data. Data sets can be classified by type and datablocks of
	such sets can be placed in files in any order. You are not limited to
	a single dataset in a single file and are free to create as many as you need.
	
	CML allows for two different methods of adding data to a data block.
	
	1. The shorthand form:
	<dataset_name>[field_name[=value][;]...]

	Example:
	<book>name=Romeo and Juliet;author=Shakespear;price=5.99;owner=;A note or two
	
	2. Normal form:
	<dataset_name>
	[field_name][value]
	[...]
	
	Example:
	<book>
	[name]Romeo and Juliet
	[author]	Shakespear
	[price] 5.99
	[owner]
	A note or two
*/

/* ------------------------------------------------------------------------
	CML Function List
	-----------------------
	virtual public void Initialize() 
	virtual public cmlData First
	virtual public cmlData Last 
	virtual public bool LoadFile(string filename) 
	virtual public bool OpenFile(string FileName)
	virtual public bool ParseFile(string data) 
	virtual public bool Load(string PrefName)
	virtual public bool Save(string PrefName)
	virtual public bool SaveFile(string FileName)

	virtual public string ToString(bool include_id = true)
	
	virtual public void ImbedCMLData() 
	virtual public bool Join(CML other)

	virtual public bool AddNode(string data_type, string add_data = "") 
	virtual public bool InsertNode(string data_type, string add_data = "") 
	virtual public bool InsertNode(int index, string data_type, string add_data="") 
	virtual public bool RemoveNode(int index) 
	virtual public bool RemoveCurrentNode() 

	virtual public bool PositionAtNode(int index) 
	virtual public bool PositionAtFirstNode() 
	virtual public bool PositionAtLastNode() 
	virtual public bool PositionAtNextNode()
	virtual public bool PositionAtPreviousNode() 
	virtual public bool PositionAtFirstNodeOfType(string data_type) 
	virtual public bool ContainsANodeOfType(string data_type) 

	virtual public cmlData GetFirstNodeOfType(string data_type) 
	virtual public int GetFirstNodeOfTypei(string data_type) 
	virtual public cmlData GetLastNodeOfType(string data_type) 
	virtual public int GetLastNodeOfTypei(string data_type) 

	virtual public List<cmlData> DataWithField(string field, string val = "") 
	
	virtual public List<cmlData> Children(int index = -1) 
	virtual public List<cmlData> AllChildNodes(int index = -1) 
	virtual public List<int> Childreni(int index = -1) 
	virtual public List<int> AllChildNodesi(int index = -1) 
	virtual public List<cmlData> AllDataOfType(string type_name, int starting_index = 0, string stop_at_data_type = "")
	virtual public List<int> AllDataOfTypei(string type_name, int starting_index = 0, string stop_at_data_type = "")

	CML Properties
	-----------------------
	public float CMLVersion
	public cmlData this [int index] 
	virtual public int Count 
	virtual public float DocumentCMLVersion

/* ----------------------------------------------------------------------- */

/* ------------------------------------------------------------------------
	cmlData Function List
	-----------------------
	public void Clear()	//removes all entries in this variable

	//sets the value of a new or existing field
	virtual public void Set(string name, string data) 
	virtual public void Seti(string name, int data) 
	virtual public void Setf(string name, int data) 

	//returns data stored in the keys, typecasted to a specific type...
	virtual public int Int(string named) 
	virtual public bool Bool(string named)
	virtual public Rect Rect4(string named) 
	virtual public float Float(string named) 
	virtual public string String(string named) 
	virtual public Vector3 Vector(string named) 
	virtual public Quaternion Quat(string named) 
	
	virtual public void AddToData(string value) 				//adds data to the unordered list: data
	virtual public void ProcessCombinedFields(string combined)	//add multiple fields at once
	virtual public string ToString(bool include_id = true)		//return the variable as a CML string
	virtual public bool Remove(string name = "value")			//remove a field from this cmlData
	virtual public cmlData Copy(bool include_id = true)			//returns a duplicate of this variable

	cmlData Properties
	-----------------------
	public string this 					[string field_name]
	virtual public int 					ID						//returns the current node's ID
	public string						data_type				//the node name of this cmlData
	public Dictionary<string, string>	defined					//all the fields stored in this cmlData
	public List<string>					data					//all unordered list items
	public string[]						Keys					//all field names in this cmlData
	public string[]						Values					//all values in this cmlData

/* ----------------------------------------------------------------------- */

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace MBS {
	public enum CMLCopyMode {new_id, old_id, no_id }
	
	public class cmlData {
		public string						data_type;
		public Dictionary<string, string>	defined;
		public List<string>					data;	
		
		static public bool operator true(cmlData v) { return null != v; }
		static public bool operator false(cmlData v) { return null == v; }
		
		public string this [string field_name] {
			get { return String( field_name ); }
			set { Set( field_name, value); }
		}
		
		public int ID {
			get {if (defined.ContainsKey("id")) return int.Parse( defined["id"] ); return 0; }
			set { Set("id", value.ToString() ); }
		}
		
		public cmlData(int id = 0) {
			data	= new List<string>();
			defined = new Dictionary<string, string>();
			ID = id;
		}
		
		public void Clear() {
			defined.Clear();
			data.Clear();
		}
		
		virtual public bool Remove(string name = "value") {
			bool result = defined.ContainsKey(name);
			if (result)
				defined.Remove(name);
			
			return result;
		}
		
		virtual public string ToString(bool include_id = true) {
			string result = "<"+data_type+">";
			foreach(var d in defined)
				if (d.Key != "id" || include_id)
					result += "\n\t[" + d.Key + "]" + d.Value;
			
			foreach(string s in data)
				if (s != string.Empty)
					result += "\n\t" + s;
			
			return result;
		}
		
		virtual public string[] Keys
		{
			get
			{
				if (defined.Count == 0)
					return null;
				string[] results = new string[defined.Count];
				defined.Keys.CopyTo(results,0);
				return results;
			}
		}
		
		virtual public string[] Values
		{
			get
			{
				if (defined.Count == 0)
					return null;
				string[] results = new string[defined.Count];
				defined.Values.CopyTo(results,0);
				return results;
			}
		}
		
		virtual public void Set(string name, string data) {
			if (defined.ContainsKey(name) )
				defined[name] = data;
			else
				defined.Add(name, data);
		}
		
		virtual public void Seti(string name, int data) {
			Set (name, data.ToString());
		}
		
		virtual public void Setf(string name, float data) {
			Set (name, data.ToString());
		}
		
		virtual public int Int(string named = "value") {
			string result = string.Empty;
			if ( defined.TryGetValue(named, out result) ) {
				int value;
				if ( int.TryParse(result, out value) )
					return value;
				else
					return 0;
			}
			
			return 0;
		}
		
		virtual public float Float(string named = "value") {
			string result = string.Empty;
			if ( defined.TryGetValue(named, out result) ) {
				float value;
				if ( float.TryParse(result, out value) )
					return value;
				else
					return 0;
			}
			
			return 0;
		}
		
		virtual public string String(string named = "value") {
			string result = string.Empty;
			if ( defined.TryGetValue(named, out result) )
				return result;
			
			return string.Empty;
		}
		
		virtual public bool Bool(string named = "value") {
			string result = string.Empty;
			int i = 0;
			if ( defined.TryGetValue(named, out result) ) {
				result = result.Trim();
				if ( result.ToLower() == "true" )
					return true;
				
				if (int.TryParse(result, out i))
					return ( i > 0);
			}
			return false;
		}
		
		virtual public Vector3 Vector(string named = "value") {
			string result = string.Empty;
			if ( defined.TryGetValue(named, out result) ) {
				
				if (result.Length > 6) { // minimum possible length for Vector3 string
					string[] splitString = result.Substring(1, result.Length - 2).Split(',');
					if (splitString.Length == 3) {
						Vector3 outVector3 = new Vector3(0,0,0);
						outVector3.x = float.Parse(splitString[0]);
						outVector3.y = float.Parse(splitString[1]);
						outVector3.z = float.Parse(splitString[2]);
						return outVector3;
					}
				}
			}
			
			return Vector3.zero;
		}
		
		virtual public Quaternion Quat(string named = "value") {
			string result = string.Empty;
			if ( defined.TryGetValue(named, out result) ) {
				
				if (result.Length > 8) { // minimum possible length for Vector3 string
					string[] splitString = result.Substring(1, result.Length - 2).Split(',');
					if (splitString.Length == 4) {
						Quaternion outQ = new Quaternion(0,0,0,0);
						outQ.x = float.Parse(splitString[0]);
						outQ.y = float.Parse(splitString[1]);
						outQ.z = float.Parse(splitString[2]);
						outQ.w = float.Parse(splitString[3]);
						return outQ;
					}
				}
			}
			
			return Quaternion.identity;
		}
		
		virtual public Rect Rect4(string named = "value") {
			string result = string.Empty;
			if ( defined.TryGetValue(named, out result) ) {
				
				if (result.Length > 40) { // minimum possible length for Vector3 string
					string[] splitString = result.Substring(1, result.Length - 2).Split(',');
					if (splitString.Length == 4) {
						Debug.Log (result + " " + result.Length);
						foreach(string s in splitString)
							Debug.Log (s);
						
						Rect outQ = new Rect(0,0,0,0);
						outQ.x = float.Parse(splitString[0].Split(':')[1]);
						outQ.y = float.Parse(splitString[1].Split(':')[1]);
						outQ.width = float.Parse(splitString[2].Split(':')[1]);
						outQ.height = float.Parse(splitString[3].Split(':')[1]);
						return outQ;
					}
				}
			}
			
			return new Rect(0,0,0,0);
		}
		
		//add data to the unordered list
		virtual public void AddToData(string value) {
			data.Add(value);
		}
		
		//add multiple fields to this cmlData at once
		virtual public void ProcessCombinedFields(string combined) {
			if (combined == string.Empty)
				return;
			
			string[] fields = combined.Split(';');
			foreach (string field in fields) {
				if (field.IndexOf('=') == -1) {
					AddToData(field.Trim());
				} else {
					string[] keyVal = field.Split('=');
					Set( keyVal[0].Trim(), keyVal[1].Trim() );
				}
			}
		}
		
		//returns a duplicate of this cmlData object
		virtual public cmlData Copy(CMLCopyMode mode = CMLCopyMode.no_id, string id_value="-1") {
			cmlData result = new cmlData();
			result.data_type = this.data_type;
			foreach (var data in this.defined) {
				if (data.Key != "id") {
					result.Set(data.Key, data.Value);
				} else {
					switch (mode) {
						//keep the original id....
					case CMLCopyMode.old_id:
						result.Set("id", data.Value);
						break;
						
					case CMLCopyMode.new_id : 
						result.Set("id", id_value);
						break;
						
					case CMLCopyMode.no_id :
						result.Remove("id");
						break;
					}
				}
			}			
			foreach (string s in this.data)
				result.data.Add(s);
			
			return result;
		}
	}
	
	public class CML {
		const float __CMLVersion = 2.0f;
		
		public List<cmlData>			Elements;	
		public cmlData					Node;
		
		static public float CMLVersion { get { return __CMLVersion; } }
		virtual public float DocumentCMLVersion { get { return PositionAtFirstNodeOfType("CML Header") ? Node.Float("Version") : 0f; }}	
		
		public cmlData this [int index] {
			get { return Count > index ? Elements [index] : null; }
		}
		
		virtual public cmlData Last {
			get { return (Elements.Count == 0) ? null : Elements[Elements.Count - 1]; }
		}
		
		virtual public cmlData First {
			get { return (Elements.Count == 0) ? null : Elements[0]; }
		}
		
		virtual public int Count {
			get { return null == Elements ? 0 : Elements.Count; }
		}
		
		public CML(string filename = "") {
			if (string.Empty != filename) {
				if (!LoadFile(filename))
					Initialize();
			} else {
				Initialize();
			}
		}
		
		//clears the CML data
		virtual public void Initialize() {
			Node = null;
			if (null != Elements)
				Elements.Clear();
			Elements	= new List<cmlData>();
		}
		
		//load CML from a specified file path
		virtual public bool OpenFile(string FileName) {
			StreamReader f = new StreamReader(FileName);
			if (null == f)
				return false;
			
			string file = string.Empty;
			string line = string.Empty;
			while ((line = f.ReadLine()) != null) 
				file += line + "\n";
			return ParseFile(file);
		}
		
		//imbed the current CML version information to the CML data
		virtual public void ImbedCMLData() {
			if (PositionAtFirstNodeOfType("CML Header")) 
				Node.Set("Version", CMLVersion.ToString());
			else
				AddNode("CML Header","Version="+CMLVersion);
		}
		
		//load CML from PlayerPrefs
		virtual public bool Load(string PrefName) {
			string cml = PlayerPrefs.GetString(PrefName, string.Empty);
			if (cml == string.Empty)
				return false;
			
			ParseFile(cml);
			return true;
		}
		
		//load CML from a Resources folder
		virtual public bool LoadFile(string filename) {
			TextAsset FileResource = (TextAsset)Resources.Load(filename, typeof(TextAsset));
			if (null != FileResource) 
				return ParseFile( FileResource.text );
			
			return false;
		}
		
		//save this CML data to a specified file path. When saving, prefer to include CML version info
		//if it is not included already, add to the bottom of the file to avoid breaking data that was
		//setup to use specific node IDs. Optionally, simply don't include this info.
		virtual public bool SaveFile(string FileName, bool include_id = true) {
			StreamWriter f = new StreamWriter(File.Open(FileName, FileMode.Create));
			if (null == f)
				return false;
			
			ImbedCMLData();
			foreach(cmlData node in Elements) {
				if (node.data_type == "CML Header")
					f.WriteLine(node.ToString(false) + "\n");
				else
					f.WriteLine(node.ToString(include_id) + "\n");
			}
			f.Close();
			return true;
		}
		
		//save this CML data to PlayerPrefs
		virtual public bool Save(string PrefName, bool include_id = true) {
			
			PlayerPrefs.SetString(PrefName, ToString(include_id) );
			return true;
		}
		
		//return this entire CML data as a formatted string that can either be printed or saved
		virtual public string ToString(bool include_id = true) {
			ImbedCMLData();
			string pref = string.Empty;
			foreach(cmlData node in Elements)
				if (node.data_type == "CML Header")
					pref += node.ToString(false) + "\n";
			else
				pref += node.ToString(include_id) + "\n";
			
			return pref;
		}
		
		virtual public bool ParseFile(string data) {
			if (data == string.Empty)
				return false;
			
			Initialize();
			string[] lines = data.Split('\n');
			foreach (string line in lines) {
				//find and remove comments from the parser
				//take into account URL strings '://'
				string l = line;
				int commentIndex = l.IndexOf("//");
				if (commentIndex > -1) {
					int urlIndex = 0;
					if (commentIndex > 0) {
						while (l.IndexOf("://", urlIndex) == commentIndex - 1) {
							urlIndex = commentIndex + 1;
							commentIndex = l.IndexOf("//", urlIndex);
						}
					}
					
					if (commentIndex > -1)
						l = l.Substring(0, commentIndex);
				}
				l = l.Trim();
				
				//if the line is empty, do nothing with it...
				//ignore lines that close off data blocks...
				if (l == string.Empty //|| l.IndexOf("</") == 0
				    ) continue;
				
				//be default add data to the last datatype created
				int tagopen = l.IndexOf('[');
				int tagclose = l.IndexOf(']');
				if (tagclose > 1 && tagopen == 0) {
					if (null != Last) {
						string keyname = l.Substring(1, tagclose - 1).Trim();
						l = l.Substring(tagclose+1, l.Length - (tagclose + 1) ).Trim();
						if (null != Last) {
							Last.Set(keyname, l);
						}
					}
				} else {
					//check to see if this is the definition of a new datatype's constant fields
					//or wether this is new data for a particular datatype
					tagopen = l.IndexOf('<');
					tagclose = l.IndexOf('>');
					if (tagclose > 1 && tagopen == 0) {
						string key_name = l.Substring(1, tagclose - 1).Trim();
						string key_value = l.Substring(tagclose+1, l.Length - (tagclose + 1) );					
						AddNode(key_name, key_value);
					} else {
						//else add it as raw data...
						if (null != Last)
							Last.AddToData(l);
					}
				}
			}
			
			Node = (Count > 0) ? First : null;
			return true;
		}
		
		//remove all cmlData from this CML data
		virtual public void Clear() {
			foreach (cmlData d in Elements)
				d.Clear();
			Elements.Clear();
		}
		
		//append another CML data file to the end of this one
		virtual public bool Join(CML other, CMLCopyMode copy_mode = CMLCopyMode.no_id) {
			if (null == other || other.Count == 0)
				return false;
			
			foreach(cmlData d in other) {
				Elements.Add( d.Copy(copy_mode, Elements.Count.ToString()) );
			}
			
			return true;
		}
		
		//create a new cmlData node
		virtual public bool AddNode(string data_type, string add_data = "") {
			if (data_type == string.Empty)
				return false;
			
			Elements.Add( new cmlData( Elements.Count ) );
			Last.data_type = data_type;
			Last.ProcessCombinedFields(add_data);
			return true;
		}
		
		//add a whole cmlData variable to the CML data
		virtual public bool CopyNode(cmlData existing) {
			cmlData copy = existing.Copy();
			Elements.Add( new cmlData( Elements.Count ) );
			Last.data_type = copy.data_type;
			foreach (var key in copy.defined)
				Last.defined.Add(key.Key, key.Value);
			foreach (string s in copy.data)
				Last.data.Add(s);
			return true;
		}
		
		#region marker_navigation
		//move the marker to various positions in the CML data
		virtual public bool PositionAtFirstNode() {
			if (Count > 0)
				Node = Elements[0];
			else
				return false;
			return true;
		}
		
		virtual public bool PositionAtLastNode() {
			if (Count > 0)
				Node = Elements[Count-1];
			else
				return false;
			return true;
		}
		
		virtual public bool PositionAtNextNode() {
			if (null == Node)
				return false;
			
			int index = Node.ID + 1;
			return PositionAtNode(index);
		}
		
		virtual public bool PositionAtPreviousNode() {
			if (null == Node)
				return false;
			
			int index = Node.ID - 1;
			return PositionAtNode(index);
		}
		
		//CML files are supposed to have sequential nodes but since the
		//data author can override this, do a test to see if the specified
		//node is the one requested or else go look for it.
		//Since a single file can have multiple sections and each section
		//could start it's index from 1, you can also provide the section's
		//node index as an offset from which to start searching
		virtual public bool PositionAtNode(int index, int offset = 0) {
			if (index + offset < 0 || index + offset >= Count)
				return false;
			
			if (Elements [ index + offset ].ID == index) {
				Node = Elements[index + offset];
				return true;
			}
			
			int found_index = FindNodeByID(index, offset);
			if (found_index < 0)
				return false;
			Node = Elements[ found_index ];
			return true;
		}
		
		//position the marker at a node with a specific ID, even if it is
		//not at the correct array index. Use when you have multiple CML
		//files loaded into the same CML variable or if you manually change
		//the ID values of nodes. NOTE: never do that !
		virtual public bool PositionAtID(int id) {
			if (id < 0)
				return false;
			
			int index = FindNodeByID(id);
			if (index == -1)
				return false;
			
			Node = Elements[index];
			return true;
		}
		#endregion 
		
		//find a node by it's ID
		virtual public int FindNodeByID(int id, int offset = 0) {
			for (int i = offset; i < Count; i++) 
			if (Elements[i].ID == id) {
				//if the node was found, point to it
				return i;
			}
			return -1;
		}
		
		//remove the node at the current marker position
		virtual public bool RemoveCurrentNode() {
			return RemoveNode( Node.ID );
		}
		
		//remove the node at index 
		virtual public bool RemoveNode(int index) {
			if ( Count <= index)
				return false;
			
			//position Node on the next node
			if (null != Node)
				if (Node.ID == index)
					if (index < Count - 1)
						Node = Elements[index + 1];
			else
				//if there isn't a "next" node, first check if there is a "previous" node
				//and go there if there is...
				if (Node.ID == 0)
					Node = null;
			else
				Node = Elements[index - 1];
			
			Elements.RemoveAt(index);
			for (int i = index; i < Elements.Count; i++)
				Elements[i].ID = i;
			
			return true;
		}
		
		//add a node at the current marker position
		virtual public bool InsertNode(string data_type, string add_data = "") {
			if (null != Node)
				return InsertNode(Node.ID, data_type, add_data);
			else
				return AddNode(data_type, add_data);
		}
		
		//add a node a specific index
		virtual public bool InsertNode(int index, string data_type, string add_data="") {
			if (data_type == string.Empty)
				return false;
			
			if (Count == 0)
				return AddNode(data_type, add_data);
			
			Elements.Insert(index, new cmlData( index ) );
			Node = Elements[index];
			Node.data_type = data_type;
			Node.ProcessCombinedFields(add_data);
			for (int i = index + 1; i < Count; i++)
				Elements[i].ID = i;
			
			return true;
		}
		
		//test if this CML variable contains at least 1 node of type data_type
		virtual public bool ContainsANodeOfType(string data_type) {
			foreach (cmlData node in Elements)
				if (node.data_type == data_type)
					return true;
			return false;
		}
		
		//return the first node of type data_type
		virtual public cmlData GetFirstNodeOfType(string data_type) {
			if ( Count == 0 )
				return null;
			
			for ( int i = 0; i < Count; i++)
				if (Elements[i].data_type == data_type)
					return Elements[i];
			return null;
		}
		
		//return the ID of the first node of type data_type
		virtual public int GetFirstNodeOfTypei(string data_type) {
			if ( Count == 0 )
				return -1;
			
			for ( int i = 0; i < Count; i++)
				if (Elements[i].data_type == data_type)
					return i;
			return -1;
		}
		
		//return the last node of type data_type
		virtual public cmlData GetLastNodeOfType(string data_type) {
			if ( Count == 0 )
				return null;
			
			for ( int i = Count -1; i >= 0; i--)
				if (Elements[i].data_type == data_type)
					return Elements[i];
			return null;
		}
		
		//return the ID of the last node of type data_type
		virtual public int GetLastNodeOfTypei(string data_type) {
			if ( Count == 0 )
				return -1;
			
			for ( int i = Count -1; i >= 0; i--)
				if (Elements[i].data_type == data_type)
					return i;
			return -1;
		}
		
		virtual public bool PositionAtFirstNodeOfType(string data_type) {
			cmlData result = GetFirstNodeOfType(data_type);
			if (null != result)
				PositionAtNode(result.ID);
			
			return (null != result);
		}
		
		//fetch all nodes under a specific node.
		//collection of nodes will end when it reaches the end of file or
		//it finds another node of the same data_type as the parent node
		//does not fetch child nodes recursively
		virtual public List<cmlData> Children(int index = -1) {
			
			//if a node is not specified, try to use the currently active Node
			if ( index == -1 ) {
				if ( null == Node )
					return null;
				else
					index = Node.ID;
			}
			
			//if an invalid node is selected, return nothing
			//also return nothing if the very last node was selected
			if ( Count <= index + 1)
				return null;
			
			//see what the data type is of the first child node...		
			string parent_data_type = Elements[index++].data_type;
			string data_type		= Elements[index].data_type;
			
			//if the child node is of the same type as the parent node
			//then consider the parent childless		
			if (data_type == parent_data_type)
				return null;
			
			//loop through all remaining databclocks and return each that matches
			//the data type of the first child node, stopping at the first datablock
			//that matches the original data type
			List<cmlData> Result = new List<cmlData>();
			Result.Add( Elements[ index ] );
			for (++index; index < Count; index++) {
				if ( Elements[ index ].data_type == data_type)
					Result.Add( Elements[ index] );
				else
					if ( Elements[ index ].data_type == parent_data_type)
						break;
			}
			return Result;
		}
		
		//recursively fetch all nodes under a specific node.
		//collection of nodes will end when it reaches the end of file or
		//it finds another node of the same data_type as the parent node
		virtual public List<cmlData> AllChildNodes(int index = -1) {
			
			//if a node is not specified, try to use the currently active Node
			if ( index == -1 ) {
				if ( null == Node )
					return null;
				else
					index = Node.ID;
			}
			
			//if an invalid node is selected, return nothing
			//also return nothing if the very last node was selected
			if ( Count <= index + 1)
				return null;
			
			//see what the data type is of the parent node...		
			string parent_data_type = Elements[index].data_type;
			
			//loop through all remaining databclocks and return each that matches
			//the data type of the first child node, stopping at the first datablock
			//that matches the original data type
			List<cmlData> Result = new List<cmlData>();
			for (++index; index < Count; index++) {
				if ( Elements[ index ].data_type == parent_data_type)
					break;
				
				Result.Add( Elements[ index] );
			}
			if (Result.Count > 0)
				return Result;
			
			return null;
		}
		
		//fetch the node IDs of all nodes under a specific node.
		//collection of nodes will end when it reaches the end of file or
		//it finds another node of the same data_type as the parent node
		//does not fetch child nodes recursively
		virtual public List<int> Childreni(int index = -1) {
			
			//if a node is not specified, try to use the currently active Node
			if ( index == -1 ) {
				if ( null == Node )
					return null;
				else
					index = Node.ID;
			}
			
			//if an invalid node is selected, return nothing
			//also return nothing if the very last node was selected
			if ( Count <= index + 1)
				return null;
			
			//see what the data type is of the first child node...		
			string parent_data_type = Elements[index++].data_type;
			string data_type		= Elements[index].data_type;
			
			//if the child node is of the same type as the parent node
			//then consider the parent childless		
			if (data_type == parent_data_type)
				return null;
			
			//loop through all remaining databclocks and return each that matches
			//the data type of the first child node, stopping at the first datablock
			//that matches the original data type
			List<int> Result = new List<int>();
			Result.Add( index  );
			for (++index; index < Count; index++) {
				if ( Elements[ index ].data_type == data_type)
					Result.Add( index );
				else
					if ( Elements[ index ].data_type == parent_data_type)
						break;
			}
			return Result;
		}
		
		//recursively fetch the node IDs of all nodes under a specific node.
		//collection of nodes will end when it reaches the end of file or
		//it finds another node of the same data_type as the parent node
		virtual public List<int> AllChildNodesi(int index = -1) {
			
			//if a node is not specified, try to use the currently active Node
			if ( index == -1 ) {
				if ( null == Node )
					return null;
				else
					index = Node.ID;
			}
			
			//if an invalid node is selected, return nothing
			//also return nothing if the very last node was selected
			if ( Count <= index + 1)
				return null;
			
			//see what the data type is of the parent node...		
			string parent_data_type = Elements[index].data_type;
			
			//loop through all remaining databclocks and return each that matches
			//the data type of the first child node, stopping at the first datablock
			//that matches the original data type
			List<int> Result = new List<int>();
			for (++index; index < Count; index++) {
				if ( Elements[ index ].data_type == parent_data_type)
					break;
				
				Result.Add( index );
			}
			if (Result.Count > 0)
				return Result;
			
			return null;
		}
		
		virtual public List<cmlData> DataWithField(string field, string val = "") {
			return NodesWithField(field,val);
		}
		
		//find all nodes that contain the specified field. If a value is provided,
		//return all nodes to contain the field with the selected value
		virtual public List<cmlData> NodesWithField(string field, string val = "") {
			List<cmlData> Result = new List<cmlData>();
			for (int index = 0; index < Count; index++) {
				foreach (var k in Elements[index].defined) 
				if (k.Key == field) {
					if (val == string.Empty)
						Result.Add( Elements[ index] );
					else
						if (k.Value == val)
							Result.Add( Elements [ index] );
					break;
				}
			}
			if (Result.Count > 0)
				return Result;
			
			return null;
		}
		
		//find the first node that contains the specified field. If a value
		//is provided, return the first node to contain the field with the selected value
		virtual public cmlData NodeWithField(string field, string val = "") {
			List<cmlData> results = NodesWithField(field, val);
			if (null != results)
				return results[0];
			return null;
		}
		
		//depreciated
		virtual public List<cmlData> AllDataOfType(string type_name, int starting_index = 0, string stop_at_data_type = "") {
			return AllNodesOfType(type_name, starting_index, stop_at_data_type);
		}
		
		//Return all nodes of type type_name, starting from the node with ID starting_index and stopping
		//as soon as it finds a node of type stop_at_data_node if specified.
		virtual public List<cmlData> AllNodesOfType(string type_name, int starting_index = 0, string stop_at_data_type = "") {
			List<cmlData> result = new List<cmlData>();
			if (starting_index >= Count)
				return result;
			
			for (int i = starting_index; i < Count; i++) {
				if (stop_at_data_type != string.Empty && Elements[i].data_type == stop_at_data_type)
					break;
				
				if (Elements[i].data_type == type_name)
					result.Add(Elements[i]);
			}				
			return result;
		}
		
		//depreciated
		virtual public List<int> AllDataOfTypei(string type_name, int starting_index = 0, string stop_at_data_type = "") {
			return AllNodesOfTypei(type_name, starting_index, stop_at_data_type);
		}
		
		//Return the node ID's of all nodes of type type_name, starting from the node with ID starting_index and stopping
		//as soon as it finds a node of type stop_at_data_node if specified.
		virtual public List<int> AllNodesOfTypei(string type_name, int starting_index = 0, string stop_at_data_type = "") {
			List<int> result = new List<int>();
			if (starting_index >= Count)
				return result;
			
			for (int i = starting_index; i < Count; i++) {
				if (stop_at_data_type != string.Empty && Elements[i].data_type == stop_at_data_type)
					break;
				
				if (Elements[i].data_type == type_name)
					result.Add(i);
			}
			
			return result;
		}
		
		//impliment the ability to use "foreach" on CML data types
		public IEnumerator GetEnumerator() {
			return new cmlI(Elements);
		}
	}
	
	//this is created merely to allow for the us of "foreach" when navigating through CMl data
	class cmlI : IEnumerator {
		List<cmlData> Elements;
		int i = -1;
		
		public cmlI(List<cmlData> data) {
			Elements = data;
		}
		
		public bool MoveNext() {
			if (++i < Elements.Count)
				return true;
			return false;
		}
		
		public void Reset() {
			i = -1;
		}
		
		public object Current {	get { return Elements[i];} }
		
	}
}
