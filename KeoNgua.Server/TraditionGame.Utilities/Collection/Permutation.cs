using System.Collections.Generic;

namespace TraditionGame.Utilities.Collection
{
    public class Permutation
    {
        private int k;
        private int n;
        private int[] array;
        private List<List<int>> result;

        public Permutation(int k, int n)
        {
            this.k = k;
            this.n = n;
            this.array = new int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                this.array[i] = i;
            }
            this.result = new List<List<int>>();
        }

        /// <summary>
        /// Get toan to to hop chap k cua n phan tu
        /// </summary>
        /// <returns></returns>
        public List<List<int>> GetAllPermutation()
        {
            Combination(1);
            return this.result;
        }

        private void AddToResult()
        {
            List<int> element = new List<int>();
            for (int i = 1; i <= k; i++)
            {
                element.Add(array[i] - 1);
            }
            this.result.Add(element);
        }

        private void Combination(int i)
        {
            for (int j = this.array[i - 1] + 1; j <= this.n - this.k + i; j++)
            {
                array[i] = j;
                if (i == k)
                {
                    AddToResult();
                }
                else
                {
                    Combination(i + 1);
                }
            }
        }
    }
}
