using System;
using System.Collections;
using System.Collections.Generic;

using ClassLibrary;

namespace Lab_12_OPP
{
    public class MyCollectionEnumerator<T> : IEnumerator<T>
    {
        private HPoint<T>[] _table;
        private int position = -1;
        private T[] _array;

        public MyCollectionEnumerator(HPoint<T>[] table, int SumChainLength)
        {
            this._table = table;

            _array = new T[SumChainLength];

            int count = 0;

            for (var i = 0; i < _table.Length; i++)
            {
                var temp = _table[i];
                if (temp != null)
                {
                    _array[count] = temp.value;
                    count += 1;
                    while (temp.next != null)
                    {
                        temp = temp.next;
                        _array[count] = temp.value;
                        count += 1;
                    }
                }
            }
        }


        // public T Current => throw new NotImplementedException();
        public T Current
        {
            get
            {
                if (position == -1 || position >= _array.Length)
                    throw new ArgumentException();
                return _array[position];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            //throw new NotImplementedException();
            if (position < _array.Length - 1)
            {
                position++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            // throw new NotImplementedException();
            position = -1; // until the first MoveNext() call.
        }
    }

    public class MyCollection<T> : ICollection<T> where T: ICloneable
    {
        public int Count { get; private set; }
        public int[] ChainLength { get; }
        public int SumChainLength { get; private set; }
        public HPoint<T>[] Table { get; }
        public bool IsReadOnly { get; }


        //public int Size;

        public MyCollection()
        {
            Count = 0;
            SumChainLength = 0;
            Table = null;
            ChainLength = null;
        }

        public MyCollection(int count)
        {
            if (count <= 0)
            {
                Count = 0;
                SumChainLength = 0;
                Table = null;
                ChainLength = null;
                return;
            }
            Count = count;
            SumChainLength = 0;
            Table = new HPoint<T>[count];
            ChainLength = new int[count];
        }


        public MyCollection(MyCollection<T> c)
        {
            var temp = (MyCollection<T>)c.Clone();
            Count = temp.Count;
            Table = temp.Table;
            ChainLength = temp.ChainLength;
            SumChainLength = temp.SumChainLength;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MyCollectionEnumerator<T>(Table, SumChainLength);
        }

        IEnumerator IEnumerable.GetEnumerator() //interd link
        {
            return GetEnumerator();
        }


        public void Add(T item)
        {
            var element = new HPoint<T>(item);
            var count = element.GetHashCode() % Count;
            if (Table != null)
            {
                if (Table[count] != null)
                {
                    var temp = Table[count];
                    while (temp.next != null) temp = temp.next;
                    temp.next = element;
                    ChainLength[count] += 1;
                    SumChainLength += 1;
                    return;
                }
                Table[count] = element;
                ChainLength[count] = 1;
            }
            SumChainLength += 1;
        }



       /* public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }*/

        /*  public void Clear()
          {
              throw new NotImplementedException();
          }*/


        public void Clear()
        {
            Array.Clear(Table, 0, Count);
            Array.Clear(ChainLength, 0, Count);
            Count = 0;
            SumChainLength = 0;
        }


        public bool Contains(T item)
        {
            HPoint<T> element = new HPoint<T>(item);
            var count = element.GetHashCode() % Count;
            if (Table != null)
            { 

                if (Table[count] != null)
                { 
                    HPoint<T> temp = Table[count];
                    while (!Equals(element.value, temp.value))
                    {
                        if (temp.next == null) { return false; }
                        temp = temp.next;
                    }
                    return true;
                }
            }
            return false;
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array != null && array.Rank != 1)
                throw new ArgumentException("Только одномерные массивы", nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length < SumChainLength)
                throw new ArgumentException("Недостаточно элементов после индекса в  массиве");

            int count = 0;
            for (var i = arrayIndex; i < this.Count; i++)
            {
                var temp = this.Table[i];
                if (temp != null)
                {
                    array[count] = (T)temp.value.Clone();
                    count += 1;
                    while (temp.next != null)
                    {
                        temp = temp.next;
                        array[count] = (T)temp.value.Clone();
                        count += 1;
                    }
                }
            }
        }

        public bool Remove(T item)
        {
            var element = new HPoint<T>(item);
            var count = element.GetHashCode() % Count;
            if (Table != null)
            {
                if (Table[count] != null)
                {
                    var temp = Table[count];
                    if (temp.value.ToString() == element.value.ToString()) // Первый элемент равен удаляемому элементу
                    {
                        if (temp.next == null) // Элемент единств в цепочке
                        {
                            Table[count] = null;
                            ChainLength[count] -= 1;
                            SumChainLength -= 1;
                            return true;
                        }

                        Table[count] = temp.next; // Элемент не единств в цепочке
                        ChainLength[count] -= 1;
                        SumChainLength -= 1;
                        return true;
                    }

                    var nextTemp = temp.next;
                    while (nextTemp.next != null) 
                    {
                        if (element.value.ToString() == nextTemp.value.ToString())
                        {
                            temp = nextTemp.next;
                            ChainLength[count] -= 1;
                            SumChainLength -= 1;
                            return true;
                        }

                        temp = nextTemp;
                        nextTemp = nextTemp.next;
                    }
                }
            }
            return false;
        }

        public object Clone()
        {
            MyCollection<T> collection1 = new MyCollection<T>(Count);
            foreach (var element in this)
            {
                var temp = (T)element.Clone();
                collection1.Add(temp);
            }
            return collection1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello World!");

            Console.WriteLine("Хэш таблица без параметров (инициализация)");

            MyCollection<Person> x = new MyCollection<Person>();

            Console.WriteLine($"Количество элементов таблицы {x.SumChainLength}");


            Console.WriteLine("Нажмите кнопку для продолжения");
            Console.Read();

            var customElement1 = new Person("AAA", "BBB", 34, 'м', 5);
            x = new MyCollection<Person>(3);
            x.Add(new Person());
            x.Add(customElement1);
            /*x.Add(new Person());
            x.Add(new Person());*/

            x.Add(new Person("A", "B", 3, 'ж', 2));
            /*x.Add(new Person("A", "B", 3, 'ж', 2));
            x.Add(new Person("A", "B", 3, 'ж', 2));
            x.Add(new Person("A", "B", 3, 'ж', 2));
            x.Add(new Person("A", "B", 3, 'ж', 2));
            x.Add(new Person("A", "B", 3, 'ж', 2));*/
            Console.WriteLine($"Количество элементов {x.SumChainLength}");

            foreach (var element in x)
            {
                Console.WriteLine(element.Name, element.Surname, element.Age);
            }

            Console.WriteLine("Нажмите кнопку чтобы продолжить!");
            Console.Read();

            MyCollection<Person> y = new MyCollection<Person>(x);
            Console.WriteLine($"Количество элементов {y.SumChainLength}");
            foreach (var element in y)
            {
                Console.WriteLine(element.Name, element.Surname, element.Age);
            }

            Console.WriteLine("Нажмите кнопку чтобы продолжить!");
            Console.Read();


            Person[] arr1 = new Person[y.SumChainLength];
            Person[] arr2 = new Person[y.SumChainLength];

            x.CopyTo(arr1, 0);
            Console.WriteLine("Проверка метода CopyTo:");
            foreach (var element in arr1)
            {
                Console.WriteLine(element.Name, element.Surname, element.Age);
            }

            
            Console.WriteLine("Нажмите кнопку чтобы продолжить!");
            Console.Read();

            Console.WriteLine("Проверка метода CopyTo:");
            
            x.CopyTo(arr2, 2);

            arr2[1] = new Person("FFF", "FFF", 21, 'ж', 54);


            foreach (var element in arr2)
            {
                Console.WriteLine(element.Name, element.Surname, element.Age);
            }
            Console.WriteLine("Нажмите кнопку чтобы продолжить!");
            Console.Read();

           // List<Student> persons = new List<Student>();
            var customElement = new Student("AAA", "BBB", 34, 'м', 5, "IVT-21-2b", 4, 3);
            MyCollection<Student> z = new MyCollection<Student>(4);
            z.Add(customElement);
            z.Add(customElement);
            z.Add(customElement);
            z.Add(new Student());
            z.Add(new Student());
            z.Add(new Student());

            foreach (var element in z)
            {
                Console.WriteLine(element.Name, element.Surname, element.Age, element.ExamMark);
            }

            Console.WriteLine($"Наличие элемента в коллекции: {z.Contains(customElement)}");
            Console.WriteLine(z.Remove(customElement));
            Console.WriteLine(z.Remove(customElement));
            Console.WriteLine(z.Remove(customElement));
            Console.WriteLine($" Наличие элемента в коллекции: {z.Contains(customElement)}");

            Console.WriteLine("Нажмите кнопку чтобы продолжить!");
            Console.Read();

            z.Clear();
            Console.WriteLine($"Количество элементов {z.SumChainLength}");




        }
    }
}
