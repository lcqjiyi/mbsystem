using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mb.Model;
using System.Windows.Forms;

namespace mb.Serve
{
    public static class WebDbServe
    {
        
        public static List<NeiXiangDanItem> GetNeiXiangDanItems(ref WebBrowser wb ){
            List<NeiXiangDanItem> Neixiangdanitems = new List<NeiXiangDanItem>();
            foreach (HtmlWindow window in wb.Document.Window.Frames)
            {
                HtmlElement HeaderElement = window.Document.GetElementById("aaaa.SearchView.Header");
                if (HeaderElement != null && HeaderElement.InnerText == "内向交货单明细")
                {
                    foreach (HtmlElement trElement in window.Document.GetElementsByTagName("tr"))
                    {
                        if (trElement.GetAttribute("userData")!= null 
                            && trElement.GetAttribute("userData").Contains("ShipmentDetails")
                            && trElement.Children[1].InnerText != "总计")
                        {
                            Neixiangdanitems.Add(new NeiXiangDanItem()
                            {
                                NeiXiangDanNumber = trElement.Children[1].InnerText,
                                DataLast = trElement.Children[2].InnerText,
                                GoodNumber = trElement.Children[4].InnerText,
                                GoodDDescription = trElement.Children[5].InnerText,
                                GridValue = trElement.Children[6].InnerText,
                                GoodColor = trElement.Children[7].InnerText,
                                GoodSize = trElement.Children[8].InnerText,
                                Address = trElement.Children[9].InnerText,
                                Quantity = trElement.Children[10].InnerText
                            });
                        }
                    }
                }
            }
            return Neixiangdanitems;
        }

        public static Booking GetBookingItems(ref WebBrowser wb)
        {
            Booking booking = new Booking();
            booking.BookingItems = new List<BookingItem>();
            foreach (HtmlWindow window in wb.Document.Window.Frames)
            {
                HtmlElement HeaderElement = window.Document.GetElementById("aaaa.ChangePackageView.Header");
                if (HeaderElement != null && HeaderElement.InnerText == "显示装箱明细")
                {

                    foreach (HtmlElement trElement in window.Document.GetElementsByTagName("tr"))
                    {
                        if (trElement.GetAttribute("userData") != null
                            && trElement.GetAttribute("userData").Contains("PackageBill.")
                            && trElement.Children[1].InnerText != "总计")
                        {
                            booking.NeiXiangDanNumber = trElement.Children[1].InnerText;
                            booking.BoxZiseNumber = trElement.Children[3].InnerText;
                            booking.BoxZisedDescription = trElement.Children[4].InnerText;
                            booking.GoodNumber = trElement.Children[5].InnerText;
                            booking.GoodDescription = trElement.Children[6].InnerText;
                            booking.Unit = trElement.Children[11].InnerText;
                            break;
                        }
                    }
                    
                    foreach (HtmlElement trElement in window.Document.GetElementsByTagName("tr"))
                    {
                        if (trElement.GetAttribute("userData") != null 
                            && trElement.GetAttribute("userData").Contains("PackageBill.")
                            && trElement.Children[1].InnerText != "总计")
                        {
                            booking.BookingItems.Add(new BookingItem()
                            {
                                BoxNumber = trElement.Children[2].InnerText,
                                GridValueColor = trElement.Children[7].InnerText.Substring(0, 2),
                                GridValueSize = trElement.Children[7].InnerText.Substring(2, 2),
                                Quantity = trElement.Children[10].InnerText,
                                BoxCount = trElement.Children[12].InnerText,
                                Weight = trElement.Children[14].InnerText
                               
                            });
                        }
                    }
                }
            }
            return booking;
        }

        public static PackList GetPackList(ref WebBrowser wb)
        {
            List<NeiXiangDanItem> NeiXiangDanitems = GetNeiXiangDanItems(ref wb);
            return GetPackList(NeiXiangDanitems);
        }

        private static PackList GetPackList(List<NeiXiangDanItem> NeiXiangDanitems)
        {
            PackList packlist = new PackList();
            if (NeiXiangDanitems.Count > 0)
            {
                packlist.NeiXiangDanNumber = NeiXiangDanitems[0].NeiXiangDanNumber;
                packlist.GoodNumber = NeiXiangDanitems[0].GoodNumber;
                packlist.Address = NeiXiangDanitems[0].Address;
            }
            packlist.GridValueItems = new List<GridValueItem>();
            foreach (NeiXiangDanItem item in NeiXiangDanitems)
            {
                packlist.GridValueItems.Add(new GridValueItem()
                {
                    GoodColor = item.GoodColor,
                    GoodSize = item.GoodSize,
                    GridValueColor = item.GridValue.Substring(0, 2),
                    GridValueSize = item.GridValue.Substring(2, 2),
                    Quantity = Convert.ToInt32(item.Quantity.Replace(",",""))
                });
            }
            return packlist;
        }

    }
}
