using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Models;

namespace VGSShop.ModelsView
{
    public class FeedBack_Dao
    {
        VGSShopContext db = null;
        public FeedBack_Dao()
        {
            db = new VGSShopContext();
        }
        public int InsertFeedBack(Feedback fb)
        {
            db.Feedbacks.Add(fb);
            db.SaveChanges();
            return fb.Id;
        }
    }
}
