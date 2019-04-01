using System;
using System.Collections.Generic;
using GitHelper;
using System.IO;

public class FileChange
{
	public FileChange() { }
	public string NEW { get; set; }
	public string ETA { get; set; }
	public string BST { get; set; }
}

public class ListOfChanges : List<FileChange>
{
	DateTime _created = DateTime.Now;
	string _branch = "";
	public ListOfChanges(List<FileChange> files, string branch)
	{
		_branch = branch;
		AddRange(files);
	}
	public ListOfChanges() { }
	public ListOfChanges(ListOfChanges parent)
	{
		_branch = parent._branch;
		_created = parent.Created;
		AddRange(parent);
	}
	public DateTime Created
	{
		get
		{
			return _created;
		}
	}
	public string Branch
	{
		get
		{
			return string.IsNullOrEmpty(_branch) ? "master" : _branch;
		}
	}
}

public class ChangesContainer
{
	static object _commitobj = new object();
	static object _lockobj = new object();
	static Dictionary<string, ListOfChanges> _dic = new Dictionary<string, ListOfChanges>();
	
	public static string ProcessPageList(string lstFiles)
	{
		string key = Guid.NewGuid().ToString();
		FileChange fc = null;
		ListOfChanges changes = new ListOfChanges();
		foreach (string fl in lstFiles.Split('?'))
		{
			string file = fl.Replace("\"", "").Trim();
			if (string.IsNullOrEmpty(file))
			{
				continue;
			}
			if (fc == null)
			{
				fc = new FileChange() { NEW = file };
				changes.Add(fc);
			}
			else
			{
				fc.ETA = file;
				fc = null;
			}
		}
		return Add(changes);
	}
	public static string Add(ListOfChanges lc)
	{
		string key = Guid.NewGuid().ToString();
		lock (_lockobj)
		{
			//cleanup old data kept more than 1 hours
			List<string> ls = new List<string>(_dic.Keys);
			foreach (string dickey in ls)
			{
				if ((DateTime.Now -_dic[dickey].Created).TotalHours > 1)
				{
					_dic.Remove(dickey);
				}
			}
			//new list setup
			_dic[key] = lc;
		}
		return key;
	}
	public static void Remove(string key)
	{
		lock (_lockobj)
		{
			if (_dic.ContainsKey(key))
			{
				_dic.Remove(key);
			}
		}
	}
	public static ListOfChanges Get(string key)
	{
		lock (_lockobj)
		{
			if (_dic.ContainsKey(key))
			{
				return new ListOfChanges(_dic[key]);
			}
			return new ListOfChanges();
		}
	}
	public static List<string> Commit(string key, List<int> indexes, string comment, string user)
	{
		List<string> output = new List<string>();
		output.Add("Starting job...");
		lock (_commitobj)
		{
			try
			{
				ListOfChanges changes = Get(key);
				string gitpath = Settings.CurrentSettings.WORKGIT;
				if (!Directory.Exists(Settings.CurrentSettings.WORKGIT))
				{
					throw new Exception("Git Directory does not exist!: " + Settings.CurrentSettings.WORKGIT);
				}
				Git git = new Git(gitpath);
				output.AddRange(git.ResetHard());
				output.AddRange(git.FetchAll());
				output.AddRange(git.Checkout(changes.Branch));
				if (git.CurrentBranch() != changes.Branch)
				{
					output.Add("Failed to switch branch!");
					return Git.DiffFriendOutput(output);
				}
				output.AddRange(git.PullOrigin());
				foreach (int i in indexes)
				{
					FileChange fc = changes[i];
					string gitfile = fc.BST.Replace(@"S:\", gitpath).Replace(@"s:\", gitpath);
					string gitdir = Path.GetDirectoryName(gitfile);
					if (!Directory.Exists(gitdir))
					{
						Directory.CreateDirectory(gitdir);
					}
					output.Add("Copying file: " + gitfile + "...");
					File.Copy(fc.NEW, gitfile, true);
					output.AddRange(git.AddFile(gitfile));
				}
				Commit oldcommit = git.GetTopCommit();
				output.AddRange(git.CommitAll("WEB: " + comment, user));
				Commit newcommit = git.GetTopCommit();
				if (oldcommit.COMMIT == newcommit.COMMIT)
				{
					output.Add("-No files have been commited!");
				}
				else
				{
					output.Add(string.Format("+{0} files have been commited!", newcommit.EnumFiles().Count));
				}
				output.AddRange(git.PushCurrentBranch());
				output.AddRange(git.ResetHard());
				output.AddRange(git.Checkout("master"));
				Remove(key);
			}
			catch (Exception e)
			{
				output.Add(e.ToString());
			}
		}
		return Git.DiffFriendOutput(output);
	}
}
