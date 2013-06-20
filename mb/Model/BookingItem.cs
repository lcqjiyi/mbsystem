using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mb.Model
{
    public class Booking
    {
        public string NeiXiangDanNumber { get; set; }

        public string BoxZiseNumber { get; set; }

        public string BoxZisedDescription { get; set; }

        public string GoodNumber { get; set; }

        public string GoodDescription { get; set; }

        public string Unit { get; set; }

        public List<BookingItem> BookingItems { get; set; }

    }
    public class BookingItem
    {
        public string BoxNumber { get; set; }

        public string GridValueColor { get; set; }

        public string GridValueSize { get; set; }

        public string Quantity { get; set; }

        public string BoxCount { get; set; }

        public string Weight { get; set; }

    }
}
