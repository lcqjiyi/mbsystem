using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mb.Model
{
    public class PackList
    {
        public string NeiXiangDanNumber { get; set; }

        public string BoxZiseNumber { get; set; }

        public string GoodNumber { get; set; }

        public string Unit { get; set; }

        public string Address { get; set; }

        public List<GridValueItem> GridValueItems { get; set; }

        public List<BoxItem> BoxItems { get; private set; }

        public int BoxNumber { get; set; }

        public float Weight { get; set; }

        public void AddBoxItem(BoxItem boxitem) { 
            BoxNumber++;
            boxitem.BoxMunber = BoxNumber;
            BoxItems.Add(boxitem);
        }

        public void boxing()
        {
            this.BoxNumber = 0;
        }
        public string[] GridValueSizeList()
        {
            string[] gridvaluesizelist = GridValueItems.Select(o => o.GridValueSize).Distinct().ToArray();
            Array.Sort<string>(gridvaluesizelist);
            return gridvaluesizelist;
        }
        public void SetMaxQuantity(string MaxQuantity)
        {
            string[] Max = MaxQuantity.Split('/');
            foreach (string Maxitem in Max)
	        {
                if (Maxitem.Contains(':'))
                {
                    string size = Maxitem.Substring(0, Maxitem.IndexOf(':'));
                    string value = Maxitem.Substring(Maxitem.IndexOf(':')+1);
                    SetMaxQuantity(size, Convert.ToInt32(value));
                }
                else
                {
                    //size:maxqty
                    foreach (string size in GridValueSizeList())
                    {
                        SetMaxQuantity(size, Convert.ToInt32(Max[0]));
                    }
                }
	        }
               
        }
        public void SetMaxQuantity(string size, int maxqty)
        {
            foreach (GridValueItem item in GridValueItems)
            {
                if (item.GridValueSize == size)
                {
                    item.MaxQuantity = maxqty;
                }
            }
        }
        public void Boxing() {
            BoxItems = new List<BoxItem>();
            boxingOneColorBox();
            boxingMoreColorBox();
            boxingLastBox();
        }
        private void boxingMaxBox()
        {
            foreach (GridValueItem Gridvalueitem in GridValueItems)
            {
                while(Gridvalueitem.Quantity >= Gridvalueitem.MaxQuantity)
                {

                    BoxItem box = BoxFactory.GetNewMaxBoxItem(Gridvalueitem);
                    AddBoxItem(box);
                }
            }
        }
        private void boxingOneColorBox()
        {
            IEnumerable<IGrouping<string, GridValueItem>> onecolorgroup = from o in GridValueItems
                                                                          where o.Quantity > 0
                                                                          group o by o.GridValueColor into g
                                                                          select g;
            foreach (GridValueItem Gridvalueitem in GridValueItems)
            {
                while (Gridvalueitem.Quantity >= Gridvalueitem.MaxQuantity)
                {

                    BoxItem box = BoxFactory.GetNewMaxBoxItem(Gridvalueitem);
                    AddBoxItem(box);
                }
            }
            foreach (IGrouping<string, GridValueItem> onecolor in onecolorgroup)
            {
                foreach (BoxItem box in BoxFactory.GetMixBoxItems(onecolor.ToList(),0.1f))
                {
                    AddBoxItem(box);
                }
                foreach (BoxItem box in BoxFactory.GetSplitBoxItem(onecolor.ToList()))
                {
                    AddBoxItem(box);
                }
            }
        }
        private void boxingMoreColorBox()
        {
            var gridvlue = GridValueItems.Where(o => o.Quantity > 0);
            foreach (BoxItem box in BoxFactory.GetMixBoxItems(gridvlue.ToList(), 0.1f))
            {
                AddBoxItem(box);
            }
            gridvlue = GridValueItems.Where(o => o.Quantity > 0);
            foreach (BoxItem box in BoxFactory.GetSplitBoxItem(gridvlue.ToList()))
            {
                AddBoxItem(box);
            }
           
        }
        private void boxingLastBox()
        {
            var gridvlue = GridValueItems.Where(o => o.Quantity > 0).ToList();
            if (gridvlue.Count>0)
            {
                BoxItem box = BoxFactory.GetLastBoxItem(gridvlue);
                AddBoxItem(box);
            }
        }
    }
    public class BoxItem {

        public int BoxMunber { get; set; }

        public int TatolQuantity { get; private set; }

        public List<GridValueItem> GridValueItems { get; private set; }

        public BoxItem() {
            GridValueItems = new List<GridValueItem>();
        }

        public void AddGridValueItem(GridValueItem gridvalue)
        {
            GridValueItems.Add(new GridValueItem()
            {
                GoodColor = gridvalue.GoodColor,
                GoodSize = gridvalue.GoodSize,
                GridValueColor = gridvalue.GridValueColor,
                GridValueSize = gridvalue.GridValueSize,
                MaxQuantity = gridvalue.MaxQuantity,
                Quantity = gridvalue.Quantity
            });
            TatolQuantity += gridvalue.Quantity;
            gridvalue.Quantity = 0;
        }

        public void AddGridValueItem(GridValueItem gridvalue,int quantity)
        {
            GridValueItems.Add(new GridValueItem()
            {
                GoodColor = gridvalue.GoodColor,
                GoodSize = gridvalue.GoodSize,
                GridValueColor = gridvalue.GridValueColor,
                GridValueSize = gridvalue.GridValueSize,
                MaxQuantity = gridvalue.MaxQuantity,
                Quantity = quantity
            });
            TatolQuantity += quantity;
            gridvalue.Quantity -= quantity;
        }

    }
    public class BoxFactory {
        public static BoxItem GetLastBoxItem(List<GridValueItem> GridValue)
        {
            BoxItem box = new BoxItem();
            foreach (var gridvalue in GridValue)
            {
                box.AddGridValueItem(gridvalue);
            }
            return box;
        }
        public static List<BoxItem> GetSplitBoxItem(List<GridValueItem> GridValue)
        {
            List<BoxItem> Result = new List<BoxItem>();
            bool havenot;
            do
            {
                havenot = false;
                int[] want = new int[GridValue.Count];
                int tatolQuantity = 0;
                for (int i = 0; i < GridValue.Count; i++)
			    {
                    if (GridValue[i].Quantity <= 0) continue;//小于0跳出
                    want[i] = 1;//设为要
                    tatolQuantity += GridValue[i].Quantity; //计算总数
                    if (tatolQuantity >= GridValue[i].MaxQuantity)
                    {
                        BoxItem boxitem = new BoxItem();
                        for (int j = 0; j < GridValue.Count; j++)
                        {
                            if (want[j] == 1 && i != j)//不是要拆分的那个箱
                            {
                                boxitem.AddGridValueItem(GridValue[j]);
                            }
                            if (want[j] == 1 && i == j)//是要拆分的那个箱
                            {
                                int CurrentQuantity = GridValue[j].Quantity - (tatolQuantity - GridValue[j].MaxQuantity);
                                boxitem.AddGridValueItem(GridValue[j], CurrentQuantity);
                            }
                        }
                        Result.Add(boxitem);
                        havenot = true;
                        break;
                    }
			    }
            } while (havenot);
            return Result;
        }
        public static BoxItem GetNewMaxBoxItem(GridValueItem gridvalue)
        {
            BoxItem box = new BoxItem();
            box.AddGridValueItem(gridvalue, gridvalue.MaxQuantity);
            return box;
        }
        /// <summary>
        /// 得到所有混箱的Boxitem,包括可多装小装的混箱
        /// </summary>
        /// <param name="GridValueItems"></param>
        /// <param name="CanMoreOrLess">可多装小装的百分数 不可多装就设为0f</param>
        /// <returns></returns>
        public static List<BoxItem> GetMixBoxItems(List<GridValueItem> GridValueItems, float CanMoreOrLess = 0F)
        {
            List<BoxItem> Result = new List<BoxItem>();
            BoxItem boxitem = GetMixBoxItem(GridValueItems,0f);
            while (boxitem != null)
            {
                Result.Add(boxitem);

                boxitem = GetMixBoxItem(GridValueItems,0f);
            }
            //判断能否多出
            if (CanMoreOrLess > 0F)
            {
                boxitem = GetMixBoxItem(GridValueItems, CanMoreOrLess);
                 while (boxitem != null)
                {
                    Result.Add(boxitem);

                    boxitem = GetMixBoxItem(GridValueItems, CanMoreOrLess);
                }
            }
            return Result;
        }
        /// <summary>
        /// 得到一个多GridValueItem的箱 即混箱
        /// </summary>
        /// <param name="GridValueItems"></param>
        /// <param name="CanMoreOrLess">可多装小装的百分数 不可多装就设为0f</param>
        /// <returns></returns>
        private static BoxItem GetMixBoxItem(List<GridValueItem> GridValueItems, float CanMoreOrLess)
        {
            int[] State = new int[GridValueItems.Count];
            for (int i = 0; i < GridValueItems.Count; i++)
            {
                State[i] = 0;
            }
            if (Recursion(GridValueItems, State, 0, 0, CanMoreOrLess))
            {
                BoxItem boxItem = new BoxItem();
                for (int i = 0; i < State.Length; i++)
                {
                    if (State[i] == 1)
                    {
                        boxItem.AddGridValueItem(GridValueItems[i]);
                    }
                }
                return boxItem;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 递归 子集和和问题可以有多装数
        /// </summary>
        /// <param name="GridValueItems"></param>
        /// <param name="State"></param>
        /// <param name="index"></param>
        /// <param name="CurrentSum">可多装小装的百分数 不可多装就设为0f</param>
        /// <returns></returns>
        private static bool Recursion(List<GridValueItem> GridValueItems, int[] State, int index, int CurrentSum, float CanMoreOrLess)
        {
            if (index == State.Length)
            {
                return false;
            }
            if (GridValueItems[index].Quantity > 0)
            {
                State[index] = 1;
                CurrentSum += GridValueItems[index].Quantity;
                int CanMoreOrLessQuantity = Convert.ToInt32(GridValueItems[index].MaxQuantity * CanMoreOrLess);
                if (CurrentSum >= GridValueItems[index].MaxQuantity - CanMoreOrLessQuantity && CurrentSum <= GridValueItems[index].MaxQuantity + CanMoreOrLessQuantity)
                {
                    return true;
                }
                if (Recursion(GridValueItems, State, index + 1, CurrentSum, CanMoreOrLess))
                {
                    return true;
                }
            }
            State[index] = 0;
            CurrentSum -= GridValueItems[index].Quantity;
            return Recursion(GridValueItems, State, index + 1, CurrentSum, CanMoreOrLess);
        }
    }
    public class GridValueItem{

        public string GridValueColor { get; set; }

        public string GridValueSize { get; set; }

        public string GoodColor { get; set; }

        public string GoodSize { get; set; }

        public int Quantity { get; set; }

        public int MaxQuantity { get; set; }

    }
}
