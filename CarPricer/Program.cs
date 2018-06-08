using System;
using NUnit.Framework;

#region Instructions
/*
 * You are tasked with writing an algorithm that determines the value of a used car, 
 * given several factors.
 * 
 *    AGE:    Given the number of months of how old the car is, reduce its value one-half 
 *            (0.5) percent.
 *            After 10 years, it's value cannot be reduced further by age. This is not 
 *            cumulative.
 *            
 *    MILES:    For every 1,000 miles on the car, reduce its value by one-fifth of a
 *              percent (0.2). Do not consider remaining miles. After 150,000 miles, it's 
 *              value cannot be reduced further by miles.
 *            
 *    PREVIOUS OWNER:    If the car has had more than 2 previous owners, reduce its value 
 *                       by twenty-five (25) percent. If the car has had no previous  
 *                       owners, add ten (10) percent of the FINAL car value at the end.
 *                    
 *    COLLISION:        For every reported collision the car has been in, remove two (2) 
 *                      percent of it's value up to five (5) collisions.
 *                    
 * 
 *    Each factor should be off of the result of the previous value in the order of
 *        1. AGE
 *        2. MILES
 *        3. PREVIOUS OWNER
 *        4. COLLISION
 *        
 *    E.g., Start with the current value of the car, then adjust for (1)age, take that  
 *    result then adjust for (2)miles, then (3)collision, and finally (4)previous owner. 
 *
 *    Note that if previous owner, had a positive effect, then it should be applied 
 *    AFTER step 4. If a negative effect, then BEFORE step 4.
 */
#endregion

namespace CarPricer
{
    public class Car
    {
        public decimal PurchaseValue { get; set; }
        public int AgeInMonths { get; set; }
        public int NumberOfMiles { get; set; }
        public int NumberOfPreviousOwners { get; set; }
        public int NumberOfCollisions { get; set; }
    }

    /// <summary>
    /// The class used to determine the value of a used car.
    /// </summary>
    public class PriceDeterminator
    {
        #region FIELDS
        /// <summary>
        /// The factor by which we reduce per month.
        /// </summary>
        private const double mAgeFactor = 0.005f;
        /// <summary>
        /// The age of the car in months.
        /// </summary>
        private const int mMaxAge = 120;

        /// <summary>
        /// The factor by which we reduce the price per
        /// 1000 miles.
        /// </summary>
        private const double mMilesFactor = 0.002f;
        /// <summary>
        /// The maximum number of miles before we stop
        /// reducing the price.
        /// </summary>
        private const int mMaxMiles = 150000;
        /// <summary>
        /// The number of miles used to trigger mileage 
        /// reduction.
        /// </summary>
        private const double mThousand = 1000;

        /// <summary>
        /// The factor by which we reduce for more 
        /// than two owners.
        /// </summary>
        private const double mPrevOwnerFactor = 0.25f;
        /// <summary>
        /// Only occurs when NumberOfPreviousOwners is 0.
        /// 10 percent increase.
        /// </summary>
        private const double mOwnerSpecial = 0.10;

        /// <summary>
        /// After this many collisions, we stop accruing. 
        /// </summary>
        private const double mMaxCollisions = 5;
        /// <summary>
        /// The factor by which we reduce the price per
        /// collision.
        /// </summary>
        private const double mCollisionFactor = 0.02f;
        #endregion

        #region METHODS
        /// <summary>
        /// Method to caculate the value of a used car.
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        public decimal DetermineCarPrice(Car car)
        {
            //The Test
            // throw new NotImplementedException("Implement here!");

            double usedValue = (double)car.PurchaseValue;
            /*************************************
            AGE: Given the number of months of how old the car is, reduce its value one-half
                (0.5) percent.
                After 10 years, it's value cannot be reduced further by age. 
                This is not cumulative.
            *************************************/
            usedValue -= usedValue * mAgeFactor * Math.Min(mMaxAge, car.AgeInMonths); 

            /************************************
            MILES: For every 1,000 miles on the car, reduce its value by one-fifth of a
            percent(0.2).Do not consider remaining miles. After 150,000 miles, it's 
            value cannot be reduced further by miles.
            *************************************/
            usedValue -= usedValue * Math.Min(mMaxMiles, car.NumberOfMiles) / mThousand * mMilesFactor;

            /*************************************
            PREVIOUS OWNER:    If the car has had more than 2 previous owners, reduce its value
            by twenty-five(25) percent.
            *************************************/
            if (car.NumberOfPreviousOwners > 2)
                usedValue -= usedValue * mPrevOwnerFactor;

            /*************************************
            COLLISION: For every reported collision the car has been in, remove two(2)
            percent of it's value up to five (5) collisions.
            *************************************/
            for (int i = 0; i < car.NumberOfCollisions && i < mMaxCollisions; i++)
                usedValue -= usedValue * mCollisionFactor;

            /*************************************
            OWNER SPECIAL - ZERO COLLISIONS:
            If the car has had no previous owners, add ten(10) percent of 
            the FINAL car value at the end.
            *************************************/
            if (car.NumberOfPreviousOwners == 0)
                usedValue += usedValue * mOwnerSpecial;
            //Round it out and convert it for return.
            return Convert.ToDecimal(Convert.ToDecimal(Math.Round(usedValue, 2)));
        }
        #endregion

    }

    [TestFixture]
    public class UnitTests
    {
        [TestCase]
        public void CalculateCarValue()
        {
            AssertCarValue(25313.40m, 35000m, 3 * 12, 50000, 1, 1);
            AssertCarValue(19688.20m, 35000m, 3 * 12, 150000, 1, 1);
            AssertCarValue(19688.20m, 35000m, 3 * 12, 250000, 1, 1);
            AssertCarValue(20090.00m, 35000m, 3 * 12, 250000, 1, 0);
            AssertCarValue(21657.02m, 35000m, 3 * 12, 250000, 0, 1);
        }

        private static void AssertCarValue(decimal expectValue, decimal purchaseValue,
        int ageInMonths, int numberOfMiles, int numberOfPreviousOwners, int
        numberOfCollisions)
        {
            Car car = new Car
            {
                AgeInMonths = ageInMonths,
                NumberOfCollisions = numberOfCollisions,
                NumberOfMiles = numberOfMiles,
                NumberOfPreviousOwners = numberOfPreviousOwners,
                PurchaseValue = purchaseValue
            };
            PriceDeterminator priceDeterminator = new PriceDeterminator();
            var carPrice = priceDeterminator.DetermineCarPrice(car);
            Assert.AreEqual(expectValue, carPrice);
        }
    }

    class Program
    {
        static void Main()
        {
            UnitTests ut = new UnitTests();
            ut.CalculateCarValue();
        }
    }
}
