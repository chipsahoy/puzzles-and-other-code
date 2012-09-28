using System;
using POG.Utils;
using POG.Forum;
namespace POG.Werewolf
{
    interface IPogDb
    {
        void AddPosts(POG.Forum.Posts posts);
        void Connect(string dbName);
        Int32 GetPlayerId(string player);
        string GetAlias(int threadId, string bolded);
        Boolean GetDayBoundaries(int threadId, int day, out int startPost, out DateTime endTime, out int endPost);
        System.Collections.Generic.List<string> GetLivePlayers(int threadId, int postNumber);
        int? GetMaxPost(int threadId);
        DateTime? GetPostTime(int threadId, int postNumber);
        SortableBindingList<Voter> GetVotes(Int32 threadId, Int32 startPost, DateTime endTime, object game);
        SortableBindingList<CensusEntry> ReadRoster(Int32 threadId);

        void SetIgnoreOnBold(int postId, int boldPosition, bool ignore);
        void WriteAlias(int threadId, string bolded, int playerId);
        void WriteThreadDefinition(Int32 threadId, String url, Boolean turbo);
        void WriteDayBoundaries(int threadId, int day, int startPost, DateTime endTime);
        void WriteUnhide(int threadId, string player, int startPostId, DateTimeOffset endTime);

        void WriteRoster(int _threadId, SortableBindingList<Forum.CensusEntry> _census);

        System.Collections.Generic.IEnumerable<Poster> GetPostersLike(string name);

        void AddPosters(System.Collections.Generic.IEnumerable<Poster> posters);
    }
}
