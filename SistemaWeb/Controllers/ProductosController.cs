using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaWeb.Models;
using System.Data.Entity.Infrastructure; // arriba del archivo


namespace SistemaWeb.Controllers
{
    public class ProductosController : Controller
    {
        private SistemaDBEntities db = new SistemaDBEntities();

        // GET: Productos
        public ActionResult Index()
        {
            var productos = db.Productos.Include(p => p.Categorias);
            return View(productos.ToList());
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["Msg"] = "⚠️ Debes seleccionar una categoría para ver detalles.";
                return RedirectToAction("Index");
            }

            var productos = db.Productos.Find(id);
            if (productos == null)
            {
                TempData["Msg"] = "❌ La categoría indicada no existe.";
                return RedirectToAction("Index");
            }

            return View(productos);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            ViewBag.CategoriaId = new SelectList(db.Categorias, "CategoriaId", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoId,Nombre,Precio,Stock,CategoriaId")] Productos productos)
        {
            if (ModelState.IsValid)
            {
                db.Productos.Add(productos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoriaId = new SelectList(db.Categorias, "CategoriaId", "Nombre", productos.CategoriaId);
            return View(productos);
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Msg"] = "⚠️ Debes seleccionar una categoría para editar.";
                return RedirectToAction("Index");
            }

            var productos = db.Productos.Find(id);
            if (productos == null)
            {
                TempData["Msg"] = "❌ La categoría no existe.";
                return RedirectToAction("Index");
            }

            return View(productos);

            // POST: Productos/Edit/5
            // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
            // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoId,Nombre,Precio,Stock,CategoriaId")] Productos productos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoriaId = new SelectList(db.Categorias, "CategoriaId", "Nombre", productos.CategoriaId);
            return View(productos);
        }

        // GET: Productos/Delete/5
        /* public ActionResult Delete(int? id)
         {
             if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             Productos productos = db.Productos.Find(id);
             if (productos == null)
             {
                 return HttpNotFound();
             }
             return View(productos);
         }

         // POST: Productos/Delete/5
         [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(int id)
         {
             Productos productos = db.Productos.Find(id);
             db.Productos.Remove(productos);
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

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                TempData["Msg"] = "⚠️ Debes seleccionar un producto para eliminar.";
                return RedirectToAction("Index");
            }

            var producto = db.Productos.Find(id);
            if (producto == null)
            {
                TempData["Msg"] = "❌ El producto ya no existe.";
                return RedirectToAction("Index");
            }

            try
            {
                // Evitar borrar si está referenciado en pedidos
                bool enUso = db.DetallesPedido.Any(d => d.ProductoId == id);
                if (enUso)
                {
                    TempData["Msg"] = "⛔ No se puede eliminar: el producto está en pedidos.";
                    return RedirectToAction("Index");
                }

                db.Productos.Remove(producto);
                db.SaveChanges();
                TempData["MsgOk"] = "✅ Producto eliminado correctamente.";
            }
            catch (DbUpdateException)
            {
                TempData["Msg"] = "⛔ No se pudo eliminar: el producto está relacionado con otros registros.";
            }
            catch (Exception)
            {
                TempData["Msg"] = "⚠️ Ocurrió un error inesperado al eliminar el producto.";
            }

            return RedirectToAction("Index");
        }
    }
}
