using System;
using POG.Forum;
using System.Collections.Generic;

namespace POG.Werewolf
{
	public interface IPogDb
	{
		void Connect(string dbName);

		Int32 GetPlayerId(string player);
		string GetAlias(int threadId, string bolded);
		Boolean GetDayBoundaries(int threadId, int day, out Int32 startPost, out DateTime endTime, out int endPost);
		IEnumerable<string> GetLivePlayers(int threadId, int postNumber);
		int? GetMaxPost(int threadId);
		DateTime? GetPostTime(int threadId, int postNumber);
		IEnumerable<VoterInfo> GetVotes(Int32 threadId, Int32 startPost, DateTime endTime, Boolean lockedVotes, object game);
		IEnumerable<CensusEntry> ReadRoster(Int32 threadId);
		System.Collections.Generic.IEnumerable<Poster> GetPostersLike(string name);
		Post GetPost(Int32 threadId, Int32 postNumber);
		Int32 GetPostId(Int32 threadId, Int32 postNumber);
		Int32 GetPostNumber(Int32 threadId, Int32 postId);

		void AddPosts(POG.Forum.Posts posts);
		void SetIgnoreOnBold(int postId, int boldPosition, bool ignore);
		void WriteAlias(int threadId, string bolded, int playerId);
		void WriteThreadDefinition(Int32 threadId, String url, Boolean turbo);
		void WriteDayBoundaries(int threadId, int day, Int32 startPost, DateTime endTime);
		void WriteUnhide(int threadId, string player, int startPostId, DateTimeOffset endTime);
		void WriteRoster(int _threadId, IEnumerable<Forum.CensusEntry> _census);
		void AddPosters(IEnumerable<Poster> posters);

		int GetPostBeforeTime(Int32 threadId, DateTime startTime);

		void ChangeBolded(int _threadId, string player, string oldbold, string newbold);

		void KillPlayer(int threadId, string name, int postNumber);
		void SubPlayer(int threadId, string oldName, string newName);
	}
}
