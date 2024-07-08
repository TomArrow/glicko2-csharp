namespace Glicko2
{
    /// <summary>
    /// Holds an individual's Glicko-2 rating.
    /// 
    /// Glicko-2 ratings are an average skill value, a standard deviation and a volatility
    /// (how consistent the player is). Prof Glickman's paper on the algorithm allows scaling
    /// of these values to be more directly comparable with existing rating systems such as
    /// Elo or USCF's derivation thereof. This implementation outputs ratings at this larger
    /// scale.
    /// </summary>
    public class Rating
    {
        private readonly RatingCalculator _ratingSystem;

        private double _rating;
        private double _ratingDeviation;
        private double _volatility;

        private double _ratingTmp;
        private double _ratingDeviationTmp;
        private double _volatilityTmp;
        /// <summary>
        /// The number of results from which the rating has been calculated.
        /// </summary>
        private int _numberOfResults;

        /// <summary>
        /// The number of results from which the temporary rating has been calculated.
        /// </summary>
        private int _numberOfResultsTmp;

        // the following variables are used to hold values temporarily whilst running calculations
        private double _workingRating;
        private double _workingRatingDeviation;
        private double _workingVolatility;

        /// <summary>
        /// Constructor. Takes the rating, deviation, and volatility default values
        /// from the rating system.
        /// </summary>
        /// <param name="ratingSystem"></param>
        public Rating(RatingCalculator ratingSystem)
        {
            _ratingSystem = ratingSystem;
            _ratingTmp = _rating = _ratingSystem.GetDefaultRating();
            _ratingDeviationTmp = _ratingDeviation = _ratingSystem.GetDefaultRatingDeviation();
            _volatilityTmp = _volatility = ratingSystem.GetDefaultVolatility();
        }

        /// <summary>
        /// Constructor. Allows you to pass in the rating, deviation, and volatility.
        /// </summary>
        /// <param name="ratingSystem"></param>
        /// <param name="initRating"></param>
        /// <param name="initRatingDeviation"></param>
        /// <param name="initVolatility"></param>
        public Rating(RatingCalculator ratingSystem, double initRating, double initRatingDeviation,
            double initVolatility)
        {
            _ratingSystem = ratingSystem;
            _rating = initRating;
            _ratingDeviation = initRatingDeviation;
            _volatility = initVolatility;
        }

        /// <summary>
        /// Return the average skill value of the player.
        /// </summary>
        /// <returns></returns>
        public double GetRating(bool temporary = false)
        {
            return temporary ? _ratingTmp : _rating;
        }

        public void SetRating(double rating, bool temporary)
        {
            if(!temporary) _rating = rating;
            _ratingTmp = rating;
        }

        /// <summary>
        /// Return the average skill value of the player scaled down
	    /// to the scale used by the algorithm's internal workings.
        /// </summary>
        /// <returns></returns>
        public double GetGlicko2Rating(bool temporary = false)
        {
            return _ratingSystem.ConvertRatingToGlicko2Scale(temporary ? _ratingTmp : _rating);
        }

        /// <summary>
        /// Set the average skill value, taking in a value in Glicko2 scale.
        /// </summary>
        /// <param name="rating"></param>
        public void SetGlicko2Rating(double rating, bool temporary)
        {
            double theRating = _ratingSystem.ConvertRatingToOriginalGlickoScale(rating);
            if (!temporary) _rating = theRating;
            _ratingTmp = theRating;
        }

        public double GetVolatility(bool temporary = false)
        {
            return temporary ? _volatilityTmp : _volatility;
        }

        public void SetVolatility(double volatility, bool temporary)
        {
            if(!temporary) _volatility = volatility;
            _volatilityTmp = volatility;
        }

        public double GetRatingDeviation(bool temporary = false)
        {
            return temporary ? _ratingDeviationTmp : _ratingDeviation;
        }

        public void SetRatingDeviation(double ratingDeviation, bool temporary)
        {
            if(!temporary) _ratingDeviation = ratingDeviation;
            _ratingDeviationTmp = ratingDeviation;
        }

        /// <summary>
        /// Return the rating deviation of the player scaled down
	    /// to the scale used by the algorithm's internal workings.
        /// </summary>
        /// <returns></returns>
        public double GetGlicko2RatingDeviation()
        {
            return _ratingSystem.ConvertRatingDeviationToGlicko2Scale(_ratingDeviation);
        }

        /// <summary>
        /// Set the rating deviation, taking in a value in Glicko2 scale.
        /// </summary>
        /// <param name="ratingDeviation"></param>
        public void SetGlicko2RatingDeviation(double ratingDeviation, bool temporary)
        {
            double theRating = _ratingSystem.ConvertRatingDeviationToOriginalGlickoScale(ratingDeviation);
            if(!temporary) _ratingDeviation = theRating;
            _ratingDeviationTmp = theRating;
        }

        /// <summary>
        /// Used by the calculation engine, to move interim calculations into their "proper" places.
        /// </summary>
        public void FinaliseRating(bool temporary)
        {
            SetGlicko2Rating(_workingRating, temporary);
            SetGlicko2RatingDeviation(_workingRatingDeviation, temporary);
            SetVolatility(_workingVolatility, temporary);

            SetWorkingRatingDeviation(0);
            SetWorkingRating(0);
            SetWorkingVolatility(0);
        }

        public int GetNumberOfResults(bool temporary=false)
        {
            return temporary ? _numberOfResultsTmp : _numberOfResults;
        }

        public void IncrementNumberOfResults(int increment, bool temporary)
        {
            if (!temporary) _numberOfResults = _numberOfResults + increment;
            _numberOfResultsTmp = _numberOfResults + increment;
        }

        public void SetWorkingVolatility(double workingVolatility)
        {
            _workingVolatility = workingVolatility;
        }

        public void SetWorkingRating(double workingRating)
        {
            _workingRating = workingRating;
        }

        public void SetWorkingRatingDeviation(double workingRatingDeviation)
        {
            _workingRatingDeviation = workingRatingDeviation;
        }
    }
}
