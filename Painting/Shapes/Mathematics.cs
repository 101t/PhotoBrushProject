using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Painting.Shapes
{
    public class Mathematics
    {
        public static float Subtract(float var1, float var2)
        {
            if (var1 > var2)
                return var1 - var2;
            return var2 - var1;
        }
        public static int Subtract(int var1, int var2)
        {
            if (var1 > var2)
                return var1 - var2;
            return var2 - var1;
        }

        public static void SetMinMax(float var1, float var2, out float min, out float max)
        {
            if (var1 > var2)
            { min = var2; max = var1; }
            else
            { min = var1; max = var2; }
        }
    }
}
