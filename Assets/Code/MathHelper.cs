namespace TopDownShooter
{

    public static class MathHelper
    {
        public static float ClampAndLinearMap(float x, float x1, float y1, float x2, float y2)
        {
            if (x < x1)
                return y1;

            if (x > x2)
                return y2;

            var a = x1;
            var b = y1;
            var c = x2;
            var d = y2;

            var y = (d - b) / (c - a) * (x - a) + b;
            return y;
        }
    }
}