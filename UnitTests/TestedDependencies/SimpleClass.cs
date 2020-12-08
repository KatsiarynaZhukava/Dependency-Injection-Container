using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.TestedDependencies
{
    class SimpleClass : ISimpleInterface
    {

        public int countValue(int i, int j, int k)
        {
            return i * j * k;
        }
    }
}
