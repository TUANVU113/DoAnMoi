using TImViecAPI.Model;

namespace TImViecAPI.Helpers
{
    public static class JobMatchingHelper
    {
        // Vector 5 chiều từ CV
        public static double[] ToVector(NoiDungHoSo cv)
        {
            return new double[]
            {
                cv.LinhVucID ?? 0,
                cv.ViTriLamViecID ?? 0,
                cv.NamKinhNghiemID ?? 0,
                cv.LoaiHinhLamViecID ?? 0,
                cv.ChucDanhID ?? 0
            };
        }

        // Vector 5 chiều từ Tin tuyển dụng
        public static double[] ToVector(TInTuyenDung tin)
        {
            return new double[]
            {
                tin.linhvucIID ?? 0,
                tin.vitriID ?? 0,
                tin.kinhnghiemID ?? 0,
                tin.loaihinhID ?? 0,
                tin.chucdanhID ?? 0
            };
        }

        // Cosine Similarity
        public static double CosineSimilarity(double[] a, double[] b)
        {
            double dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }
            if (normA == 0 || normB == 0) return 0;
            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}