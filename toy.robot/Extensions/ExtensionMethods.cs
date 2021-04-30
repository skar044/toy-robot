namespace Toy.Robot.Extensions
{
    public static class ExtensionMethods
    {
        public static (int x, int y) IndexOf<T>(this T[,] matrix, T value)
        {
            var w = matrix.GetLength(0); // width
            var h = matrix.GetLength(1); // height

            for (var x = 0; x < w; x++)
            {
                for (var y = 0; y < h; y++)
                {
                    if (matrix[x, y].Equals(value))
                        return (x, y);
                }
            }

            return (-1, -1);
        }
    }
}