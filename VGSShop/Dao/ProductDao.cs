using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Models;
using VGSShop.ModelsView;

namespace VGSShop.Dao
{
    public class ProductDao
    {
        VGSShopContext db = null;
        public ProductDao()
        {
            db = new VGSShopContext();
        }
        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.ProductName.Contains(keyword)).Select(x => x.ProductName).ToList();
        }
        public List<ProductViewModel> Search(string keyword, ref int totalRecord, int pageIndex = 1, int pageSize = 2)
        {
            totalRecord = db.Products.Where(x => x.ProductName == keyword).Count();
            var model = (from a in db.Products
                         join b in db.Categories
                         on a.CatId equals b.CatId
                         where a.ProductName.Contains(keyword)
                         select new
                         {
                             CateMetaTitle = b.Alias,
                             CateName = b.CatName,
                             CreatedDate = a.DateCreated,
                             ID = a.ProductId,
                             Images = a.Thumb,
                             Name = a.ProductName,
                             MetaTitle = a.Alias,
                             Price = a.Price
                         }).AsEnumerable().Select(x => new ProductViewModel()
                         {
                             CateMetaTitle = x.MetaTitle,
                             CateName = x.Name,
                             CreatedDate = x.CreatedDate,
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Price = x.Price
                         });
            model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return model.ToList();
        }
        public bool ChangeStatus(int id)
        {
            var user = db.Products.Find(id);
            user.Active = !user.Active;
            db.SaveChanges();
            return user.Active;
        }
        public bool Delete(int id)
        {
            try
            {
                var user = db.Products.Find(id);
                db.Products.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
