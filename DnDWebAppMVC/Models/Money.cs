using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Models
{
    public class Money
    {
        private const int PLATINUM_TO_COPPER = 1000;
        private const int GOLD_TO_COPPER = 100;
        private const int ELECTRUM_TO_COPPER = 50;
        private const int SILVER_TO_COPPER = 10;
        
        public int Platinum { get; set; }
        public int Gold { get; set; }
        public int Electrum { get; set; }
        public int Silver { get; set; }
        public int Copper { get; set; }

        public void SetMoney(int copper)
        {
            Platinum = Convert(ref copper, PLATINUM_TO_COPPER);
            Gold = Convert(ref copper, GOLD_TO_COPPER);
            Electrum = Convert(ref copper,ELECTRUM_TO_COPPER);
            Silver = Convert(ref copper, SILVER_TO_COPPER);
            Copper = copper;
        }

        public int GetCopper()
        {
            var copper = Copper;
            copper += Silver * SILVER_TO_COPPER;
            copper += Electrum * ELECTRUM_TO_COPPER;
            copper += Gold * GOLD_TO_COPPER;
            copper += Platinum * PLATINUM_TO_COPPER;
            return copper;
        }

        private int Convert(ref int baseCurrency, int convertionRate)
        {
            var item = baseCurrency / convertionRate;
            baseCurrency -= item * convertionRate;
            return item;
        }
    }
}
