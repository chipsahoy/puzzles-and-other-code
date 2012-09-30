using System;
using POG.Utils;
using POG.Forum;
namespace POG.Werewolf
{
    interface IPogDb
    {
        void Connect(string dbName);

        Int32 GetPlayerId(string player);
        string GetAlias(int threadId, string bolded);
        Boolean GetDayBoundaries(int threadId, int day, out DateTime startTime, out DateTime endTime, out int endPost);
        System.Collections.Generic.List<string> GetLivePlayers(int threadId, int postNumber);
        int? GetMaxPost(int threadId);
        DateTime? GetPostTime(int threadId, int postNumber);
        SortableBindingList<Voter> GetVotes(Int32 threadId, DateTime startTime, DateTime endTime, object game);
        SortableBindingList<CensusEntry> ReadRoster(Int32 threadId);
        System.Collections.Generic.IEnumerable<Poster> GetPostersLike(string name);
        Post GetPost(Int32 threadId, Int32 postNumber);
        Int32 GetPostId(Int32 threadId, Int32 postNumber);
        Int32 GetPostNumber(Int32 threadId, Int32 postId);

        void AddPosts(POG.Forum.Posts posts);
        void SetIgnoreOnBold(int postId, int boldPosition, bool ignore);
        void WriteAlias(int threadId, string bolded, int playerId);
        void WriteThreadDefinition(Int32 threadId, String url, Boolean turbo);
        void WriteDayBoundaries(int threadId, int day, DateTime startTime, DateTime endTime);
        void WriteUnhide(int threadId, string player, int startPostId, DateTimeOffset endTime);
        void WriteRoster(int _threadId, SortableBindingList<Forum.CensusEntry> _census);
        void AddPosters(System.Collections.Generic.IEnumerable<Poster> posters);

        int GetPostBeforeTime(Int32 threadId, DateTime startTime);
    }
}
