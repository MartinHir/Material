using ClosedXML.Excel;
using MaterialData.exceptions;
using MaterialData.models;
using MaterialData.models.material;
using MaterialData.repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace MaterialREST.Controllers
{
    [Route("material/export")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private ExportRepository export;

        public ExportController()
        {
            export = new ExportRepository(new DcvEntities(Properties.Resources.ResourceManager.GetString("ProductionConnection")));
        }

        [HttpPost]
        public ActionResult Post([FromBody] search materials)
        {
            try
            {
                var file = export.Export(materials);
                MemoryStream stream = GetStream(file);
                Response.Headers.Add(new KeyValuePair<string, StringValues>("fileName", "Material.xlsx"));
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", export.fileName);
            }
            catch(NotFoundException e)
            {
                Response.StatusCode = 404;
                Response.WriteAsync(e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public MemoryStream GetStream(XLWorkbook file)
        {
            MemoryStream stream = new MemoryStream();
            file.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}