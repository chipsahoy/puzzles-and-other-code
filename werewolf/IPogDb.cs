using System;
using POG.Utils;
namespace POG.Werewolf
{
    interface IPogDb
    {
        void AddPosts(POG.Forum.Posts posts);
        void Connect(string dbName);
        string GetAlias(int threadId, string bolded);
        Boolean GetDayBoundaries(int threadId, int day, out int startPost, out DateTime endTime, out int endPost);
        System.Collections.Generic.List<string> GetLivePlayers(int threadId, int postNumber);
        int? GetMaxPost(int threadId);
        DateTime? GetPostTime(int threadId, int postNumber);
        SortableBindingList<Voter> GetVotes(Int32 threadId, Int32 startPost, DateTime endTime, object game);
        void KillPlayer(int threadId, string player, string cause, string team, int post);
        void ReplacePlayerList(int threadId, System.Collections.Generic.IEnumerable<string> players);
        void SetIgnoreOnBold(int postId, int boldPosition, bool ignore);
        void SubPlayer(int threadId, string player, string sub, int post);
        void WriteAlias(int threadId, string bolded, string player);
        void WriteDayBoundaries(int threadId, string url, Boolean turbo, int day, int startPost, DateTime endTime);
        void WriteUnhide(int threadId, string player, int startPostId, DateTimeOffset endTime);
    }
}
