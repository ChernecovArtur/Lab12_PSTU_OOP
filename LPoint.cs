using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace Lab_12_OPP
{
    public class HPoint<TValue>
    {
        public int key;//ключ
        public TValue value;//значение
        public HPoint<TValue> next;//ссылка на следующий элемент
        static Random rnd = new Random();

        public HPoint(TValue s)
        {
            value = s;
            key = GetHashCode();
            next = null;
        }

        public override string ToString()
        {
            return key + ":" + value.ToString();
        }

        public override int GetHashCode()
        {


            int code = 0;
            string str = value.ToString();
            if (str != null)
            {
                foreach (char c in str)
                    code += (int)c;
                return code;
            }
            return code;
        }
    }
}
