using System;
namespace Plants.info.API.Common.Data.Utils
{
	public static class Util
	{
        /// <summary>
        /// Returns the substring starting at the end of the stringForIndex
        /// </summary>
        /// <param name="fullString"></param>
        /// <param name="stringForIndex">word of choice</param>
        /// <param name="shiftPlaces">any additional places you want to shift</param>
        /// <returns></returns>
        public static string? GetSubstring(string fullString, string stringForIndex, int shiftPlaces = 0)
		{
			if (fullString == "" || stringForIndex == "") return null;
            
            var index = fullString.IndexOf(stringForIndex);

			if (index == -1) return null;

			index += stringForIndex.Length + 1;

			if ((index + shiftPlaces) > fullString.Length) return null; 

            var subString = fullString.Substring(index + shiftPlaces);

			return subString; 
        }

	}
}

