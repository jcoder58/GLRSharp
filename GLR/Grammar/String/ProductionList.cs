using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLR.Grammar.String {
    public class ProductionList : ProductionList<string>  {

        public static implicit operator ProductionList(string terminal) {
            return new ProductionList() { 
                new StringTerminal(terminal) };
        }
    }
}
