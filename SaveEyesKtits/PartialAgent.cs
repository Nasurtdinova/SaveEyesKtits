using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveEyesKtits
{
    public partial class Agent
    {
        public int CountSale => ProductSale.Where(a => a.SaleDate.Year == DateTime.Now.Year).Count();
        
        public int SizeDiscount
        {
            get
            {
                decimal sum = ProductSale.Sum(a => a.Product.MinCostForAgent * a.ProductCount);
                if (sum > 0 && sum < 10000)
                    return 0;
                else if (sum >= 10000 && sum < 50000)
                    return 5;
                else if (sum >= 50000 && sum < 150000)
                    return 10;
                else if (sum >= 150000 && sum < 500000)
                    return 20;
                else
                    return 25;
            }
        }
        public string Background
        {
            get
            {
                if (SizeDiscount == 25)
                    return "Green";
                else
                    return "White";
            }
        }
    }
}
