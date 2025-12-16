using TImViecAPI.Model;
using System.Collections.Generic;
using System.Linq;

namespace TImViecAPI.Helpers
{
    public static class JobMatchingHelper
    {
        // Lấy maxId chung từ CV + tất cả tin
        public static int GetMaxId(NoiDungHoSo cv, IEnumerable<TInTuyenDung> jobs)
        {
            var allIds = new List<int>
            {
                cv.LinhVucID ?? 0,
                cv.ViTriLamViecID ?? 0,
                cv.NamKinhNghiemID ?? 0,
                cv.LoaiHinhLamViecID ?? 0,
                cv.ChucDanhID ?? 0
            };

            foreach (var job in jobs)
            {
                allIds.Add(job.linhvucIID ?? 0);
                allIds.Add(job.vitriID ?? 0);
                allIds.Add(job.kinhnghiemID ?? 0);
                allIds.Add(job.loaihinhID ?? 0);
                allIds.Add(job.chucdanhID ?? 0);
            }

            return allIds.Any(id => id > 0) ? allIds.Max() : 1;
        }

        // ToVector cho CV (dùng maxId chung)
        public static double[] ToVector(NoiDungHoSo cv, int maxId)
        {
            var vector = new double[5 * maxId];

            if (cv.LinhVucID.HasValue && cv.LinhVucID.Value > 0) vector[cv.LinhVucID.Value - 1] = 1.0;
            if (cv.ViTriLamViecID.HasValue && cv.ViTriLamViecID.Value > 0) vector[maxId + cv.ViTriLamViecID.Value - 1] = 1.0;
            if (cv.NamKinhNghiemID.HasValue && cv.NamKinhNghiemID.Value > 0) vector[2 * maxId + cv.NamKinhNghiemID.Value - 1] = 1.0;
            if (cv.LoaiHinhLamViecID.HasValue && cv.LoaiHinhLamViecID.Value > 0) vector[3 * maxId + cv.LoaiHinhLamViecID.Value - 1] = 1.0;
            if (cv.ChucDanhID.HasValue && cv.ChucDanhID.Value > 0) vector[4 * maxId + cv.ChucDanhID.Value - 1] = 1.0;

            return vector;
        }

        // ToVector cho Tin (dùng cùng maxId)
        public static double[] ToVector(TInTuyenDung job, int maxId)
        {
            var vector = new double[5 * maxId];

            if (job.linhvucIID.HasValue && job.linhvucIID.Value > 0) vector[job.linhvucIID.Value - 1] = 1.0;
            if (job.vitriID.HasValue && job.vitriID.Value > 0) vector[maxId + job.vitriID.Value - 1] = 1.0;
            if (job.kinhnghiemID.HasValue && job.kinhnghiemID.Value > 0) vector[2 * maxId + job.kinhnghiemID.Value - 1] = 1.0;
            if (job.loaihinhID.HasValue && job.loaihinhID.Value > 0) vector[3 * maxId + job.loaihinhID.Value - 1] = 1.0;
            if (job.chucdanhID.HasValue && job.chucdanhID.Value > 0) vector[4 * maxId + job.chucdanhID.Value - 1] = 1.0;

            return vector;
        }

        // Cosine giữ nguyên
        public static double CosineSimilarity(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0;

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