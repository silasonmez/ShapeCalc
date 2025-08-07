
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXApplication1.Models;
using Microsoft.AspNet.Identity;  //user ıd getirebilmek için ekledimm 
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;



namespace DXApplication1.Controllers
{
    [Authorize]
    public class ShapeInputController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShapeInput input)
        {
            if (ModelState.IsValid)
            {
                input.UserId = User.Identity.GetUserId(); // 🟡 Identity kullanımı
                db.ShapeInputs.Add(input);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(input);
        }
        private List<SelectListItem> GetShapeTypeList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Kare", Value = "Kare" },
                new SelectListItem { Text = "Dikdörtgen", Value = "Dikdörtgen" },
                new SelectListItem { Text = "Üçgen", Value = "Üçgen" }
            };
        }


        // GET: Edit
        public ActionResult Edit(int? id)
        {
            if (id == null) return HttpNotFound();
            var input = db.ShapeInputs.Find(id);
            if (input == null) return HttpNotFound();
            return View(input);
        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {
            if (id == null) return HttpNotFound();
            var input = db.ShapeInputs.Find(id);
            if (input == null) return HttpNotFound();
            return View(input);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var input = db.ShapeInputs.Find(id);
            db.ShapeInputs.Remove(input);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Details (opsiyonel)
        public ActionResult Details(int? id)
        {
            if (id == null) return HttpNotFound();
            var input = db.ShapeInputs.Find(id);
            if (input == null) return HttpNotFound();
            return View(input);
        }


        //Sayfa açıldığında GridView’in içini doldurur.
        public ActionResult GridViewPartial()
        {
            var userId = User.Identity.GetUserId();
            var data = db.ShapeInputs
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            ViewBag.DefaultShape = ViewBag.DefaultShape;
            return PartialView("_GridViewPartial", data);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialAddNew([ModelBinder(typeof(DevExpress.Web.Mvc.DevExpressEditorsBinder))] ShapeInput item)
        {
            
            ModelState.Remove("CreatedAt");
            ModelState.Remove("IsCalculated");
            
            if (ModelState.IsValid)
            {
                              
                item.UserId = User.Identity.GetUserId();
                item.IsCalculated = false ;
                if (item.ShapeType == "Kare")
                {
                    item.Parameter2 = item.Parameter1;
                }
                
                db.ShapeInputs.Add(item);
                db.SaveChanges();
            }
            else
            {
                ViewData["EditError"] = "Lütfen tüm alanları doğru doldurun.";
            }

            var userId = User.Identity.GetUserId();
            var data = db.ShapeInputs.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToList();
            return PartialView("_GridViewPartial", data);

        }
        

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate([ModelBinder(typeof(DevExpress.Web.Mvc.DevExpressEditorsBinder))] ShapeInput item)
        {
            var existingItem = db.ShapeInputs.Find(item.Id);
            var currentUserId = User.Identity.GetUserId();
            
            if (existingItem == null || existingItem.UserId != currentUserId)
            {
                ViewData["EditError"] = "Bu veriyi güncelleyemezsiniz.";
            }
            else if (ModelState.IsValid)
            {
                existingItem.ShapeType = item.ShapeType;
                existingItem.Parameter1 = item.Parameter1;
                
                // 🔽✅ Kare ise parametre2 = parametre1 yap
                if (item.ShapeType == "Kare")
                {
                    existingItem.Parameter2 = item.Parameter1;
                }
                else
                {
                    existingItem.Parameter2 = item.Parameter2;
                }

                existingItem.CreatedAt = item.CreatedAt;

                db.SaveChanges();
            }

            else
            {
                ViewData["EditError"] = "Güncelleme başarısız. Alanları kontrol edin.";
            }

            // 🔽🔽🔽 sadece giriş yapan kullanıcının verilerini getir
            var userId = User.Identity.GetUserId();
            var data = db.ShapeInputs.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToList();

            return PartialView("_GridViewPartial", data);
        }


        [HttpGet]
        public ActionResult Index(string shape = null)
        {
            var userId = User.Identity.GetUserId();

            var data = db.ShapeInputs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            ViewBag.DefaultShape = shape;

            return View( data); // Eğer view dosyanın adı "Index.cshtml" ise
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialDelete(int Id)
        {
            var item = db.ShapeInputs.Find(Id);
            var currentUserId = User.Identity.GetUserId();
            if (item != null)
            {
                db.ShapeInputs.Remove(item);
                db.SaveChanges();
            }
            var userId = User.Identity.GetUserId();
            var data = db.ShapeInputs.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToList();

            return PartialView("_GridViewPartial", data);
        }


        /// <summary>
        /// "Hesapla" butonundan AJAX ile çağrılır.
        /// InputApi'ye forward-pending gönderir, kısa süre pending=0 olana kadar bekler
        /// ve JSON yanıt döner (Redirect yok).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Hesapla()
        {
            // Portaldaki login kullanıcı
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Json(new { ok = false, message = "Kullanıcı oturumu bulunamadı (userId boş)." });

            // InputApi URL (Portal Web.config -> appSettings)
            var baseUrl = ConfigurationManager.AppSettings["InputApiBaseUrl"]; // örn: https://localhost:44325/
            if (string.IsNullOrWhiteSpace(baseUrl))
                return Json(new { ok = false, message = "InputApiBaseUrl appSetting'i bulunamadı." });

            try
            {
                using (var http = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromSeconds(10)
                })
                {
                    // 0) Ping – InputApi ayakta mı?
                    var ping = await http.GetAsync("api/input/status?userId=" + userId);
                    var pingBody = await ping.Content.ReadAsStringAsync();
                    if (!ping.IsSuccessStatusCode)
                        return Json(new { ok = false, message = $"InputApi ping hatası: {(int)ping.StatusCode} {ping.ReasonPhrase} - {pingBody}" });

                    // 1) Bekleyenleri ComputeApi'ye ilet (InputApi üzerinden)
                    var resp = await http.PostAsync($"api/input/forward-pending?userId={userId}", null);
                    var body = await resp.Content.ReadAsStringAsync(); // ör: {"forwarded": 2}
                    if (!resp.IsSuccessStatusCode)
                        return Json(new { ok = false, message = $"forward-pending HATA: {(int)resp.StatusCode} {resp.ReasonPhrase} - {body}" });

                    // 2) Kısa polling: pending=0 olana kadar bekle (maks ~4 sn)
                    for (int i = 0; i < 10; i++)
                    {
                        var st = await http.GetAsync($"api/input/status?userId={userId}");
                        dynamic s = JsonConvert.DeserializeObject(await st.Content.ReadAsStringAsync());
                        if ((int)s.pending == 0) break;
                        await Task.Delay(400);
                    }

                    return Json(new { ok = true, message = "Hesaplama için gönderildi.", forwarded = body });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = "Portal çağrısı hata: " + ex.Message });
            }
        }


    }
}