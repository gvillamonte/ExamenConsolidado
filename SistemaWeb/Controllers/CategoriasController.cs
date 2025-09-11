using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaWeb.Models;
using System.Data.Entity.Infrastructure; // DbUpdateException
using System.Data.SqlClient;             // (opcional) si quieres inspeccionar SQL


namespace SistemaWeb.Controllers
{
    public class CategoriasController : Controller
    {
        private SistemaDBEntities db = new SistemaDBEntities();

        // GET: Categorias
        public ActionResult Index()
        {
            return View(db.Categorias.ToList());
        }

        // GET: Categorias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["Msg"] = "⚠️ Debes seleccionar una categoría para ver detalles.";
                return RedirectToAction("Index");
            }

            var categoria = db.Categorias.Find(id);
            if (categoria == null)
            {
                TempData["Msg"] = "❌ La categoría indicada no existe.";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoriaId,Nombre")] Categorias categorias)
        {
            if (ModelState.IsValid)
            {
                db.Categorias.Add(categorias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categorias);
        }

        // GET: Categorias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Msg"] = "⚠️ Debes seleccionar una categoría para editar.";
                return RedirectToAction("Index");
            }

            var categoria = db.Categorias.Find(id);
            if (categoria == null)
            {
                TempData["Msg"] = "❌ La categoría no existe.";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }
        /*public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categorias categorias = db.Categorias.Find(id);
            if (categorias == null)
            {
                return HttpNotFound();
            }
            return View(categorias);
        }*/

        // POST: Categorias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoriaId,Nombre")] Categorias categorias)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categorias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categorias);
        }

        // GET: Categorias/Delete/5
        // GET: Productos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Msg"] = "⚠️ Debes seleccionar un producto para eliminar.";
                return RedirectToAction("Index");
            }

            var producto = db.Productos.Find(id);
            if (producto == null)
            {
                TempData["Msg"] = "❌ El producto indicado no existe.";
                return RedirectToAction("Index");
            }

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var producto = db.Productos.Find(id);
            if (producto == null)
            {
                TempData["Msg"] = "❌ El producto ya no existe.";
                return RedirectToAction("Index");
            }

            try
            {
                // Evitar FK: ¿aparece en detalles de pedidos?
                bool enUso = db.DetallesPedido.Any(d => d.ProductoId == id);
                if (enUso)
                {
                    TempData["Msg"] = "⛔ No se puede eliminar el producto porque está en pedidos.";
                    return RedirectToAction("Index");
                }

                db.Productos.Remove(producto);
                db.SaveChanges();
                TempData["MsgOk"] = "✅ Producto eliminado correctamente.";
            }
            catch (DbUpdateException)
            {
                TempData["Msg"] = "⛔ No se pudo eliminar: el producto está siendo usado por otros registros.";
            }
            catch (Exception)
            {
                TempData["Msg"] = "⚠️ Ocurrió un error inesperado al eliminar el producto.";
            }

            return RedirectToAction("Index");
        }

        /* public ActionResult Delete(int? id)
         {
             if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             Categorias categorias = db.Categorias.Find(id);
             if (categorias == null)
             {
                 return HttpNotFound();
             }
             return View(categorias);
         }/*

         // POST: Categorias/Delete/5

         /* [HttpPost, ActionName("Delete")]
          [ValidateAntiForgeryToken]
          public ActionResult DeleteConfirmed(int id)
          {
              Categorias categorias = db.Categorias.Find(id);
              db.Categorias.Remove(categorias);
              db.SaveChanges();
              return RedirectToAction("Index");
          }

          protected override void Dispose(bool disposing)
          {
              if (disposing)
              {
                  db.Dispose();
              }
              base.Dispose(disposing);
          }*/
       /* [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var categoria = db.Categorias.Find(id);
            if (categoria == null)
            {
                TempData["Msg"] = "❌ La categoría ya no existe.";
                return RedirectToAction("Index");
            }

            try
            {
                // Evitar FK: tiene productos asociados
                bool enUso = db.Productos.Any(p => p.CategoriaId == id);
                if (enUso)
                {
                    TempData["Msg"] = "⛔ No se puede eliminar la categoría porque tiene productos asociados.";
                    return RedirectToAction("Index");
                }

                db.Categorias.Remove(categoria);
                db.SaveChanges();
                TempData["MsgOk"] = "✅ Categoría eliminada correctamente.";
            }
            catch (DbUpdateException)
            {
                // Por si hay otra relación no prevista
                TempData["Msg"] = "⛔ No se pudo eliminar: la categoría está siendo usada por otros registros.";
            }
            catch (Exception)
            {
                TempData["Msg"] = "⚠️ Ocurrió un error inesperado al eliminar la categoría.";
            }

            return RedirectToAction("Index");
        }*/
    }
}
