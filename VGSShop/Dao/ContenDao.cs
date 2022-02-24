using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Models;

namespace VGSShop.Dao
{
    public class ContenDao
    {
        VGSShopContext db = null;
        public ContenDao()
        {
            db = new VGSShopContext();
        }
        public Feedback GetActiveContact()
        {
            return db.Feedbacks.Single(x => x.Status == true);
        }
        public int InsertFeedBack(Feedback fb)
        {
            db.Feedbacks.Add(fb);
            db.SaveChanges();
            return fb.Id;
        }
    }
}
