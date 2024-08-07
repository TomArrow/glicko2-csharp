﻿using System.Collections.Generic;

namespace Glicko2
{
    /// <summary>
    /// This class holds the results accumulated over a rating period.
    /// </summary>
    public class RatingPeriodResults
    {
        private readonly List<Result> _results = new List<Result>();
        private readonly HashSet<Rating> _participants = new HashSet<Rating>();
        private readonly HashSet<Rating> _activeParticipants = new HashSet<Rating>();

        /// <summary>
        /// Create an empty result set.
        /// </summary>
        public RatingPeriodResults()
        {
        }

        /// <summary>
        /// Constructor that allows you to initialise the list of participants.
        /// </summary>
        /// <param name="participants"></param>
        public RatingPeriodResults(HashSet<Rating> participants)
        {
            _participants = participants;
        }

        /// <summary>
        /// Add a result to the set.
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        public void AddResult(Rating winner, Rating loser, double weight = 1.0)
        {
            lock (_results)
            {
                var result = new Result(winner, loser,false, weight);

                _activeParticipants.Add(winner);
                _activeParticipants.Add(loser);
                _results.Add(result);
            }
        }

        /// <summary>
        /// Record a draw between two players and add to the set.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void AddDraw(Rating player1, Rating player2)
        {
            lock (_results)
            {
                var result = new Result(player1, player2, true);

                _activeParticipants.Add(player1);
                _activeParticipants.Add(player2);
                _results.Add(result);
            }
        }

        /// <summary>
        /// Get count of recorded results.
        /// </summary>
        /// <returns>Count of results recorded so far</returns>
        public int GetResultCount()
        {
            lock (_results)
            {
                return _results.Count;
            }
        }

        /// <summary>
        /// Get count of participants who were active in this rating period.
        /// </summary>
        /// <returns>Count of participants who were active in this rating period</returns>
        public int GetActiveParticipantCount()
        {
            lock (_results)
            {
                return _activeParticipants.Count;
            }
        }

        /// <summary>
        /// Get a list of the results for a given player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public IList<Result> GetResults(Rating player)
        {
            var filteredResults = new List<Result>();

            lock (_results)
            {
                foreach (var result in _results)
                {
                    if (result.Participated(player))
                    {
                        filteredResults.Add(result);
                    }
                }
            }

            return filteredResults;
        }

        /// <summary>
        /// Get all the participants whose results are being tracked.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Rating> GetParticipants()
        {
            lock (_results)
            {
                // Run through the results and make sure all players have been pushed into the participants set.
                foreach (var result in _results)
                {
                    _participants.Add(result.GetWinner());
                    _participants.Add(result.GetLoser());
                }

                return _participants;
            }
        }

        /// <summary>
        /// Add a participant to the rating period, e.g. so that their rating will
        /// still be calculated even if they don't actually compete.
        /// </summary>
        /// <param name="rating"></param>
        public void AddParticipant(Rating rating)
        {
            lock (_results)
            {
                _participants.Add(rating);
            }
        }

        /// <summary>
        /// Clear the result set.
        /// </summary>
        public void Clear()
        {
            lock (_results)
            {
                _results.Clear();
                _activeParticipants.Clear();
            }
        }
    }
}
