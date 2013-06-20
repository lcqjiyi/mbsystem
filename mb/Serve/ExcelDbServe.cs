using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using mb.Model;

namespace mb.Serve
{
    public class ExcelDbServe
    {
        public static void PackListToExcel(PackList packlist,float boxweight, string path){
           
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet0");
            int index = 1;
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < 15; i++)
            {
                row.CreateCell(i);
            }
            foreach (var item in packlist.BoxItems)
            {
                foreach (var item1 in item.GridValueItems)
                {
                    row = sheet.CreateRow(index);
                    for (int i = 0; i < 15; i++)
                    {
                        row.CreateCell(i);
                    }
                    index++;
                }
            }
            sheet.GetRow(0).GetCell(0).SetCellValue("内向交货单号");
            sheet.GetRow(0).GetCell(1).SetCellValue("箱码");
            sheet.GetRow(0).GetCell(2).SetCellValue("包装箱物料");
            sheet.GetRow(0).GetCell(3).SetCellValue("包装箱规格");
            sheet.GetRow(0).GetCell(4).SetCellValue("商品码");
            sheet.GetRow(0).GetCell(5).SetCellValue("商品描述");
            sheet.GetRow(0).GetCell(6).SetCellValue("网格值");
            sheet.GetRow(0).GetCell(7).SetCellValue("颜色");
            sheet.GetRow(0).GetCell(8).SetCellValue("规格");
            sheet.GetRow(0).GetCell(9).SetCellValue("每箱数量");
            sheet.GetRow(0).GetCell(10).SetCellValue("单位");
            sheet.GetRow(0).GetCell(11).SetCellValue("箱数");
            sheet.GetRow(0).GetCell(12).SetCellValue("总数");
            sheet.GetRow(0).GetCell(13).SetCellValue("单箱重量");
            sheet.GetRow(0).GetCell(14).SetCellValue("尾箱");
            index = 1;
            foreach (BoxItem boxItem in packlist.BoxItems)
            {
                foreach (GridValueItem GridValue in boxItem.GridValueItems)
                {
                    sheet.GetRow(index).GetCell(0).SetCellValue(packlist.NeiXiangDanNumber);
                    sheet.GetRow(index).GetCell(1).SetCellValue(boxItem.BoxMunber);
                    sheet.GetRow(index).GetCell(2).SetCellValue(packlist.BoxZiseNumber);
                    //sheet.GetRow(index).GetCell(3).SetCellValue("");
                    sheet.GetRow(index).GetCell(4).SetCellValue(packlist.GoodNumber);
                    //sheet.GetRow(index).GetCell(5).SetCellValue("");
                    sheet.GetRow(index).GetCell(6).SetCellValue(GridValue.GridValueColor+GridValue.GridValueSize);
                    sheet.GetRow(index).GetCell(7).SetCellValue(GridValue.GoodColor);
                    sheet.GetRow(index).GetCell(8).SetCellValue(GridValue.GoodSize);
                    sheet.GetRow(index).GetCell(9).SetCellValue(GridValue.Quantity);
                    sheet.GetRow(index).GetCell(10).SetCellValue(packlist.Unit);
                    sheet.GetRow(index).GetCell(11).SetCellValue(0);
                    sheet.GetRow(index).GetCell(12).SetCellValue(0);
                    sheet.GetRow(index).GetCell(13).SetCellValue(0);//单箱重量
                    //sheet.GetRow(index).GetCell(14).SetCellValue("");
                    index++;
                }
                sheet.GetRow(index - boxItem.GridValueItems.Count).GetCell(11).SetCellValue(1);
                sheet.GetRow(index - boxItem.GridValueItems.Count).GetCell(12).SetCellValue(boxItem.TatolQuantity);
                sheet.GetRow(index - boxItem.GridValueItems.Count).GetCell(13).SetCellValue(boxItem.TatolQuantity * packlist.Weight + boxweight);
                
            }
            FileStream fs = new FileStream(path,FileMode.OpenOrCreate);
            workbook.Write(fs);
            fs.Close();
        }
    }
}
