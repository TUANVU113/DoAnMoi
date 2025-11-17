// Jobs/ClusteringJob.cs
//using Microsoft.EntityFrameworkCore;
//using TImViecAPI.Data;
//using TImViecAPI.Helpers;
//using TImViecAPI.Models;
//using TImViecAPI.Services;

//namespace TImViecAPI.Jobs
//{
//    public class ClusteringJob
//    {
//        private readonly AppDbContext _context;
//        private readonly ClusteringService _clustering;

//        public ClusteringJob(AppDbContext context)
//        {
//            _context = context;
//            _clustering = new ClusteringService();
//        }

//        public async Task RunAsync()
//        {
//            Console.WriteLine("[CLUSTERING] Bắt đầu phân cụm...");

//            var cvs = await _context.HoSo
//                .Where(h => h.NoiDungHoSo != null)
//                .Select(h => new
//                {
//                    NoiDung = h.NoiDungHoSo!,
//                    hsid = h.hsid
//                })
//                .ToListAsync();

//            // SỬA: PHẢI LÀ "Đang tuyển"
//            var jobs = await _context.TInTuyenDung
//                .Where(t => t.TrangThai == "Đã duyệt" && t.HanNop >= DateTime.Today)
//                .ToListAsync();

//            Console.WriteLine($"[CLUSTERING] Tìm thấy {cvs.Count} CV, {jobs.Count} tin");

//            if (!cvs.Any() || !jobs.Any())
//            {
//                Console.WriteLine("[CLUSTERING] Không đủ dữ liệu!");
//                return;
//            }

//            var allData = new List<double[]>();
//            var cvHoSoIds = new List<int>();
//            var jobIds = new List<int>();

//            foreach (var item in cvs)
//            {
//                allData.Add(VectorHelper.ToVector(item.NoiDung));
//                cvHoSoIds.Add(item.hsid);
//            }

//            foreach (var job in jobs)
//            {
//                allData.Add(VectorHelper.ToVector(job));
//                jobIds.Add(job.ttdid);
//            }

//            var labels = _clustering.Predict(allData.ToArray(), k: 5);

//            // CÁCH 1: DỪNG TRACKING HOÀN TOÀN (EF Core 7+)
//            await _context.UngVien_Cluster.ExecuteDeleteAsync();
//            await _context.TinTuyenDung_Cluster.ExecuteDeleteAsync();

//            // CÁCH 2: NẾU DÙNG EF Core 6 TRỞ XUỐNG
//            // var uvClusters = await _context.UngVien_Cluster.AsNoTracking().ToListAsync();
//            // var tinClusters = await _context.TinTuyenDung_Cluster.AsNoTracking().ToListAsync();
//            // _context.UngVien_Cluster.RemoveRange(uvClusters);
//            // _context.TinTuyenDung_Cluster.RemoveRange(tinClusters);
//            // await _context.SaveChangesAsync();

//            // ADD MỚI – AN TOÀN 100%
//            for (int i = 0; i < cvHoSoIds.Count; i++)
//            {
//                var hoSo = await _context.HoSo
//                    .AsNoTracking()
//                    .FirstOrDefaultAsync(h => h.hsid == cvHoSoIds[i]);

//                if (hoSo?.ungvienID != null)
//                {
//                    _context.UngVien_Cluster.Add(new UngVien_Cluster
//                    {
//                        ungvienID = hoSo.ungvienID.Value,
//                        ClusterID = labels[i],
//                        NgayCapNhat = DateOnly.FromDateTime(DateTime.Now)
//                    });
//                }
//            }

//            for (int i = 0; i < jobIds.Count; i++)
//            {
//                _context.TinTuyenDung_Cluster.Add(new TinTuyenDung_Cluster
//                {
//                    ttdid = jobIds[i],
//                    ClusterID = labels[cvHoSoIds.Count + i],
//                    NgayCapNhat = DateOnly.FromDateTime(DateTime.Now)
//                });
//            }

//            await _context.SaveChangesAsync();

//            Console.WriteLine($"[CLUSTERING] Hoàn thành! {cvs.Count} CV, {jobs.Count} tin → 5 cụm");
//        }
//    }
//}