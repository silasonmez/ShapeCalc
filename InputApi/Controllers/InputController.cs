using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using InputApi.Data;     // ← DbContext
using InputApi.Models;  // ← DTO & Entity
using System.Data.Entity.Infrastructure;   // (DbUpdateException)
using System.Net;                          // (HttpStatusCode)
using InputApi.Services;
using System.Web;


namespace InputApi.Controllers
{
    [RoutePrefix("api/input")]
    public class InputController : ApiController
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // Portal buraya POST ediyor.

        [HttpPost]
        [Route("bulk")]
        public async Task<IHttpActionResult> ReceiveFromPortal([FromBody] List<ShapeInputCreateDto> items)
        {
            var userId = Request.Headers.Contains("X-User-Id")
                ? Request.Headers.GetValues("X-User-Id").FirstOrDefault()
                : null;

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("X-User-Id header gerekli.");

            if (items == null || items.Count == 0)
                return BadRequest("Boş liste.");

            // 1) DB’ye yaz (Id oluşsun, IsCalculated=false)
            var now = DateTime.UtcNow;
            var entities = items.Select(x => new ShapeInput
            {
                UserId = userId,
                ShapeType = x.ShapeType?.Trim().ToLowerInvariant(),
                Parameter1 = x.Parameter1,
                Parameter2 = x.Parameter2,
                IsCalculated = false,
                CreatedAt = now
            }).ToList();

            db.ShapeInputs.AddRange(entities);

            
            try
            {
                db.SaveChanges(); // Id’ler burada oluşur
            }
            catch (DbUpdateException ex)
            {
                var detail = ex.InnerException?.InnerException?.Message
                             ?? ex.InnerException?.Message
                             ?? ex.Message;

                // Hata varsa detayını görmek için koyduk
                return Content(HttpStatusCode.InternalServerError, new
                {
                    error = "DbUpdateException",
                    message = detail
                });
            }
            

            // 2) ComputeApi’ye gönderilecek payload
              
            var toCalc = entities
                .Where(e => e.Area == null)
                .Select(e => new ShapeInputCalcDto
            {
                Id = e.Id,
                ShapeType = e.ShapeType,
                Parameter1 = e.Parameter1,
                Parameter2 = e.Parameter2,
                UserId = e.UserId
            }).ToList();


            // 4) ComputeApi’ye POST
            using (var client = new HttpClient())
            {
                // ComputeApi portunu kendi projen açıldığında adres çubuğundan al
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var selector = (IEndpointSelector)HttpContext.Current.Application["EndpointSelector"];

                var json = JsonConvert.SerializeObject(toCalc);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var fullUrl = selector.Next() + "api/calculate";  // http://localhost:7001/api/calculate
                var resp = await client.PostAsync(fullUrl, content);


                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    return Content(resp.StatusCode, body);
                }
                
            }

            return Ok(new { inserted = entities.Count, forwarded = toCalc.Count });
        }
        [HttpPost]
        [Route("forward-pending")]
        public async Task<IHttpActionResult> ForwardPending([FromUri] string userId)
        {


            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId zorunlu.");

            // 1) Bu kullanıcıya ait bekleyenleri çek
            var toCalc = db.ShapeInputs
                .Where(x => x.UserId == userId && !x.IsCalculated)
                .Select(e => new ShapeInputCalcDto
                {
                    Id = e.Id,
                    ShapeType = e.ShapeType,
                    Parameter1 = e.Parameter1,
                    Parameter2 = e.Parameter2,
                    UserId = e.UserId
                })
                .ToList();

            if (toCalc.Count == 0)
                return Ok(new { forwarded = 0 });

            // 2) paketleme
            var packets = toCalc
                .Select((v, i) => new { v, i })
                .GroupBy(t => t.i / 3)
                .Select(g => g.Select(t => t.v).ToList())
                .ToList();

            // 3) ComputeApi'ye gönder
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var selector = (IEndpointSelector)HttpContext.Current.Application["EndpointSelector"];

                foreach (var p in packets)
                {
                    var json = JsonConvert.SerializeObject(p);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var fullUrl = selector.Next() + "api/calculate";
                    var resp = await client.PostAsync(fullUrl, content);


                    if (!resp.IsSuccessStatusCode)
                    {
                        var body = await resp.Content.ReadAsStringAsync();
                        return Content(resp.StatusCode, body);
                    }
                }
            }

            return Ok(new { forwarded = toCalc.Count });
        }


        // İzlemek için küçük bir status
        [HttpGet]
        [Route("status")]
        public IHttpActionResult Status([FromUri] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId zorunlu");

            var total = db.ShapeInputs.Count(x => x.UserId == userId);
            var pending = db.ShapeInputs.Count(x => x.UserId == userId && !x.IsCalculated);
            var done = total - pending;

            return Ok(new { total, pending, done });
        }

    }
}
