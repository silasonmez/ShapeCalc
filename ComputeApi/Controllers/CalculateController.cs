using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ComputeApi.Data;     
using ComputeApi.Models;   

namespace ComputeApi.Controllers
{
    [RoutePrefix("api")]
    public class CalculateController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // InputApi’den hesaplanacak kayıtlar gelir (Id dolu!)
        [HttpPost]
        [Route("calculate")]
        public IHttpActionResult Calculate([FromBody] List<ShapeInputCalcDto> inputs)
        {
            System.Diagnostics.Debug.WriteLine($"[{Request.RequestUri.Host}:{Request.RequestUri.Port}] Paket alındı. Eleman sayısı: {inputs.Count}");
            Console.WriteLine($"[{Request.RequestUri.Host}:{Request.RequestUri.Port}] Paket alındı. Eleman sayısı: {inputs.Count}");
            if (inputs == null || inputs.Count == 0)
                return BadRequest("Boş liste.");
            

            var port = System.Web.HttpContext.Current.Request.Url.Port;

            foreach (var input in inputs)
            {
                double? area = null;
                
                var t = (input.ShapeType ?? "").Trim().ToLowerInvariant();

                switch (t)
                {
                    case "kare":
                        area = input.Parameter1 * input.Parameter1;
                        
                        break;
                    case "küp":
                        area = 6 * input.Parameter1 * input.Parameter1;
                        
                        break;
                    case "dikdörtgen":
                        if (input.Parameter2.HasValue)
                            area = input.Parameter1 * input.Parameter2.Value;
                            
                        break;
                    case "üçgen":
                        if (input.Parameter2.HasValue)
                            area = 0.5 * input.Parameter1 * input.Parameter2.Value;
                        break;
                    default:
                        // Geçersiz shapeType → atla
                        continue;
                }
                Console.WriteLine($"[PORT {port}] ✅ {t.ToUpper()} - ID={input.Id} - Alan={area}");
                System.Diagnostics.Debug.WriteLine($"[PORT {port}] ✅ {t.ToUpper()} - ID={input.Id} - Alan={area}");

                var entity = db.ShapeInputs.FirstOrDefault(x => x.Id == input.Id);
                if (entity != null)
                {
                    entity.Area = area;
                    
                    entity.IsCalculated = true;
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
            }

            db.SaveChanges();
            return Ok("Hesaplama tamamlandı");
        }

        [HttpGet]
        [Route("diag/whoami")]
        public IHttpActionResult WhoAmI()
        {
            var dbName = db.Database.SqlQuery<string>("SELECT DB_NAME()").FirstOrDefault();
            var cs = db.Database.Connection.ConnectionString;
            return Ok(new { dbName, cs });
        }
    }
}
