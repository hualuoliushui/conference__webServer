using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Vote
{
    public class VoteService
    {
        private static int VoteNameMin = 1;
        private static int VoteNameMax = 50;

        private static int VoteDescriptionMax = 255;

        private static int VoteOptionMin = 1;
        private static int VoteOptionMax = 50;

        private static bool checkFormat(string voteName, string voteDescription, List<string> voteOptions)
        {
            if (voteName.Length >= VoteNameMin
                && voteName.Length <= VoteNameMax
                && voteDescription.Length <= VoteDescriptionMax)
            {
                foreach (string item in voteOptions)
                {
                    if (item.Length < VoteOptionMin
                        || item.Length > VoteOptionMax)
                    {
                        return false;
                    }
                }
                return true; //所有项均符合
            }
            else
                return false;
        }

        public static Status getAll(int agendaID, out List<Vote> votes)
        {
            votes = new List<Vote>();


            return Status.SUCCESS;
        }
    }
}