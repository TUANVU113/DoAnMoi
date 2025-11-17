namespace TImViecAPI.Services
{
    public class ClusteringService
    {
        public int[] Predict(double[][] data, int k = 5)
        {
            if (data.Length == 0) return Array.Empty<int>();

            var centroids = InitializeCentroids(data, k);
            var labels = new int[data.Length];
            bool changed;

            do
            {
                changed = false;
                for (int i = 0; i < data.Length; i++)
                {
                    int closest = GetClosestCentroid(data[i], centroids);
                    if (labels[i] != closest)
                    {
                        labels[i] = closest;
                        changed = true;
                    }
                }
                if (changed)
                    centroids = UpdateCentroids(data, labels, k);
            } while (changed);

            return labels;
        }

        private int GetClosestCentroid(double[] point, double[][] centroids)
        {
            int closest = 0;
            double minDist = double.MaxValue;
            for (int i = 0; i < centroids.Length; i++)
            {
                double dist = EuclideanDistance(point, centroids[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = i;
                }
            }
            return closest;
        }

        private double[][] InitializeCentroids(double[][] data, int k)
        {
            var rnd = new Random();
            var centroids = new double[k][];
            var used = new HashSet<int>();
            for (int i = 0; i < k; i++)
            {
                int idx;
                do { idx = rnd.Next(data.Length); } while (used.Contains(idx));
                used.Add(idx);
                centroids[i] = (double[])data[idx].Clone();
            }
            return centroids;
        }

        private double[][] UpdateCentroids(double[][] data, int[] labels, int k)
        {
            var centroids = new double[k][];
            var counts = new int[k];
            for (int i = 0; i < k; i++)
                centroids[i] = new double[data[0].Length];

            for (int i = 0; i < data.Length; i++)
            {
                int label = labels[i];
                counts[label]++;
                for (int j = 0; j < data[i].Length; j++)
                    centroids[label][j] += data[i][j];
            }

            for (int i = 0; i < k; i++)
                if (counts[i] > 0)
                    for (int j = 0; j < centroids[i].Length; j++)
                        centroids[i][j] /= counts[i];

            return centroids;
        }

        private double EuclideanDistance(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
                sum += Math.Pow(a[i] - b[i], 2);
            return Math.Sqrt(sum);
        }
    }
}
